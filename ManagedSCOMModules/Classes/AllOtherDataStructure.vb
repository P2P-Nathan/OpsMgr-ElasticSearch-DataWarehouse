Imports Microsoft.EnterpriseManagement.HealthService
Imports Newtonsoft.Json
Imports System.Xml

Public Structure AllOtherDataStructure
    Public SourceHealthServiceID As Guid
    Public DataItemDate As Date
    Public DataItemTypeName As String
    Public Payload As XmlNode

    Public Sub New(ByRef dataitem As DataItemBase, ByRef XmlDocReader As XmlDocument)
        XmlDocReader.Load(dataitem.GetItemXml())
        SourceHealthServiceID = dataitem.SourceHealthServiceId
        DataItemDate = dataitem.DateCreated
        DataItemTypeName = dataitem.DataItemTypeName
        Payload = XmlDocReader.SelectSingleNode("/DataItem")
    End Sub

End Structure
