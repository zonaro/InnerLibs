Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

''' <summary>
''' Módulo de manipulaçao de MIME Types
''' </summary>
Public Module FileTypeExtensions

    ''' <summary>
    ''' Retorna o Mime Type a partir da extensão de um arquivo
    ''' </summary>
    ''' <param name="Extension">extensão do arquivo</param>
    ''' <returns>string mime type</returns>
    <Extension>
    Public Function GetFileType(Extension As String) As List(Of String)
        Return FileType.GetFileType(Extension).MimeTypes
    End Function

    ''' <summary>
    ''' Retorna o Mime Type a partir de um arquivo
    ''' </summary>
    ''' <param name="File">Arquivo</param>
    ''' <returns>string mime type</returns>
    <Extension>
    Public Function GetFileType(File As FileInfo) As List(Of String)
        Return GetFileType(File.Extension)
    End Function

    ''' <summary>
    ''' Retorna o Mime Type a partir de de um formato de Imagem
    ''' </summary>
    ''' <param name="RawFormat">Formato de Imagem</param>
    ''' <returns>string mime type</returns>
    <Extension>
    Public Function GetFileType(RawFormat As ImageFormat) As List(Of String)
        Try
            For Each img In ImageCodecInfo.GetImageEncoders()
                If img.FormatID = RawFormat.Guid Then
                    Return img.FilenameExtension.GetFileType()
                End If
            Next
        Catch ex As Exception
        End Try
        Return GetFileType(".png")
    End Function

    ''' <summary>
    ''' Retorna o Mime Type a partir de de uma Imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <returns>string mime type</returns>
    <Extension>
    Public Function GetFileType(Image As Image) As List(Of String)
        Return Image.RawFormat.GetFileType
    End Function

    ''' <summary>
    ''' Retorna um Objeto FileType a partir de uma string MIME Type, Nome ou Extensão de Arquivo
    ''' </summary>
    ''' <param name="MimeTypeOrExtensionOrPathOrDataURI"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToFileType(MimeTypeOrExtensionOrPathOrDataURI As String) As FileType
        Return New FileType(MimeTypeOrExtensionOrPathOrDataURI)
    End Function

    ''' <summary>
    ''' Retorna um icone de acordo com o arquivo
    ''' </summary>
    ''' <param name="File">Arquivo</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetIcon(File As FileInfo) As Icon
        Try
            Return System.Drawing.Icon.ExtractAssociatedIcon(File.FullName)
        Catch ex As Exception
            Return SystemIcons.WinLogo
        End Try
    End Function

End Module

''' <summary>
''' Classe que representa um MIME Type
''' </summary>
Public Class FileType

    ''' <summary>
    ''' Extensão do arquivo
    ''' </summary>
    ''' <returns></returns>
    Public Property Extensions As New List(Of String)

    ''' <summary>
    ''' Tipo do arquivo (MIME Type String)
    ''' </summary>
    ''' <returns></returns>
    Public Property MimeTypes As New List(Of String)({"application/octet-stream"})

    ''' <summary>
    ''' Descrição do tipo de arquivo
    ''' </summary>
    ''' <returns></returns>
    Public Property Description As String = "Unknown File"

    ''' <summary>
    ''' Verifica se Tipo de arquivo é de imagem
    ''' </summary>
    ''' <returns></returns>
    Public Function IsImage() As Boolean
        Return FirstType.Contains("image")
    End Function

    ''' <summary>
    ''' Verifica se Tipo de arquivo é de audio
    ''' </summary>
    ''' <returns></returns>
    Public Function IsAudio() As Boolean
        Return FirstType.Contains("audio")
    End Function

    ''' <summary>
    ''' Verifica se Tipo de arquivo é de audio
    ''' </summary>
    ''' <returns></returns>
    Public Function IsVideo() As Boolean
        Return FirstType.Contains("video")
    End Function

    ''' <summary>
    ''' Verifica se Tipo de arquivo é de audio
    ''' </summary>
    ''' <returns></returns>
    Public Function IsText() As Boolean
        Return FirstType.Contains("text")
    End Function

    ''' <summary>
    ''' Verifica se Tipo de arquivo é de audio
    ''' </summary>
    ''' <returns></returns>
    Public Function IsApplication() As Boolean
        Return FirstType.Contains("application")
    End Function

    ''' <summary>
    ''' Retorna o tipo do MIME Type (antes da barra)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property FirstType As List(Of String)
        Get
            Return MimeTypes.Select(Function(p) p.ToLower.Trim.Split("/")(0).Trim).Distinct.ToList
        End Get
    End Property

    ''' <summary>
    ''' Retorna o subtipo do MIME Type (depois da barra)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SubType As List(Of String)
        Get
            Return MimeTypes.Select(Function(p) p.ToLower.Trim.Split("/")(1)).Distinct.ToList
        End Get
    End Property

    ''' <summary>
    ''' Retorna uma Lista com todos os MIME Types suportados
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetFileTypeList() As FileTypeList
        Dim r As String = [Assembly].GetExecutingAssembly().GetResourceFileText("InnerLibs.mimes.json")
        Return New FileTypeList(r.ParseJSON(Of List(Of FileType)))
    End Function

    ''' <summary>
    ''' Retorna uma lista de strings contendo todos os MIME Types
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetFileTypeStringList() As List(Of String)
        Dim l As New List(Of String)
        l.AddRange(New FileType().MimeTypes)
        GetFileTypeList.ForEach(Sub(p) l.AddRange(p.MimeTypes))
        Return l
    End Function

    ''' <summary>
    ''' Traz uma lista de extensões de acordo com o MIME type especificado
    ''' </summary>
    ''' <param name="MIME">MIME Type String</param>
    ''' <returns></returns>
    Public Shared Function GetExtensions(MIME As String) As List(Of String)
        For Each item As FileType In GetFileTypeList()
            If item.MimeTypes.Contains(MIME) Then
                Return item.Extensions
            End If
        Next
        Return New List(Of String)
    End Function

    ''' <summary>
    ''' Retorna um objeto FileType a partir de uma extensão de Arquivo ou FileType string
    ''' </summary>
    ''' <param name="MimeTypeOrExtensionOrPathOrDataURI"></param>
    ''' <returns></returns>
    Public Shared Function GetFileType(MimeTypeOrExtensionOrPathOrDataURI As String) As FileType
        Dim l = GetFileTypeList()
        Dim ismime As Boolean = True

        Try
            Return New DataURI(MimeTypeOrExtensionOrPathOrDataURI).ToFileType
        Catch ex As Exception
        End Try

        Try
            Dim newmime = Path.GetExtension(MimeTypeOrExtensionOrPathOrDataURI)
            If newmime.IsNotBlank Then
                MimeTypeOrExtensionOrPathOrDataURI = newmime
                ismime = False
            End If
        Catch ex As Exception
        End Try
        If Not ismime Then
            MimeTypeOrExtensionOrPathOrDataURI = "." & MimeTypeOrExtensionOrPathOrDataURI.TrimAny(" ", ".")
        End If
        For Each item As FileType In l
            If (MimeTypeOrExtensionOrPathOrDataURI.IsIn(item.Extensions) Or MimeTypeOrExtensionOrPathOrDataURI.IsIn(item.MimeTypes)) Then
                Return item
            End If
        Next
        Return New FileType
    End Function

    ''' <summary>
    ''' Constroi um MIME Type Default
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Constroi um File Type a partir de um Arquivo (FileInfo)
    ''' </summary>
    ''' <param name="File">Fileinfo com o Arquivo</param>
    Public Sub New(File As FileInfo)
        Build(File.Extension)
    End Sub

    ''' <summary>
    ''' Constroi um File Type a partir da extensão ou MIME Type de um Arquivo
    ''' </summary>
    ''' <param name="MimeTypeOrExtensionOrPathOrDataURI">Extensão do arquivo</param>
    Public Sub New(MimeTypeOrExtensionOrPathOrDataURI As String)
        Build(MimeTypeOrExtensionOrPathOrDataURI)
    End Sub

    Private Sub Build(Extension As String)
        Dim item = GetFileType(Extension)
        Me.Extensions = item.Extensions
        Me.MimeTypes = item.MimeTypes
        Me.Description = item.Description.ToProper
    End Sub

    ''' <summary>
    ''' Retorna uma string com o primeiro MIME TYPE do arquivo
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return Me.MimeTypes.First
    End Function

    ''' <summary>
    ''' Retorna uma string representando um filtro de caixa de dialogo WinForms
    ''' </summary>
    ''' <returns></returns>
    Public Function ToFilterString() As String
        Dim r As String = ""
        For Each ext In Me.Extensions
            r.Append("*" & ext & ";")
        Next
        r = Me.Description & "|" & r.RemoveLastIf(";")
        Return r
    End Function

End Class

''' <summary>
''' Lista com Tipos de arquivo ultilizada para filtro e validação
''' </summary>
Public Class FileTypeList
    Inherits List(Of FileType)

    ''' <summary>
    ''' Cria uma nova lista a partir de tipos de arquivos
    ''' </summary>
    ''' <param name="FileTypes">Tipos de Arquivos</param>
    Public Sub New(ParamArray FileTypes As FileType())
        If FileTypes.Length > 0 Then Me.AddRange(FileTypes)
    End Sub

    ''' <summary>
    ''' Cria uma nova lista a partir de uma lista de tipos de arquivos
    ''' </summary>
    ''' <param name="FileTypeList">Tipos de Arquivos</param>
    Public Sub New(FileTypeList As List(Of FileType))
        Me.New(FileTypeList.ToArray)
    End Sub

    ''' <summary>
    ''' Cria uma nova lista a partir de um critério de filtro
    ''' </summary>
    ''' <param name="predicate">Criterio de busca</param>
    Public Sub New(predicate As Func(Of FileType, Boolean))
        Me.New(FileType.GetFileTypeList.Where(predicate).ToArray)
    End Sub



    ''' <summary>
    ''' Retorna uma string representando um filtro de caixa de dialogo WinForms
    ''' </summary>
    ''' <returns></returns>
    Public Function ToFilterString() As String
        Dim r As String = ""
        For Each ext In Me
            r.Append(ext.ToFilterString & "|")
        Next
        Return r.RemoveLastIf("|")
    End Function

    ''' <summary>
    ''' Aplica um filtro no OpenFileDialog
    ''' </summary>
    ''' <param name="Dialog">Dialogo</param>
    Public Sub ApplyDialogFilter(ByRef Dialog As OpenFileDialog)
        Dialog.Filter = Me.ToFilterString
    End Sub

    ''' <summary>
    ''' Aplica um filtro no SaveFileDialog
    ''' </summary>
    ''' <param name="Dialog">Dialogo</param>
    Public Sub ApplyDialogFilter(ByRef Dialog As SaveFileDialog)
        Dialog.Filter = Me.ToFilterString
    End Sub

    ''' <summary>
    ''' Busca arquivos que correspondam com as extensões desta lista
    ''' </summary>
    ''' <param name="Directory">   Diretório</param>
    ''' <param name="SearchOption">Tipo de busca</param>
    ''' <returns></returns>
    Public Function SearchFiles(Directory As DirectoryInfo, Optional SearchOption As SearchOption = SearchOption.AllDirectories) As List(Of FileInfo)
        Dim support As New List(Of String)
        For Each f In Me
            For Each ext In f.Extensions
                support.Add("*" & ext.PrependIf(".", Not ext.StartsWith(".")))
            Next
        Next
        Return Directory.SearchFiles(SearchOption, support.ToArray)
    End Function

    ''' <summary>
    ''' Retorna todas as extensões da lista
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Extensions As List(Of String)
        Get
            Dim l As New List(Of String)
            For Each ext In Me
                l.AddRange(ext.Extensions.Select(Function(p) p.ToLower.Trim))
            Next
            Return l.Distinct.ToList
        End Get
    End Property

    ''' <summary>
    ''' Retorna todas os MIME Types da lista
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MimeTypes(Optional Filter As String = "") As List(Of String)
        Get
            Dim l As New List(Of String)
            For Each ext In Me
                l.AddRange(ext.MimeTypes.Where(Function(Y) If(Filter.IsBlank, True, Y.Split("/")(0).Trim.ToLower = Filter.Trim.ToLower)).Select(Function(p) p.ToLower.Trim))
            Next
            Return l.Distinct.ToList
        End Get
    End Property

End Class