Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.CompilerServices

''' <summary>
''' Modulo de Imagem
''' </summary>
''' <remarks></remarks>
'''
Public Module Images

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
        Return Img.CropToSquare()?.CropToEllipsis(Background)
    End Function

    ''' <summary>
    ''' Corta a imagem em uma elipse
    ''' </summary>
    ''' <param name="Img">Imagem</param>
    ''' <param name="Background">Cor do fundo</param>
    ''' <returns></returns>
    <Extension()> Public Function CropToEllipsis(Img As Image, Optional Background As Color? = Nothing) As Image
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
    ''' Insere uma imagem de marca Dágua na imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <param name="WaterMarkImage">Imagem de Marca Dagua</param>
    ''' <param name="X">Posição X</param>
    ''' <param name="Y">Posição Y</param>
    ''' <returns></returns>
    <Extension()> Public Function Watermark(Image As Image, WaterMarkImage As Image, Optional X As Integer = -1, Optional Y As Integer = -1) As Image
        ' a imagem onde iremos aplicar a marca dágua
        Dim bm_Resultado As New Bitmap(Image)

        ' a imagem que será usada como marca d'agua
        Dim bm_marcaDagua As New Bitmap(WaterMarkImage.Resize(bm_Resultado.Width - 5, bm_Resultado.Height - 5))

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
    ''' Insere uma imagem em outra imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <param name="InsertedImage">Imagem de Marca Dagua</param>
    ''' <param name="X">Posição X</param>
    ''' <param name="Y">Posição Y</param>
    ''' <returns></returns>
    <Extension()> Public Function Insert(Image As Image, InsertedImage As Image, Optional X As Integer = -1, Optional Y As Integer = -1) As Image
        Dim bm_Resultado As New Bitmap(Image)
        Dim bm_marcaDagua As New Bitmap(InsertedImage.Resize(bm_Resultado.Width - 5, bm_Resultado.Height - 5))
        If X < 0 Then X = (bm_Resultado.Width - bm_marcaDagua.Width) \ 2
        If Y < 0 Then Y = (bm_Resultado.Height - bm_marcaDagua.Height) \ 2
        Dim gr As Graphics = Graphics.FromImage(bm_Resultado)
        gr.DrawImage(bm_marcaDagua, X, Y)
        Return bm_Resultado
    End Function

    <Extension> Public Function CreateSolidImage(Color As Color, Width As Integer, Height As Integer) As Image
        Dim Bmp = New Bitmap(Width, Height)
        Using gfx = Graphics.FromImage(Bmp)
            Using brush = New SolidBrush(Color)
                gfx.FillRectangle(brush, 0, 0, Width, Height)
            End Using
        End Using
        Return Bmp
    End Function


    <Extension()> Public Function DrawString(img As Image, Text As String, Optional Font As Font = Nothing, Optional Color As Color? = Nothing, Optional X As Integer = -1, Optional Y As Integer = -1) As Image
        Dim bitmap As Bitmap = New Bitmap(img)
        Using graphics As Graphics = Graphics.FromImage(bitmap)

            Font = If(Font, New Font("Arial", bitmap.Width / 10))

            Dim tamanho = graphics.MeasureString(Text, Font, New Size(bitmap.Width, bitmap.Height))

            X = X.LimitRange(-1, img.Width)
            Y = Y.LimitRange(-1, img.Height)

            If X = -1 Then
                X = (bitmap.Width / 2) - (tamanho.Width / 2)
            End If

            If Y = -1 Then
                Y = (bitmap.Height / 2) - (tamanho.Height / 2)
            End If

            Color = If(Color, bitmap.GetPixel(X, Y).GetContrastColor(50))

            Dim B = New SolidBrush(Color)

            graphics.DrawString(Text, Font, B, X, Y)

        End Using

        Return bitmap
    End Function

    <Extension()> Public Function Monochrome(Image As Image, Color As Color, Optional Alpha As Single = 0) As Image
        Return Grayscale(Image).Translate(Color.R, Color.G, Color.B, Alpha)
    End Function

    ''' <summary>
    ''' Inverte as cores de uma imagem
    ''' </summary>
    ''' <param name="Img"></param>
    ''' <returns></returns>
    <Extension()> Public Function Negative(ByVal img As Image) As Image
        Dim copia As New Bitmap(img)

        Dim cm As ColorMatrix = New ColorMatrix(New Single()() _
                           {New Single() {-1, 0, 0, 0, 0},
                            New Single() {0, -1, 0, 0, 0},
                            New Single() {0, 0, -1, 0, 0},
                            New Single() {0, 0, 0, 1, 0},
                            New Single() {0, 0, 0, 0, 1}})

        draw_adjusted_image(copia, cm)
        Return copia

    End Function

    <Extension()> Public Function Translate(ByVal img As Image, Color As Color, Optional ByVal Alpha As Single = 0) As Image
        Return Translate(img, Color.R, Color.G, Color.B, Alpha)
    End Function

    <Extension()> Public Function Translate(ByVal img As Image, ByVal Red As Single, ByVal Green As Single, ByVal Blue As Single, Optional ByVal Alpha As Single = 0) As Image

        Dim sr, sg, sb, sa As Single
        Dim copia As New Bitmap(img)
        ' noramlize the color components to 1
        sr = Red / 255
        sg = Green / 255
        sb = Blue / 255
        sa = Alpha / 255

        ' create the color matrix
        Dim cm = New ColorMatrix(New Single()() _
                       {New Single() {1, 0, 0, 0, 0},
                        New Single() {0, 1, 0, 0, 0},
                        New Single() {0, 0, 1, 0, 0},
                        New Single() {0, 0, 0, 1, 0},
                        New Single() {sr, sg, sb, sa, 1}})

        ' apply the matrix to the image
        draw_adjusted_image(copia, cm)
        Return copia
    End Function

    Private Function draw_adjusted_image(ByVal img As Image,
                ByVal cm As ColorMatrix) As Boolean
        Try
            Dim bmp As New Bitmap(img) ' create a copy of the source image
            Dim imgattr As New ImageAttributes()
            Dim rc As New Rectangle(0, 0, img.Width, img.Height)
            Dim g As Graphics = Graphics.FromImage(img)

            ' associate the ColorMatrix object with an ImageAttributes object
            imgattr.SetColorMatrix(cm)

            ' draw the copy of the source image back over the original image,
            'applying the ColorMatrix
            g.DrawImage(bmp, rc, 0, 0, img.Width, img.Height,
                               GraphicsUnit.Pixel, imgattr)

            g.Dispose()

            Return True
        Catch
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Converte uma Imagem para Escala de cinza
    ''' </summary>
    ''' <param name="img">imagem original</param>
    ''' <returns></returns>
    <Extension> Public Function Grayscale(ByVal img As Image) As Image
        Dim copia = New Bitmap(img)
        Dim cm As ColorMatrix = New ColorMatrix(New Single()() _
                           {New Single() {0.299, 0.299, 0.299, 0, 0},
                            New Single() {0.587, 0.587, 0.587, 0, 0},
                            New Single() {0.114, 0.114, 0.114, 0, 0},
                            New Single() {0, 0, 0, 1, 0},
                            New Single() {0, 0, 0, 0, 1}})
        draw_adjusted_image(copia, cm)
        Return copia
    End Function

    <Extension()> Public Function CompareARGB(Color1 As Color, Color2 As Color, Optional IgnoreAlpha As Boolean = True) As Boolean
        Return CompareARGB(Color1, IgnoreAlpha, Color2)
    End Function

    <Extension()> Public Function CompareARGB(Color1 As Color, IgnoreAlpha As Boolean, ParamArray Colors As Color()) As Boolean
        Colors = Colors.NullAsEmpty()
        Return Colors.Any(Function(Color2) (Color1.R = Color2.R) AndAlso (Color1.G = Color2.G) AndAlso (Color1.B = Color2.B) AndAlso If(IgnoreAlpha, True, (Color1.A = Color2.A)))
    End Function

    <Extension> Public Function MakeDarker(img As Image, Optional percent As Single = 50) As Image
        Dim lockedBitmap = New Bitmap(img)
        For y = 0 To lockedBitmap.Height - 1
            For x = 0 To lockedBitmap.Width - 1
                Dim oldColor = lockedBitmap.GetPixel(x, y)
                If Not oldColor.CompareARGB(True, Color.Transparent, Color.Black, Color.White) Then
                    Dim newColor = oldColor.MakeDarker(percent)
                    lockedBitmap.SetPixel(x, y, newColor)
                End If
            Next
        Next
        Return lockedBitmap
    End Function

    <Extension> Public Function MakeLighter(img As Image, Optional percent As Single = 50) As Image
        Dim lockedBitmap = New Bitmap(img)
        For y = 0 To lockedBitmap.Height - 1
            For x = 0 To lockedBitmap.Width - 1
                Dim oldColor = lockedBitmap.GetPixel(x, y)
                If Not oldColor.CompareARGB(True, Color.Transparent, Color.Black, Color.White) Then
                    Dim newColor = oldColor.MakeLighter(percent)
                    lockedBitmap.SetPixel(x, y, newColor)
                End If
            Next
        Next
        Return lockedBitmap
    End Function

    <Extension> Public Function BrightnessContrastAndGamma(originalimage As Image, Brightness As Single, Contrast As Single, Gamma As Single) As Image
        Dim adjustedImage As New Bitmap(originalimage)
        Gamma = Gamma.SetMinValue(1.0F)
        Contrast = Contrast.SetMinValue(1.0F)
        Dim adjustedBrightness As Single = Brightness.SetMinValue(1.0F) - 1.0F
        Dim ptsArray As Single()() = {New Single() {Contrast, 0, 0, 0, 0}, New Single() {0, Contrast, 0, 0, 0}, New Single() {0, 0, Contrast, 0, 0}, New Single() {0, 0, 0, 1.0F, 0}, New Single() {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}}
        Dim imageAttributes As ImageAttributes = New ImageAttributes()
        imageAttributes.ClearColorMatrix()
        imageAttributes.SetColorMatrix(New ColorMatrix(ptsArray), ColorMatrixFlag.[Default], ColorAdjustType.Bitmap)
        imageAttributes.SetGamma(Gamma, ColorAdjustType.Bitmap)
        Dim g As Graphics = Graphics.FromImage(adjustedImage)
        g.DrawImage(originalimage, New Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), 0, 0, originalimage.Width, originalimage.Height, GraphicsUnit.Pixel, imageAttributes)
        Return adjustedImage
    End Function

    ''' <summary>
    ''' Remove os excessos de uma cor de fundo de uma imagem deixando apenas seu conteudo
    ''' </summary>
    ''' <param name="Img"></param>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    <Extension()> Public Function Trim(Img As Image, Optional Color As Color? = Nothing) As Image
        Dim bitmap = New Bitmap(Img)

        Color = If(Color, bitmap.GetPixel(0, 0))

        Dim w As Integer = bitmap.Width
        Dim h As Integer = bitmap.Height

        Dim IsAllWhiteRow As Func(Of Integer, Boolean) = Function(row)
                                                             For i As Integer = 0 To w - 1
                                                                 If bitmap.GetPixel(i, row).ToArgb <> Color.Value.ToArgb Then
                                                                     Return False
                                                                 End If
                                                             Next
                                                             Return True

                                                         End Function

        Dim IsAllWhiteColumn As Func(Of Integer, Boolean) = Function(col)
                                                                For i As Integer = 0 To h - 1
                                                                    If bitmap.GetPixel(col, i).ToArgb <> Color.Value.ToArgb Then
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
        Dim finalImage As Image = New Bitmap(Image)
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
                parts(0) = CalculateValueFromPercent(parts(0).AdjustBlankSpaces, Original.Width).RoundDecimal.ToString
            End If

            If parts(1).AdjustBlankSpaces.EndsWith("%") Then
                parts(1) = CalculateValueFromPercent(parts(1).AdjustBlankSpaces, Original.Height).RoundDecimal.ToString
            End If

            size = New Size(parts(0).ToInteger, parts(1).ToInteger)
        Else
            If Percent.AdjustBlankSpaces.EndsWith("%") Then
                Percent = Percent.Trim("%").AdjustBlankSpaces
            End If
            If Percent.IsNumber Then
                size.Width = CalculateValueFromPercent(Percent.ToInteger, Original.Width).RoundDecimal.ToString
                size.Height = CalculateValueFromPercent(Percent.ToInteger, Original.Height).RoundDecimal.ToString
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
        Dim fullsizeImage As Image = New Bitmap(Original)

        ' Prevent using images internal thumbnail
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
        Text = Text.ReplaceMany(" ", "px", " ", ";", ":").ToLower.Trim
        Text = Text.Replace("largura", "width")
        Text = Text.Replace("altura", "height")
        Text = Text.Replace("l ", "w ")
        Text = Text.Replace("a ", "h ")
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
                Case Text Like "*-*"
                    s.Width = Text.Split({"-"}, StringSplitOptions.RemoveEmptyEntries)(0)
                    s.Height = Text.Split({"-"}, StringSplitOptions.RemoveEmptyEntries)(1)
                    Exit Select
                Case Text Like "*_*"
                    s.Width = Text.Split({"_"}, StringSplitOptions.RemoveEmptyEntries)(0)
                    s.Height = Text.Split({"_"}, StringSplitOptions.RemoveEmptyEntries)(1)
                    Exit Select
                Case Else
                    s.Width = Text.Split({" "}, StringSplitOptions.RemoveEmptyEntries)(0)
                    s.Height = Text.Split({" "}, StringSplitOptions.RemoveEmptyEntries)(1)
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
    ''' Retorna uma lista com as N cores mais utilizadas na imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <returns>uma lista de Color</returns>
    <Extension>
    Public Function GetMostUsedColors(Image As Image, Optional Count As Integer = 10) As IEnumerable(Of Color)
        Return New Bitmap(Image).GetMostUsedColors().Take(Count)
    End Function

    ''' <summary>
    ''' Retorna uma lista com as cores utilizadas na imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <returns>uma lista de Color</returns>
    <Extension>
    Public Function GetMostUsedColors(Image As Bitmap) As IEnumerable(Of Color)
        Return GetMostUsedColorsIncidence(Image).Select(Function(x) x.Key).Where(Function(cor) cor <> Color.Empty AndAlso cor <> Color.Transparent AndAlso {cor.R, cor.G, cor.B}.All(Function(x) x > 0))
    End Function

    ''' <summary>
    ''' Retorna uma lista com as cores utilizadas na imagem
    ''' </summary>
    ''' <param name="Image">Imagem</param>
    ''' <returns>uma lista de Color</returns>
    <Extension>
    Public Function GetMostUsedColorsIncidence(Image As Bitmap) As Dictionary(Of Color, Integer)

        Dim dctColorIncidence As New Dictionary(Of Integer, Integer)
        If Image IsNot Nothing AndAlso Image.Width > 0 AndAlso Image.Height > 0 Then
            Dim coluna As Integer = 0
            While coluna < Image.Size.Width
                Dim linha As Integer = 0
                While linha < Image.Size.Height
                    Dim pixelColor = Image.GetPixel(coluna, linha).ToArgb()
                    If dctColorIncidence.Keys.Contains(pixelColor) Then
                        dctColorIncidence(pixelColor) = dctColorIncidence(pixelColor) + 1
                    Else
                        dctColorIncidence.Add(pixelColor, 1)
                    End If
                    linha = linha + 1
                End While
                coluna = coluna + 1
            End While
        End If
        Return dctColorIncidence.OrderByDescending(Function(x) x.Value).ToDictionary(Function(x) Color.FromArgb(x.Key), Function(x) x.Value)
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