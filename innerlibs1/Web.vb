Imports System.Collections.Specialized
Imports System.Drawing
Imports System.IO
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.SessionState
Imports System.Web.UI
Imports System.Xml

''' <summary>
''' Métodos de requisição
''' </summary>
Public NotInheritable Class AJAX

    ''' <summary>
    ''' Retorna o conteúdo de uma página
    ''' </summary>
    ''' <param name="URL">URL de requisiçao</param>
    ''' <param name="Parameters">Parametros da URL</param>
    ''' <param name="ContentType">Conteudo</param>
    ''' <param name="Encoding">Codificação</param>
    ''' <param name="FilePath">Caminho do arquivo</param>
    ''' <returns>conteudo no formato especificado</returns>
    Public Shared Function Request(Of Type)(URL As String, Optional Parameters As NameValueCollection = Nothing, Optional ContentType As String = "application/x-www-form-urlencoded", Optional Encoding As Encoding = Nothing, Optional FilePath As String = "") As Object
        If URL.IsURL Then
            Using client As New WebClient()
                client.Encoding = If(Encoding, Encoding.UTF8)
                client.Headers(HttpRequestHeader.ContentType) = ContentType
                Dim responsebytes As Byte()
                If IsNothing(Parameters) Then
                    responsebytes = client.DownloadData(URL)
                Else
                    responsebytes = client.UploadValues(URL, Parameters)
                End If
                Select Case GetType(Type)
                    Case GetType(String)
                        Return client.Encoding.GetString(responsebytes)
                    Case GetType(FileInfo)
                        Return responsebytes.WriteToFile(FilePath)
                    Case GetType(Byte)
                        Return responsebytes
                    Case GetType(XmlDocument)
                        Dim ms As New MemoryStream(responsebytes)
                        Dim x As New XmlDocument
                        x.Load(ms)
                        Return x
                    Case GetType(Image), GetType(Bitmap)
                        Dim ms As New MemoryStream(responsebytes)
                        Return Image.FromStream(ms)
                    Case Else
                        Return client.Encoding.GetString(responsebytes).ParseJSON(Of Type)
                End Select
            End Using
        Else
            Throw New Exception("URL inválida")
            Return Nothing
        End If
    End Function



    ''' <summary>
    ''' Realiza um POST em uma URL e retorna um Objeto convertido para o tipo especificado
    ''' </summary>
    ''' <typeparam name="Type">Classe do Tipo</typeparam>
    ''' <param name="URL">URL do Post</param>
    ''' <param name="Parameters">Parametros da URL</param>
    ''' <param name="ContentType">Tipo de conteudo que está sendo enviado</param>
    ''' <returns></returns>
    Public Shared Function POST(Of Type)(URL As String, Parameters As NameValueCollection, Optional ContentType As String = "multipart/form-data", Optional Encoding As Encoding = Nothing) As Type
        Return AJAX.Request(Of Type)(URL, Parameters, ContentType, Encoding)
    End Function

    ''' <summary>
    ''' Realiza um GET em uma URL
    ''' </summary>
    ''' <param name="URL">URL do Post</param>
    ''' <returns></returns>
    Public Shared Function [GET](Of Type)(URL As String, Optional Encoding As Encoding = Nothing) As Type
        Return AJAX.Request(Of Type)(URL, Nothing, "application/x-www-form-urlencoded", Encoding)
    End Function

    ''' <summary>
    ''' Faz o download de um arquivo diretamente em um diretório
    ''' </summary>
    ''' <param name="URL">URL de requisiçao</param>
    ''' <param name="Parameters">Parametros da URL</param>
    ''' <param name="ContentType">Conteudo</param>
    ''' <param name="Encoding">Codificação</param>
    ''' <param name="FilePath">Caminho do arquivo</param>
    ''' <returns></returns>
    Public Shared Function DownloadFile(URL As String, FilePath As String, Optional Parameters As NameValueCollection = Nothing, Optional ContentType As String = "multipart/form-data", Optional Encoding As Encoding = Nothing) As FileInfo
        Return AJAX.Request(Of FileInfo)(URL, Parameters, ContentType, Encoding, FilePath)
    End Function

    ''' <summary>
    ''' Template de resposta de requisiçoes ajax. Facilita respostas de RestAPI em JSON
    ''' </summary>
    Public Class Response
        ''' <summary>
        ''' Status da requisicao
        ''' </summary>
        ''' <returns></returns>
        Property status As String
        ''' <summary>
        ''' Mensagem retornada ao ciente
        ''' </summary>
        ''' <returns></returns>
        Property message As String
        ''' <summary>
        ''' Objeto adicionado a resposta, ele será serializado em JSON
        ''' </summary>
        ''' <returns></returns>
        Property response As Object

        Public Sub New()

        End Sub

        Public Sub New(Status As String)
            Me.status = Status
        End Sub

        Public Sub New(Status As String, Message As String)
            Me.status = Status
            Me.message = Message
        End Sub

        Public Sub New(Status As String, Message As String, Response As Object)
            Me.status = Status
            Me.message = Message
            Me.response = Response
        End Sub

        ''' <summary>
        ''' Retorna um JSON deste objeto
        ''' </summary>
        ''' <returns></returns>
        Public Function ToJSON()
            Return Me.SerializeJSON
        End Function
    End Class
End Class




''' <summary>
''' Modulo Web
''' </summary>
''' <remarks></remarks>
''' 
Public Module Web

    ''' <summary>
    ''' Cria um objeto a partir de uma requisiçao AJAX
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="TheObject">Objeto</param>
    ''' <param name="URL">URL de requisiçao</param>
    ''' <param name="Parameters">Parametros da URL</param>
    ''' <param name="ContentType">Conteudo</param>
    ''' <param name="Encoding">Codificação</param>
    <Extension()>
    Public Sub CreateFromAjax(Of Type)(ByRef TheObject As Type, URL As String, Optional Parameters As NameValueCollection = Nothing, Optional ContentType As String = "application/x-www-form-urlencoded", Optional Encoding As Encoding = Nothing)
        TheObject = AJAX.Request(Of Type)(URL, Parameters, ContentType, Encoding)
    End Sub


    ''' <summary>
    ''' Destroi a Sessão e todos os cookies de uma aplicação ASP.NET
    ''' </summary>
    ''' <param name="Page">Pagina atual</param>
    <Extension()>
    Public Function DestroySessionAndCookies(Page As Page) As String
        Try
            For Each key As String In Page.Request.Cookies.AllKeys
                Dim c = Page.Request.Cookies(key)
                c.Expires = Now.AddMonths(-1)
                Page.Response.AppendCookie(c)
            Next
            Page.Session.Abandon()
            Return "OK"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    ''' <summary>
    ''' Cria um cookie guardando valores especificos da sessão atual (1 dia de duração)
    ''' </summary>
    ''' <param name="Session">Sessão</param>
    ''' <param name="CookieName">Nome do Cookie</param>
    ''' <param name="SessionKeys">As keys especificas que você quer guardar</param>
    ''' <returns>Um cookie com os valores da sessão</returns>
    <Extension>
    Public Function ToCookie(Session As HttpSessionState, CookieName As String, ParamArray SessionKeys As String()) As HttpCookie
        Dim Cookie As HttpCookie = New HttpCookie(CookieName)
        For Each item In SessionKeys
            Cookie(item) = Session(item)
        Next
        Cookie.Expires = Tomorrow
        Return Cookie
    End Function

    ''' <summary>
    ''' Cria um cookie guardando valores especificos da sessão atual
    ''' </summary>
    ''' <param name="Session">Sessão</param>
    ''' <param name="CookieName">Nome do Cookie</param>
    ''' <param name="Expires">Data de expiração</param>
    ''' <param name="SessionKeys">As keys especificas que você quer guardar</param>
    ''' <returns>Um cookie com os valores da sessão</returns>
    <Extension>
    Public Function ToCookie(Session As HttpSessionState, CookieName As String, Expires As DateTime, ParamArray SessionKeys As String()) As HttpCookie
        Dim Cookie As HttpCookie = New HttpCookie(CookieName)
        For Each item In SessionKeys
            Cookie(item) = Session(item)
        Next
        Cookie.Expires = Expires
        Return Cookie
    End Function

    ''' <summary>
    ''' Cria um cookie guardando todos os valores da sessão atual
    ''' </summary>
    ''' <param name="Session">Sessão</param>
    ''' <param name="CookieName">Nome do Cookie</param>
    ''' <param name="Expires">Data de expiração</param>
    ''' <returns>Um cookie com os valores da sessão</returns>
    <Extension()>
    Public Function ToCookie(Session As HttpSessionState, Optional CookieName As String = "", Optional Expires As DateTime = Nothing) As HttpCookie
        Dim Keys As New List(Of String)
        For i = 0 To Session.Contents.Count - 1
            Keys.Add(Session.Keys(i).ToString())
        Next
        Return Session.ToCookie(CookieName.IsNull(My.Application.Info.AssemblyName), If(Expires = Nothing, Now.AddDays(1), Expires), Keys.ToArray)
    End Function

    ''' <summary>
    ''' Adciona um parametro a Query String de uma URL
    ''' </summary>
    ''' <param name="Url">Uri</param>
    ''' <param name="Key">Nome do parâmetro</param>
    ''' <param name="Value">Valor do Parâmetro</param>
    ''' <returns></returns>
    <Extension>
    Public Function AddParameter(Url As Uri, Key As String, Value As String) As Uri
        Dim uriBuilder = New UriBuilder(Url)
        Dim query = HttpUtility.ParseQueryString(uriBuilder.Query)
        query(Key) = Value
        uriBuilder.Query = query.ToString()
        Return New Uri(uriBuilder.ToString())
    End Function



    ''' <summary>
    ''' Monta um Comando SQL para executar uma procedure especifica e trata parametros espicificos de uma URL como parametros da procedure
    ''' </summary>
    ''' <param name="Request">Requisicao HTTP</param>
    ''' <param name="ProcedureName">Nome da Procedure</param>
    ''' <param name="QueryStringKeys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>

    <Extension()>
    Public Function ToProcedure(Request As HttpRequest, ByVal ProcedureName As String, ParamArray QueryStringKeys() As String) As String
        ProcedureName = "EXEC " & ProcedureName & " "
        For Each Key In QueryStringKeys
            If Key.IsNotBlank Then
                ProcedureName.Append("@" & Key & "=" & Request(Key).IsNull(Quotes:=Not Request(Key).IsNumber()) & ", ")
            End If
        Next

        Return ProcedureName.Trim.TrimEnd(",")
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar uma procedure especifica e trata todos os parametros de uma URL como parametros da procedure
    ''' </summary>
    ''' <param name="Request">Requisicao HTTP</param>
    ''' <param name="ProcedureName">Nome da Procedure</param> 
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()>
    Public Function ToProcedure(Request As HttpRequest, ByVal ProcedureName As String) As String
        Return Request.ToProcedure(ProcedureName, Request.QueryString.AllKeys)
    End Function

    ''' <summary>
    ''' Escreve um texto e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="Text">Texto</param>
    <Extension>
    Public Sub WriteEnd(Response As HttpResponse, Text As String)
        Response.Write(Text)
        Response.[End]()
    End Sub

    ''' <summary>
    ''' Escreve um arquivo CSV e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="CSVString">String com o conteudo do arquivo CSV</param>
    ''' <param name="FileName">Nome do arquivo CSV</param>
    <Extension()>
    Public Sub WriteCSV(Response As HttpResponse, CSVString As String, Optional FileName As String = "CSV")
        Response.ContentType = GetFileType(".csv").First
        Response.AppendHeader("content-disposition", "attachment; filename=" & FileName)
        Response.End()
    End Sub

    ''' <summary>
    ''' Escreve um JSON e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="JSON">String JSON</param>
    <Extension>
    Public Sub WriteJSON(Response As HttpResponse, JSON As String)
        Response.ContentType = "application/json"
        Response.WriteEnd(JSON)
    End Sub

    ''' <summary>
    ''' Escreve uma imagem e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="Image">Imagem</param>
    ''' <param name="MimeType">Formato MIME Type</param>
    <Extension()>
    Public Sub WriteImage(Response As HttpResponse, Image As Byte(), MimeType As String)
        Response.Clear()
        Response.ContentType = MimeType
        Response.BinaryWrite(Image)
        Response.End()
    End Sub

    ''' <summary>
    ''' Escreve uma imagem e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="Image">Imagem</param>
    ''' <param name="MimeType">Formato MIME Type</param>
    <Extension()>
    Public Sub WriteImage(Response As HttpResponse, Image As Byte(), MimeType As FileType)
        Response.Clear()
        Response.ContentType = MimeType.ToString
        Response.BinaryWrite(Image)
        Response.End()
    End Sub

    ''' <summary>
    ''' Escreve uma imagem e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="Image">Imagem</param>
    <Extension()>
    Public Sub WriteImage(Response As HttpResponse, Image As Image)
        Response.Clear()
        Response.ContentType = Image.GetFileType.First
        Response.BinaryWrite(Image.ToBytes)
        Response.End()
    End Sub

    ''' <summary>
    ''' Escreve um JSON e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="JSON">Objeto de resposta AJAX</param>
    <Extension>
    Public Sub WriteJSON(Response As HttpResponse, JSON As AJAX.Response)
        Response.ContentType = "application/json"
        Response.WriteEnd(JSON.ToJSON)
    End Sub

    ''' <summary>
    ''' Escreve um JSON e finaliza um HttpResponse
    ''' </summary>
    '''<typeparam name="Type">Tipo de Objeto que será Serializado em JSON</typeparam>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="JSON">Objeto de resposta AJAX</param>
    <Extension>
    Public Sub WriteJSON(Of Type)(Response As HttpResponse, JSON As Type)
        Response.ContentType = "application/json"
        Response.WriteEnd(JSON.SerializeJSON)
    End Sub

    ''' <summary>
    ''' Escreve um XML e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="XML">String XML</param>
    <Extension>
    Public Sub WriteXML(Response As HttpResponse, XML As String)
        Response.ContentType = "application/xml"
        Response.WriteEnd(XML)
    End Sub

    ''' <summary>
    ''' Escreve um XML e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="XML">String XML</param>
    <Extension>
    Public Sub WriteXML(Response As HttpResponse, XML As XmlDocument)
        Response.WriteXML(XML.ToXMLString)
    End Sub

    ''' <summary>
    ''' Escreve um script na página
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="ScriptOrURL">Texto ou URL absoluta do Script</param>
    <Extension>
    Public Sub WriteScript(Response As HttpResponse, ScriptOrURL As String)
        If ScriptOrURL.IsURL Then
            Response.Write("<script src=" & ScriptOrURL.Quote & " ></script>")
        Else
            Response.Write("<script>" + ScriptOrURL + "</script>")
        End If

    End Sub


    ''' <summary>
    ''' Captura o ID de um video do youtube em uma URL
    ''' </summary>
    ''' <param name="URL">URL do video</param>
    ''' <returns>ID do video do youtube</returns>

    Public Function GetYoutubeVideoId(URL As String) As String
        If URL.IsURL Then
            Return Regex.Match(URL.Replace("&feature=youtu.be"), "(?:https?:\/\/)?(?:www\.)?youtu(?:.be\/|be\.com\/watch\?v=|be\.com\/v\/)(.{8,})").Groups(1).Value
        Else
            Throw New Exception("URL Inválida")
        End If
    End Function

    ''' <summary>
    ''' Captura o ID de um video do youtube em uma URL
    ''' </summary>
    ''' <param name="URL">URL do video</param>
    ''' <returns>ID do video do youtube</returns>
    <Extension>
    Public Function GetYoutubeVideoId(URL As Uri) As String
        Return GetYoutubeVideoId(URL.AbsoluteUri)
    End Function


    ''' <summary>
    ''' Verifica se um site está indisponível usando o serviço IsUp.Me
    ''' </summary>
    ''' <param name="Url">Url</param>
    ''' <returns>True para site fora do Ar</returns>

    <Extension>
    Public Function IsDown(Url As String) As Boolean
        Dim content As String = AJAX.Request(Of String)("http://downforeveryoneorjustme.com/" & Url)
        If content.Contains("It's just you") Then
            Return False
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' Verifica se um site está disponível usando o serviço IsUp.Me
    ''' </summary>
    ''' <param name="Url">Url</param>
    ''' <returns>False para site fora do Ar</returns>

    <Extension>
    Public Function IsUp(Url As String) As Boolean
        Return Not Url.IsDown()
    End Function

    ''' <summary>
    ''' Verifica se um site está indisponível usando o serviço IsUp.Me
    ''' </summary>
    ''' <param name="Url">Url</param>
    ''' <returns>True para site fora do Ar</returns>

    Public Function IsDown(Url As Uri) As Boolean
        Return Url.AbsoluteUri.IsDown()
    End Function

    ''' <summary>
    ''' Verifica se um site está disponível usando o serviço IsUp.Me
    ''' </summary>
    ''' <param name="Url">Url</param>
    ''' <returns>False para site fora do Ar</returns>

    Public Function IsUp(Url As Uri) As Boolean
        Return Url.AbsoluteUri.IsUp()
    End Function


End Module