
Namespace BitLy


    ''' <summary>
    ''' Classe para gerar e manipular URL encurtadas pelo Bit.Ly
    ''' </summary>
    Public Class BitLy

        Public Property Login As String
        Public Property API_KEY As String

        Public NotInheritable Class ShortUrl

            Public Property status_code As Integer
            Public Property status_txt As String
            Public Property data As ShortUrlData

            ''' <summary>
            ''' Data da URL Encurtada
            ''' </summary>
            NotInheritable Class ShortUrlData
                Public Property long_url As String
                Public Property url As String
                Public Property hash As String
                Public Property global_hash As String
                Public Property new_hash As Integer
            End Class
        End Class
        ''' <summary>
        ''' Inicializa a API do Bit Ly com o Login e a Key
        ''' </summary>
        ''' <param name="Login">Login da sua conta do Bit.y (http://bit.ly/account/your_api_key).</param>
        ''' <param name="ApiKey">API key da sua conta do Bit.ly (http://bit.ly/account/your_api_key).</param>
        Public Sub New(Login As String, ApiKey As String)
            Me.Login = Login
            Me.API_KEY = ApiKey
        End Sub

        ''' <summary>Encurta a URL usando bit.ly</summary>
        ''' <param name="longUrl">URL Longa</param>
        ''' <param name="addHistory">Salva a URL no histórico da conta</param>
        ''' <returns>Um Objet SHortUrl do Bit.Ly</returns>
        Public Function GetShortURL(LongUrl As String, Optional AddHistory As Boolean = True, Optional Domain As String = "bit.ly") As BitLy.ShortUrl
            Return AJAX.Request(Of BitLy.ShortUrl)("http://api.bit.ly/v3/shorten?login=" & Me.Login & "&apiKey=" & Me.API_KEY & "&uri=" & LongUrl & "&domain=" & Domain & "&history=" & If(AddHistory, 1, 0) & "&format=json")
        End Function

        ''' <summary>Encurta a URL usando bit.ly</summary>
        ''' <param name="longUrl">URL Longa</param>
        ''' <param name="addHistory">Salva a URL no histórico da conta</param>
        ''' <returns>Um Objet SHortUrl do Bit.Ly</returns>
        Public Function GetShortURL(LongUrl As Uri, Optional AddHistory As Boolean = True, Optional Domain As String = "bit.ly") As BitLy.ShortUrl
            Return GetShortURL(LongUrl.AbsoluteUri, AddHistory, Domain)
        End Function
    End Class

End Namespace