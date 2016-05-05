Imports System.Reflection
Imports System.IO

Public Class LibraryLoader
    Dim curAsm As Assembly
    Public Sub New(ThisAssembly As Assembly, AutoLoad As Boolean)
        curAsm = ThisAssembly
        If AutoLoad Then
            LoadLibrariesFromResources()
        End If
    End Sub
    Private Function LoadLibrary(ModName As String, ResourceName As String) As Boolean
        P2PLogging.WriteToEventLogInfo("Loading Resource " + ResourceName)
        Dim ba As Byte() = Nothing
        Dim LoadedModule As [Module]

        Using stm As Stream = curAsm.GetManifestResourceStream(ResourceName)
            ' Either the file is not existed or it is not mark as embedded resource
            If stm Is Nothing Then
                P2PLogging.WriteToEventLogError(ResourceName & Convert.ToString(" is not found in Embedded Resources."))
            End If

            ' Get byte[] from the file from embedded resource
            ba = New Byte(CInt(stm.Length) - 1) {}
            stm.Read(ba, 0, CInt(stm.Length))

            LoadedModule = curAsm.LoadModule(ModName, ba)

        End Using
        If LoadedModule.Name = ModName Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub LoadLibrariesFromResources()
        Try
            If LoadLibrary("Elasticsearch.Net", "ElasticSearchManagedModules.Elasticsearch.Net.dll") Then
                P2PLogging.WriteToEventLogInfo("Loaded Elasticsearch.Net successfully")
            Else
                P2PLogging.WriteToEventLogError("Failed to Load Elasticsearch.Net successfully")
            End If

            If LoadLibrary("Nest", "ElasticSearchManagedModules.Nest.dll") Then
                P2PLogging.WriteToEventLogInfo("Loaded Nest successfully")
            Else
                P2PLogging.WriteToEventLogError("Failed to Load Nest successfully")
            End If

            If LoadLibrary("Newtonsoft.Json", "ElasticSearchManagedModules.Newtonsoft.Json.dll") Then
                P2PLogging.WriteToEventLogInfo("Loaded Newtonsoft.Json successfully")
            Else
                P2PLogging.WriteToEventLogError("Failed to Load Newtonsoft.Json successfully")
            End If
        Catch ex As Exception
            P2PLogging.LogErrorDetailsShare("Something went completly wrong when trying to Load Modules", ex)
        End Try

    End Sub
End Class
