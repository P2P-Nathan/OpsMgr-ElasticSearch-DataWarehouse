Imports Microsoft.EnterpriseManagement.HealthService
Imports System.Xml
Imports System.Xml.XPath

Public Structure WinEventDataStructure
    Public EventOriginID As Guid
    Public PublisherId As Guid
    Public SourceHealthServiceID As Guid
    Public PublisherName As String
    Public EventSourceName As String
    Public Channel As String
    Public LoggingComputer As String
    Public EventNumber As Long
    Public EventCategory As String
    Public EventLevel As String
    Public UserName As String
    Public RawDescription As String
    Public LCID As String
    Public CollectDescription As String
    Public Params As String
    Public EventData As String
    Public EventDisplayNumber As Integer
    Public EventDescription As String
    Public CorrelationActivityId As Guid
    Public CorrelationRelatedActivityId As Guid
    Public Keywords As String
    Public EventDate As Date
    Public ProcessId As Integer
    Public ThreadId As Integer

    Public Sub New(ByRef dataitem As DataItemBase)
        EventDate = dataitem.DateCreated
        SourceHealthServiceID = dataitem.SourceHealthServiceId
        Dim reader As XmlReader = dataitem.GetItemXml()
        Do While reader.Name <> "EventOriginId"
            reader.Read()
        Loop
        If reader.Name = "EventOriginID" Then EventOriginID = Guid.Parse(reader.ReadElementContentAsString()) : reader.Read() Else EventOriginID = Nothing
        If reader.Name = "PublisherId" Then PublisherId = Guid.Parse(reader.ReadElementContentAsString()) : reader.Read() Else PublisherId = Nothing
        If reader.Name = "PublisherName" Then PublisherName = reader.ReadElementContentAsString() : reader.Read() Else PublisherName = Nothing
        If reader.Name = "EventSourceName" Then EventSourceName = reader.ReadElementContentAsString() : reader.Read() Else EventSourceName = Nothing
        If reader.Name = "Channel" Then Channel = reader.ReadElementContentAsString() : reader.Read() Else Channel = Nothing
        If reader.Name = "LoggingComputer" Then LoggingComputer = reader.ReadElementContentAsString() : reader.Read() Else LoggingComputer = Nothing
        If reader.Name = "EventNumber" Then EventNumber = Long.Parse(reader.ReadElementContentAsString()) : reader.Read() Else EventNumber = Nothing
        If reader.Name = "EventCategory" Then EventCategory = reader.ReadElementContentAsString() : reader.Read() Else EventCategory = Nothing
        If reader.Name = "EventLevel" Then EventLevel = reader.ReadElementContentAsString() : reader.Read() Else EventLevel = Nothing
        If reader.Name = "UserName" Then UserName = reader.ReadElementContentAsString() : reader.Read() Else UserName = Nothing
        If reader.Name = "RawDescription" Then RawDescription = reader.ReadElementContentAsString() : reader.Read() Else RawDescription = Nothing
        If reader.Name = "LCID" Then LCID = reader.ReadElementContentAsString() : reader.Read() Else LCID = Nothing
        If reader.Name = "CollectDescription" Then CollectDescription = reader.ReadElementContentAsString() : reader.Read() Else CollectDescription = Nothing
        If reader.Name = "Params" Then Params = reader.ReadElementContentAsString() : reader.Read() Else Params = Nothing
        If reader.Name = "EventData" Then EventData = reader.ReadOuterXml() : reader.Read() Else EventData = Nothing 'This could use a re-vist to better get the data.
        If reader.Name = "EventDisplayNumber" Then EventDisplayNumber = Integer.Parse(reader.ReadElementContentAsString()) : reader.Read() Else EventDisplayNumber = Nothing
        If reader.Name = "EventDescription" Then EventDescription = reader.ReadElementContentAsString() : reader.Read() Else EventDescription = Nothing
        If reader.Name = "CorrelationActivityId" Then CorrelationActivityId = Guid.Parse(reader.ReadElementContentAsString()) : reader.Read() Else CorrelationActivityId = Nothing
        If reader.Name = "CorrelationRelatedActivityId" Then CorrelationRelatedActivityId = Guid.Parse(reader.ReadElementContentAsString()) : reader.Read() Else CorrelationRelatedActivityId = Nothing
        If reader.Name = "Keywords" Then Keywords = reader.ReadElementContentAsString() : reader.Read() Else Keywords = Nothing
        If reader.Name = "ProcessId" Then ProcessId = Integer.Parse(reader.ReadElementContentAsString()) : reader.Read() Else ProcessId = Nothing
        If reader.Name = "ThreadId" Then ThreadId = Integer.Parse(reader.ReadElementContentAsString()) : reader.Read() Else ThreadId = Nothing
        reader.Dispose()
    End Sub

End Structure
