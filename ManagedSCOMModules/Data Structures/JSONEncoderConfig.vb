Public Structure JSONEncoderConfig
    Friend PerfItemPrefix As String
    Friend WinEventPrefix As String
    Friend OtherItemPrefix As String
    Friend EnableDateRolling As Boolean
    Friend DateRollingFormat As String
    Friend ReturnMultipleItems As Boolean
    Friend ComputerName As String

    Public Sub New(Config As Xml.XmlReader)
        Dim XMLConfig As New Xml.XmlDocument
        XMLConfig.Load(Config)

        'Config data is pulled via Xpath, like in MPs
        PerfItemPrefix = XMLConfig.SelectSingleNode("/Configuration/PerfItemPrefix").InnerText
        WinEventPrefix = XMLConfig.SelectSingleNode("/Configuration/WinEventPrefix").InnerText
        OtherItemPrefix = XMLConfig.SelectSingleNode("/Configuration/OtherItemPrefix").InnerText
        EnableDateRolling = Boolean.Parse(XMLConfig.SelectSingleNode("/Configuration/EnableDateRolling").InnerText)
        DateRollingFormat = XMLConfig.SelectSingleNode("/Configuration/DateRollingFormat").InnerText
        ReturnMultipleItems = Boolean.Parse(XMLConfig.SelectSingleNode("/Configuration/ReturnMultipleItems").InnerText)
        ComputerName = XMLConfig.SelectSingleNode("/Configuration/ComputerName").InnerText
        VerifyAndFixConfig()
    End Sub

    Private Sub VerifyAndFixConfig()
        'Indexes need to be lowercase
        PerfItemPrefix = PerfItemPrefix.ToLower()
        WinEventPrefix = WinEventPrefix.ToLower()
        OtherItemPrefix = OtherItemPrefix.ToLower()

        If EnableDateRolling = False Then
            DateRollingFormat = Nothing
        Else
            'If Date Rolling is true, Lets try to cast it once
            Try
                Dim TestString As String
                TestString = Now.ToString(DateRollingFormat)
            Catch ex As Exception
                'We Failed to Parse this string, fail
                Throw New Exception("Date Rolling Format is Bad")
            End Try
        End If

    End Sub
End Structure
