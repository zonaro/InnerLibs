
Imports System.Collections.Specialized
Imports System.Net

Public NotInheritable Class SlackClient

    Public Shared ReadOnly DefaultWebHookUri As New Uri("https://hooks.slack.com/services/.../.../...")

    Private ReadOnly _webHookUri As Uri

    Public Sub New(webHookUri As Uri)
        Me._webHookUri = webHookUri
    End Sub

    Public Sub New(webHookUri As String)
        Me._webHookUri = New Uri(webHookUri)
    End Sub

    Public Function SendSlackMessage(message As SlackMessage) As String
        Using webClient As New WebClient()
            webClient.Headers.Add("Content-Type", "application/json")
            Dim aa As New NameValueCollection
            aa("data") = message.SerializeJSON
            ' ...handle response...
            Return webClient.Encoding.GetString(webClient.UploadValues(Me._webHookUri, "POST", aa))
        End Using
    End Function

    Public NotInheritable Class SlackMessage

        Public Property Channel As String
        Public Property Username
        Public Property Text
        Public Property Icon_emoji As String

    End Class
End Class

