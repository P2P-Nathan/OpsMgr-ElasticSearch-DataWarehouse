Public Class NoiseLogger
    Private HenryIV As New EventLog("MyNoisyApp", ".", "Henry IV")
    Private HenryVIPart1 As New EventLog("MyNoisyApp", ".", "Henry VI Part 1")
    Private HenryVIPart2 As New EventLog("MyNoisyApp", ".", "Henry VI Part 2")
    Private HenryVIPart3 As New EventLog("MyNoisyApp", ".", "Henry VI Part 3")
    Private Allswellthatendswell As New EventLog("MyNoisyApp", ".", "Alls well that ends well")
    Private Asyoulikeit As New EventLog("MyNoisyApp", ".", "As you like it")
    Private AntonyandCleopatra As New EventLog("MyNoisyApp", ".", "Antony and Cleopatra")
    Private AComedyofErrors As New EventLog("MyNoisyApp", ".", "A Comedy of Errors")
    Private Coriolanus As New EventLog("MyNoisyApp", ".", "Coriolanus")
    Private Cymbeline As New EventLog("MyNoisyApp", ".", "Cymbeline")
    Private Hamlet As New EventLog("MyNoisyApp", ".", "Hamlet")
    Private HenryV As New EventLog("MyNoisyApp", ".", "Henry V")
    Private HenryVIII As New EventLog("MyNoisyApp", ".", "Henry VIII")
    Private KingJohn As New EventLog("MyNoisyApp", ".", "King John")
    Private JuliusCaesar As New EventLog("MyNoisyApp", ".", "Julius Caesar")
    Private KingLear As New EventLog("MyNoisyApp", ".", "King Lear")
    Private LovesLaboursLost As New EventLog("MyNoisyApp", ".", "Loves Labours Lost")
    Private macbeth As New EventLog("MyNoisyApp", ".", "macbeth")
    Private Measureformeasure As New EventLog("MyNoisyApp", ".", "Measure for measure")
    Private MerchantofVenice As New EventLog("MyNoisyApp", ".", "Merchant of Venice")
    Private MerryWivesofWindsor As New EventLog("MyNoisyApp", ".", "Merry Wives of Windsor")
    Private AMidsummernightsdream As New EventLog("MyNoisyApp", ".", "A Midsummer nights dream")
    Private MuchAdoaboutnothing As New EventLog("MyNoisyApp", ".", "Much Ado about nothing")
    Private Othello As New EventLog("MyNoisyApp", ".", "Othello")
    Private Pericles As New EventLog("MyNoisyApp", ".", "Pericles")
    Private RichardII As New EventLog("MyNoisyApp", ".", "Richard II")
    Private RichardIII As New EventLog("MyNoisyApp", ".", "Richard III")
    Private RomeoandJuliet As New EventLog("MyNoisyApp", ".", "Romeo and Juliet")
    Private TamingoftheShrew As New EventLog("MyNoisyApp", ".", "Taming of the Shrew")
    Private TheTempest As New EventLog("MyNoisyApp", ".", "The Tempest")
    Private TimonofAthens As New EventLog("MyNoisyApp", ".", "Timon of Athens")
    Private TitusAndronicus As New EventLog("MyNoisyApp", ".", "Titus Andronicus")
    Private TroilusandCressida As New EventLog("MyNoisyApp", ".", "Troilus and Cressida")
    Private TwelfthNight As New EventLog("MyNoisyApp", ".", "Twelfth Night")
    Private TwoGentlemenofVerona As New EventLog("MyNoisyApp", ".", "Two Gentlemen of Verona")
    Private AWintersTale As New EventLog("MyNoisyApp", ".", "A Winters Tale")


    Public Sub Write(appName As String, Entry As String, IDint As Integer)
        Select Case appName
            Case "Henry IV"
                HenryIV.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Henry VI Part 1"
                HenryVIPart1.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Henry VI Part 2"
                HenryVIPart2.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Henry VI Part 3"
                HenryVIPart3.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Alls well that ends well"
                Allswellthatendswell.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "As you like it"
                Asyoulikeit.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Antony and Cleopatra"
                AntonyandCleopatra.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "A Comedy of Errors"
                AComedyofErrors.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Coriolanus"
                Coriolanus.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Cymbeline"
                Cymbeline.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Hamlet"
                Hamlet.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Henry V"
                HenryV.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Henry VIII"
                HenryVIII.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "King John"
                KingJohn.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Julius Caesar"
                JuliusCaesar.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "King Lear"
                KingLear.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Loves Labours Lost"
                LovesLaboursLost.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "macbeth"
                macbeth.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Measure for measure"
                Measureformeasure.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Merchant of Venice"
                MerchantofVenice.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Merry Wives of Windsor"
                MerryWivesofWindsor.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "A Midsummer nights dream"
                AMidsummernightsdream.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Much Ado about nothing"
                MuchAdoaboutnothing.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Othello"
                Othello.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Pericles"
                Pericles.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Richard II"
                RichardII.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Richard III"
                RichardIII.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Romeo and Juliet"
                RomeoandJuliet.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Taming of the Shrew"
                TamingoftheShrew.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "The Tempest"
                TheTempest.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Timon of Athens"
                TimonofAthens.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Titus Andronicus"
                TitusAndronicus.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Troilus and Cressida"
                TroilusandCressida.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Twelfth Night"
                TwelfthNight.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "Two Gentlemen of Verona"
                TwoGentlemenofVerona.WriteEntry(Entry, EventLogEntryType.Information, IDint)
            Case "A Winters Tale"
                AWintersTale.WriteEntry(Entry, EventLogEntryType.Information, IDint)
        End Select
    End Sub

End Class