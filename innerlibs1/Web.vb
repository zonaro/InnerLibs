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
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Xml

''' <summary>
''' Métodos de requisição
''' </summary>
Public NotInheritable Class AJAX

    ''' <summary>
    ''' Retorna o conteúdo de uma página
    ''' </summary>
    ''' <param name="URL">        URL de requisiçao</param>
    ''' <param name="Parameters"> Parametros da URL</param>
    ''' <param name="ContentType">Conteudo</param>
    ''' <param name="Encoding">   Codificação</param>
    ''' <param name="FilePath">   Caminho do arquivo</param>
    ''' <returns>conteudo no formato especificado</returns>
    Public Shared Function Request(Of Type)(URL As String, Optional Method As String = "GET", Optional Parameters As NameValueCollection = Nothing, Optional ContentType As String = "application/x-www-form-urlencoded", Optional Encoding As Encoding = Nothing, Optional FilePath As String = "") As Object
        If URL.IsURL Then
            Dim client As New WebClient
            client.Encoding = If(Encoding, Encoding.UTF8)
            Using client
                Dim responsebytes As Byte()
                If IsNothing(Parameters) Then
                    responsebytes = client.DownloadData(URL)
                Else
                    responsebytes = client.UploadValues(URL, Method, Parameters)
                End If
                Select Case GetType(Type)
                    Case GetType(String)
                        Return client.Encoding.GetString(responsebytes)
                    Case GetType(Integer), GetType(Long), GetType(Decimal), GetType(Short), GetType(Double)
                        Return client.Encoding.GetString(responsebytes).ChangeType(Of Type)
                    Case GetType(FileInfo)
                        Return responsebytes.WriteToFile(FilePath)
                    Case GetType(Byte), GetType(Byte())
                        Return responsebytes
                    Case GetType(XmlDocument)
                        Using ms As New MemoryStream(responsebytes)
                            Dim x As New XmlDocument
                            x.Load(ms)
                            Return x
                        End Using
                    Case GetType(HtmlParser.HtmlDocument)
                        Return New HtmlParser.HtmlDocument(URL)
                    Case GetType(Drawing.Image), GetType(Bitmap)
                        Using ms As New MemoryStream(responsebytes)
                            Return Drawing.Image.FromStream(ms)
                        End Using
                    Case Else
                        Return client.Encoding.GetString(responsebytes).ParseJSON(Of Type)
                End Select
            End Using
        Else
            Throw New Exception("Invalid URL")
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Realiza um POST em uma URL e retorna um Objeto convertido para o tipo especificado
    ''' </summary>
    ''' <typeparam name="Type">Classe do Tipo</typeparam>
    ''' <param name="URL">        URL do Post</param>
    ''' <param name="Parameters"> Parametros da URL</param>
    ''' <returns></returns>
    Public Shared Function POST(Of Type)(URL As String, Parameters As NameValueCollection, Optional Encoding As Encoding = Nothing) As Type
        Return AJAX.Request(Of Type)(URL, "POST", Parameters, Nothing, Encoding)
    End Function

    ''' <summary>
    ''' Realiza um GET em uma URL
    ''' </summary>
    ''' <param name="URL">URL do Post</param>
    ''' <returns></returns>
    Public Shared Function [GET](Of Type)(URL As String, Optional Encoding As Encoding = Nothing) As Type
        Return AJAX.Request(Of Type)(URL, "GET", Nothing, "application/x-www-form-urlencoded", Encoding)
    End Function

    ''' <summary>
    ''' Faz o download de um arquivo diretamente em um diretório
    ''' </summary>
    ''' <param name="URL">        URL de requisiçao</param>
    ''' <param name="Parameters"> Parametros da URL</param>
    ''' <param name="ContentType">Conteudo</param>
    ''' <param name="Encoding">   Codificação</param>
    ''' <param name="FilePath">   Caminho do arquivo</param>
    ''' <returns></returns>
    Public Shared Function DownloadFile(URL As String, FilePath As String, Optional Method As String = "GET", Optional Parameters As NameValueCollection = Nothing, Optional ContentType As String = "multipart/form-data", Optional Encoding As Encoding = Nothing) As FileInfo
        Return AJAX.Request(Of FileInfo)(URL, Method, Parameters, ContentType, Encoding, FilePath)
    End Function

    ''' <summary>
    ''' Template de resposta de requisiçoes ajax. Facilita respostas de RestAPI em JSON
    ''' </summary>
    Public Class Response

        ''' <summary>
        ''' Status da requisicao
        ''' </summary>
        ''' <returns></returns>
        Property status As String = ""

        ''' <summary>
        ''' Mensagem retornada ao ciente
        ''' </summary>
        ''' <returns></returns>
        Property message As String = ""

        ''' <summary>
        ''' Objeto adicionado a resposta, ele será serializado em JSON
        ''' </summary>
        ''' <returns></returns>
        Property response As Object

        ''' <summary>
        ''' Tempo que a API demora para responder (calculado automaticamente no metodo <see cref="Response.ToJSON(String)"/>)
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property timeout As Long
            Get
                Return t
            End Get
        End Property

        Private t As Long = Now.Ticks

        Public Sub New(Optional Status As String = "", Optional Message As String = "", Optional Response As Object = Nothing)
            Me.status = Status
            Me.message = Message
            Me.response = Response
        End Sub

        ''' <summary>
        ''' Processa a resposta e retorna um JSON deste objeto. É um alias para <see cref="Response.ToJSON(String)"/>
        ''' </summary>
        Public Function SerializeJSON(Optional DateFormat As String = "yyyy-MM-dd hh:mm:ss") As String
            Return ToJSON(DateFormat)
        End Function

        ''' <summary>
        ''' Processa a resposta e retorna um JSON deste objeto
        ''' </summary>
        Public Function ToJSON(Optional DateFormat As String = "yyyy-MM-dd hh:mm:ss") As String
            Try
                Me.status = If(status.IsBlank, "success", status)
                t = Now.Ticks - t
                Return Text.SerializeJSON(Me, DateFormat)
            Catch ex As Exception
                Me.status = If(status.IsBlank, "error", status)
                Me.message = If(message.IsBlank, ex.Message, message)
                Me.response = Nothing
                t = Now.Ticks - t
                Return Text.SerializeJSON(Me, DateFormat)
            End Try
        End Function

    End Class

End Class

''' <summary>
''' Modulo Web
''' </summary>
''' <remarks></remarks>
Public Module Web

    ''' <summary>
    ''' Minifica uma folha de estilo CSS
    ''' </summary>
    ''' <param name="CSS">String contendo o CSS</param>
    ''' <returns></returns>
    Public Function MinifyCSS(CSS As String) As String
        CSS = Regex.Replace(CSS, "[a-zA-Z]+#", "#")
        CSS = Regex.Replace(CSS, "[\n\r]+\s*", String.Empty)
        CSS = Regex.Replace(CSS, "\s+", " ")
        CSS = Regex.Replace(CSS, "\s?([:,;{}])\s?", "$1")
        CSS = CSS.Replace(";}", "}")
        CSS = Regex.Replace(CSS, "([\s:]0)(px|pt|%|em)", "$1")
        ' Remove comments from CSS
        CSS = Regex.Replace(CSS, "/\*[\d\D]*?\*/", String.Empty)
        Return CSS
    End Function

    ''' <summary>
    ''' Verifica se o computador está conectado com a internet
    ''' </summary>
    ''' <returns></returns>
    Public Function IsConnected(Optional Test As String = "http://google.com") As Boolean
        Try
            Using client = New WebClient()
                Using stream = client.OpenRead(Test)
                    Return True
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Cria um objeto a partir de uma requisiçao AJAX
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="TheObject">  Objeto</param>
    ''' <param name="URL">        URL de requisiçao</param>
    ''' <param name="Parameters"> Parametros da URL</param>
    ''' <param name="ContentType">Conteudo</param>
    ''' <param name="Encoding">   Codificação</param>
    <Extension()>
    Public Sub CreateFromAjax(Of Type)(ByRef TheObject As Type, URL As String, Method As String, Optional Parameters As NameValueCollection = Nothing, Optional ContentType As String = "application/x-www-form-urlencoded", Optional Encoding As Encoding = Nothing)
        TheObject = AJAX.Request(Of Type)(URL, Method, Parameters, ContentType, Encoding)
    End Sub

    ''' <summary>
    ''' Destroi a Sessão, cache e cookies de uma aplicação ASP.NET
    ''' </summary>
    ''' <param name="Page">Pagina atual</param>
    <Extension()>
    Public Function DestroySessionAndCookies(Page As HttpApplication) As String
        Try
            Page.Context.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate")
            Page.Context.Response.AddHeader("Pragma", "no-cache")
            Page.Context.Response.AddHeader("Expires", "0")
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
    ''' <param name="Session">    Sessão</param>
    ''' <param name="CookieName"> Nome do Cookie</param>
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
    ''' <param name="Session">    Sessão</param>
    ''' <param name="CookieName"> Nome do Cookie</param>
    ''' <param name="Expires">    Data de expiração</param>
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
    ''' <param name="Session">   Sessão</param>
    ''' <param name="CookieName">Nome do Cookie</param>
    ''' <param name="Expires">   Data de expiração</param>
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
    ''' <param name="Url">  Uri</param>
    ''' <param name="Key">  Nome do parâmetro</param>
    ''' <param name="Value">Valor do Parâmetro</param>
    ''' <returns></returns>
    <Extension>
    Public Function AddParameter(Url As Uri, Key As String, Value As String) As Uri
        Dim UriBuilder = New UriBuilder(Url)
        Dim query = HttpUtility.ParseQueryString(UriBuilder.Query)
        query(Key) = Value
        UriBuilder.Query = query.ToString()
        Url = New Uri(UriBuilder.ToString())
        Return Url
    End Function

    ''' <summary>
    ''' Reescreve a URL original a partir de uma REGEX aplicada em uma URL amigavel
    ''' </summary>
    ''' <param name="App">       Aplicaçao HTTP</param>
    ''' <param name="URLPattern">REGEX da URL</param>
    ''' <param name="OriginalURL">       URL original</param>
    <Extension> Public Function RewriteUrl(App As HttpApplication, URLPattern As String, OriginalURL As String) As Boolean
        Dim theurl = App.Request.Path
        If New Regex(URLPattern, RegexOptions.IgnoreCase).Match(theurl).Success Then
            Dim novaurl = If(App.Request.IsSecureConnection, "https://", "http://") & App.Request.Url.GetDomain & "/" & OriginalURL.ToString
            novaurl = String.Format(novaurl, App.Request.Url.Segments)
            Dim novauri = New Uri(novaurl)
            For Each param In App.Request.QueryString.AllKeys
                novauri = novauri.AddParameter(param, App.Request(param))
            Next
            theurl = novauri.PathAndQuery
            App.Context.RewritePath("~" & theurl)
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar uma procedure especifica e trata parametros espicificos de
    ''' uma URL como parametros da procedure
    ''' </summary>
    ''' <param name="Request">        Requisicao HTTP</param>
    ''' <param name="ProcedureName">  Nome da Procedure</param>
    ''' <param name="QueryStringKeys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>

    <Extension()>
    Public Function ToProcedure(Request As HttpRequest, ByVal ProcedureName As String, ParamArray QueryStringKeys() As String) As String
        Return "EXEC " & ProcedureName & " " & QueryStringKeys.Select(Function(key) " @" & key & "=" & UrlDecode(Request(key)).IsNull(Quotes:=Not UrlDecode(Request(key)).IsNumber())).ToArray.Join(",")
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar uma procedure especifica e trata todos os parametros de
    ''' uma URL como parametros da procedure
    ''' </summary>
    ''' <param name="Request">      Requisicao HTTP</param>
    ''' <param name="ProcedureName">Nome da Procedure</param>
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()>
    Public Function ToProcedure(Request As HttpRequest, ByVal ProcedureName As String) As String
        Return Request.ToProcedure(ProcedureName, Request.QueryString.AllKeys)
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar um INSERT e trata parametros espicificos de
    ''' uma URL como as colunas da tabela de destino
    ''' </summary>
    ''' <param name="Request">        Requisicao HTTP</param>
    ''' <param name="TableName">  Nome da Procedure</param>
    ''' <param name="QueryStringKeys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()> Public Function ToUPDATE(Request As NameValueCollection, ByVal TableName As String, WhereClausule As String, ParamArray QueryStringKeys As String())
        If QueryStringKeys Is Nothing OrElse QueryStringKeys.Length = 0 Then
            QueryStringKeys = Request.AllKeys
        End If
        Dim cmd As String = "UPDATE " & TableName & Environment.NewLine & " set "
        For Each col In QueryStringKeys
            cmd.Append(String.Format(" {0} = {1},", col, UrlDecode(Request(col)).IsNull(Quotes:=Not UrlDecode(Request(col)).IsNumber)) & Environment.NewLine)
        Next
        cmd = cmd.TrimAny(Environment.NewLine, " ", ",") & If(WhereClausule.IsNotBlank, " WHERE " & WhereClausule.TrimAny(" ", "where", "WHERE"), "")
        Debug.WriteLine(cmd.Wrap(Environment.NewLine))
        Return cmd
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar um INSERT e trata parametros espicificos de
    ''' uma URL como as colunas da tabela de destino
    ''' </summary>
    ''' <param name="Request">        Requisicao HTTP</param>
    ''' <param name="TableName">  Nome da Procedure</param>
    ''' <param name="QueryStringKeys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()> Public Function ToUPDATE(Request As HttpRequest, ByVal TableName As String, WhereClausule As String, ParamArray QueryStringKeys As String())
        Return Request.FlatRequest.ToUPDATE(TableName, WhereClausule, QueryStringKeys)
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar um INSERT e trata parametros espicificos de
    ''' uma URL como as colunas da tabela de destino
    ''' </summary>
    ''' <param name="Request">        Requisicao HTTP</param>
    ''' <param name="TableName">  Nome da Procedure</param>
    ''' <param name="QueryStringKeys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()> Public Function ToINSERT(Request As HttpRequest, ByVal TableName As String, ParamArray QueryStringKeys As String())
        Return Request.FlatRequest.ToINSERT(TableName, QueryStringKeys)
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar um INSERT e trata parametros espicificos de
    ''' uma URL como as colunas da tabela de destino
    ''' </summary>
    ''' <param name="Request">        Requisicao HTTP</param>
    ''' <param name="TableName">  Nome da Procedure</param>
    ''' <param name="QueryStringKeys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()> Public Function ToINSERT(Request As NameValueCollection, ByVal TableName As String, ParamArray QueryStringKeys As String())
        If QueryStringKeys Is Nothing OrElse QueryStringKeys.Length = 0 Then
            QueryStringKeys = Request.AllKeys
        End If
        Dim s = String.Format("INSERT INTO " & TableName & " ({0}) values ({1})", QueryStringKeys.Join(","), QueryStringKeys.Select(Function(p) Request(p).UrlDecode.IsNull(Quotes:=Not Request(p).UrlDecode.IsNumber)).ToArray.Join(","))
        Debug.WriteLine(s.Wrap(Environment.NewLine))
        Return s
    End Function


    ''' <summary>
    ''' Escreve um texto e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="Text">    Texto</param>
    <Extension>
    Public Sub WriteEnd(Response As HttpResponse, Text As String)
        Response.Write(Text)
        Response.[End]()
    End Sub

    ''' <summary>
    ''' Esreve um script de Redirect na pagina e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response"></param>
    ''' <param name="Url">Url de Redirect</param>
    ''' <param name="Message">Mensagem na pagina</param>
    ''' <param name="RefreshTime">Tempo em segundos para dar Redirect</param>
    <Extension> Public Sub WriteRedirect(Response As HttpResponse, Url As Uri, Optional Message As String = "", Optional RefreshTime As Integer = 1)
        Response.Clear()
        Response.ContentType = New FileType(".html").ToString
        Response.WriteEnd("<html><head><META http-equiv=""refresh"" content=""" & RefreshTime.SetMinValue(1) & ";URL=" & Url.AbsoluteUri & """></head><body>" & Message & "</body></html>")
    End Sub

    ''' <summary>
    ''' Escreve um arquivo CSV e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response"> HttpResponse</param>
    ''' <param name="CSVString">String com o conteudo do arquivo CSV</param>
    ''' <param name="FileName"> Nome do arquivo CSV</param>
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
    ''' <param name="JSON">    String JSON</param>
    <Extension>
    Public Sub WriteJSON(Response As HttpResponse, JSON As String)
        Response.ContentType = "application/json"
        Response.WriteEnd(JSON)
    End Sub

    ''' <summary>
    ''' Escreve uma imagem e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="Image">   Imagem</param>
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
    ''' <param name="Image">   Imagem</param>
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
    ''' <param name="Image">   Imagem</param>
    <Extension()>
    Public Sub WriteImage(Response As HttpResponse, Image As Drawing.Image, Optional ImageFormat As Drawing.Imaging.ImageFormat = Nothing)
        Response.Clear()
        Response.ContentType = Image.GetFileType.First
        Response.BinaryWrite(Image.ToBytes(ImageFormat))
        Response.End()
    End Sub

    ''' <summary>
    ''' Escreve um JSON e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="JSON">    Objeto de resposta AJAX</param>
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
    ''' Escreve um JSON e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="HttpResponse">Response</param>
    ''' <param name="Response">Objeto anexado ao JSON</param>
    '''<param name="Message">Mensagem</param>
    '''<param name="Status">Status</param>
    <Extension>
    Public Sub WriteJSON(HttpResponse As HttpResponse, Status As String, Message As String, Optional Response As Object = Nothing)
        HttpResponse.WriteJSON(New AJAX.Response(Status, Message, Response))
    End Sub

    ''' <summary>
    ''' Escreve um XML e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="XML">     String XML</param>
    <Extension>
    Public Sub WriteXML(Response As HttpResponse, XML As String)
        Response.ContentType = "application/xml"
        Response.WriteEnd(XML)
    End Sub

    ''' <summary>
    ''' Escreve um XML e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="Response">HttpResponse</param>
    ''' <param name="XML">     String XML</param>
    <Extension>
    Public Sub WriteXML(Response As HttpResponse, XML As XmlDocument)
        Response.WriteXML(XML.ToXMLString)
    End Sub

    ''' <summary>
    ''' Escreve um script na página
    ''' </summary>
    ''' <param name="Response">   HttpResponse</param>
    ''' <param name="ScriptOrURL">Texto ou URL absoluta do Script</param>
    <Extension>
    Public Sub WriteScript(Response As HttpResponse, ScriptOrURL As String)
        If ScriptOrURL.IsURL Then
            Response.Write("<script src=" & ScriptOrURL.Quote & " ></script>")
        Else
            Response.Write("<script>" & ScriptOrURL & "</script>")
        End If

    End Sub

    ''' <summary>
    ''' Escreve um <see cref="htmlparser.HtmlDocument"/> e finaliza um HttpResponse
    ''' </summary>
    ''' <param name="HttpResponse">Response</param>
    <Extension>
    Public Sub WriteJSON(HttpResponse As HttpResponse, Document As HtmlParser.HtmlDocument)
        HttpResponse.WriteEnd(Document.ToString)
    End Sub


    ''' <summary>
    ''' Captura o Username ou UserID de uma URL do Facebook
    ''' </summary>
    ''' <param name="URL">URL do Facebook</param>
    ''' <returns></returns>
    <Extension> Public Function GetFacebookUsername(URL As String) As String
        If URL.IsURL AndAlso URL.GetDomain.ToLower.IsAny("facebook.com", "fb.com") Then
            Return Regex.Match(URL, "(?:(?:http|https):\/\/)?(?:www.)?facebook.com\/(?:(?:\w)*#!\/)?(?:pages\/)?(?:[?\w\-]*\/)?(?:profile.php\?id=(?=\d.*))?([\w\-]*)?").Groups(1).Value
        Else
            Throw New Exception("Invalid Facebook URL")
        End If
    End Function

    ''' <summary>
    ''' Captura o Username ou UserID de uma URL do Facebook
    ''' </summary>
    ''' <param name="URL">URL do Facebook</param>
    ''' <returns></returns>
    <Extension> Public Function GetFacebookUsername(URL As Uri) As String
        Return GetFacebookUsername(URL.AbsoluteUri)
    End Function

    ''' <summary>
    ''' Captura a Thumbnail de um video do youtube
    ''' </summary>
    ''' <param name="URL">Url do Youtube</param>
    ''' <returns></returns>
    Public Function GetYoutubeThumbnail(URL As String) As Drawing.Image
        Return AJAX.GET(Of Drawing.Image)("http://img.youtube.com/vi/" & GetVideoId(URL) & "/hqdefault.jpg")
    End Function

    ''' <summary>
    ''' Captura a Thumbnail de um video do youtube
    ''' </summary>
    ''' <param name="URL">Url do Youtube</param>
    ''' <returns></returns>
    Public Function GetYoutubeThumbnail(URL As Uri) As Drawing.Image
        Return GetYoutubeThumbnail(URL.AbsoluteUri)
    End Function

    ''' <summary>
    ''' Captura o ID de um video do YOUTUBE ou VIMEO em uma URL
    ''' </summary>
    ''' <param name="URL">URL do video</param>
    ''' <returns>ID do video do youtube ou Vimeo</returns>

    Public Function GetVideoId(URL As String) As String
        If URL.IsURL Then
            Select Case True
                Case URL.GetDomain.ContainsAny("youtube", "youtu")
                    Return Regex.Match(URL.Replace("&feature=youtu.be"), "(?:https?:\/\/)?(?:www\.)?youtu(?:.be\/|be\.com\/watch\?v=|be\.com\/v\/)(.{8,})").Groups(1).Value
                Case URL.GetDomain.ContainsAny("vimeo")
                    Return Regex.Match(URL, "vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)").Groups(1).Value
                Case Else
                    Throw New Exception("Invalid Youtube or Vimeo URL")
            End Select
        Else
            Throw New Exception("Invalid Youtube or Vimeo URL")
        End If
    End Function

    ''' <summary>
    ''' Captura o ID de um video do youtube em uma URL
    ''' </summary>
    ''' <param name="URL">URL do video</param>
    ''' <returns>ID do video do youtube</returns>
    <Extension>
    Public Function GetVideoId(URL As Uri) As String
        Return GetVideoId(URL.AbsoluteUri)
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

    ''' <summary>
    ''' Retorna uma string HTML com os options de um <see cref="HtmlSelect"/>
    ''' </summary>
    ''' <param name="Control">COntrole HTMLSelect</param>
    ''' <returns></returns>
    <Extension()> Public Function ExtractOptions(Control As HtmlSelect) As String
        Dim options = ""
        For Each item In Control.Items
            options.Append("<option value=" & item.Value.ToString.Quote & " " & If(item.Selected, "selected", "") & ">" & item.Text & "</option>")
        Next
        Return options
    End Function


    ''' <summary>
    ''' Retorna uma string HTML de um <see cref="HtmlSelect"/>
    ''' </summary>
    ''' <param name="Control">COntrole HTMLSelect</param>
    ''' <returns></returns>
    <Extension()> Public Function ToHtmlString(Control As HtmlSelect) As String
        Dim t As New HtmlParser.HtmlElement("select")
        t.InnerHTML = Control.ExtractOptions
        t.ID = Control.ID
        For Each att As String In Control.Attributes.Keys
            t.Attribute(att) = Control.Attributes(att)
        Next
        Return t.HTML
    End Function

    ''' <summary>
    ''' Seleciona Valores de um <see cref="HtmlSelect"/>
    ''' </summary>
    ''' <param name="Control">Controle <see cref="HtmlSelect"/></param>
    ''' <param name="Values"> Valores que devem receber a propriedade select</param>
    <Extension()> Public Function SelectValues(Control As HtmlSelect, ParamArray Values As String()) As HtmlSelect
        If Values IsNot Nothing AndAlso Values.Length > 1 Then
            Control.Multiple = True
        End If
        For Each i As ListItem In Control.Items
            i.Selected = Values.Select(Function(x) x.ToLower).ToArray.Contains(i.Value.ToLower.ToString)
        Next
        Return Control
    End Function

    ''' <summary>
    ''' Seleciona Valores de um <see cref="HtmlSelect"/>
    ''' </summary>
    ''' <param name="Control">Controle <see cref="HtmlSelect"/></param>
    ''' <param name="predicate"> Predicado que filtra os valores que devem receber a propriedade select</param>
    <Extension()> Public Function SelectValues(Control As HtmlSelect, Predicate As Func(Of ListItem, Boolean)) As HtmlSelect
        Dim values As New List(Of ListItem)
        For Each item In Control.Items
            If Predicate(item) = True Then
                values.Add(item)
            End If
        Next
        If values.Count > 1 Then
            Control.Multiple = True
        End If
        For Each i As ListItem In values
            i.Selected = True
        Next
        Return Control
    End Function

    ''' <summary>
    ''' Seleciona Valores de um <see cref="HtmlSelect"/>
    ''' </summary>
    ''' <param name="Control">Controle <see cref="HtmlSelect"/></param>
    ''' <param name="predicate"> Predicado que filtra os valores que devem receber a propriedade select</param>
    <Extension()> Public Function DisselectValues(Control As HtmlSelect, Predicate As Func(Of ListItem, Boolean)) As HtmlSelect
        Dim values As New List(Of ListItem)
        For Each item In Control.Items
            If Predicate(item) = True Then
                values.Add(item)
            End If
        Next
        If values.Count > 1 Then
            Control.Multiple = True
        End If
        For Each i As ListItem In values
            i.Selected = False
        Next
        Return Control
    End Function

    ''' <summary>
    ''' Desseleciona Valores de um <see cref="HtmlSelect"/>
    ''' </summary>
    ''' <param name="Control">Controle <see cref="HtmlSelect"/></param>
    ''' <param name="Values"> Valores que serao desselecionados</param>
    <Extension()> Public Function DisselectValues(Control As HtmlSelect, ParamArray Values As String()) As HtmlSelect
        For Each i As ListItem In Control.Items
            If IsNothing(Values) OrElse Values.LongCount = 0 OrElse i.Value.IsIn(Values) Then i.Selected = False
        Next
        Return Control
    End Function

    ''' <summary>
    ''' Adiciona um novo <see cref="ListItem"/> ao <see cref="HtmlSelect"/>
    ''' </summary>
    ''' <param name="Control"> Controle <see cref="HtmlSelect"/></param>
    ''' <param name="Text">    Texto do Item</param>
    ''' <param name="Value">   Valor do Item</param>
    ''' <remarks>A mudança de estado do item (selected) é aplicada apoós a inserção do item no select</remarks>
    ''' <returns></returns>
    <Extension()> Public Function AddItem(Control As HtmlSelect, Text As String, Optional Value As String = "") As ListItem
        Dim li = New ListItem(Text, Value.IfBlank(Text))
        Control.Items.Add(li)
        Return li
    End Function

    ''' <summary>
    ''' Realiza um download parcial de um <see cref="Byte()"/>
    ''' </summary>
    ''' <param name="Context">Context HTTP</param>
    ''' <param name="Bytes">Byte array</param>
    ''' <param name="FileType">Tipo do Arquivo</param>
    <Extension()> Public Sub RangeDownload(ByRef Context As HttpContext, ByRef Bytes As Byte(), ByVal FileType As FileType)
        Context.RangeDownload(Bytes, FileType.ToString)
    End Sub

    ''' <summary>
    ''' Realiza um download parcial de um <see cref="Byte()"/>
    ''' </summary>
    ''' <param name="Context">Context HTTP</param>
    ''' <param name="Bytes">Byte array</param>
    ''' <param name="ContentType">MIME Type do Download</param>
    <Extension()>
    Public Sub RangeDownload(ByRef Context As HttpContext, ByRef Bytes As Byte(), ByVal ContentType As String)
        Context.Response.ContentType = ContentType
        Dim size As Long
        Dim start As Long
        Dim theend As Long
        Dim length As Long
        Dim fp As Long = 0
        Using reader As New StreamReader(New MemoryStream(Bytes))
            size = reader.BaseStream.Length
            start = 0
            theend = size - 1
            length = size
            '/ Now that we've gotten so far without errors we send the accept range header
            '* At the moment we only support single ranges.
            '* Multiple ranges requires some more work to ensure it works correctly
            '* and comply with the spesifications: http://www.w3.org/Protocols/rfc2616/rfc2616-sec19.html#sec19.2
            '*
            '* Multirange support annouces itself with:
            '* header('Accept-Ranges: bytes');
            '*
            '* Multirange content must be sent with multipart/byteranges mediatype,
            '* (mediatype = mimetype)
            '* as well as a boundry header to indicate the various chunks of data.
            '*/
            Context.Response.AddHeader("Accept-Ranges", "0-" + size)
            '// header('Accept-Ranges: bytes')
            '// multipart/byteranges
            '// http://www.w3.org/Protocols/rfc2616/rfc2616-sec19.html#sec19.2

            If (Not String.IsNullOrEmpty(Context.Request.ServerVariables("HTTP_RANGE"))) Then
                Dim anotherStart As Long = start
                Dim anotherEnd As Long = theend
                Dim arr_split As String() = Context.Request.ServerVariables("HTTP_RANGE").Split("=") 'new char[] { Convert.ToChar("=") })
                Dim range As String = arr_split(1)

                '// Make sure the client hasn't sent us a multibyte range
                If (range.IndexOf(",") > -1) Then
                    '// (?) Shoud this be issued here, or should the first
                    '// range be used? Or should the header be ignored and
                    '// we output the whole content?
                    Context.Response.AddHeader("Content-Range", "bytes " & start & "-" & theend & "/" & size)
                    Throw New HttpException(416, "Requested Range Not Satisfiable")
                End If

                '// If the range starts with an '-' we start from the beginning
                '// If not, we forward the file pointer
                '// And make sure to get the end byte if spesified
                If (range.StartsWith("-")) Then
                    '// The n-number of the last bytes is requested
                    anotherStart = size - Convert.ToInt64(range.Substring(1))
                Else
                    arr_split = range.Split("-")
                    anotherStart = Convert.ToInt64(arr_split(0))
                    Dim temp As Long = 0
                    If (arr_split.Length > 1 AndAlso Int64.TryParse(arr_split(1).ToString(), temp)) Then
                        anotherEnd = Convert.ToInt64(arr_split(1))
                    Else
                        anotherEnd = size
                    End If
                End If
                '/* Check the range and make sure it's treated according to the specs.
                ' * http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html
                ' */
                '// End bytes can not be larger than $end.
                If (anotherEnd > theend) Then
                    anotherEnd = theend
                Else
                    anotherEnd = anotherEnd
                End If
                '// Validate the requested range and return an error if it's not correct.
                If (anotherStart > anotherEnd Or anotherStart > size - 1 Or anotherEnd >= size) Then
                    Context.Response.AddHeader("Content-Range", "bytes " + start + "-" + theend + "/" + size)
                    Throw New HttpException(416, "Requested Range Not Satisfiable")
                End If

                start = anotherStart
                theend = anotherEnd

                length = theend - start + 1 '// Calculate new content length
                fp = reader.BaseStream.Seek(start, SeekOrigin.Begin)
                Context.Response.StatusCode = 206
            End If
        End Using

        '// Notify the client the byte range we'll be outputting
        Context.Response.AddHeader("Content-Range", "bytes " & start & "-" & theend & "/" & size)
        Context.Response.AddHeader("Content-Length", length.ToString())
        '// Start buffered download
        Context.Response.OutputStream.Write(Bytes, fp, length)
        Context.Response.End()
    End Sub

End Module