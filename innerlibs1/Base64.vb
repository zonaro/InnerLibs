Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions


''' <summary>
''' Classe para Extrair informaçoes de uma DATAURL
''' </summary>
Public Class DataURI

    ''' <summary>
    ''' Cria um novo DATAURL a aprtir de uma string
    ''' </summary>
    ''' <param name="DataURI"></param>
    Sub New(DataURI As String)
        Try
            Dim regex = New Regex("^data:(?<mimeType>(?<mime>\w+)\/(?<extension>\w+));(?<encoding>\w+),(?<data>.*)", RegexOptions.Compiled)
            Dim match = regex.Match(DataURI)
            Mime = match.Groups("mime").Value.ToLower
            Extension = match.Groups("extension").Value.ToLower
            Encoding = match.Groups("encoding").Value.ToLower
            Data = match.Groups("data").Value
            If {Mime, Extension, Encoding, Data}.Any(Function(x) x.IsBlank) Then
                Throw New Exception("Some parts are blank")
            End If
        Catch ex As Exception
            Throw New ArgumentException("DataURI not Valid", ex)
        End Try
    End Sub

    ''' <summary>
    ''' String Base64 ou Base32
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Data As String

    ''' <summary>
    ''' Tipo de encoding (32 ou 64)
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Encoding As String

    ''' <summary>
    ''' Tipo do arquivo encontrado
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Mime As String

    ''' <summary>
    ''' Extensão do tipo do arquivo
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Extension As String

    ''' <summary>
    ''' MIME type completo
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property FullMimeType As String
        Get
            Return Mime & "/" & Extension
        End Get
    End Property

    ''' <summary>
    ''' Informaçoes referentes ao tipo do arquivo
    ''' </summary>
    ''' <returns></returns>
    Function ToFileType() As FileType
        Return New FileType(Me.FullMimeType)
    End Function

    ''' <summary>
    ''' Retorna uma string da dataURL
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return "data:" & FullMimeType & ";" & Encoding & "," & Data
    End Function

    ''' <summary>
    ''' Converte esta dataURI em Bytes()
    ''' </summary>
    ''' <returns></returns>
    Public Function ToBytes() As Byte()
        Return Me.ToString.Base64ToBytes
    End Function

    ''' <summary>
    ''' Transforma este datauri em arquivo
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    Public Function WriteToFile(Path As String) As FileInfo
        Return Me.ToBytes.WriteToFile(Path)
    End Function




End Class

''' <summary>
''' Modulo para manipulação de imagens e Strings Base64
''' </summary>
''' <remarks></remarks>
Public Module Base64

    ''' <summary>
    ''' Retorna TRUE se o texto for um dataurl valido
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Function IsDataURL(Text As String) As Boolean
        Try
            Return New DataURI(Text).ToString.IsNotBlank
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Encoda uma string em Base64
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="Encoding"></param>
    ''' <returns></returns>
    <Extension()> Public Function Btoa(Text As String, Optional Encoding As Encoding = Nothing) As String
        Return Convert.ToBase64String(If(Encoding, New UTF8Encoding(False)).GetBytes(Text))
    End Function

    ''' <summary>
    ''' Decoda uma string em Base64
    ''' </summary>
    ''' <param name="Base"></param>
    ''' <param name="Encoding"></param>
    ''' <returns></returns>
    <Extension()> Public Function Atob(Base As String, Optional Encoding As Encoding = Nothing) As String
        Return If(Encoding, New UTF8Encoding(False)).GetString(Convert.FromBase64String(Base))
    End Function

    ''' <summary>
    ''' Arruma os caracteres de uma string Base64
    ''' </summary>
    ''' <param name="Base64StringOrDataUrl">Base64String ou DataURL</param>
    ''' <returns>Retorna apenas a Base64</returns>
    <Extension()> Public Function FixBase64(Base64StringOrDataUrl As String) As String
        Dim dummyData As String = Base64StringOrDataUrl.GetAfter(",").Trim().Replace(" ", "+")
        If dummyData.Length Mod 4 > 0 Then
            dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length Mod 4, "="c)
        End If
        Return dummyData
    End Function

    ''' <summary>
    ''' Converte um Array de Bytes em uma string Base64
    ''' </summary>
    ''' <param name="Bytes">Array de Bytes</param>
    ''' <returns></returns>
    <Extension> Public Function ToBase64(Bytes As Byte()) As String
        Return Convert.ToBase64String(Bytes)
    End Function

    ''' <summary>
    ''' Converte um Array de Bytes em uma DATA URL Completa
    ''' </summary>
    ''' <param name="Bytes">Array de Bytes</param>
    ''' <param name="Type">Tipo de arquivo</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToDataURL(Bytes As Byte(), Optional Type As FileType = Nothing) As String
        Return "data:" & If(Type, New FileType).ToString & ";base64," & Bytes.ToBase64()
    End Function

    ''' <summary>
    ''' Converte um Array de Bytes em uma DATA URL Completa
    ''' </summary>
    ''' <param name="Bytes">Array de Bytes</param>
    ''' <param name="MimeType">Tipo de arquivo</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToDataURL(Bytes As Byte(), MimeType As String) As String
        Return "data:" & MimeType & ";base64," & Bytes.ToBase64()
    End Function

    ''' <summary>
    ''' Converte um arquivo uma DATA URL Completa
    ''' </summary>
    ''' <param name="File">Arquivo</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToDataURL(File As FileInfo) As String
        Return ToDataURL(File.ToBytes, New FileType(File.Extension))
    End Function

    ''' <summary>
    ''' Transforma uma imagem em uma URL Base64
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <returns>Uma DataURI em string</returns>
    <Extension()>
    Public Function ToDataURL(Image As System.Drawing.Image) As String
        Return "data:" & Image.GetFileType.First.ToLower.Replace("application/octet-stream", GetFileType(".png").First) & ";base64," & Image.ToBase64()
    End Function

    ''' <summary>
    ''' Converte uma Imagem para String Base64
    ''' </summary>
    ''' <param name="OriginalImage">Imagem original, tipo Image() (Picturebox.Image, Picturebox.BackgroundImage etc.)</param>
    ''' <param name="OriginalImageFormat">Formato da imagem de acordo com sua extensão (JPG, PNG, GIF etc.)</param>
    ''' <returns>Uma string em formato Base64</returns>

    <Extension()>
    Public Function ToBase64(ByVal OriginalImage As Image, ByVal OriginalImageFormat As System.Drawing.Imaging.ImageFormat) As String
        Using ms As New MemoryStream
            OriginalImage.Save(ms, OriginalImageFormat)
            Dim imageBytes As Byte() = ms.ToArray()
            Return Convert.ToBase64String(imageBytes)
        End Using
    End Function

    ''' <summary>
    ''' Converte uma imagem para DataURI trocando o MIME Type
    ''' </summary>
    ''' <param name="OriginalImage">Imagem</param>
    ''' <param name="OriginalImageFormat">Formato da Imagem</param>
    ''' <returns>Uma data URI com a imagem convertida</returns>
    <Extension> Public Function ToDataURL(ByVal OriginalImage As Image, ByVal OriginalImageFormat As System.Drawing.Imaging.ImageFormat) As String
        Return OriginalImage.ToBase64(OriginalImageFormat).Base64ToImage().ToDataURL()
    End Function

    ''' <summary>
    ''' Converte uma Imagem para String Base64
    ''' </summary>
    ''' <param name="OriginalImage">Imagem original, tipo Image() (Picturebox.Image, Picturebox.BackgroundImage etc.)</param>
    ''' <returns>Uma string em formato Base64</returns>

    <Extension()>
    Public Function ToBase64(ByVal OriginalImage As Image) As String
        Using ms As New MemoryStream
            OriginalImage.Save(ms, GetImageFormat(OriginalImage))
            Dim imageBytes As Byte() = ms.ToArray()
            Return Convert.ToBase64String(imageBytes)
        End Using
    End Function

    ''' <summary>
    ''' Converte uma Imagem da WEB para String Base64
    ''' </summary>
    ''' <param name="ImageURL">Caminho da imagem</param>
    ''' <returns>Uma string em formato Base64</returns>

    <Extension()>
    Public Function ToBase64(ImageURL As Uri) As String
        Dim imagem As System.Drawing.Image = GetImage(ImageURL.AbsoluteUri)
        Using m As New MemoryStream()
            imagem.Save(m, imagem.RawFormat)
            Dim imageBytes As Byte() = m.ToArray()
            Dim base64String As String = Convert.ToBase64String(imageBytes)
            Return base64String
        End Using
    End Function

    ''' <summary>
    ''' Converte uma Imagem da WEB para String Base64
    ''' </summary>
    ''' <param name="ImageURL">Caminho da imagem</param>
    ''' <param name="OriginalImageFormat">Formato da imagem de acordo com sua extensão (JPG, PNG, GIF etc.)</param>
    ''' <returns>Uma string em formato Base64</returns>

    <Extension()>
    Public Function ToBase64(ImageURL As String, ByVal OriginalImageFormat As System.Drawing.Imaging.ImageFormat) As String
        Dim imagem As System.Drawing.Image = System.Drawing.Image.FromStream(System.Net.WebRequest.Create(String.Format(ImageURL)).GetResponse().GetResponseStream())
        Using m As New MemoryStream()
            imagem.Save(m, OriginalImageFormat)
            Dim imageBytes As Byte() = m.ToArray()
            Dim base64String As String = Convert.ToBase64String(imageBytes)
            Return base64String
        End Using
    End Function

    ''' <summary>
    ''' Converte uma String DataURL ou Base64 para Imagem
    ''' </summary>
    ''' <param name="DataUrlOrBase64String">A string Base64 a ser convertida</param>
    ''' <param name="Width">Altura da nova imagem (não preencher retorna o tamanho original da imagem)</param>
    ''' <param name="Height">Largura da nova imagem (não preencher retorna o tamanho original da imagem)</param>
    ''' <returns>Uma imagem (componente Image)</returns>

    <Extension()>
    Public Function Base64ToImage(DataUrlOrBase64String As String, Optional Width As Integer = 0, Optional Height As Integer = 0) As Image
        Try
            If DataUrlOrBase64String.IsBlank Then Return Nothing
            If DataUrlOrBase64String.Contains(",") Then
                DataUrlOrBase64String = DataUrlOrBase64String.Split(",")(1)
            End If
            Dim imageBytes As Byte() = Convert.FromBase64String(DataUrlOrBase64String.FixBase64)
            Dim ms = New MemoryStream(imageBytes, 0, imageBytes.Length)
            ms.Write(imageBytes, 0, imageBytes.Length)
            If Width > 0 And Height > 0 Then
                Return Resize(Image.FromStream(ms, True), Width, Height, False)
            Else
                Return Image.FromStream(ms, True)
            End If
        Catch ex As Exception
            Throw New InvalidDataException("Invalid Base64 or DataURL string or Base64 format is not an Image", ex)
        End Try
    End Function


    ''' <summary>
    ''' Converte um array de bytes para imagem
    ''' </summary>
    ''' <param name="Bytes">Bytes</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToImage(Bytes As Byte()) As Image
        Using s As New MemoryStream(Bytes)
            Return Image.FromStream(s)
        End Using
    End Function





    ''' <summary>
    ''' Converte uma DATAURL ou Base64 String em um array de Bytes
    ''' </summary>
    ''' <param name="Base64StringOrDataURL">Base64 String ou DataURL</param>
    ''' <returns></returns>
    <Extension()> Public Function Base64ToBytes(Base64StringOrDataURL As String) As Byte()
        Return Convert.FromBase64String(Base64StringOrDataURL.FixBase64)
    End Function

    ''' <summary>
    ''' Cria um arquivo fisico a partir de uma Base64 ou DataURL
    ''' </summary>
    ''' <param name="Base64StringOrDataURL"></param>
    ''' <param name="FilePath"></param>
    ''' <returns></returns>
    <Extension()> Public Function CreateFileFromDataURL(Base64StringOrDataURL As String, FilePath As String) As FileInfo
        Return Base64StringOrDataURL.Base64ToBytes.WriteToFile(FilePath)
    End Function

End Module