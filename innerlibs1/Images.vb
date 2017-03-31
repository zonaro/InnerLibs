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


    Public Function InsertWatermark(Image As Image, Watermark As String, Optional X As Integer = -1, Optional Y As Integer = -1) As Image
        Return InsertWatermark(Image, Watermark.DrawImage(Image.Width, Image.Height))
    End Function
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