Public Class P2PMapLogging
    Const _AppName As String = "WriteToES_SCOMModule"
    Const _AppVer As String = "1.2.0.1"

    Public Sub New()
        WriteTrace("Created New ETW JSON Encoder")
    End Sub

    Public Sub WriteTrace(TraceDetails As String)
        WriteToEventLogInfo(TraceDetails)
    End Sub

    Public Sub SubmittedBatches(BatchCount As Integer)
        WriteToEventLogInfo(BatchCount.ToString())
    End Sub

    'Public Sub StartTimer(TimerName As String)
    '    ESTimer.TimerStarted(_AppName, _AppVer, TimerName, Now.Ticks.ToString())
    'End Sub

    'Public Sub StopTimer(TimerName As String)
    '    ESTimer.TimerStopped(_AppName, _AppVer, TimerName, Now.Ticks.ToString())
    'End Sub

    Public Sub ModuleActivity(ActivtyDetails As String)
        WriteToEventLogInfo(ActivtyDetails)
    End Sub

    Public Sub WriteInformation(InfoDetails As String)
        WriteToEventLogInfo(InfoDetails)
    End Sub

    Public Sub WriteError(ErrorDetails As String)
        WriteToEventLogError(ErrorDetails)
    End Sub

    Public Sub WriteWarning(WarningDetails As String)
        WriteToEventLogError(WarningDetails)
    End Sub

    Public Sub LogErrorDetails(ByVal ErrorDetails As String, Optional ByVal ErrorObject As Exception = Nothing)
        If ErrorObject IsNot Nothing Then
            WriteToEventLogError((ErrorDetails + " :: " + ErrorObject.Message))
        Else
            WriteToEventLogError(ErrorDetails)
        End If


        If ErrorObject IsNot Nothing Then
            If ErrorObject.InnerException IsNot Nothing Then
                Dim ErrDetailsStr As String = ("The Inner Exception was: " + ErrorObject.InnerException.Message)
                WriteToEventLogError(ErrDetailsStr)
            End If
        End If
    End Sub

    Public Shared Sub WriteToEventLogError(ByVal Entry As String)
        Dim appName As String = "P2P Encoder Module"
        Dim eventType As EventLogEntryType = EventLogEntryType.Error
        Dim logName As String = "P2P Encoder"
        Dim objEventLog As New EventLog()
        If Not EventLog.SourceExists(appName) Then
            EventLog.CreateEventSource(appName, logName)
        End If
        objEventLog.Source = appName
        objEventLog.WriteEntry(Entry, eventType)
    End Sub

    Public Shared Sub WriteToEventLogInfo(ByVal Entry As String)
        Dim appName As String = "P2P Encoder Module"
        Dim eventType As EventLogEntryType = EventLogEntryType.Information
        Dim logName As String = "P2P Encoder"
        Dim objEventLog As New EventLog()
        If Not EventLog.SourceExists(appName) Then
            EventLog.CreateEventSource(appName, logName)
        End If
        objEventLog.Source = appName
        objEventLog.WriteEntry(Entry, eventType)
    End Sub

End Class