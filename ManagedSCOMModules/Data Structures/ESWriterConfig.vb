Imports System.Xml

Friend Structure ParsedConfigData
    Public ESNode As String
    Public WinEvtIndex As String
    Public OtherIndex As String

    Public Sub New(configuration As XmlReader, SkipIndex As Boolean)

        If SkipIndex Then
            Try
                'Try to read our configuration elements 
                Dim XMLConfig As New XmlDocument
                XMLConfig.Load(configuration)
                'Config data is blank with a skip
                ESNode = XMLConfig.SelectSingleNode("/Configuration/ElasticSearchNode").InnerText
                WinEvtIndex = ""
                OtherIndex = ""
            Catch ex As Exception

                Throw New Exception("We cant's start, configuration could not be parsed.")
            End Try
        Else
            Try
                'Try to read our configuration elements 
                Dim XMLConfig As New XmlDocument
                XMLConfig.Load(configuration)

                'Config data is pulled via Xpath, like in MPs
                ESNode = XMLConfig.SelectSingleNode("/Configuration/ElasticSearchNode").InnerText
                WinEvtIndex = XMLConfig.SelectSingleNode("/Configuration/WinEventIndex").InnerText
                OtherIndex = XMLConfig.SelectSingleNode("/Configuration/AllOtherIndex").InnerText
            Catch ex As Exception

                Throw New Exception("We cant's start, configuration could not be parsed.")
            End Try
        End If



    End Sub



End Structure