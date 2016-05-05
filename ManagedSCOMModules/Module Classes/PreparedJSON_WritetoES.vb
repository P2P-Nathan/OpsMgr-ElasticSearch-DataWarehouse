Imports Microsoft.EnterpriseManagement.HealthService
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Xml
Imports Microsoft.Win32
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports Nest

<MonitoringModule(ModuleType.WriteAction)> <ModuleOutput(False)>
Public NotInheritable Class PreparedJSON_WritetoES
    'Inherit the ModuleBase we will be working off of
    Inherits ModuleBase(Of DataItemBase)

    'Shared objects are accessable from all instances
    Shared Logger As P2PLogging
    Shared ShutdownInProgress As Boolean = False

    'This global object will help to control data useage with SyncLocks.
    Private shutdownLock As Object

    'These items are Global, reducing the instantiation cost of reuse
    Private ESConnector As ElasticSearchConnector
    Private DataItemCollection As DataItemProcessor

    Public Overrides Sub Start()

        SyncLock shutdownLock

            'We don't need to continue if Shutdowns are starting.
            If ShutdownInProgress Then
                Return
            End If

            'Request the first data batch.  
            'The ME keyword unambiguously refers to this instance
            Me.ModuleHost.RequestNextDataItem()

        End SyncLock

    End Sub

    Public Overrides Sub Shutdown()

        'Lock to prevent other operations during Shutdown
        SyncLock shutdownLock
            ShutdownInProgress = True
            Me.Finalize()
        End SyncLock

    End Sub


    Public Sub New(moduleHost As ModuleHost(Of DataItemBase), configuration As XmlReader, previousState As Byte())
        'Call the Base Constructor 
        MyBase.New(moduleHost)

        Try
            Logger = New P2PLogging
            'Verify we Have everything we need
            If configuration Is Nothing Then Throw New Exception("configuration is nothing")

            ''Add the Additional Libraries to our instantiation
            'Dim LibraryLoader As New LibraryLoader(Assembly.GetExecutingAssembly(), False)
            'LibraryLoader.LoadLibrariesFromResources()

            'Extract from Config what we need and initiate the connections.
            Dim InstanceConfig As ParsedConfigData
            InstanceConfig = ExtractConfigData(configuration)

            'Create the Classes we will use throughout the life of this module

            DataItemCollection = New DataItemProcessor(Logger)
            ESConnector = New ElasticSearchConnector(InstanceConfig.ESNode, InstanceConfig.OtherIndex, InstanceConfig.WinEvtIndex, Logger)
        Catch ex As Exception
            Logger.LogErrorDetails("Failed to Start Module", ex)
        Finally
            Logger.WriteInformation("Completed PrePared JSON Write Startup")
            shutdownLock = New Object()
        End Try


    End Sub

    <InputStream(0)>
    Public Sub OnNewDataItems(dataItems As DataItemBase(), logicallyGrouped As Boolean, acknowledgedCallback As DataItemAcknowledgementCallback, acknowledgedState As Object, completionCallback As DataItemProcessingCompleteCallback, completionState As Object)

        If (acknowledgedCallback Is Nothing AndAlso completionCallback IsNot Nothing) OrElse (acknowledgedCallback IsNot Nothing AndAlso completionCallback Is Nothing) Then
            Throw New ArgumentOutOfRangeException("acknowledgedCallback, completionCallback", "Only one of acknowledgedCallback and completionCallback is non-null together")
        End If
        Dim ackNeeded As Boolean = acknowledgedCallback IsNot Nothing

        SyncLock shutdownLock
            Try
                ' If we have been shutdown stop processing.
                If ShutdownInProgress Then
                    Logger.WriteInformation("Shutdown in Progress")
                    Return
                End If


                Dim SB As New Text.StringBuilder
                SB.EnsureCapacity(dataItems.Count * 300)

                'Get each item pulled
                For Each JSONItem As JSONEncodedDataItem In dataItems
                    SB.Append(JSONItem.EncodedItem)
                Next

                'Post Async
                ESConnector.ProccessBulkAsyncJSON(SB.ToString(), 8)
                SB.Clear()
            Catch ex As Exception
                Logger.LogErrorDetails("Failed to Process Data Items:  ", ex)
            End Try

            If ackNeeded Then

                ' Send the ack and completion back for the input.
                acknowledgedCallback(acknowledgedState)
                completionCallback(completionState)

                ' Know that we have sent back both the completion and
                ' ack we can request the next data item.
                ModuleHost.RequestNextDataItem()

            Else
                ModuleHost.RequestNextDataItem()
            End If
        End SyncLock

    End Sub

    Public Shared Function LoadResource(ResourceName As String) As Assembly
        Dim ba As Byte() = Nothing
        Dim resource As String = ResourceName
        Dim curAsm As Assembly = Assembly.GetExecutingAssembly()
        Using stm As IO.Stream = curAsm.GetManifestResourceStream(resource)
            ba = New Byte(CInt(stm.Length) - 1) {}
            stm.Read(ba, 0, CInt(stm.Length))

            Return Assembly.Load(ba)
        End Using
    End Function

    Private Function ExtractConfigData(configuration As XmlReader) As ParsedConfigData
        Dim ReturnData As New ParsedConfigData

        Try
            'Try to read our configuration elements 
            Dim XMLConfig As New XmlDocument
            XMLConfig.Load(configuration)
            Logger.WriteTrace("XML config is " + XMLConfig.OuterXml)

            'Config data is pulled via Xpath, like in MPs
            ReturnData.ESNode = XMLConfig.SelectSingleNode("/Configuration/ElasticSearchNode").InnerText
            ReturnData.WinEvtIndex = XMLConfig.SelectSingleNode("/Configuration/WinEventIndex").InnerText
            ReturnData.OtherIndex = XMLConfig.SelectSingleNode("/Configuration/AllOtherIndex").InnerText
            Logger.WriteTrace("Parsed Config--- Node = " + ReturnData.ESNode + "; WinEvtIndex = " + ReturnData.WinEvtIndex + "; AllOtherIndex = " + ReturnData.OtherIndex)
        Catch ex As Exception
            Logger.WriteError("Failed to read the configuration as start")
            Logger.LogErrorDetails(ex.ToString, ex)
            Throw New Exception("We cant's start, configuration could not be parsed.")
        End Try

        Return ReturnData

    End Function

    Private Structure ParsedConfigData
        Public ESNode As String
        Public WinEvtIndex As String
        Public OtherIndex As String
    End Structure
End Class

