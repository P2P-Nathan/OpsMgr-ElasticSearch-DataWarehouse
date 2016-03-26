Imports System.IO
Imports System.Reflection
Imports System.Security.Cryptography

Module Module1
    Sub Main()
        On Error Resume Next

        Dim EventMsgArrList As New List(Of EventMsg)
        Dim EventMsgArrList2 As EventMsg()
        Dim EventMsgArrList3 As EventMsg()

        Dim MyLogger As New NoiseLogger
        Dim MyProcess As Process = Process.GetCurrentProcess()
        MyProcess.PriorityClass = ProcessPriorityClass.Idle

        Console.WriteLine("Welcome!")
        Dim curAsm As Assembly = Assembly.GetExecutingAssembly()
        Dim stm As Stream = curAsm.GetManifestResourceStream("MyNoisyApp.Shakespeare.csv")
        Dim reader = New StreamReader(stm)
        Console.WriteLine("Stream Loaded, Creating List")
        While Not reader.EndOfStream
            EventMsgArrList.Add(New EventMsg(reader.ReadLine()))
        End While
        Console.WriteLine("List Made, Shuffle IT!")
        'We've got Junk loaded!
        ReDim EventMsgArrList2(EventMsgArrList.Count)
        ReDim EventMsgArrList3(EventMsgArrList.Count)

        EventMsgArrList.Shuffle()
        EventMsgArrList.Reverse()
        EventMsgArrList.Reverse(1, New Random().Next(10, EventMsgArrList.Count))
        EventMsgArrList.Shuffle()
        EventMsgArrList.Reverse()
        EventMsgArrList.Reverse(1, New Random().Next(10, EventMsgArrList.Count))
        EventMsgArrList.Shuffle()
        EventMsgArrList.Reverse()
        EventMsgArrList.Reverse(1, New Random().Next(10, EventMsgArrList.Count))
        EventMsgArrList.Shuffle()
        EventMsgArrList.CopyTo(EventMsgArrList2, 0)
        EventMsgArrList.Reverse()
        EventMsgArrList.Reverse(1, New Random().Next(10, EventMsgArrList.Count))
        EventMsgArrList.Shuffle()
        EventMsgArrList.Reverse(1, New Random().Next(10, EventMsgArrList.Count))
        EventMsgArrList.Shuffle()
        EventMsgArrList.Reverse(1, New Random().Next(10, EventMsgArrList.Count))
        EventMsgArrList.Shuffle()
        EventMsgArrList.CopyTo(EventMsgArrList3, 0)
        EventMsgArrList.Shuffle()
        EventMsgArrList.Reverse(1, New Random().Next(10, EventMsgArrList.Count))
        EventMsgArrList.Shuffle()

        Dim Passes As Integer = 0

        While (1 = 1)
            Console.WriteLine("Lets Make Events! - " & Now.ToShortTimeString())
            For I = 0 To EventMsgArrList.Count
                MyLogger.Write(EventMsgArrList(I).Application, EventMsgArrList(I).Text, EventMsgArrList(I).ID)
                MyLogger.Write(EventMsgArrList2(I).Application, EventMsgArrList2(I).Text, EventMsgArrList2(I).ID)
                MyLogger.Write(EventMsgArrList3(I).Application, EventMsgArrList3(I).Text, EventMsgArrList3(I).ID)
                If Passes > 10 Then
                    Threading.Thread.Sleep(25)
                    Passes = 0
                    Console.WriteLine(Now.Second.ToString())
                End If
                Passes += 1
            Next
            Threading.Thread.Sleep(500)
        End While



    End Sub

    <System.Runtime.CompilerServices.Extension> _
    Public Sub Shuffle(Of T)(list As IList(Of T))
        Dim provider As New RNGCryptoServiceProvider()
        Dim n As Integer = list.Count
        While n > 1
            Dim box As Byte() = New Byte(0) {}
            Do
                provider.GetBytes(box)
            Loop While Not (box(0) < n * ([Byte].MaxValue / n))
            Dim k As Integer = (box(0) Mod n)
            n -= 1
            Dim value As T = list(k)
            list(k) = list(n)
            list(n) = value
        End While
    End Sub

End Module
