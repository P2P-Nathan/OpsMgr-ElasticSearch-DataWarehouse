Imports Microsoft.EnterpriseManagement.HealthService

Public Class DataItemProcessor
    Public HasWindowsEvents As Boolean = False
    Public HasNONWindowsEvents As Boolean = False
    Public HasPerfItems As Boolean = False
    Private Logger As P2PLogging
    Private WindowsEvents As New List(Of WinEventDataStructure)(20)
    Private PerfEvents As New List(Of PerfItemDataStructure)(40)
    Private AllOtherItems As New List(Of AllOtherDataStructure)(5)
    Private XMLReaderDoc As New Xml.XmlDocument

    Public Sub New(LoggerIn As P2PLogging)
        Logger = LoggerIn
    End Sub

    Public Sub ProcessandLoadData(ByRef InboundDataItems As DataItemBase())

        ''Logger.WriteTrace("Processing New Inbound DataItems as Windows Events")

        For Each DataItem As DataItemBase In InboundDataItems
            If DataItem.DataItemTypeName = "Microsoft.Windows.EventData" Then
                HasWindowsEvents = True
                Try
                    WindowsEvents.Add(New WinEventDataStructure(DataItem))
                Catch ex As Exception
                    'Maybe it was nothing?
                    Logger.LogErrorDetails("Failed to Created Windows Event", ex)
                End Try

            ElseIf DataItem.DataItemTypeName = "System.Performance.Data" Then
                HasPerfItems = True
                Try
                    PerfEvents.Add(New PerfItemDataStructure(DataItem))
                Catch ex As Exception
                    'Maybe it was nothing?
                    Logger.LogErrorDetails("Failed to Created Perf Item", ex)
                End Try
            Else
                HasNONWindowsEvents = True
                Try
                    AllOtherItems.Add(New AllOtherDataStructure(DataItem, XMLReaderDoc))
                Catch ex As Exception
                    'Maybe it was nothing?
                    Logger.LogErrorDetails("Failed to Created Other type item", ex)
                End Try
            End If

        Next

    End Sub


    Public Function GetWinEventsToSubmit() As WinEventDataStructure()
        If WindowsEvents.Count > 0 Then
            Try
                ''Logger.WriteTrace("Retriving Windows Event Collection")
                Return WindowsEvents.ToArray()
            Catch ex As Exception
                Logger.LogErrorDetails("Failed to get WinEventList to Submit.", ex)
                Return Nothing
            End Try
        Else
            'Logger.WriteTrace("Found to EventLogLists to return.")
            Return Nothing
        End If

    End Function

    Public Function GetPerfItemsToSubmit() As PerfItemDataStructure()
        If PerfEvents.Count > 0 Then
            Return PerfEvents.ToArray()
        Else
            Return Nothing
        End If
    End Function

    Public Function GetNonWindowsEvents() As AllOtherDataStructure()
        If AllOtherItems.Count > 0 Then
            Return AllOtherItems.ToArray()
        Else
            Return Nothing
        End If
    End Function

    Sub CleanCollection()
        HasWindowsEvents = False
        HasNONWindowsEvents = False
        HasPerfItems = False
        WindowsEvents.Clear()
        PerfEvents.Clear()
        AllOtherItems.Clear()
    End Sub

End Class
