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
    ''' Inverte as cores de uma imagem
    ''' </summary>
    ''' <param name="Img"></param>
    ''' <returns></returns>
    <Extension()> Public Function InvertImageColors(Img As Image) As Bitmap
        Dim bm As New Bitmap(Img)
        Dim X As Integer
        Dim Y As Integer
        Dim r As Integer
        Dim g As Integer
        Dim b As Integer

        For X = 0 To bm.Width - 1
            For Y = 0 To bm.Height - 1
                r = 255 - bm.GetPixel(X, Y).R
                g = 255 - bm.GetPixel(X, Y).G
                b = 255 - bm.GetPixel(X, Y).B
                bm.SetPixel(X, Y, Color.FromArgb(r, g, b))
            Next Y
        Next X
        Return bm
    End Function

    ''' <summary>
    ''' Corta uma imagem para um quadrado perfeito a partir do centro
    ''' </summary>
    ''' <param name="img">Imagem</param>
    ''' <param name="WidthHeight">Tamanho do quadrado em pixels</param>
    ''' <returns></returns>
    <Extension> Public Function CropToSquare(Img As Image, Optional WidthHeight As Integer = 0) As Image
        If WidthHeight < 1 Then
            WidthHeight = If(Img.Height > Img.Width, Img.Width, Img.Height)
        End If
        Return Img.Crop(WidthHeight, WidthHeight)
    End Function

    ''' <summary>
    ''' Corta a imagem em um circulo
    ''' </summary>
    ''' <param name="Img">Imagem</param>
    ''' <param name="Background">Cor do fundo</param>
    ''' <returns></returns>
    <Extension()> Public Function CropToCircle(Img As Image, Optional Background As Color? = Nothing) As Image
        Dim dstImage As Bitmap = New Bitmap(Img.Width, Img.Height)
        Dim g As Graphics = Graphics.FromImage(dstImage)
        Background = If(Background, Color.Transparent)
        Using br As Brush = New SolidBrush(Background)
            g.FillRectangle(br, 0, 0, dstImage.Width, dstImage.Height)
        End Using
        Dim path As New GraphicsPath()
        path.AddEllipse(0, 0, dstImage.Width, dstImage.Height)
        g.SetClip(path)
        g.DrawImage(Img, 0, 0)
        Return dstImage
    End Function

    ''' <summary>
    ''' Rotaciona uma imagem para sua pocisão original caso ela já tenha sido rotacionada (EXIF)
    ''' </summary>
    ''' <param name="Img">Imagem</param>
    ''' <returns>TRUE caso a imagem ja tenha sido rotacionada</returns>
    <Extension()>
    Public Function TestAndRotate(ByRef Img As Image) As Boolean
        Dim rft As RotateFlipType = RotateFlipType.RotateNoneFlipNone
        Dim properties As PropertyItem() = Img.PropertyItems
        Dim bReturn As Boolean = False
        For Each p As PropertyItem In properties
            If p.Id = 274 Then
                Dim orientation As Short = BitConverter.ToInt16(p.Value, 0)
                Select Case orientation
                    Case 1
                        rft = RotateFlipType.RotateNoneFlipNone
                    Case 3
                        rft = RotateFlipType.Rotate180FlipNone
                    Case 6
                        rft = RotateFlipType.Rotate90FlipNone
                    Case 8
                        rft = RotateFlipType.Rotate270FlipNone
                End Select
            End If
        Next
        If rft <> RotateFlipType.RotateNoneFlipNone Then
            Img.RotateFlip(rft)
            bReturn = True
        End If
        Return bReturn

    End Function

    ''' <summary>
    ''' Insere um texto de marca Dágua na imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <param name="Watermark">TExto de Marca Dagua</param>
    ''' <param name="X">Posição X</param>
    ''' <param name="Y">Posição Y</param>
    ''' <returns></returns>
    <Extension()> Public Function InsertWatermark(Image As Image, Watermark As String, Optional Font As String = "Arial", Optional FontColor As Color? = Nothing, Optional BackColor As Color? = Nothing, Optional X As Integer = -1, Optional Y As Integer = -1) As Image
        BackColor = If(BackColor, New Bitmap(Image).GetPixel(0, 0))
        Dim _Font = New Font(Font, 8)
        While TextRenderer.MeasureText(Watermark, _Font).Width.ChangeType(Of Decimal).CalculatePercent(Image.Width) <= 70
            _Font = New Font(_Font.FontFamily.Name, _Font.Size + 1)
        End While
        Dim w = Watermark.DrawImage(_Font, If(FontColor, Color.DimGray), BackColor)
        Return Image.InsertWatermark(w)
    End Function

    ''' <summary>
    ''' Insere uma imagem de marca Dágua na imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <param name="Watermark">Imagem de Marca Dagua</param>
    ''' <param name="X">Posição X</param>
    ''' <param name="Y">Posição Y</param>
    ''' <returns></returns>
    <Extension()> Public Function InsertWatermark(Image As Image, WaterMark As Image, Optional X As Integer = -1, Optional Y As Integer = -1) As Image
        ' a imagem onde iremos aplicar a marca dágua
        Dim bm_Resultado As New Bitmap(Image)

        ' a imagem que será usada como marca d'agua
        Dim bm_marcaDagua As New Bitmap(WaterMark.Resize(bm_Resultado.Width - 5, bm_Resultado.Height - 5))

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
    ''' Remove os excessos de uma cor de fundo de uma imagem deixando apenas seu conteudo
    ''' </summary>
    ''' <param name="Img"></param>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    <Extension()> Public Function Trim(Img As Image, Color As Color) As Image

        Dim bitmap = New Bitmap(Img)
        Dim w As Integer = bitmap.Width
        Dim h As Integer = bitmap.Height

        Dim IsAllWhiteRow As Func(Of Integer, Boolean) = Function(row)
                                                             For i As Integer = 0 To w - 1
                                                                 If bitmap.GetPixel(i, row).ToArgb <> Color.ToArgb Then
                                                                     Return False
                                                                 End If
                                                             Next
                                                             Return True

                                                         End Function

        Dim IsAllWhiteColumn As Func(Of Integer, Boolean) = Function(col)
                                                                For i As Integer = 0 To h - 1
                                                                    If bitmap.GetPixel(col, i).ToArgb <> Color.ToArgb Then
                                                                        Return False
                                                                    End If
                                                                Next
                                                                Return True

                                                            End Function

        Dim leftMost As Integer = 0
        For col As Integer = 0 To w - 1
            If IsAllWhiteColumn(col) Then
                leftMost = col + 1
            Else
                Exit For
            End If
        Next

        Dim rightMost As Integer = w - 1
        For col As Integer = rightMost To 1 Step -1
            If IsAllWhiteColumn(col) Then
                rightMost = col - 1
            Else
                Exit For
            End If
        Next

        Dim topMost As Integer = 0
        For row As Integer = 0 To h - 1
            If IsAllWhiteRow(row) Then
                topMost = row + 1
            Else
                Exit For
            End If
        Next

        Dim bottomMost As Integer = h - 1
        For row As Integer = bottomMost To 1 Step -1
            If IsAllWhiteRow(row) Then
                bottomMost = row - 1
            Else
                Exit For
            End If
        Next

        If rightMost = 0 AndAlso bottomMost = 0 AndAlso leftMost = w AndAlso topMost = h Then
            Return bitmap
        End If

        Dim croppedWidth As Integer = rightMost - leftMost + 1
        Dim croppedHeight As Integer = bottomMost - topMost + 1

        Try
            Dim target As New Bitmap(croppedWidth, croppedHeight)
            Using g As Graphics = Graphics.FromImage(target)
                g.DrawImage(bitmap, New RectangleF(0, 0, croppedWidth, croppedHeight), New RectangleF(leftMost, topMost, croppedWidth, croppedHeight), GraphicsUnit.Pixel)
            End Using
            Return target
        Catch ex As Exception
            Throw New Exception(String.Format("Values are top={0} bottom={1} left={2} right={3}", topMost, bottomMost, leftMost, rightMost), ex)
        End Try
    End Function

    ''' <summary>
    ''' Escreve uma string em uma imagem
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <param name="Font">Fonte que será usada</param>
    ''' <param name="TextColor">Cor do texto</param>
    ''' <param name="BackColor">Cor de fundo</param>
    ''' <returns></returns>
    <Extension>
    Public Function DrawImage(Text As String, Optional Font As Font = Nothing, Optional TextColor As Color? = Nothing, Optional BackColor As Color? = Nothing) As Image

        If Not TextColor.HasValue Then TextColor = Color.Black
        Font = If(Font, New Font("Arial", 20))
        Dim textsize = TextRenderer.MeasureText(Text, Font)
        textsize.Width = textsize.Width + 1
        textsize.Height = textsize.Height + 1

        Dim bmp As New Bitmap(textsize.Width, textsize.Height)
        Dim graph As Graphics = Graphics.FromImage(bmp)
        Dim point As New Point(textsize.Width / 2, textsize.Height / 2)
        Dim BrushForeColor As New SolidBrush(TextColor.Value)
        If BackColor.HasValue Then
            Dim BrushBackColor As New SolidBrush(BackColor.Value)
            graph.FillRectangle(BrushBackColor, 0, 0, textsize.Width, textsize.Height)
        End If

        Dim stringFormat As New StringFormat()
        stringFormat.Alignment = StringAlignment.Center
        stringFormat.LineAlignment = StringAlignment.Center

        graph.DrawString(Text, Font, BrushForeColor, point, stringFormat)
        Return bmp
    End Function

    ''' <summary>
    ''' Cropa uma imagem a patir do centro
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <param name="Size">Tamanho</param>
    ''' <returns></returns>
    <Extension()> Public Function Crop(Image As Image, Size As Size) As Image
        Return Image.Crop(Size.Width, Size.Height)
    End Function

    ''' <summary>
    ''' Cropa uma imagem a patir do centro
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <param name="maxWidth">Largura maxima</param>
    ''' <param name="maxHeight">Altura maxima</param>
    ''' <returns></returns>
    <Extension()> Public Function Crop(Image As Image, MaxWidth As Integer, MaxHeight As Integer) As Image
        Dim jpgInfo As ImageCodecInfo = ImageCodecInfo.GetImageEncoders().Where(Function(codecInfo) codecInfo.MimeType = "image/png").First()
        Dim finalImage As Image = Image
        Dim bitmap As System.Drawing.Bitmap = Nothing

        Dim left As Integer = 0
        Dim top As Integer = 0
        Dim srcWidth As Integer = MaxWidth
        Dim srcHeight As Integer = MaxHeight
        bitmap = New System.Drawing.Bitmap(MaxWidth, MaxHeight)
        Dim croppedHeightToWidth As Double = CDbl(MaxHeight) / MaxWidth
        Dim croppedWidthToHeight As Double = CDbl(MaxWidth) / MaxHeight

        If Image.Width > Image.Height Then
            srcWidth = CInt(Math.Round(Image.Height * croppedWidthToHeight))
            If srcWidth < Image.Width Then
                srcHeight = Image.Height
                left = (Image.Width - srcWidth) / 2
            Else
                srcHeight = CInt(Math.Round(Image.Height * (CDbl(Image.Width) / srcWidth)))
                srcWidth = Image.Width
                top = (Image.Height - srcHeight) / 2
            End If
        Else
            srcHeight = CInt(Math.Round(Image.Width * croppedHeightToWidth))
            If srcHeight < Image.Height Then
                srcWidth = Image.Width
                top = (Image.Height - srcHeight) / 2
            Else
                srcWidth = CInt(Math.Round(Image.Width * (CDbl(Image.Height) / srcHeight)))
                srcHeight = Image.Height
                left = (Image.Width - srcWidth) / 2
            End If
        End If
        Using g As Graphics = Graphics.FromImage(bitmap)
            g.SmoothingMode = SmoothingMode.HighQuality
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.CompositingQuality = CompositingQuality.HighQuality
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.DrawImage(Image, New Rectangle(0, 0, bitmap.Width, bitmap.Height), New Rectangle(left, top, srcWidth, srcHeight), GraphicsUnit.Pixel)
            finalImage = bitmap
        End Using
        Using encParams As New EncoderParameters(1)
            encParams.Param(0) = New EncoderParameter(Encoder.Quality, CLng(100))
            Return finalImage
        End Using

    End Function

    ''' <summary>
    ''' Converte uma Imagem para Escala de cinza
    ''' </summary>
    ''' <param name="source">imagem original</param>
    ''' <returns></returns>
    <Extension()> Public Function ConvertToGrayscale(ByVal source As Image) As Bitmap
        Dim source_bit = New Bitmap(source)
        Dim bm As Bitmap = New Bitmap(source.Width, source.Height)
        For y As Integer = 0 To bm.Height - 1
            For x As Integer = 0 To bm.Width - 1
                Dim c As Color = source_bit.GetPixel(x, y)
                If Not c = Color.Transparent Then
                    Dim average As Integer = (Convert.ToInt32(c.R) + Convert.ToInt32(c.G) + Convert.ToInt32(c.B)) / 3
                    bm.SetPixel(x, y, Color.FromArgb(average, average, average))
                End If
            Next
        Next
        Return bm
    End Function

    ''' <summary>
    ''' Redimensiona uma imagem para o tamanho definido por uma porcentagem
    ''' </summary>
    ''' <param name="Original"></param>
    ''' <param name="Percent">Porcentagem ( no formato '30% 'ou '20% x 10%')</param>
    ''' <param name="OnlyResizeIfWider"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ResizePercent(Original As Image, Percent As String, Optional OnlyResizeIfWider As Boolean = True) As Image
        Dim size = New Size
        If Percent.Contains("x") Then
            Dim parts = Percent.Split("x")
            If parts(0).AdjustBlankSpaces.EndsWith("%") Then
                parts(0) = CalculateValueFromPercent(parts(0).AdjustBlankSpaces, Original.Width).Round.ToString
            End If

            If parts(1).AdjustBlankSpaces.EndsWith("%") Then
                parts(1) = CalculateValueFromPercent(parts(1).AdjustBlankSpaces, Original.Height).Round.ToString
            End If

            size = New Size(parts(0).ToInteger, parts(1).ToInteger)
        Else
            If Percent.AdjustBlankSpaces.EndsWith("%") Then
                Percent = Percent.Trim("%").AdjustBlankSpaces
            End If
            If Percent.IsNumber Then
                size.Width = CalculateValueFromPercent(Percent.ToInteger, Original.Width).Round.ToString
                size.Height = CalculateValueFromPercent(Percent.ToInteger, Original.Height).Round.ToString
            End If

        End If

        Return Original.Resize(size, OnlyResizeIfWider)
    End Function

    <Extension()>
    Public Function ResizePercent(Original As Image, Percent As Decimal, Optional OnlyResizeIfWider As Boolean = True) As Image
        Return Original.ResizePercent(Percent.ToPercentString(), OnlyResizeIfWider)
    End Function

    ''' <summary>
    ''' Redimensiona e converte uma Imagem
    ''' </summary>
    ''' <param name="Original">Imagem Original</param>
    ''' <param name="ResizeExpression">uma string contendo uma expressão de tamanho</param>
    ''' <param name="OnlyResizeIfWider">Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada</param>
    ''' <returns></returns>
    <Extension()>
    Public Function Resize(Original As Image, ResizeExpression As String, Optional OnlyResizeIfWider As Boolean = True) As Image
        If ResizeExpression.Contains("%") Then
            Return Original.ResizePercent(ResizeExpression, OnlyResizeIfWider)
        Else
            Dim s = ResizeExpression.ToSize()
            Return Original.Resize(s, OnlyResizeIfWider)
        End If

    End Function

    ''' <summary>
    ''' Redimensiona e converte uma Imagem
    ''' </summary>
    ''' <param name="Original">Imagem Original</param>
    ''' <param name="Size">Tamanho</param>
    ''' <param name="OnlyResizeIfWider">Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada</param>
    ''' <returns></returns>
    <Extension()>
    Public Function Resize(Original As Image, Size As Size, Optional OnlyResizeIfWider As Boolean = True) As Image
        Return Original.Resize(Size.Width, Size.Height, OnlyResizeIfWider)
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
    ''' Interperta uma string de diversas formas e a transforma em um <see cref="Size"/>
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns></returns>
    <Extension> Public Function ToSize(ByVal Text As String) As Size
        Dim s As New Size
        Text = Text.ReplaceMany("", "px", " ", ";", ":").ToLower.Trim
        Try
            Select Case True
                Case Text.IsNumber
                    s.Width = Text
                    s.Height = Text
                    Exit Select
                Case Text Like "width*" And Not Text Like "*height*"
                    s.Width = Text.GetAfter("width")
                    s.Height = Text.GetAfter("width")
                    Exit Select
                Case Text Like "height*" And Not Text Like "*width*"
                    s.Width = Text.GetAfter("height")
                    s.Height = Text.GetAfter("height")
                    Exit Select
                Case Text Like "w*" And Not Text Like "*h*"
                    s.Width = Text.GetAfter("w")
                    s.Height = Text.GetAfter("w")
                    Exit Select
                Case Text Like "h*" And Not Text Like "*w*"
                    s.Width = Text.GetAfter("h")
                    s.Height = Text.GetAfter("h")
                    Exit Select
                Case Text Like "width*height*"
                    s.Width = Text.GetBetween("width", "height")
                    s.Height = Text.GetAfter("height")
                    Exit Select

                Case Text Like "height*width*"
                    s.Height = Text.GetBetween("height", "width")
                    s.Width = Text.GetAfter("width")
                    Exit Select

                Case Text Like "w*h*"
                    s.Width = Text.GetBetween("w", "h")
                    s.Height = Text.GetAfter("h")
                    Exit Select

                Case Text Like "h*w*"
                    s.Height = Text.GetBetween("h", "w")
                    s.Width = Text.GetAfter("w")
                    Exit Select

                Case Text Like "*x*"
                    s.Width = Text.Split({"x"}, StringSplitOptions.RemoveEmptyEntries)(0)
                    s.Height = Text.Split({"x"}, StringSplitOptions.RemoveEmptyEntries)(1)
                    Exit Select

                Case Text Like "*by*"
                    s.Width = Text.Split({"by"}, StringSplitOptions.RemoveEmptyEntries)(0)
                    s.Height = Text.Split({"by"}, StringSplitOptions.RemoveEmptyEntries)(1)
                    Exit Select

                Case Text Like "*por*"
                    s.Width = Text.Split({"por"}, StringSplitOptions.RemoveEmptyEntries)(0)
                    s.Height = Text.Split({"por"}, StringSplitOptions.RemoveEmptyEntries)(1)
                    Exit Select
                Case Text Like "*,*"
                    s.Width = Text.Split({","}, StringSplitOptions.RemoveEmptyEntries)(0)
                    s.Height = Text.Split({","}, StringSplitOptions.RemoveEmptyEntries)(1)
                    Exit Select
                Case Else
            End Select
        Catch ex As Exception

        End Try

        Return s
    End Function

    ''' <summary>
    ''' Lista com todos os formatos de imagem
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ImageTypes As Imaging.ImageFormat() = {Drawing.Imaging.ImageFormat.Bmp, Drawing.Imaging.ImageFormat.Emf, Drawing.Imaging.ImageFormat.Exif, Drawing.Imaging.ImageFormat.Gif, Drawing.Imaging.ImageFormat.Icon, Drawing.Imaging.ImageFormat.Jpeg, Drawing.Imaging.ImageFormat.Png, Drawing.Imaging.ImageFormat.Tiff, Drawing.Imaging.ImageFormat.Wmf}

    ''' <summary>
    ''' Retorna o formato da imagem correspondente a aquela imagem
    ''' </summary>
    ''' <param name="OriginalImage"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetImageFormat(OriginalImage As Image) As Imaging.ImageFormat
        Return ImageTypes.Where(Function(p) p.Guid = OriginalImage.RawFormat.Guid).FirstOr(Imaging.ImageFormat.Png)
    End Function

    ''' <summary>
    ''' Pega o encoder a partir de um formato de imagem
    ''' </summary>
    ''' <param name="RawFormat">Image format</param>
    ''' <returns>image codec info.</returns>
    <Runtime.CompilerServices.Extension>
    Public Function GetEncoderInfo(RawFormat As ImageFormat) As ImageCodecInfo
        Return ImageCodecInfo.GetImageDecoders().Where(Function(c) c.FormatID = RawFormat.Guid).FirstOr(ImageCodecInfo.GetImageDecoders().Where(Function(c) c.FormatID = ImageFormat.Png.Guid).First)
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
    Public Function GetMostUsedColors(Image As Image, Optional Count As Integer = 10) As List(Of Color)
        Return New Bitmap(Image).GetMostUsedColors
    End Function

    ''' <summary>
    ''' Retorna uma lista com as <paramref name="Count"/> cores mais utilizadas na imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <returns>uma lista de Color</returns>
    <Extension>
    Public Function GetMostUsedColors(Image As Bitmap, Optional Count As Integer = 10) As List(Of Color)
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
                    pixelColor.Increment
                Else
                    dctColorIncidence.Add(pixelColor, 1)
                End If
                col.Increment
            End While
            row.Increment
        End While

        ' note that there are those who argue that a
        ' .NET Generic Dictionary is never guaranteed
        ' to be sorted by methods like this
        Dim dctSortedByValueHighToLow = dctColorIncidence.OrderByDescending(Function(x) x.Value).ToDictionary(Function(x) x.Key, Function(x) x.Value)

        ' this should be replaced with some elegant Linq ?
        For Each kvp As KeyValuePair(Of Integer, Integer) In dctSortedByValueHighToLow
            Dim cor = Color.FromArgb(kvp.Key)
            If cor <> Color.Empty AndAlso cor <> Color.Transparent Then
                TenMostUsedColors.Add(cor)
                TenMostUsedColorIncidences.Add(kvp.Value)
            End If
        Next

        MostUsedColor = Color.FromArgb(dctSortedByValueHighToLow.First().Key)
        MostUsedColorIncidence = dctSortedByValueHighToLow.First().Value
        Return TenMostUsedColors.Take(Count)
    End Function

    ''' <summary>
    ''' Transforma uma imagem em um stream
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToStream(Image As Image, Optional Format As ImageFormat = Nothing) As Stream
        Dim s As Stream = New System.IO.MemoryStream()
        Image.Save(s, If(Format, ImageFormat.Png))
        s.Position = 0
        Return s
    End Function

    ''' <summary>
    ''' Transforma uma imagem em array de bytes
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <returns></returns>
    <Extension()> Public Function ToBytes(Image As Image, Optional Format As ImageFormat = Nothing) As Byte()
        Using ms = Image.ToStream(Format)
            Return ms.ToBytes()
        End Using
    End Function

    ''' <summary>
    ''' redimensiona e Cropa uma imagem, aproveitando a maior parte dela
    ''' </summary>
    ''' <param name="Image"></param>
    ''' <param name="Width"></param>
    ''' <param name="Height"></param>
    ''' <returns></returns>
    <Extension()> Public Function ResizeCrop(Image As Image, Width As Integer, Height As Integer) As Image
        Return Image.Resize(Width, Height, False).Crop(Width, Height)
    End Function


    ''' <summary>
    ''' redimensiona e Cropa uma imagem, aproveitando a maior parte dela
    ''' </summary>
    ''' <param name="Image"></param>
    ''' <param name="Width"></param>
    ''' <param name="Height"></param>
    ''' <returns></returns>
    <Extension()> Public Function ResizeCrop(Image As Image, Width As Integer, Height As Integer, OnlyResizeIfWider As Boolean) As Image
        Return Image.Resize(Width, Height, OnlyResizeIfWider).Crop(Width, Height)
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
        url &= If("g/", Grayscale)
        url &= (Me.Size.Width & "/" & Me.Size.Height)
        url &= ("?")
        url &= If("&blur", Blur)
        If Index > -1 Then
            url &= ("&image=" & Index)
        Else
            url &= ("&random")
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
        url &= If("g/", Grayscale)
        url &= (Me.Size.Width & "/" & Me.Size.Height)
        url &= If("/" & Category, Category.IsNotBlank)
        url &= If("/" & Index.LimitRange(1, 10), Index > -1)
        url &= If("/" & System.Web.HttpUtility.UrlEncode(Text), Text.IsNotBlank)
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