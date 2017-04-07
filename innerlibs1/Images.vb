Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

''' <summary>
''' Modulo de Imagem
''' </summary>
''' <remarks></remarks>
''' 
Public Module Images

    ''' <summary>
    ''' Insere um texto de marca Dágua na imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <param name="Watermark">TExto de Marca Dagua</param>
    ''' <param name="X">Posição X</param>
    ''' <param name="Y">Posição Y</param>
    ''' <returns></returns>
    Public Function InsertWatermark(Image As Image, Watermark As String, Optional X As Integer = -1, Optional Y As Integer = -1) As Image
        Return InsertWatermark(Image, Watermark.DrawImage(Image.Width, Image.Height))
    End Function

    ''' <summary>
    ''' Insere uma imagem de marca Dágua na imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <param name="Watermark">Imagem de Marca Dagua</param>
    ''' <param name="X">Posição X</param>
    ''' <param name="Y">Posição Y</param>
    ''' <returns></returns>
    Public Function InsertWatermark(Image As Image, WaterMark As Image, Optional X As Integer = -1, Optional Y As Integer = -1) As Image
        ' a imagem que será usada como marca d'agua
        Dim bm_marcaDagua As New Bitmap(WaterMark)
        ' a imagem onde iremos aplicar a marca dágua
        Dim bm_Resultado As New Bitmap(Image)
        If X < 0 Then X = (bm_Resultado.Width - bm_marcaDagua.Width) \ 2   'centraliza a marca d'agua
        If Y < 0 Then Y = (bm_Resultado.Height - bm_marcaDagua.Height) \ 2   'centraliza a marca d'agua
        Const ALPHA As Byte = 128
        ' Define o componente Alpha do pixel
        Dim clr As Color
        For py As Integer = 0 To bm_marcaDagua.Height - 1
            For px As Integer = 0 To bm_marcaDagua.Width - 1
                clr = bm_marcaDagua.GetPixel(px, py)
                bm_marcaDagua.SetPixel(px, py, Color.FromArgb(ALPHA, clr.R, clr.G, clr.B))
            Next px
        Next py
        ' Define a marca dagua como transparente
        bm_marcaDagua.MakeTransparent(bm_marcaDagua.GetPixel(0, 0))
        ' Copia o resultado na imagem
        Dim gr As Graphics = Graphics.FromImage(bm_Resultado)
        gr.DrawImage(bm_marcaDagua, X, Y)
        Return bm_Resultado
    End Function


    ''' <summary>
    ''' Escreve uma string em uma imagem
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <param name="Width">Largura da imagem</param>
    ''' <param name="Height">Altura da imagem</param>
    ''' <param name="Font">Fonte que será usada</param>
    ''' <param name="TextColor">Cor do texto</param>
    ''' <param name="BackColor">Cor de fundo</param>
    ''' <returns></returns>
    <Extension>
    Public Function DrawImage(Text As String, Width As Integer, Height As Integer, Optional Font As Font = Nothing, Optional TextColor As Color? = Nothing, Optional BackColor As Color? = Nothing) As Image
        If Not TextColor.HasValue Then TextColor = Color.Black
        Font = If(Font, New Font("Arial", 12))
        Dim bmp As New Bitmap(Width, Height)
        Dim graph As Graphics = Graphics.FromImage(bmp)
        Dim point As New PointF(5.0F, 5.0F)
        Dim BrushForeColor As New SolidBrush(TextColor.Value)
        If BackColor.HasValue Then
            Dim BrushBackColor As New SolidBrush(BackColor.Value)
            graph.FillRectangle(BrushBackColor, 0, 0, Width, Height)
        End If
        graph.DrawString(Text, Font, BrushForeColor, point)
        Return bmp
    End Function


    ''' <summary>
    ''' Redimensiona e converte uma Imagem
    ''' </summary>
    ''' <param name="Original">Imagem Original</param>
    ''' <param name="NewWidth">Nova Largura</param>
    ''' <param name="MaxHeight">Altura máxima</param>
    ''' <param name="OnlyResizeIfWider">Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada</param>
    ''' <returns></returns>
    <Extension()>
    Public Function Resize(Original As Image, NewWidth As Integer, MaxHeight As Integer, Optional OnlyResizeIfWider As Boolean = True) As Image
        Dim fullsizeImage As Image = Original

        ' Prevent using images internal thumbnail
        fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone)
        fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone)

        If OnlyResizeIfWider Then
            If fullsizeImage.Width <= NewWidth Then
                NewWidth = fullsizeImage.Width
            End If
        End If

        Dim newHeight As Integer = fullsizeImage.Height * NewWidth / fullsizeImage.Width
        If newHeight > MaxHeight Then
            'Resize with height instead
            NewWidth = fullsizeImage.Width * MaxHeight / fullsizeImage.Height
            newHeight = MaxHeight
        End If

        fullsizeImage = fullsizeImage.GetThumbnailImage(NewWidth, newHeight, Nothing, IntPtr.Zero)
        Return fullsizeImage
    End Function




    ''' <summary>
    ''' Pega o encoder a partir de um formato de imagem
    ''' </summary>
    ''' <param name="RawFormat">Image format</param>
    ''' <returns>image codec info.</returns>

    <Runtime.CompilerServices.Extension>
    Private Function GetEncoderInfo(RawFormat As ImageFormat) As ImageCodecInfo
        Return ImageCodecInfo.GetImageDecoders().SingleOrDefault(Function(c) c.FormatID = RawFormat.Guid)
    End Function

    ''' <summary>
    ''' Combina 2 ou mais imagens em uma única imagem
    ''' </summary>
    ''' <param name="Images">Lista de Imagens para combinar</param>
    ''' <param name="VerticalFlow">Se TRUE, combina as Imagens verticalmente (Uma em baixo da outra), caso contrario as imagens serão combinadas horizontalmente (Uma do lado da outra da esquerda para a direita)</param>
    ''' <returns>Um Bitmap com a combinaçao de todas as imagens da Lista</returns>
    Public Function CombineImages(VerticalFlow As Boolean, ParamArray Images() As Image) As System.Drawing.Bitmap
        Return CombineImages(Images, VerticalFlow)
    End Function

    ''' <summary>
    ''' Combina 2 ou mais imagens em uma única imagem
    ''' </summary>
    ''' <param name="Images">Lista de Imagens para combinar</param>
    ''' <param name="VerticalFlow">Se TRUE, combina as Imagens verticalmente (Uma em baixo da outra), caso contrario as imagens serão combinadas horizontalmente (Uma do lado da outra da esquerda para a direita)</param>
    ''' <returns>Um Bitmap com a combinaçao de todas as imagens da Lista</returns>
    <Extension>
    Public Function CombineImages(Images As List(Of Image), Optional VerticalFlow As Boolean = False) As System.Drawing.Bitmap
        Return CombineImages(Images.ToArray(), VerticalFlow)
    End Function

    ''' <summary>
    ''' Combina 2 ou mais imagens em uma única imagem
    ''' </summary>
    ''' <param name="Images">Array de Imagens para combinar</param>
    ''' <param name="VerticalFlow">Se TRUE, combina as Imagens verticalmente (Uma em baixo da outra), caso contrario as imagens serão combinadas horizontalmente (Uma do lado da outra da esquerda para a direita)</param>
    ''' <returns>Um Bitmap com a combinaçao de todas as imagens do Array</returns>
    <Extension>
    Public Function CombineImages(Images As Image(), Optional VerticalFlow As Boolean = False) As System.Drawing.Bitmap
        Dim imagemFinal As Bitmap = Nothing
        Dim width As Integer = 0
        Dim height As Integer = 0

        For Each image As Image In Images
            'cria um bitmap a partir do arquivo e o inclui na lista
            Dim bitmap As New System.Drawing.Bitmap(image)

            'atualiza o tamanho da imagem bitmap final
            If VerticalFlow Then
                height += bitmap.Height
                width = If(bitmap.Width > width, bitmap.Width, width)
            Else
                width += bitmap.Width
                height = If(bitmap.Height > height, bitmap.Height, height)



            End If
        Next

        'cria um bitmap para tratar a imagem combinada
        imagemFinal = New System.Drawing.Bitmap(width, height)

        'Obtem o objeto gráfico da imagem 
        Using g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(imagemFinal)
            'define a cor de fundo
            g.Clear(System.Drawing.Color.White)

            'percorre imagem por imagem e gera uma unica imagem final
            Dim offset As Integer = 0
            For Each image As System.Drawing.Bitmap In Images
                If VerticalFlow Then
                    g.DrawImage(image, New Rectangle(0, offset, image.Width, image.Height))
                    offset += image.Height
                Else
                    g.DrawImage(image, New Rectangle(offset, 0, image.Width, image.Height))
                    offset += image.Width

                End If
            Next
        End Using

        Return imagemFinal
    End Function



    ''' <summary>
    ''' Retorna uma lista com as 10 cores mais utilizadas na imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <returns>uma lista de Color</returns>
    <Extension>
    Public Function GetMostUsedColors(Image As Bitmap) As List(Of Color)
        Dim TenMostUsedColorIncidences As List(Of Integer)
        Dim TenMostUsedColors As List(Of Color)
        Dim MostUsedColor As Color
        Dim MostUsedColorIncidence As Integer

        Dim pixelColor As Integer

        Dim dctColorIncidence As Dictionary(Of Integer, Integer)
        TenMostUsedColors = New List(Of Color)()
        TenMostUsedColorIncidences = New List(Of Integer)()

        MostUsedColor = Color.Empty
        MostUsedColorIncidence = 0

        ' does using Dictionary<int,int> here
        ' really pay-off compared to using
        ' Dictionary<Color, int> ?

        ' would using a SortedDictionary be much slower, or ?

        dctColorIncidence = New Dictionary(Of Integer, Integer)()

        ' this is what you want to speed up with unmanaged code
        Dim row As Integer = 0
        While row < Image.Size.Width
            Dim col As Integer = 0
            While col < Image.Size.Height
                pixelColor = Image.GetPixel(row, col).ToArgb()

                If dctColorIncidence.Keys.Contains(pixelColor) Then
                    System.Math.Max(System.Threading.Interlocked.Increment(dctColorIncidence(pixelColor)), dctColorIncidence(pixelColor) - 1)
                Else
                    dctColorIncidence.Add(pixelColor, 1)
                End If
                System.Math.Max(System.Threading.Interlocked.Increment(col), col - 1)
            End While
            System.Math.Max(System.Threading.Interlocked.Increment(row), row - 1)
        End While

        ' note that there are those who argue that a
        ' .NET Generic Dictionary is never guaranteed
        ' to be sorted by methods like this
        Dim dctSortedByValueHighToLow = dctColorIncidence.OrderByDescending(Function(x) x.Value).ToDictionary(Function(x) x.Key, Function(x) x.Value)

        ' this should be replaced with some elegant Linq ?
        For Each kvp As KeyValuePair(Of Integer, Integer) In dctSortedByValueHighToLow.Take(10)
            TenMostUsedColors.Add(Color.FromArgb(kvp.Key))
            TenMostUsedColorIncidences.Add(kvp.Value)
        Next

        MostUsedColor = Color.FromArgb(dctSortedByValueHighToLow.First().Key)
        MostUsedColorIncidence = dctSortedByValueHighToLow.First().Value
        Return TenMostUsedColors
    End Function

    ''' <summary>
    ''' Transforma uma imagem em array de bytes
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToBytes(Image As Image, Optional Format As ImageFormat = Nothing) As Byte()
        Using mStream As New MemoryStream()
            Image.Save(mStream, If(Format, ImageFormat.Jpeg))
            Return mStream.ToArray()
        End Using
    End Function

End Module

''' <summary>
''' Retorna imagens de diversos serviços para serem usadas como marcação ou sugestão.
''' </summary>
Public Class PictureService

    Class Picture
        ''' <summary>
        ''' URL da imagem
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property URL As Uri

        ''' <summary>
        ''' Objeto imagem
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Image As Image
            Get
                Return AJAX.GET(Of Image)(URL.AbsoluteUri)
            End Get

        End Property

        ''' <summary>
        ''' DATA URL da imagem (base64)
        ''' </summary>
        ''' <param name="ImageFormat">Formato de Imagem</param>
        ''' <returns></returns>
        ReadOnly Property DataURL(Optional ImageFormat As ImageFormat = Nothing) As String
            Get
                If IsNothing(ImageFormat) Then ImageFormat = ImageFormat.Jpeg
                Return Image.ToDataURL(ImageFormat)
            End Get
        End Property

        ''' <summary>
        ''' Bytes da imagem
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Bytes As Byte()
            Get
                Return DataURL.ToBytes
            End Get
        End Property

        Friend Sub New(URL As String)
            Me.URL = New Uri(URL)
        End Sub

    End Class

    ''' <summary>
    ''' Tamanho da imagem que será gerada pelo serviço
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Size As Size

    ''' <summary>
    ''' Retorna uma imagem usando Placehold.It
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="Color"></param>
    ''' <param name="TextColor"></param>
    ''' <returns></returns>
    Public Function PlaceHold(Optional Text As String = "", Optional Color As Color? = Nothing, Optional TextColor As Color? = Nothing) As Picture

        Dim ReColor = If(Color, System.Drawing.Color.Gray)
        Dim ReColor2 = If(TextColor, System.Drawing.Color.Black)
        Dim cor1 = ReColor.ToHexadecimal(False)
        Dim cor2 = ReColor2.ToHexadecimal(False)
        Text = If(Text.IsBlank, Me.Size.Width & "x" & Me.Size.Height, Text.Replace(" ", "+"))
        Dim URL As String = "http://placehold.it/" & Me.Size.Width & "x" & Me.Size.Height & "/" & cor1 & "/" & cor2 & "?text=" & Text
        URL = URL.Replace(" ", "_")
        Return New Picture(URL)
    End Function

    ''' <summary>
    ''' Retorna uma Imagem utilizando Unsplash.it
    ''' </summary>
    ''' <param name="Index">Index da imagem</param>
    ''' <param name="Grayscale">Imagem em escala de cinza</param>
    ''' <param name="Blur">Imagem desbotada</param>
    ''' <returns></returns>
    Public Function Unsplash(Optional Index As Integer = -1, Optional Grayscale As Boolean = False, Optional Blur As Boolean = False) As Picture

        Dim url = "https://unsplash.it/"
        url.AppendIf("g/", Grayscale)
        url.Append(Me.Size.Width & "/" & Me.Size.Height)
        url.Append("?")
        url.AppendIf("&blur", Blur)
        If Index > -1 Then
            url.Append("&image=" & Index)
        Else
            url.Append("&random")
        End If
        Return New Picture(url)
    End Function

    ''' <summary>
    ''' Retorna uma imagem usando LoremPixel.com
    ''' </summary>
    ''' <param name="Category">Categoria da imagem</param>
    ''' <param name="Index">Indice da imagem</param>
    ''' <param name="Grayscale">Imagem em escala da cinza</param>
    ''' <param name="Text">Texto adicional</param>
    ''' <returns></returns>
    Public Function LoremPixel(Optional Category As String = "", Optional Index As Integer = -1, Optional Grayscale As Boolean = False, Optional Text As String = "") As Picture

        Dim url = "http://lorempixel.com/"
        url.AppendIf("g/", Grayscale)
        url.Append(Me.Size.Width & "/" & Me.Size.Height)
        url.AppendIf("/" & Category, Category.IsNotBlank)
        url.AppendIf("/" & Index.LimitRange(1, 10), Index > -1)
        url.AppendIf("/" & System.Web.HttpUtility.UrlEncode(Text), Text.IsNotBlank)
        Return New Picture(url)


    End Function

    ''' <summary>
    ''' Retorna uma imagem usando Pipsum.com
    ''' </summary>
    ''' <returns></returns>
    Public Function Pipsum() As Picture
        Return New Picture("http://pipsum.com/" & Me.Size.Width.LimitRange(1, 2560) & "x" & Me.Size.Height.LimitRange(1, 1600) & ".jpg")
    End Function

    ''' <summary>
    ''' Retorna uma imagem de qualquer serviço aleatóriamente
    ''' </summary>
    ''' <param name="OnlyPhotos">Apenas serviços de fotografias (Exclui Placehold.It)</param>
    ''' <returns></returns>
    Public Function AnyService(Optional OnlyPhotos As Boolean = True) As Picture
        Select Case RandomNumber(1, If(OnlyPhotos, 3, 4))
            Case 1
                Return Pipsum()
            Case 2
                Return LoremPixel()
            Case 3
                Return Unsplash()
            Case Else
                Return PlaceHold()
        End Select
    End Function

    ''' <summary>
    ''' Cria uma nova Picture com um tamanho especifico
    ''' </summary>
    ''' <param name="Size">Tamanho</param>
    Public Sub New(Size As Size)
        Me.Size = Size
    End Sub

    ''' <summary>
    ''' Cria uma nova Picture com altura e largura especificados
    ''' </summary>
    ''' <param name="Width">Largura</param>
    ''' <param name="Height">Altura</param>
    Public Sub New(Width As Integer, Height As Integer)
        Me.Size = New Size(Width, Height)
    End Sub



End Class