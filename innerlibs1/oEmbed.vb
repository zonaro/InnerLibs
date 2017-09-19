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
        Return AJAX.GET(Of oEmbed())("http://oembed.com/providers.json")
    End Function

    ''' <summary>
    ''' Cria uma nova <see cref="oEmbed"/> a partir de uma URL utilizando a mesma como <see cref="EndPoint"/>
    ''' </summary>
    ''' <param name="URL">Url</param>
    Sub New(URL As String)
        If URL.IsURL Then
            Try
                Dim provider As oEmbed = GetProviders().Where(Function(x) x.provider_url.Contains(URL.GetDomain)).First
                Me.provider_name = provider.provider_name
                Me.provider_url = provider.provider_url
                Me.endpoints = provider.endpoints
                Me.url = URL
            Catch ex As Exception
                isdefault = True
                Me.provider_name = GetTitle(URL.GetDomain)
                Me.provider_url = URL.GetDomain.Prepend("http://")
                Me.endpoints = {New Endpoint With {
                    .discovery = False,
                    .formats = {"json", "xml"},
                    .schemes = {},
                    .url = URL
                }}
            End Try

        Else
            Throw New ArgumentException("Invalid URL")
        End If
    End Sub

    Private isdefault As Boolean = False

    ''' <summary>
    ''' Cria uma nova <see cref="oEmbed"/>
    ''' </summary>
    Public Sub New()

    End Sub

    ''' <summary>
    ''' Retorna o um objeto contendo as informaçoes da URL
    ''' </summary>
    ''' <param name="EndPointIndex">Index do <see cref="Endpoint"/> que será usado. Deixe 0 para utilizar o primeiro</param>
    ''' <returns></returns>
    Public ReadOnly Property Response(Optional EndPointIndex As Integer = 0) As Object
        Get
            EndPointIndex = EndPointIndex.LimitRange(0, Me.endpoints.Count)
            If isdefault Then
                Dim o As New Object
                o("title") = GetTitle(Me.url)
                o("html") = "<a href=" & Me.url.Quote & " target=""_blank"">" & o("title") & "</a>"
                Return o
            Else
                If Me.endpoints(EndPointIndex).url.Contains("{format}") Then
                    Return AJAX.GET(Of Object)(Me.endpoints(EndPointIndex).url.Replace("{format}", "json") & "?url=" & Me.url.UrlEncode)
                Else
                    Return AJAX.GET(Of Object)(Me.endpoints(EndPointIndex).url & "?url=" & Me.url.UrlEncode & "&format=json")
                End If
            End If
        End Get
    End Property


    ''' <summary>
    ''' Retorna o um objeto convertido para um tipo contendo as informaçoes da URL
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="EndPointIndex">Index do <see cref="Endpoint"/> que será usado. Deixe 0 para utilizar o primeiro</param>
    ''' <returns></returns>
    Public Function GetResponse(Of Type)(Optional EndPointIndex As Integer = 0) As Type
        Return CType(Response(EndPointIndex), Type)
    End Function

    ''' <summary>
    ''' Informaçoes sobre o endpoint do <see cref="oEmbed"/>
    ''' </summary>
    Public Class Endpoint
        Public Property Schemes As String()
        Public Property Url As String
        Public Property Formats As String()
        Public Property Discovery As Boolean?
    End Class

    Public Property Provider_name As String
    Public Property Provider_url As String
    Public Property Endpoints As Endpoint()
    Public Property Url As String

End Class