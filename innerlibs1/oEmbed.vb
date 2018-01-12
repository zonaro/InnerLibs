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
    ''' Cria uma nova <see cref="oEmbed"/> a partir de uma URL utilizando o dominio da mesma como <see cref="EndPoint"/>
    ''' </summary>
    ''' <param name="URL">Url</param>
    Sub New(URL As String)
        If URL.IsURL Then
            Try
                Dim provider As oEmbed = GetProviders().Where(Function(x) x.Provider_url.Contains(URL.GetDomain)).First
                Me.Provider_name = provider.Provider_name
                Me.Provider_url = provider.Provider_url
                Me.Endpoints = provider.Endpoints
                Me.Url = URL
            Catch ex As Exception
                isdefault = True
                Me.Provider_name = GetTitle(URL.GetDomain)
                Me.Provider_url = URL.GetDomain.Prepend("http://")
                Me.Endpoints = {New Endpoint With {
                    .Discovery = False,
                    .Formats = {"json", "xml"},
                    .Schemes = {},
                    .Url = URL
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
    ''' Retorna o um <see cref="Object"/> contendo as informaçoes da URL
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Response As Object
        Get
            Return GetResponse(Of Object)(0)
        End Get
    End Property


    ''' <summary>
    ''' Retorna o um objeto convertido para um tipo contendo as informaçoes da URL
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto (utilize <see cref="Object"/> quando nao houver certeza do formato do Response</typeparam>
    ''' <param name="EndPointIndex">Index do <see cref="Endpoint"/> que será usado. Deixe 0 para utilizar o primeiro</param>
    ''' <returns></returns>
    Public Function GetResponse(Of Type As Class)(Optional EndPointIndex As Integer = 0) As Type

        EndPointIndex = EndPointIndex.LimitRange(0, Me.Endpoints.Count)
        If isdefault Then
            Dim o As New Object
            o("title") = GetTitle(Me.Url)
            o("html") = "<a href=" & Me.Url.Quote & " title=" & o("title").ToString.Quote & " target=""_blank"">" & o("title") & "</a>"
            Return o
        Else
            If Me.Endpoints(EndPointIndex).Url.Contains("{format}") Then
                Return AJAX.GET(Of Type)(Me.Endpoints(EndPointIndex).Url.Replace("{format}", "json") & "?url=" & Me.Url.UrlEncode)
            Else
                Return AJAX.GET(Of Type)(Me.Endpoints(EndPointIndex).Url & "?url=" & Me.Url.UrlEncode & "&format=json")
            End If
        End If
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