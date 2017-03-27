Imports System.IO
Imports System.Runtime.CompilerServices

Public Module FontAwesome

    ''' <summary>
    ''' Retorna a classe do icone do FontAwesome que representa melhor o arquivo
    ''' </summary>
    ''' <param name="File">Arquivo</param>
    ''' <returns></returns>
    <Extension>
    Public Function GetIconByFileType(File As FileInfo) As String
        Return File.Extension.GetIconByFileExtension()
    End Function
    ''' <summary>
    ''' Retorna a classe do icone do FontAwesome que representa melhor o arquivo
    ''' </summary>
    ''' <param name="MIME">MIME Type do Arquivo</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetIconByFileType(MIME As FileType)
        Return MIME.Extensions.First.GetIconByFileExtension()
    End Function

    ''' <summary>
    ''' Retorna a classe do icone do FontAwesome que representa melhor o arquivo
    ''' </summary>
    ''' <param name="Extension">Arquivo</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetIconByFileExtension(Extension As String) As String
        Select Case Extension.RemoveAny(".").ToLower
            Case "png", "jpg", "gif", "jpeg", "psd", "ai", "drw", "ttf", "svg"
                Return "fa-file-picture-o"
            Case "doc", "docx"
                Return "fa-file-word-o"
            Case "pdf"
                Return "fa-file-pdf-o"
            Case "ppt", "pptx"
                Return "fa-file-powerpoint-o"
            Case "xls", "xlsx"
                Return "fa-file-excel-o"
            Case "html", "htm", "php", "cpp", "vb", "cs", "jsp", "xml", "js", "css", "aspx", "ascx", "ashx", "config"
                Return "fa-file-code-o"
            Case "apk"
                Return "fa-android"
            Case "ios", "ipa"
                Return "fa-apple"
            Case "xap", "appx"
                Return "fa-windows"
            Case "zip", "rar", "tar", "jar", "gz", "iso", "7zip"
                Return "fa-file-archive-o"
            Case "avi", "mpeg", "mp4", "3gp", "mkv", "wmv", "rmvb"
                Return "fa-file-video-o"
            Case "txt", "otf", "otd", "ttf", "rtf", "csv", "xps", "cdr", "cfg"
                Return "fa-file-text-o"
            Case "mp3", "mp2", "wma", "wav", "ogg", "flac", "aac"
                Return "fa-file-audio-o"
            Case "gb", "gba", "n64", "rom", "z64", "gbc", "wad"
                Return "fa-gamepad"
            Case "exe", "bat"
                Return "fa-window-maximize"
            Case "sql"
                Return "fa-database"
            Case Else
        End Select
        Return "fa-file-o"
    End Function

    Public Const CDNFontAwesomeCSS = "https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css"


End Module
