Imports Nest
Imports Elasticsearch.Net.ConnectionPool
Imports Newtonsoft.Json

Public Class ElasticSearchConnector
    Private ESClient As ElasticClient
    Private Logger As P2PLogging
    Private BulkWriteOperations As New List(Of Task)(10)
    Private WinEventPrefix As String

    Public Sub New(ESNodeAddress As String, ConnectionIndex As String, WinEvtPrefix As String, ETWLog As P2PLogging)
        Logger = ETWLog
        WinEventPrefix = WinEvtPrefix
        Try
            Dim Settings As New ConnectionSettings(New Uri(ESNodeAddress))
            Settings.SniffOnStartup(True)
            Settings.SniffOnConnectionFault(True)
            Settings.SniffLifeSpan(New TimeSpan(0, 45, 0))
            Settings.SetTimeout(10000)
            Settings.SetDefaultIndex((ConnectionIndex.ToLower))
            Settings.SetDeadTimeout(5000)
            Settings.MaximumRetries(1)
            Err.Clear()
            ESClient = New ElasticClient(Settings)
        Catch ex As Exception
            Logger.LogErrorDetails("We Failed to make our connection to: " + ESNodeAddress, ex)
            Throw ex
        End Try
        LogNode(ESClient.NodesInfo())
    End Sub

    Private Sub LogNode(NodeInfo As Nest.INodeInfoResponse) 
        For Each Node As Generic.KeyValuePair(Of String, NodeInfo) In NodeInfo.Nodes
            Dim MsgStr As String
            MsgStr = "Connected To Host: " + Node.Value.Hostname + ". The Transport address is " + Node.Value.TransportAddress + ".  The Process ID of the Running Instance Is: " + Node.Value.Process.Id.ToString()
            Logger.WriteTrace(MsgStr)
        Next
    End Sub

    Public Sub IndexBulkWinEventsParallel(items As WinEventDataStructure(), BatchSize As Integer, MaxConcurrent As Integer)
        'We will split these if there are too many.
        If items.Count > BatchSize Then
            Dim Lists As List(Of WinEventDataStructure)() = GetEqualSizeWinEventGroups(items, BatchSize)
            For Each WinEvtList As List(Of WinEventDataStructure) In Lists
                Dim IndexJson As String = MakeJSONInsertForWinEvents(WinEvtList.ToArray(), WinEventPrefix)
                ProccessBulkAsyncJSON(IndexJson, MaxConcurrent)
            Next
        Else
            Dim IndexJson As String = MakeJSONInsertForWinEvents(items, WinEventPrefix)
            Dim Result As Elasticsearch.Net.ElasticsearchResponse(Of Elasticsearch.Net.DynamicDictionary) = ESClient.Raw.Bulk(IndexJson)
            If Result IsNot Nothing Then
                If Result.Response.Values(1).ToString() = "True" Then
                    'We have Errors
                    Logger.WriteError("Errors were returned from the Bulk index attempt: " + Result.Response.Values(2).ToString.Substring(0, 1000))
                End If
            End If
        End If
    End Sub

    Public Sub ProccessBulkAsyncJSON(JSON As String, MaxConcurrent As Integer)
        If BulkWriteOperations.Count < MaxConcurrent Then
            'We have room to grow
            BulkWriteOperations.Add(BulkAsyncToESAndReport(JSON))
        Else
            'We Need Space
            'Lets Clean out complete tasks
            BulkWriteOperations.RemoveAll(Function(x) x.IsCompleted = True)
            'We need one of these to complete before proceeding, it doesn't need to be cleaned out of the list through
            System.Threading.Tasks.Task.WaitAny(BulkWriteOperations.ToArray())
            BulkWriteOperations.Add(BulkAsyncToESAndReport(JSON))
        End If
    End Sub

    Private Async Function BulkAsyncToESAndReport(JSON As String) As Task
        Try
            Dim R As Task(Of Elasticsearch.Net.ElasticsearchResponse(Of Elasticsearch.Net.DynamicDictionary)) = ESClient.Raw.BulkAsync(JSON)
            'Logger.WriteInformation("Submited Bulk JSON to ElasticSearch")
            Await R
            If R.Result.Response.Values(1).ToString() = "True" Then
                'We have Errors
                Logger.WriteError("We Have Encountered Errors:  " + R.Result.Response.Values(2).ToString.Substring(0, 1000))
            End If
        Catch ex As Exception
            'Did we get through our process?
        End Try


    End Function

    Private Function MakeJSONInsertForWinEvents(WinEvents As WinEventDataStructure(), IndexPrefix As String) As String
        'Logger.WriteTrace("Starting to Generate JSON with IndexPrefix of: " + IndexPrefix)

        Dim SB As New Text.StringBuilder
        SB.EnsureCapacity(WinEvents.Count * 300)
        For Each Doc As WinEventDataStructure In WinEvents
            SB.Append("{""index"":{""_index"":""")
            SB.Append(IndexPrefix.ToLower().Replace(" ", "_"))
            SB.Append(Doc.Channel.ToLower().Replace(" ", "_") + """,""_type"":""")
            SB.Append(Doc.EventSourceName.ToLower().Replace(" ", "_") + """}}")
            SB.AppendLine()
            SB.Append(JsonConvert.SerializeObject(Doc))
            SB.AppendLine()
        Next

        Return SB.ToString()
    End Function

    Private Function GetEqualSizeWinEventGroups(WinEvents As WinEventDataStructure(), GroupSize As Integer) As List(Of WinEventDataStructure)()
        'Get modules to get number of groups
        Dim NumOfGroups As Integer = CInt(Math.Ceiling(WinEvents.Count / GroupSize))

        Dim lists As New List(Of List(Of WinEventDataStructure))
        Dim itemCount As Integer = WinEvents.Count
        Dim maxCount As Integer = CInt(Math.Ceiling(itemCount / NumOfGroups))
        Dim skipCount As Integer = 0

        For number As Integer = NumOfGroups To 1 Step -1
            Dim takeCount As Integer = Math.Min(maxCount, CInt(Math.Ceiling(itemCount / number)))
            lists.Add(WinEvents.Skip(skipCount).Take(takeCount).ToList())
            itemCount -= takeCount
            skipCount += takeCount
        Next
        Return lists.ToArray()

    End Function

    Private Function GetEqualSizePerfItemsGroups(PerfItems As PerfItemDataStructure(), GroupSize As Integer) As List(Of PerfItemDataStructure)()
        'Get modules to get number of groups
        Dim NumOfGroups As Integer = CInt(Math.Ceiling(PerfItems.Count / GroupSize))

        Dim lists As New List(Of List(Of PerfItemDataStructure))
        Dim itemCount As Integer = PerfItems.Count
        Dim maxCount As Integer = CInt(Math.Ceiling(itemCount / NumOfGroups))
        Dim skipCount As Integer = 0

        For number As Integer = NumOfGroups To 1 Step -1
            Dim takeCount As Integer = Math.Min(maxCount, CInt(Math.Ceiling(itemCount / number)))
            lists.Add(PerfItems.Skip(skipCount).Take(takeCount).ToList())
            itemCount -= takeCount
            skipCount += takeCount
        Next
        Return lists.ToArray()

    End Function

    Sub IndexBulkPerfEventsParallel(items As PerfItemDataStructure(), BatchSize As Integer, MaxConCurrent As Integer)
        If items.Count > BatchSize Then
            Dim Lists As List(Of PerfItemDataStructure)() = GetEqualSizePerfItemsGroups(items, BatchSize)
            For Each PerfItemList As List(Of PerfItemDataStructure) In Lists
                Dim IndexJson As String = MakeJSONInsertForPerfItems(PerfItemList.ToArray(), "Performance")
                ProccessBulkAsyncJSON(IndexJson, MaxConCurrent)
            Next
        Else
            Dim IndexJson As String = MakeJSONInsertForPerfItems(items, "Perf")
            ProccessBulkAsyncJSON(IndexJson, MaxConCurrent)
        End If
    End Sub

    Private Function MakeJSONInsertForPerfItems(PerfItemList As PerfItemDataStructure(), PerfItemPrefix As String) As String
        Dim SB As New Text.StringBuilder(PerfItemList.Count * 300)
        For Each Doc As PerfItemDataStructure In PerfItemList
            SB.Append("{""index"":{""_index"":""")
            SB.Append(PerfItemPrefix.ToLower().Replace(" ", "_"))
            SB.Append(""",""_type"":""")
            SB.AppendLine(Doc.ObjectName.ToLower().Replace(" ", "_") + """}}")
            SB.AppendLine(JsonConvert.SerializeObject(Doc))
        Next
        Return SB.ToString()
    End Function

    Sub IndexBulkOtherEventsParallel(OtherItems As AllOtherDataStructure(), BatchSize As Integer, MaxConCurrent As Integer)
        Throw New NotImplementedException
    End Sub

    Protected Overrides Sub Finalize()
        System.Threading.Tasks.Task.WaitAll(BulkWriteOperations.ToArray())
    End Sub

End Class
