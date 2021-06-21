Imports System.Collections.Specialized
Imports System.IO
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions

Imports System.Xml



''' <summary>
''' Modulo Web
''' </summary>
''' <remarks></remarks>
Public Module Web


    <Extension> Public Function ParseQueryString(ByVal URL As Uri) As NameValueCollection
        Return URL.Query.ParseQueryString()
    End Function

    ''' <summary>
    ''' Retorna o Titulo do arquivo a partir do nome do arquivo
    ''' </summary>
    ''' <param name="Info"> Arquivo ou Diretório</param>
    ''' <returns></returns>
    <Extension()> Function FileNameAsTitle(Info As FileSystemInfo) As String
        Return Path.GetFileNameWithoutExtension(Info.Name).CamelAdjust().Replace("_", " ").ToTitle
    End Function

    ''' <summary>
    ''' Retorna o Titulo do arquivo a partir do nome do arquivo
    ''' </summary>
    ''' <param name="FileName"> Arquivo ou Diretório</param>
    ''' <returns></returns>
    <Extension()> Function FileNameAsTitle(FileName As String) As String
        Return Path.GetFileNameWithoutExtension(FileName).CamelAdjust().Replace("_", " ").ToTitle
    End Function

    ''' <summary>
    ''' Minifica uma folha de estilo CSS
    ''' </summary>
    ''' <param name="CSS">String contendo o CSS</param>
    ''' <returns></returns>
    <Extension()> Public Function MinifyCSS(CSS As String) As String
        If CSS.IsNotBlank() Then
            CSS = Regex.Replace(CSS, "[a-zA-Z]+#", "#")
            CSS = Regex.Replace(CSS, "[\n\r]+\s*", String.Empty)
            CSS = Regex.Replace(CSS, "\s+", " ")
            CSS = Regex.Replace(CSS, "\s?([:,;{}])\s?", "$1")
            CSS = CSS.Replace(";}", "}")
            CSS = Regex.Replace(CSS, "([\s:]0)(px|pt|%|em)", "$1")
            ' Remove comments from CSS
            CSS = Regex.Replace(CSS, "/\*[\d\D]*?\*/", String.Empty)
        End If
        Return CSS
    End Function

    ''' <summary>
    ''' Minifica um arquivo JavaScript
    ''' </summary>
    ''' <param name="Js">String contendo o Javascript</param>
    ''' <returns></returns>
    <Extension()> Public Function MinifyJS(Js As String) As String
        Return New JSMin().Minify(Js)
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
    ''' Adciona um parametro a Query String de uma URL
    ''' </summary>
    ''' <param name="Url">  Uri</param>
    ''' <param name="Key">  Nome do parâmetro</param>
    ''' <param name="Values">Valor do Parâmetro</param>
    ''' <returns></returns>
    <Extension>
    Public Function AddParameter(Url As Uri, Key As String, Append As Boolean, ParamArray Values As String()) As Uri
        Dim UriBuilder = New UriBuilder(Url)
        Dim query = UriBuilder.Query.ParseQueryString()
        If Values Is Nothing OrElse Append = False Then
            If query.AllKeys.Contains(Key) Then
                query.Remove(Key)
            End If
        End If
        For Each v In If(Values, {})
            query.Add(Key, v)
        Next
        UriBuilder.Query = query.ToString()
        Url = New Uri(UriBuilder.ToString())
        Return Url
    End Function

    ''' <summary>
    ''' Adciona um parametro a Query String de uma URL
    ''' </summary>
    ''' <param name="Url">  Uri</param>
    ''' <param name="Key">  Nome do parâmetro</param>
    ''' <param name="Values">Valor do Parâmetro</param>
    ''' <returns></returns>
    <Extension>
    Public Function AddParameter(Url As Uri, Key As String, ParamArray Values As String()) As Uri
        Return Url.AddParameter(Key, True, Values)
    End Function

    ''' <summary>
    ''' Adciona um parametro a Query String de uma URL
    ''' </summary>
    ''' <param name="Url">  Uri</param>
    ''' <param name="Key">  Nome do parâmetro</param>
    ''' <param name="Values">Valor do Parâmetro</param>
    ''' <returns></returns>
    <Extension()> Public Function RemoveParameter(Url As Uri, ParamArray Keys As String()) As Uri
        Dim UriBuilder = New UriBuilder(Url)
        Dim query = (UriBuilder.Query).ParseQueryString
        Keys = If(Keys IsNot Nothing AndAlso Keys.Count > 0, Keys, query.AllKeys)
        For Each k In Keys
            Try
                query.Remove(k)
            Catch ex As Exception
            End Try
        Next
        UriBuilder.Query = query.ToString
        Return UriBuilder.Uri
    End Function


    ''' <summary>
    ''' Retorna os segmentos de uma url
    ''' </summary>
    ''' <param name="Url"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetUrlSegments(Url As String) As IEnumerable(Of String)

        Dim l As New List(Of String)
        Dim p As New Regex("(?<!\?.+)(?<=\/)[\w-.]+(?=[/\r\n?]|$)", RegexOptions.Singleline + RegexOptions.IgnoreCase)
        Dim gs = p.Matches(Url)
        For Each g As Match In gs
            l.Add(g.Value)
        Next
        Return l

    End Function

    ''' <summary>
    ''' Substitui os parametros de rota de uma URL por valores de um objeto
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="obj"></param>
    ''' <param name="UrlPattern"></param>
    ''' <returns></returns>
    <Extension()> Public Function ReplaceUrlParameters(Of T)(UrlPattern As String, obj As T) As String
        UrlPattern = Regex.Replace(UrlPattern, "{([^:]+)\s*:\s*(.+?)(?<!\\)}", "{$1}")
        If obj IsNot Nothing Then UrlPattern = UrlPattern.Inject(obj)
        Return UrlPattern.RemoveLastEqual("/")
    End Function

    <Extension()> Public Function RemoveUrlParameters(UrlPattern As String) As String
        UrlPattern = Regex.Replace(UrlPattern, "{([^:]+)\s*:\s*(.+?)(?<!\\)}", "")
        Return UrlPattern.RemoveLastEqual("/")
    End Function


    ''' <summary>
    ''' Monta um Comando SQL para executar uma procedure especifica e trata parametros espicificos de
    ''' uma URL como parametros da procedure
    ''' </summary>
    ''' <param name="NVC">        Requisicao HTTP</param>
    ''' <param name="ProcedureName">  Nome da Procedure</param>
    ''' <param name="Keys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>

    <Extension()>
    Public Function ToProcedure(NVC As NameValueCollection, ByVal ProcedureName As String, ParamArray Keys() As String) As String
        Return NVC.ToDictionary().ToProcedure(ProcedureName, Keys)
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar uma procedure especifica e trata parametros espicificos de
    ''' uma URL como parametros da procedure
    ''' </summary>
    ''' <param name="Dic">        Requisicao HTTP</param>
    ''' <param name="ProcedureName">  Nome da Procedure</param>
    ''' <param name="Keys">CHaves de Dicionário que devem ser utilizadas</param>
    ''' <returns>Uma string com o comando montado</returns>

    <Extension()>
    Public Function ToProcedure(Dic As IDictionary(Of String, Object), ByVal ProcedureName As String, ParamArray Keys() As String) As String
        Keys = If(Keys, {})
        If Keys.Count = 0 Then
            Keys = Dic.Keys.ToArray()
        Else
            Keys = Dic.Keys.ToArray().Where(Function(x) x.IsLikeAny(Keys)).ToArray
        End If
        Dim tmp As DateTime
        Return "EXEC " & ProcedureName & " " & Keys.Select(Function(key) " @" & key & "=" & UrlDecode(If((DateTime.TryParse(Dic(key), tmp)), CType(Dic(key), Date).ToSQLDateString(), "" & Dic(key))).Wrap("'")).ToArray.Join(", ")
    End Function





    ''' <summary>
    ''' Monta um Comando SQL para executar um INSERT e trata parametros espicificos de
    ''' uma URL como as colunas da tabela de destino
    ''' </summary>
    ''' <param name="Request">        Requisicao HTTP</param>
    ''' <param name="TableName">  Nome da tabela</param>
    ''' <param name="Keys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()> Public Function ToUPDATE(Request As NameValueCollection, ByVal TableName As String, WhereClausule As String, ParamArray Keys As String()) As String
        Keys = If(Keys, {})
        If Keys.Count = 0 Then
            Keys = Request.AllKeys
        Else
            Keys = Request.AllKeys.Where(Function(x) x.IsLikeAny(Keys)).ToArray
        End If
        Dim cmd As String = "UPDATE " & TableName & Environment.NewLine & " set "
        For Each col In Keys
            cmd &= (String.Format(" {0} = {1},", col, UrlDecode(Request(col)).Wrap("'")) & Environment.NewLine)
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
    ''' <param name="TableName">  Nome da tabela</param>
    ''' <param name="Keys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()> Public Function ToINSERT(Request As NameValueCollection, ByVal TableName As String, ParamArray Keys As String()) As String
        Keys = If(Keys, {})
        If Keys.Count = 0 Then
            Keys = Request.AllKeys
        Else
            Keys = Request.AllKeys.Where(Function(x) x.IsLikeAny(Keys)).ToArray
        End If
        Dim s = String.Format("INSERT INTO " & TableName & " ({0}) values ({1})", Keys.Join(","), Keys.Select(Function(p) Request(p).UrlDecode.Wrap("'")).ToArray.Join(","))
        Debug.WriteLine(s.Wrap(Environment.NewLine))
        Return s
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar um SELECT com filtros a partir de um <see cref="Dictionary(Of String, Object)"/>
    ''' </summary>
    ''' <param name="Dic">        Dicionario</param>
    ''' <param name="TableName">  Nome da Tabela</param>
    ''' <param name="FilterKeys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>

    <Extension()>
    Public Function ToSQLFilter(Dic As IDictionary(Of String, Object), ByVal TableName As String, CommaSeparatedColumns As String, LogicConcatenation As LogicConcatenationOperator, ParamArray FilterKeys() As String) As String
        Dim CMD = "SELECT " & CommaSeparatedColumns.IfBlank("*") & " FROM " & TableName
        FilterKeys = If(FilterKeys, {})

        If FilterKeys.Count = 0 Then
            FilterKeys = Dic.Keys.ToArray()
        Else
            FilterKeys = Dic.Keys.ToArray().Where(Function(x) x.IsLikeAny(FilterKeys)).ToArray
        End If

        FilterKeys = FilterKeys.Where(Function(x) Dic(x) IsNot Nothing AndAlso Dic(x).ToString().IsNotBlank()).ToArray()

        If FilterKeys.Count > 0 Then
            CMD = CMD & " WHERE " & FilterKeys.Select(Function(key) " " & key & "=" & UrlDecode("" & Dic(key)).Wrap("'")).ToArray.Join(" " & [Enum].GetName(GetType(LogicConcatenationOperator), LogicConcatenation) & " ")
        End If
        Return CMD
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar um SELECT com filtros a partir de um <see cref="NameValueCollection"/>
    ''' </summary>
    ''' <param name="NVC">        Colecao</param>
    ''' <param name="TableName">  Nome da Tabela</param>
    ''' <param name="FilterKeys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()>
    Public Function ToSQLFilter(NVC As NameValueCollection, ByVal TableName As String, CommaSeparatedColumns As String, LogicConcatenation As LogicConcatenationOperator, ParamArray FilterKeys() As String) As String
        Return NVC.ToDictionary.ToSQLFilter(TableName, CommaSeparatedColumns, LogicConcatenation, FilterKeys)
    End Function



    Public Enum LogicConcatenationOperator
        [AND]
        [OR]
    End Enum


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
    Public Function GetYoutubeThumbnail(URL As String) As Byte()
        Return GetFile("http://img.youtube.com/vi/" & GetVideoId(URL) & "/hqdefault.jpg")
    End Function

    Public Function GetString(URL) As String
        Dim s = ""
        Using c As New WebClient
            s = c.DownloadString(URL)
        End Using
        Return s
    End Function
    Public Function GetFile(URL As String) As Byte()
        Dim s As Byte()
        Using c As New WebClient
            s = c.DownloadData(URL)
        End Using
        Return s
    End Function

    Public Function GetImage(URL As String) As System.Drawing.Image
        Return GetFile(URL).ToImage()
    End Function


    ''' <summary>
    ''' Captura a Thumbnail de um video do youtube
    ''' </summary>
    ''' <param name="URL">Url do Youtube</param>
    ''' <returns></returns>
    Public Function GetYoutubeThumbnail(URL As Uri) As Byte()
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
                    Return Regex.Match(URL.ReplaceNone("&feature=youtu.be"), "(?:https?:\/\/)?(?:www\.)?youtu(?:.be\/|be\.com\/watch\?v=|be\.com\/v\/)(.{8,})").Groups(1).Value
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
        Dim content As String = GetString("http://downforeveryoneorjustme.com/" & Url)
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