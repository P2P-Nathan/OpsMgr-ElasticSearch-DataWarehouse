Imports Microsoft.EnterpriseManagement.HealthService
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Xml
Imports Microsoft.Win32
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports Nest

<MonitoringModule(ModuleType.WriteAction)> <ModuleOutput(False)> _
Public NotInheritable Class WriteToES
    'Inherit the ModuleBase we will be working off of
    Inherits ModuleBase(Of DataItemBase)

    'Shared objects are accessable from all instances
    Shared Logger As P2PLogging
    Shared ShutdownInProgress As Boolean

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

        'Verify we Have everything we need
        If configuration Is Nothing Then Throw New Exception("configuration is nothing")

        'Add the Additional Libraries to our instantiation
        Dim LibraryLoader As New LibraryLoader(Assembly.GetExecutingAssembly(), False)
        LibraryLoader.LoadLibrariesFromResources()

        'Extract from Config what we need and initiate the connections.
        Dim InstanceConfig As ParsedConfigData
        InstanceConfig = ExtractConfigData(configuration)

        'Create the Classes we will use throughout the life of this module
        Logger = New P2PLogging
        DataItemCollection = New DataItemProcessor(Logger)
        shutdownLock = New Object()
        ESConnector = New ElasticSearchConnector(InstanceConfig.ESNode, InstanceConfig.OtherIndex, InstanceConfig.WinEvtIndex, Logger)

    End Sub

    <InputStream(0)> _
    Public Sub OnNewDataItems(dataItems As DataItemBase(), logicallyGrouped As Boolean, acknowledgedCallback As DataItemAcknowledgementCallback, acknowledgedState As Object, completionCallback As DataItemProcessingCompleteCallback, completionState As Object)

        ' If we have been shutdown stop processing.
        If ShutdownInProgress Then
            Logger.WriteInformation("Shutdown in Progress")
            Return
        End If

        Try
            DataItemCollection.ProcessandLoadData(dataItems)
            If DataItemCollection.HasWindowsEvents Then ESConnector.IndexBulkWinEventsParallel(DataItemCollection.GetWinEventsToSubmit(), 400, 8)
            If DataItemCollection.HasPerfItems Then ESConnector.IndexBulkPerfEventsParallel(DataItemCollection.GetPerfItemsToSubmit(), 800, 8)
            If DataItemCollection.HasNONWindowsEvents Then ESConnector.IndexBulkOtherEventsParallel(DataItemCollection.GetNonWindowsEvents(), 200, 8)
            DataItemCollection.CleanCollection()
        Catch ex As Exception
            Logger.LogErrorDetails("Failed to Process Data Items:  ", ex)
            DataItemCollection = New DataItemProcessor(Logger)
        End Try

        SyncLock shutdownLock
            Me.ModuleHost.RequestNextDataItem()
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

