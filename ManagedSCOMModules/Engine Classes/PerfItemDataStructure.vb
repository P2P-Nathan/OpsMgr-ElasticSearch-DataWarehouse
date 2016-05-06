Imports Microsoft.EnterpriseManagement.HealthService
Imports System.Xml
Imports System.Xml.XPath

Public Structure PerfItemDataStructure
    Public TimeSampled As Date
    Public sourceHealthServiceID As Guid
    Public ComputerName As String
    Public ObjectName As String
    Public CounterName As String
    Public InstanceName As String
    Public IsNull As Boolean
    Public Value As Double
    Public SampleCount As Integer

    Public Sub New(ByVal dataitem As DataItemBase, Optional ByVal CompName As String = "") ', Logger As ETWLogging)
        sourceHealthServiceID = dataitem.SourceHealthServiceId
        'TimeSampled = dataitem.DateCreated
        ComputerName = CompName
        Dim reader As XmlReader = dataitem.GetItemXml()
        Do While reader.Name <> "TimeSampled" And reader.Name IsNot Nothing
            reader.Read()
        Loop
        If reader.Name = "TimeSampled" Then TimeSampled = reader.ReadElementContentAsDateTime() Else ObjectName = Nothing
        If reader.Name = "ObjectName" Then ObjectName = reader.ReadElementContentAsString() Else ObjectName = Nothing
        If reader.Name = "CounterName" Then CounterName = reader.ReadElementContentAsString() Else CounterName = Nothing
        If reader.Name = "InstanceName" Then InstanceName = reader.ReadElementContentAsString() Else InstanceName = Nothing
        If reader.Name = "IsNull" Then IsNull = Boolean.Parse(reader.ReadElementContentAsString()) Else IsNull = Nothing
        If reader.Name = "Value" Then Value = Double.Parse(reader.ReadElementContentAsString()) Else Value = Nothing
        If reader.Name = "SampleCount" Then SampleCount = Integer.Parse(reader.ReadElementContentAsString()) Else SampleCount = Nothing
        reader.Dispose()
    End Sub

End Structure
