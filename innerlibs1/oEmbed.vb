Imports System.Xml

''' <summary>
''' Classe para integrar a api oEmbed em aplicações .NET
''' </summary>
Public Class oEmbed

    ''' <summary>
    ''' Retorna uma lista de provedores compativeis com oEmbed
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetProviders() As oEmbed()
        Return Json.DeserializeJSON(Of oEmbed())(GetResourceFileText(Reflection.Assembly.GetExecutingAssembly, "InnerLibs.providers.json"))
    End Function

    ''' <summary>
    ''' Cria uma nova <see cref="oEmbed"/> a partir de uma URL utilizando a mesma como <see cref="EndPoint"/>
    ''' </summary>
    ''' <param name="URL">Url</param>
    Sub New(URL As String)
        If URL.IsURL Then
            Dim provider As oEmbed = GetProviders().Where(Function(x) x.provider_url.Contains(URL.GetDomain)).First
            Me.provider_name = provider.provider_name
            Me.provider_url = provider.provider_url
            Me.endpoints = provider.endpoints
            Me.url = URL
        Else
            Throw New ArgumentException("Invalid URL")
        End If
    End Sub

    ''' <summary>
    ''' Cria uma nova <see cref="oEmbed"/>
    ''' </summary>
    Public Sub New()

    End Sub

    ''' <summary>
    ''' Retorna o um objeto contendo as informaçoes da URL
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="EndPointIndex">Index do <see cref="Endpoint"/> que será usado. Deixe 0 para utilizar o primeiro</param>
    ''' <returns></returns>
    Public Function [Get](Of Type)(Optional EndPointIndex As Integer = 0) As Type
        Dim format As String = "json"
        If GetType(Type) = GetType(XmlDocument) Then
            format = "xml"
        End If
        If Me.endpoints(EndPointIndex).url.Contains("{format}") Then
            Return AJAX.GET(Of Type)(Me.endpoints(EndPointIndex).url.Replace("{format}", format) & "?url=" & Me.url.UrlEncode)
        Else
            Return AJAX.GET(Of Type)(Me.endpoints(EndPointIndex).url & "?url=" & Me.url.UrlEncode & "&format=" & format)
        End If
    End Function

    ''' <summary>
    ''' Informaçoes sobre o endpoint do <see cref="oEmbed"/>
    ''' </summary>
    Public Class Endpoint
        Public Property schemes As String()
        Public Property url As String
        Public Property formats As String()
        Public Property discovery As Boolean?
    End Class

    Public Property provider_name As String
    Public Property provider_url As String
    Public Property endpoints As Endpoint()
    Public Property url As String

End Class