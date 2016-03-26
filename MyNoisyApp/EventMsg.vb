Public Class EventMsg
    Public Application As String
    Public ID As Integer
    Public Text As String

    Public Sub New(ByRef Line As String)
        Dim values = Line.Split(",")
        Application = values(0)
        ID = Integer.Parse(values(1))
        Text = values(2)
    End Sub
End Class
