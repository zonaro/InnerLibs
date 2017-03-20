Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Web
''' <summary>
''' Modulo para manipulação de imagens e Strings Base64
''' </summary>
''' <remarks></remarks>
Public Module Base64

    ''' <summary>
    ''' Transforma uma imagem em uma URL Base64
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <returns>Uma DataURI em string</returns>
    <Extension()>
    Public Function ToDataURI(Image As Image) As String
        Return "data:" & Image.GetFileType.First.ToLower.Replace("application/octet-stream", GetFileType(".jpg").First) & ";base64," & Image.ToBase64()
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
    <Extension> Public Function ToDataURI(ByVal OriginalImage As Image, ByVal OriginalImageFormat As System.Drawing.Imaging.ImageFormat) As String
        Return OriginalImage.ToBase64(OriginalImageFormat).ToImage().ToDataURI()
    End Function
    ''' <summary>
    ''' Converte uma Imagem para String Base64
    ''' </summary>
    ''' <param name="OriginalImage">Imagem original, tipo Image() (Picturebox.Image, Picturebox.BackgroundImage etc.)</param>
    ''' <returns>Uma string em formato Base64</returns>

    <Extension()>
    Public Function ToBase64(ByVal OriginalImage As Image) As String
        Using ms As New MemoryStream
            OriginalImage.Save(ms, OriginalImage.RawFormat)
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
    Public Function ToBase64(ImageURL As String) As String
        Dim imagem As System.Drawing.Image = System.Drawing.Image.FromStream(System.Net.WebRequest.Create(String.Format(ImageURL)).GetResponse().GetResponseStream())
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
    ''' Converte uma String Base64 para Imagem
    ''' </summary>
    ''' <param name="Base64String">A string Base64 a ser convertida</param>
    ''' <param name="Width">Altura da nova imagem (não preencher retorna o tamanho original da imagem)</param>
    ''' <param name="Height">Largura da nova imagem (não preencher retorna o tamanho original da imagem)</param>
    ''' <returns>Uma imagem (componente Image())</returns>

    <Extension()>
    Public Function ToImage(Base64String As String, Optional Width As Integer = 0, Optional Height As Integer = 0) As Image
        Dim imageBytes As Byte() = Convert.FromBase64String(Base64String)
        Dim ms = New MemoryStream(imageBytes, 0, imageBytes.Length)
        ms.Write(imageBytes, 0, imageBytes.Length)
        If Width > 0 And Height > 0 Then
            Return Resize(Image.FromStream(ms, True), Width, Height, False)
        Else
            Return Image.FromStream(ms, True)
        End If

    End Function

    ''' <summary>
    ''' Converte um httpPostedFile para imagem
    ''' </summary>
    ''' <param name="PostedFile">Arquivo HttpPostedFile</param>
    ''' <returns>uma Image</returns>
    <Extension>
    Public Function ToImage(PostedFile As HttpPostedFile) As Image
        Return Image.FromStream(PostedFile.InputStream)
    End Function

    ''' <summary>
    ''' Converte uma Imagem dem HttpPostedFile para String Base64
    ''' </summary>
    ''' <param name="PostedFile">Arquivo de Imagem</param>
    ''' <param name="DataUrl">Especifica se a resposta deve ser em DataURI ou apenas a Base64</param>
    ''' <returns>Uma string em formato Base64</returns>
    <Extension>
    Public Function ToBase64(PostedFile As HttpPostedFile, Optional DataUrl As Boolean = False) As String

        Dim input As Stream = PostedFile.InputStream
        Dim streamLength As Integer = Convert.ToInt32(input.Length)
        Dim fileData As Byte() = New Byte(streamLength) {}
        input.Read(fileData, 0, streamLength)
        Return If(DataUrl, "data:" & PostedFile.ContentType.ToLower() & ";base64," & Convert.ToBase64String(fileData.ToArray()), Convert.ToBase64String(fileData.ToArray()))

    End Function


    ''' <summary>
    ''' Converte uma Imagem dem HttpPostedFile para uma Data URI
    ''' </summary>
    ''' <param name="PostedFile">Arquivo de Imagem</param>
    ''' <returns>Uma data URI Base64</returns>
    <Extension>
    Public Function ToDataURI(PostedFile As HttpPostedFile) As String
        Return PostedFile.ToBase64(True)
    End Function
End Module