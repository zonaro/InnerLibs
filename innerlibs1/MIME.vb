Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Xml

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
        Return FileType.GetFileType(Extension).GetMimeTypesOrDefault
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
    Public Function GetIcon(File As FileSystemInfo) As Icon
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
    Public Property MimeTypes As New List(Of String)

    Public Function GetMimeTypesOrDefault() As IEnumerable(Of String)
        Return If(MimeTypes, New List(Of String)).DefaultIfEmpty("application/octet-stream")
    End Function

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
        Return FirstTypes.Contains("image")
    End Function

    ''' <summary>
    ''' Verifica se Tipo de arquivo é de audio
    ''' </summary>
    ''' <returns></returns>
    Public Function IsAudio() As Boolean
        Return FirstTypes.Contains("audio")
    End Function

    ''' <summary>
    ''' Verifica se Tipo de arquivo é de audio
    ''' </summary>
    ''' <returns></returns>
    Public Function IsVideo() As Boolean
        Return FirstTypes.Contains("video")
    End Function

    ''' <summary>
    ''' Verifica se Tipo de arquivo é de audio
    ''' </summary>
    ''' <returns></returns>
    Public Function IsText() As Boolean
        Return FirstTypes.Contains("text")
    End Function

    ''' <summary>
    ''' Verifica se Tipo de arquivo é de audio
    ''' </summary>
    ''' <returns></returns>
    Public Function IsApplication() As Boolean
        Return FirstTypes.Contains("application")
    End Function

    ''' <summary>
    ''' Retorna o tipo do MIME Type (antes da barra)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property FirstTypes As IEnumerable(Of String)
        Get
            Return GetMimeTypesOrDefault.Select(Function(p) p.ToLower.Trim.GetBefore("/")).Distinct
        End Get
    End Property

    ''' <summary>
    ''' Retorna o subtipo do MIME Type (depois da barra)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SubTypes As IEnumerable(Of String)
        Get
            Return GetMimeTypesOrDefault.Select(Function(p) p.ToLower.Trim.GetAfter("/")).Distinct
        End Get
    End Property

    Private Shared l As New FileTypeList

    ''' <summary>
    ''' Retorna uma Lista com todos os MIME Types suportados
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetFileTypeList(Optional Reset As Boolean = False) As FileTypeList
        If Reset OrElse l Is Nothing OrElse l.Any = False Then
            Dim r As String = [Assembly].GetExecutingAssembly().GetResourceFileText("InnerLibs.mimes.xml")
            Dim doc = New XmlDocument()
            doc.LoadXml(r)
            l = New FileTypeList()
            For Each node As XmlNode In doc("mimes").ChildNodes
                Dim ft = l.FirstOr(Function(x) x.Description = node("Description").InnerText.AdjustBlankSpaces, New FileType())

                With ft
                    .Description = node("Description").InnerText.AdjustBlankSpaces

                    For Each item As XmlNode In node("MimeTypes").ChildNodes
                        .MimeTypes.Add(item.InnerText.AdjustBlankSpaces)
                    Next

                    For Each item As XmlNode In node("Extensions").ChildNodes
                        .Extensions.Add(item.InnerText.AdjustBlankSpaces)
                    Next

                    .MimeTypes = .MimeTypes.Distinct().ToList()
                    .Extensions = .Extensions.Distinct().ToList()
                End With
                If Not l.Contains(ft) Then l.Add(ft)
            Next
        End If
        Return l
    End Function

    ''' <summary>
    ''' Retorna uma lista de strings contendo todos os MIME Types
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetFileTypeStringList(Optional FileTypeList As FileTypeList = Nothing) As IEnumerable(Of String)
        Return If(FileTypeList, GetFileTypeList()).SelectMany(Function(x) x.GetMimeTypesOrDefault).Distinct()
    End Function

    ''' <summary>
    ''' Traz uma lista de extensões de acordo com o MIME type especificado
    ''' </summary>
    ''' <param name="MIME">MIME Type String</param>
    ''' <returns></returns>
    Public Shared Function GetExtensions(MIME As String, Optional FileTypeList As FileTypeList = Nothing) As List(Of String)
        For Each item As FileType In If(FileTypeList, GetFileTypeList())
            If item.GetMimeTypesOrDefault.Contains(MIME) Then
                Return item.Extensions
            End If
        Next
        Return New List(Of String)
    End Function

    Public Shared Function GetFileType(MimeTypeOrExtensionOrPathOrDataURI As IEnumerable(Of String), Optional FileTypeList As FileTypeList = Nothing) As FileTypeList
        Return New FileTypeList(MimeTypeOrExtensionOrPathOrDataURI.Select(Function(x) GetFileType(x)).ToArray())
    End Function

    ''' <summary>
    ''' Retorna um objeto FileType a partir de uma extensão de Arquivo ou FileType string
    ''' </summary>
    ''' <param name="MimeTypeOrExtensionOrPathOrDataURI"></param>
    ''' <returns></returns>
    Public Shared Function GetFileType(MimeTypeOrExtensionOrPathOrDataURI As String, Optional FileTypeList As FileTypeList = Nothing) As FileType

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
            MimeTypeOrExtensionOrPathOrDataURI = "." & MimeTypeOrExtensionOrPathOrDataURI.TrimAny(True, " ", ".")
        End If

        Return If(FileTypeList, GetFileTypeList()).FirstOr(Function(x) (x.Extensions.ToArray().Union(x.GetMimeTypesOrDefault.ToArray())).Contains(MimeTypeOrExtensionOrPathOrDataURI, StringComparer.InvariantCultureIgnoreCase), New FileType)

    End Function

    Public Function SearchFiles(Directory As DirectoryInfo, Optional SearchOption As SearchOption = SearchOption.AllDirectories) As IEnumerable(Of FileInfo)
        Return Directory.SearchFiles(SearchOption, Extensions.Select(Function(ext) "*" & ext.PrependIf(".", Not ext.StartsWith("."))).ToArray)
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
    Public Sub New(File As FileInfo, Optional FileTypeList As FileTypeList = Nothing)
        Build(File.Extension, FileTypeList)
    End Sub

    ''' <summary>
    ''' Constroi um File Type a partir da extensão ou MIME Type de um Arquivo
    ''' </summary>
    ''' <param name="MimeTypeOrExtensionOrPathOrDataURI">Extensão do arquivo</param>
    Public Sub New(MimeTypeOrExtensionOrPathOrDataURI As String, Optional FileTypeList As FileTypeList = Nothing)
        Build(MimeTypeOrExtensionOrPathOrDataURI.ToLower, FileTypeList)
    End Sub

    Friend Sub Build(Extension As String, Optional FileTypeList As FileTypeList = Nothing)
        Dim item = GetFileType(Extension, FileTypeList)
        Me.Extensions = item.Extensions
        Me.MimeTypes = item.MimeTypes
        Me.Description = item.Description.ToProperCase
    End Sub

    ''' <summary>
    ''' Retorna uma string com o primeiro MIME TYPE do arquivo
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return Me.GetMimeTypesOrDefault.First
    End Function

    ''' <summary>
    ''' Retorna uma string representando um filtro de caixa de dialogo WinForms
    ''' </summary>
    ''' <returns></returns>
    Public Function ToFilterString() As String
        Dim r As String = ""
        For Each ext In Me.Extensions
            r &= ("*" & ext & ";")
        Next
        r = Me.Description & "|" & r.RemoveLastEqual(";")
        Return r
    End Function

End Class

''' <summary>
''' Lista com Tipos de arquivo ultilizada para filtro e validação
''' </summary>
Public Class FileTypeList
    Inherits List(Of FileType)

    ''' <summary>
    ''' Cria uma nova lista vazia
    ''' </summary>
    Public Sub New()
    End Sub


    ''' <summary>
    ''' Cria uma nova lista a partir de mime types, caminhos ou extensoes
    ''' </summary>
    ''' <param name="FileTypes">Tipos de Arquivos</param>
    Public Sub New(ParamArray FileTypes As String())
        Me.New(If(FileTypes, {}).Select(Function(x) New FileType(x)).ToArray)
    End Sub

    ''' <summary>
    ''' Cria uma nova lista a partir de tipos de arquivos
    ''' </summary>
    ''' <param name="FileTypes">Tipos de Arquivos</param>
    Public Sub New(ParamArray FileTypes As FileType())
        Me.AddRange(If(FileTypes, {}))
    End Sub

    ''' <summary>
    ''' Cria uma nova lista a partir de uma lista de tipos de arquivos
    ''' </summary>
    ''' <param name="FileTypeList">Tipos de Arquivos</param>
    Public Sub New(FileTypeList As List(Of FileType))
        Me.New(If(FileTypeList, New List(Of FileType)).ToArray)
    End Sub

    ''' <summary>
    ''' Cria uma nova lista a partir de um critério de filtro
    ''' </summary>
    ''' <param name="predicate">Criterio de busca</param>
    Public Sub New(predicate As Func(Of FileType, Boolean))
        Me.New(Nothing, predicate)
    End Sub

    ''' <summary>
    ''' Cria uma nova lista a partir de um critério de filtro
    ''' </summary>
    ''' <param name="predicate">Criterio de busca</param>
    Public Sub New(FileTypeList As FileTypeList, predicate As Func(Of FileType, Boolean))
        Me.New(If(FileTypeList, FileType.GetFileTypeList).Where(predicate).ToArray)
    End Sub

    ''' <summary>
    ''' Retorna uma string representando um filtro de caixa de dialogo WinForms
    ''' </summary>
    ''' <returns></returns>
    Public Function ToFilterString() As String
        Dim r As String = ""
        For Each ext In Me
            r &= (ext.ToFilterString & "|")
        Next
        Return r.RemoveLastEqual("|")
    End Function

    ''' <summary>
    ''' Busca arquivos que correspondam com as extensões desta lista
    ''' </summary>
    ''' <param name="Directory">   Diretório</param>
    ''' <param name="SearchOption">Tipo de busca</param>
    ''' <returns></returns>
    Public Function SearchFiles(Directory As DirectoryInfo, Optional SearchOption As SearchOption = SearchOption.AllDirectories) As IEnumerable(Of FileInfo)
        Return Directory.SearchFiles(SearchOption, Extensions.Select(Function(ext) "*" & ext.PrependIf(".", Not ext.StartsWith("."))).ToArray)
    End Function

    ''' <summary>
    ''' Retorna todas as extensões da lista
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Extensions As IEnumerable(Of String)
        Get
            Return Me.SelectMany(Function(x) x.Extensions).Distinct()
        End Get
    End Property

    ''' <summary>
    ''' Retorna todas os MIME Types da lista
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MimeTypes As IEnumerable(Of String)
        Get
            Return Me.SelectMany(Function(x) x.GetMimeTypesOrDefault).Distinct()
        End Get
    End Property

    Public ReadOnly Property SubTypes As IEnumerable(Of String)
        Get
            Return Me.SelectMany(Function(x) x.SubTypes).Distinct()
        End Get
    End Property

    Public ReadOnly Property FirstTypes As IEnumerable(Of String)
        Get
            Return Me.SelectMany(Function(x) x.FirstTypes).Distinct()
        End Get
    End Property

    Public ReadOnly Property Descriptions As IEnumerable(Of String)
        Get
            Return Me.SelectMany(Function(x) x.Description).Distinct()
        End Get
    End Property

End Class