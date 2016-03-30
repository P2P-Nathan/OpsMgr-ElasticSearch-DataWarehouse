Imports Microsoft.EnterpriseManagement.HealthService
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Xml
Imports Microsoft.Win32

<MonitoringModule(ModuleType.Condition)>
<ModuleOutput(True)>
Public Class EncodeJSONData
    '   Portion of Code Sourced from https://scommm.codeplex.com/
    Inherits ModuleBase(Of JSONEncodedDataItem)
    ''' <summary>
    ''' Keep track if we must return a single data item or a collection of data items
    ''' </summary>
    Private ReadOnly boolreturnMultipleItems As Boolean

    ''' <summary>
    ''' Object we use for doing locking to check to see if we have been
    ''' shutdown or not.  
    ''' </summary>
    Private ReadOnly shutdownLock As Object


    ''' <summary>
    ''' 'Shared objects are accessable from all instances
    ''' </summary>
    Shared Logger As P2PLogging

#Region "XML Element Names"

    Private Const ConfigurationElementName As String = "Configuration"
    Private Const ReturnMultipleItems As String = "ReturnMultipleItems"
    Private Const ComputerNameTag As String = "ComputerName"

#End Region

    ''' <summary>
    ''' Boolean value tracking if the module has been shutdown or not.
    ''' </summary>
    Private boolshutdown As Boolean

    ''' <summary>
    ''' Complex Type used to Process and produce Items as an Array of Typed Objects
    ''' </summary>
    Private ElasticEncoder As ElasticDocEncoder

    ''' <summary>
    ''' Constructor of the Module is called when the Module is initialized with the moduleHost pointer and the Xml configuration
    ''' along with the previous state, if any. If this is a stateful module, whatever state the module stored last would be handed 
    ''' back to the module. This is just a byte array.
    ''' </summary>
    ''' <param name="moduleHost">The host object provides the services the
    ''' module needs to interact with the Health Service.</param>
    ''' <param name="configuration">XML reader giving the configuration of this module.</param>
    ''' <param name="previousState">Previous state of the module.  This must be null since this module never calls SaveState</param>
    Public Sub New(moduleHost As ModuleHost(Of JSONEncodedDataItem), configuration As XmlReader, previousState As Byte())
        MyBase.New(moduleHost)

        If moduleHost Is Nothing Then
            Logger.LogErrorDetails("Error moduleHost is null")
            Throw New ArgumentNullException("moduleHost")
        End If

        If configuration Is Nothing Then
            Logger.LogErrorDetails("Error configuration is null")
            Throw New ArgumentNullException("configuration")
        End If

        If previousState IsNot Nothing Then
            Logger.LogErrorDetails("Error previousState is null")
            ' Since this module never calls SaveState this value should be null.
            Throw New ArgumentOutOfRangeException("previousState")
        End If

        ' Create the shutdown block
        shutdownLock = New Object()

        ' set the default
        boolreturnMultipleItems = True

        'Create the Classes we will use throughout the life of this module
        Logger = New P2PLogging


        Try
            configuration.MoveToContent()

            configuration.ReadStartElement(EncodeJSONData.ConfigurationElementName)
            configuration.ReadStartElement(EncodeJSONData.ReturnMultipleItems)
            boolreturnMultipleItems = configuration.ReadContentAsBoolean()
            Dim ComputerName As String
            configuration.ReadStartElement(EncodeJSONData.ReturnMultipleItems)
            ComputerName = configuration.ReadContentAsString()
            ElasticEncoder = New ElasticDocEncoder(ComputerName, Logger)
            configuration.ReadEndElement()
            configuration.ReadEndElement()
        Catch xe As XmlException
            Logger.LogErrorDetails([String].Format("Error configuration is invalid. {0}", configuration.ReadOuterXml()))
            Throw New ModuleException("Invalid Xml Config received.", xe)
        Finally
            Logger.WriteTrace("Exiting EncodeJSONData Constructor")
        End Try
    End Sub

    ''' <summary>
    ''' Do any cleanup that you have to do in this method
    ''' </summary>
    Public Overrides Sub Shutdown()
        SyncLock shutdownLock
            Debug.Assert(Not boolshutdown)

            boolshutdown = True
        End SyncLock
    End Sub

    ''' <summary>
    ''' This function is called by the Health Service, so it is a necessary method. You should explicitly call RequestNextDataItem() to
    ''' receive data in this method
    ''' </summary>
    Public Overrides Sub Start()
        ' since Shutdown, OnNewDataItems and Start acquire the lock before doing any action, only one of them can execute code in the code
        ' segment inside the lock { ... } code block
        SyncLock shutdownLock
            If boolshutdown Then
                Return
            End If

            ' Request the first data batch.
            ModuleHost.RequestNextDataItem()
        End SyncLock
    End Sub

    ''' <summary>
    ''' This is the function that is called when new data is available. We
    ''' read the data items and append the configured string to it and output it
    ''' immediately
    ''' </summary>
    ''' <param name="dataItems">data received</param>
    ''' <param name="logicalSet">Is the data batch a logical set</param>
    ''' <param name="acknowledgedCallback">Optional. Callback to be invoked when the module accepts responsibility for the data item.</param>
    ''' <param name="acknowledgedState">Parameter that must be passed in the call to acknowledgedCallback</param>
    ''' <param name="completionCallback">Optional. Callback to be invoked when the module has completed processing this data batch.</param>
    ''' <param name="completionState">Parameter that must be passed in the call to completionCallback</param>
    '''/ It is critical have this attribute. For every input (number of inputs is defined in the MP), there should be a corresponding input method
    '''/ InputStream(0) says that this is the input method for Input-0. If we did not have this methodAttribute, Health Service would be unable to find
    '''/ an input method for port0. (ports are numbered from 0 to (MaxInput -1)). If there are 5 Inputs defined for the Module in the MP,
    '''/ then there should be a correponding method with InputStream(0), InputStream(1)...InputStream(4). Although it is possible to have a 
    '''/ single method handle all the inputs, it would not be able to tell which port the input came from since the port nunmber is not passed
    '''/ along with the data, although the method may able to tell which port the input came from based on the DataItem type
    <InputStream(0)>
    Public Sub OnNewDataItems(dataItems As DataItemBase(), logicalSet As Boolean, acknowledgedCallback As DataItemAcknowledgementCallback, acknowledgedState As Object, completionCallback As DataItemProcessingCompleteCallback, completionState As Object)
        ' Either both delegates are null or neither should be.
        If (acknowledgedCallback Is Nothing AndAlso completionCallback IsNot Nothing) OrElse (acknowledgedCallback IsNot Nothing AndAlso completionCallback Is Nothing) Then
            Throw New ArgumentOutOfRangeException("acknowledgedCallback, completionCallback", "Only one of acknowledgedCallback and completionCallback is non-null together")
        End If

        ' If an ack was requested on the data we received we want to
        ' request an ack on the data we post.  If there was no ack on the
        ' input data it doesn't make sense to request an ack on the
        ' output.
        Dim ackNeeded As Boolean = acknowledgedCallback IsNot Nothing

        ' Acquire the lock guarding against shutdown.
        SyncLock shutdownLock
            ' If the module has been shutdown we should stop processing.
            If boolshutdown Then
                Return
            End If

            ' Create the list we will use for storing output
            Dim outputDataItems As New List(Of JSONEncodedDataItem)()
            Dim allDataItemsOneString As String = ""
            Dim corruptLogicalSet As Boolean = False
            Dim JSONencodedString As String
            ' Dim Reader As XmlReader

            ElasticEncoder.SetIndexPrefix(Now.ToString("yyyy-MM-dd"))
            ' Loop through all input data items processing them

            For Each dataItem As DataItemBase In dataItems
                ' When processing the input data items as a best practice
                ' we should always check for MalformedDataItemException on
                Try
                    JSONencodedString = ElasticEncoder.EncodeSingleInsert(dataItem)
                    Logger.WriteTrace("Encoded as follow")
                    Logger.WriteTrace(JSONencodedString)

                    ' if we return multiple itemas default just queue the new dataitem, if not build the output string allIn1Item
                    If boolreturnMultipleItems Then
                        Dim outputDataItem As New JSONEncodedDataItem(JSONencodedString)
                        'reader = outputDataItem.GetItemXml()
                        'reader.MoveToContent()
                        outputDataItems.Add(outputDataItem)
                    Else
                        allDataItemsOneString += JSONencodedString
                    End If
                Catch generatedExceptionName As MalformedDataItemException

                    If logicalSet Then
                        corruptLogicalSet = True
                        ModuleHost.NotifyDroppedMalformedDataItems(dataItems.Length)
                        ' We are going to drop the whole batch and bail
                        Exit Try
                    Else
                        ' Not a set we can drop out this individual item only.
                        ' Notify the host that we dropped a data item.
                        ModuleHost.NotifyDroppedMalformedDataItems(1)
                    End If
                Finally
                    'Cleanup??
                End Try
            Next

            If Not boolreturnMultipleItems AndAlso Not [String].IsNullOrEmpty(allDataItemsOneString) Then
                'if we need to return a single item
                Try
                    Dim outputDataItem As New JSONEncodedDataItem(allDataItemsOneString)
                    outputDataItems.Add(outputDataItem)
                Catch generatedExceptionName As MalformedDataItemException
                    corruptLogicalSet = True
                    ModuleHost.NotifyDroppedMalformedDataItems(dataItems.Length)
                End Try
            End If

            ' If we ended up with no data items due to corruption or any
            ' items in a logical set were corrupted we will have no
            ' output.  We need to give any acks and then request the next
            ' data item.
            If outputDataItems.Count = 0 OrElse corruptLogicalSet Then
                If ackNeeded Then
                    acknowledgedCallback(acknowledgedState)
                    completionCallback(completionState)
                End If

                ModuleHost.RequestNextDataItem()

                Return
            End If

            If ackNeeded Then
                ' We want to forward on the acknowledgement on input data
                ' to the next module.  We create an anonymous delegate to
                ' process handling the ack.
                Dim ackDelegate As DataItemAcknowledgementCallback = Sub(ackState As Object)
                                                                         ' We set this parameter to null when calling
                                                                         ' PostOutputDataItems so we expect this parameter to be null here.
                                                                         Debug.Assert(ackState Is Nothing)

                                                                         SyncLock shutdownLock
                                                                             ' If we have been shutdown stop processing.
                                                                             If boolshutdown Then
                                                                                 Return
                                                                             End If

                                                                             ' Send the ack and completion back for the input.
                                                                             acknowledgedCallback(acknowledgedState)
                                                                             completionCallback(completionState)

                                                                             ' Know that we have sent back both the completion and
                                                                             ' ack we can request the next data item.
                                                                             ModuleHost.RequestNextDataItem()
                                                                         End SyncLock

                                                                     End Sub

                ModuleHost.PostOutputDataItems(outputDataItems.ToArray(), logicalSet, ackDelegate, Nothing)
            Else
                ' No ack was requested on input.  We can post the output
                ' and then immediately request the next data items
                ModuleHost.PostOutputDataItems(outputDataItems.ToArray(), logicalSet)

                ModuleHost.RequestNextDataItem()
            End If
        End SyncLock
    End Sub

End Class
