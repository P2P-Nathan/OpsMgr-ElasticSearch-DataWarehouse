Imports Microsoft.EnterpriseManagement.HealthService

Public Class ElasticDocEncoder
    Private ConfigData As JSONEncoderConfig
    Private Logger As P2PMapLogging
    Private reader As Xml.XmlReader
    Private IndexSuffix As String
    Private XMLReaderDoc As New Xml.XmlDocument

    Public Sub New(Conf As JSONEncoderConfig, LoggerIn As P2PMapLogging)
        ConfigData = Conf
        Logger = LoggerIn
    End Sub

    Friend Function EncodeSingleInsert(dataItem As DataItemBase) As String
        'get the whole input DataItem
        Dim TypedItem As Object
        Dim DataItemType As String
        Dim IndexHeaderJSON As Text.StringBuilder

        'Pull Type to String to reduce Xml calls
        DataItemType = dataItem.DataItemTypeName

        'Cast the item and get an Index Header
        TypedItem = GetTypeCastedDataItem(dataItem, DataItemType)
        IndexHeaderJSON = GetIndexHeader(DataItemType)

        IndexHeaderJSON.Append(Newtonsoft.Json.JsonConvert.SerializeObject(TypedItem))
        IndexHeaderJSON.AppendLine()
        Return IndexHeaderJSON.ToString()

    End Function

    Friend Sub SetIndexSuffix(NewSuffix As String)
        IndexSuffix = NewSuffix.ToLower().Replace(" ", "_")
    End Sub
    Private Function GetIndexHeader(dataItemType As String) As Text.StringBuilder
        Dim SB As New Text.StringBuilder
        SB.EnsureCapacity(300)
        SB.Append("{""index"":{""_index"":""")
        If dataItemType = "System.Performance.Data" Then
            SB.Append(ConfigData.PerfItemPrefix)
            SB.Append(IndexSuffix)
            SB.Append(""",""_type"":""winperf""}}")
        ElseIf dataItemType = "Microsoft.Windows.EventData" Then
            SB.Append(ConfigData.WinEventPrefix)
            SB.Append(IndexSuffix)
            SB.Append(""",""_type"":""eventdata""}}")
        Else
            SB.Append(ConfigData.OtherItemPrefix)
            SB.Append(IndexSuffix)
            SB.Append(""",""_type"":""unknown""}}")
        End If
        SB.AppendLine()
        Return SB
    End Function

    Private Function GetTypeCastedDataItem(ByVal dataItem As DataItemBase, ByVal DataItemType As String) As Object
        If DataItemType = "System.Performance.Data" Then
            Try
                Return New PerfItemDataStructure(dataItem, ConfigData.ComputerName)
            Catch ex As Exception
                'Maybe it was nothing?
                Logger.LogErrorDetails("Failed to Created Perf Item", ex)
                Return Nothing
            End Try
        ElseIf DataItemType = "Microsoft.Windows.EventData" Then
            Try
                Return New WinEventDataStructure(dataItem)
            Catch ex As Exception
                'Maybe it was nothing?
                Logger.LogErrorDetails("Failed to Created Windows Event", ex)
                Return Nothing
            End Try
        Else
            Try
                Return New AllOtherDataStructure(dataItem, XMLReaderDoc)
            Catch ex As Exception
                'Maybe it was nothing?
                Logger.LogErrorDetails("Failed to Created Other type item", ex)
                Return Nothing
            End Try
        End If
    End Function
End Class
