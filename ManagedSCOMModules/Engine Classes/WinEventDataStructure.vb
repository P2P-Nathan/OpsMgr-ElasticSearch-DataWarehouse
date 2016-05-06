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

    Public Sub New(ByVal dataitem As DataItemBase)
        EventDate = dataitem.DateCreated
        SourceHealthServiceID = dataitem.SourceHealthServiceId
        Dim reader As XmlReader = dataitem.GetItemXml()
        Do While reader.Name <> "EventOriginId" And reader.Name IsNot Nothing
            reader.Read()
        Loop
        If reader.Name = "EventOriginId" Then EventOriginID = Guid.Parse(reader.ReadElementContentAsString()) Else EventOriginID = Nothing
        If reader.Name = "PublisherId" Then PublisherId = Guid.Parse(reader.ReadElementContentAsString()) Else PublisherId = Nothing
        If reader.Name = "PublisherName" Then PublisherName = reader.ReadElementContentAsString() Else PublisherName = Nothing
        If reader.Name = "EventSourceName" Then EventSourceName = reader.ReadElementContentAsString() Else EventSourceName = Nothing
        If reader.Name = "Channel" Then Channel = reader.ReadElementContentAsString() Else Channel = Nothing
        If reader.Name = "LoggingComputer" Then LoggingComputer = reader.ReadElementContentAsString() Else LoggingComputer = Nothing
        If reader.Name = "EventNumber" Then EventNumber = Long.Parse(reader.ReadElementContentAsString()) Else EventNumber = Nothing
        If reader.Name = "EventCategory" Then EventCategory = reader.ReadElementContentAsString() Else EventCategory = Nothing
        If reader.Name = "EventLevel" Then EventLevel = reader.ReadElementContentAsString() Else EventLevel = Nothing
        If reader.Name = "UserName" Then UserName = reader.ReadElementContentAsString() Else UserName = Nothing
        If reader.Name = "RawDescription" Then RawDescription = reader.ReadElementContentAsString() Else RawDescription = Nothing
        If reader.Name = "LCID" Then LCID = reader.ReadElementContentAsString() Else LCID = Nothing
        If reader.Name = "CollectDescription" Then CollectDescription = reader.ReadElementContentAsString() Else CollectDescription = Nothing
        If reader.Name = "Params" Then Params = reader.ReadOuterXml() Else Params = Nothing
        If reader.Name = "EventData" Then EventData = reader.ReadOuterXml() Else EventData = Nothing 'This could use a re-vist to better get the data.
        If reader.Name = "EventDisplayNumber" Then EventDisplayNumber = Integer.Parse(reader.ReadElementContentAsString()) Else EventDisplayNumber = Nothing
        If reader.Name = "EventDescription" Then EventDescription = reader.ReadElementContentAsString() Else EventDescription = Nothing
        If reader.Name = "CorrelationActivityId" Then CorrelationActivityId = Guid.Parse(reader.ReadElementContentAsString()) Else CorrelationActivityId = Nothing
        If reader.Name = "CorrelationRelatedActivityId" Then CorrelationRelatedActivityId = Guid.Parse(reader.ReadElementContentAsString()) Else CorrelationRelatedActivityId = Nothing
        If reader.Name = "Keywords" Then Keywords = reader.ReadElementContentAsString() Else Keywords = Nothing
        If reader.Name = "ProcessId" Then ProcessId = Integer.Parse(reader.ReadElementContentAsString()) Else ProcessId = Nothing
        If reader.Name = "ThreadId" Then ThreadId = Integer.Parse(reader.ReadElementContentAsString()) Else ThreadId = Nothing
        reader.Dispose()
    End Sub

End Structure
