using System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Extensions.Colors;

namespace Extensions
{




    public static partial class Util
    {

 

        /// <summary>
        /// Remove the background of an image.
        /// </summary>
        /// <param name="original">The original image.</param>
        /// <param name="background">The background color.</param>
        /// <returns>The image with the background removed.</returns>
        public static Image RemoveBackground(this Image original, Color? background = null)
        {
            if (original != null)
            {
                Bitmap bmp = new Bitmap(original.Width, original.Height);

                if (background == null || background.Value == Color.Transparent)
                {
                    // Detect the background color automatically.
                    background = DetectBackgroundColor(original);
                }

                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        Color pixelColor = ((Bitmap)original).GetPixel(j, i);

                        if (pixelColor != background)
                        {
                            bmp.SetPixel(j, i, pixelColor);
                        }
                    }
                }

                return bmp;
            }
            return null;
        }

        /// <summary>
        /// Detects the background color of an image.
        /// </summary>
        /// <param name="original">The original image.</param>
        /// <returns>The detected background color.</returns>
        private static Color DetectBackgroundColor(this Image original)
        {
            using (Bitmap bmp = new Bitmap(original.Width, original.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(original, 0, 0);
                }

                return bmp.GetPixel(0, 0);
            }
        }


        /// <summary>
        /// Retorna uma lista com as cores utilizadas na imagem
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <returns>uma lista de Color</returns>
        public static Dictionary<HSVColor, int> ColorPallette(this Image Img, int PixelateSize = 0)
        {
            Bitmap image;
            if (PixelateSize > 0)
            {
                image = (Bitmap)Img.Pixelate(PixelateSize);
            }
            else
            {
                image = new Bitmap(Img);
            }

            var dctColorIncidence = new Dictionary<int, int>();
            using (image)
                if (image != null && image.Width > 0 && image.Height > 0)
                {
                    int coluna = 0;
                    while (coluna < image.Size.Width)
                    {
                        int linha = 0;
                        while (linha < image.Size.Height)
                        {
                            int pixelColor = image.GetPixel(coluna, linha).ToArgb();
                            if (dctColorIncidence.ContainsKey(pixelColor))
                            {
                                dctColorIncidence[pixelColor] = dctColorIncidence[pixelColor] + 1;
                            }
                            else
                            {
                                dctColorIncidence.Add(pixelColor, 1);
                            }

                            linha++;
                        }

                        coluna++;
                    }
                }

            return dctColorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => new HSVColor(Color.FromArgb(x.Key)), x => x.Value);
        }


        /// <summary>
        /// Combina 2 ou mais imagens em uma única imagem
        /// </summary>
        /// <param name="Images">Lista de Imagens para combinar</param>
        /// <param name="VerticalFlow">
        /// Se TRUE, combina as Imagens verticalmente (Uma em baixo da outra), caso contrario as
        /// imagens serão combinadas horizontalmente (Uma do lado da outra da esquerda para a direita)
        /// </param>
        /// <returns>Um Bitmap com a combinaçao de todas as imagens da Lista</returns>
        public static Bitmap CombineImages(bool VerticalFlow, params Image[] Images) => Images.CombineImages(VerticalFlow);

        /// <summary>
        /// Combina 2 ou mais imagens em uma única imagem
        /// </summary>
        /// <param name="Images">Array de Imagens para combinar</param>
        /// <param name="VerticalFlow">
        /// Se TRUE, combina as Imagens verticalmente (Uma em baixo da outra), caso contrario as
        /// imagens serão combinadas horizontalmente (Uma do lado da outra da esquerda para a direita)
        /// </param>
        /// <returns>Um Bitmap com a combinaçao de todas as imagens do Array</returns>
        public static Bitmap CombineImages(this IEnumerable<Image> Images, bool VerticalFlow = false)
        {
            int width = 0;
            int height = 0;
            foreach (var image in Images ?? Array.Empty<Image>())
            {
                // cria um bitmap a partir do arquivo e o inclui na lista
                var bitmap = new Bitmap(image);

                // atualiza o tamanho da imagem bitmap final
                if (VerticalFlow)
                {
                    height += bitmap.Height;
                    width = bitmap.Width > width ? bitmap.Width : width;
                }
                else
                {
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;
                }
                bitmap.Dispose();
            }

            // cria um bitmap para tratar a imagem combinada
            Bitmap imagemFinal = new Bitmap(width, height);

            // Obtem o objeto gráfico da imagem
            using (var g = Graphics.FromImage(imagemFinal))
            {
                // define a cor de fundo
                g.Clear(Color.White);

                // percorre imagem por imagem e gera uma unica imagem final
                int offset = 0;
                foreach (Bitmap image in Images.Cast<Bitmap>())
                {
                    if (VerticalFlow)
                    {
                        g.DrawImage(image, new System.Drawing.Rectangle(0, offset, image.Width, image.Height));
                        offset += image.Height;
                    }
                    else
                    {
                        g.DrawImage(image, new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }
            }

            return imagemFinal;
        }


        /// <summary>
        /// Cropa uma imagem a patir do centro
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="SizeExpression">Tamanho</param>
        /// <returns></returns>
        public static Image Crop(this Image Image, string SizeExpression) => Image?.Crop(SizeExpression.ParseSize());

        /// <summary>
        /// Cropa uma imagem a patir do centro
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="Size">Tamanho</param>
        /// <returns></returns>
        public static Image Crop(this Image Image, Size Size) => Image?.Crop(Size.Width, Size.Height);

        /// <summary>
        /// Cropa uma imagem a patir do centro
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="MaxWidth">Largura maxima</param>
        /// <param name="MaxHeight">Altura maxima</param>
        /// <returns></returns>
        public static Image Crop(this Image Image, int MaxWidth, int MaxHeight)
        {
            var jpgInfo = ImageCodecInfo.GetImageEncoders().Where(codecInfo => codecInfo.MimeType == "image/png").First();
            Image finalImage = new Bitmap(Image);
            Bitmap bitmap = null;
            int left = 0;
            int top = 0;
            int srcWidth = MaxWidth;
            int srcHeight = MaxHeight;
            bitmap = new Bitmap(MaxWidth, MaxHeight);
            double croppedHeightToWidth = MaxHeight / (double)MaxWidth;
            double croppedWidthToHeight = MaxWidth / (double)MaxHeight;
            if (Image.Width > Image.Height)
            {
                srcWidth = (int)Math.Round(Math.Round(Image.Height * croppedWidthToHeight));
                if (srcWidth < Image.Width)
                {
                    srcHeight = Image.Height;
                    left = (int)Math.Round((Image.Width - srcWidth) / 2d);
                }
                else
                {
                    srcHeight = (int)Math.Round(Math.Round(Image.Height * (Image.Width / (double)srcWidth)));
                    srcWidth = Image.Width;
                    top = (int)Math.Round((Image.Height - srcHeight) / 2d);
                }
            }
            else
            {
                srcHeight = (int)Math.Round(Math.Round(Image.Width * croppedHeightToWidth));
                if (srcHeight < Image.Height)
                {
                    srcWidth = Image.Width;
                    top = (int)Math.Round((Image.Height - srcHeight) / 2d);
                }
                else
                {
                    srcWidth = (int)Math.Round(Math.Round(Image.Width * (Image.Height / (double)srcHeight)));
                    srcHeight = Image.Height;
                    left = (int)Math.Round((Image.Width - srcWidth) / 2d);
                }
            }

            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(Image, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), new System.Drawing.Rectangle(left, top, srcWidth, srcHeight), GraphicsUnit.Pixel);
                finalImage = bitmap;
            }

            using (var encParams = new EncoderParameters(1))
            {
                encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100);
                finalImage.RotateFlip(Image.GetRotateFlip());
                return finalImage;
            }
        }

        /// <summary>
        /// Corta a imagem em um circulo
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <param name="Background">Cor do fundo</param>
        /// <returns></returns>
        public static Image CropToCircle(this Image Img, Color? Background = default) => Img.CropToSquare()?.CropToEllipsis(Background);

        /// <summary>
        /// Corta a imagem em uma elipse
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <param name="Background">Cor do fundo</param>
        /// <returns></returns>
        public static Image CropToEllipsis(this Image Img, Color? Background = default)
        {
            var dstImage = new Bitmap(Img.Width, Img.Height);
            var g = Graphics.FromImage(dstImage);
            Background = Background ?? Color.Transparent;
            using (Brush br = new SolidBrush((Color)Background))
            {
                g.FillRectangle(br, 0, 0, dstImage.Width, dstImage.Height);
            }

            var path = new GraphicsPath();
            path.AddEllipse(0, 0, dstImage.Width, dstImage.Height);
            g.SetClip(path);
            g.DrawImage(Img, 0, 0);
            return dstImage;
        }

        /// <summary>
        /// Corta uma imagem para um quadrado perfeito a partir do centro
        /// </summary>
        /// <param name="img">Imagem</param>
        /// <param name="WidthHeight">Tamanho do quadrado em pixels</param>
        /// <returns></returns>
        public static Image CropToSquare(this Image Img, int WidthHeight = 0)
        {
            if (WidthHeight < 1)
            {
                WidthHeight = Img.Height > Img.Width ? Img.Width : Img.Height;
            }

            return Img.Crop(WidthHeight, WidthHeight);
        }


        public static Image DrawImage(this string text, int Width, int Height, Font Font = null, Color? Color = default, int X = -1, int Y = -1)
        {
            return text.DrawImage(new Size(Width, Height), Font, Color, X, Y);
        }
        public static Image DrawImage(this string text, Size imageSize, Font Font = null, Color? Color = default, int X = -1, int Y = -1)
        {
            var b = new Bitmap(imageSize.Width, imageSize.Height);
            return b.DrawString(text, Font, Color, X, Y);
        }
        public static Image DrawString(this Image img, string Text, Font Font = null, Color? Color = default, int X = -1, int Y = -1)
        {
            var bitmap = new Bitmap(img);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                Font = Font ?? new Font("Arial", (float)(bitmap.Width / 10d));
                var tamanho = graphics.MeasureString(Text, Font, new Size(bitmap.Width, bitmap.Height));
                X = X.LimitRange(-1, img.Width);
                Y = Y.LimitRange(-1, img.Height);
                if (X == -1)
                {
                    X = RoundInt(bitmap.Width / 2d - tamanho.Width / 2f);
                }

                if (Y == -1)
                {
                    Y = RoundInt(bitmap.Height / 2d - tamanho.Height / 2f);
                }

                Color = Color ?? bitmap.GetPixel(X, Y).GetContrastColor(50f);
                var B = new System.Drawing.SolidBrush((Color)Color);
                graphics.DrawString(Text, Font, B, X, Y);
            }

            return bitmap;
        }


        /// <summary>
        /// Retorna o Mime TEntity a partir de de um formato de Imagem
        /// </summary>
        /// <param name="RawFormat">Formato de Imagem</param>
        /// <returns>string mime type</returns>
        public static IEnumerable<string> GetMimeTypes(this ImageFormat RawFormat)
        {
            try
            {
                RawFormat = RawFormat ?? ImageFormat.Png;
                foreach (var img in ImageCodecInfo.GetImageEncoders())
                {
                    if (img.FormatID == RawFormat.Guid)
                    {
                        return img.FilenameExtension.GetFileType().MimeTypes;
                    }
                }
            }
            catch
            {
            }

            return GetFileType(".png").MimeTypes;
        }

        public static string GetMimeType(this ImageFormat RawFormat)
        {
            try
            {
                RawFormat = RawFormat ?? ImageFormat.Png;
                foreach (var img in ImageCodecInfo.GetImageEncoders())
                {
                    if (img.FormatID == RawFormat.Guid)
                    {
                        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                        return codecs.First(codec => codec.FormatID == RawFormat.Guid).MimeType;
                    }
                }
            }
            catch
            {
            }

            return "image/png";
        }

        /// <summary>
        /// Retorna o Mime TEntity a partir de de uma Imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>string mime type</returns>
        public static IEnumerable<string> GetMimeTypes(this Image Image) => Image?.RawFormat.GetMimeTypes() ?? Array.Empty<string>();
        public static string GetMimeType(this Image Image) => Image?.RawFormat.GetMimeType() ?? "image/png";

        /// <summary>
        /// Retorna uma lista com as N cores mais utilizadas na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>uma lista de Color</returns>
        public static IEnumerable<HSVColor> GetMostUsedColors(this Image Image, int Count) => new Bitmap(Image).GetMostUsedColors().Take(Count);

        /// <summary>
        /// Retorna uma lista com as cores utilizadas na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>uma lista de Color</returns>
        public static IEnumerable<HSVColor> GetMostUsedColors(this Image Image) => Image.ColorPallette().Keys;


        /// <summary>
        /// Retorna o formato da imagem correspondente a aquela imagem
        /// </summary>
        /// <param name="OriginalImage"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(this Image OriginalImage) => ImageTypes.Where(p => p.Guid == OriginalImage.RawFormat.Guid).FirstOr(ImageFormat.Png);


        /// <summary>
        /// Pega o encoder a partir de um formato de imagem
        /// </summary>
        /// <param name="RawFormat">Image format</param>
        /// <returns>image codec info.</returns>
        public static ImageCodecInfo GetEncoderInfo(this ImageFormat RawFormat) => ImageCodecInfo.GetImageDecoders().Where(c => c.FormatID == RawFormat.Guid).FirstOr(ImageCodecInfo.GetImageDecoders().Where(c => c.FormatID == ImageFormat.Png.Guid).First());


        public static Image BrightnessContrastAndGamma(this Image originalimage, float Brightness, float Contrast, float Gamma)
        {
            var adjustedImage = new Bitmap(originalimage);
            Gamma = Gamma.SetMinValue(1.0f);
            Contrast = Contrast.SetMinValue(1.0f);
            float adjustedBrightness = Brightness.SetMinValue(1.0f) - 1.0f;
            var ptsArray = new[] { new float[] { Contrast, 0f, 0f, 0f, 0f }, new float[] { 0f, Contrast, 0f, 0f, 0f }, new float[] { 0f, 0f, Contrast, 0f, 0f }, new float[] { 0f, 0f, 0f, 1.0f, 0f }, new float[] { adjustedBrightness, adjustedBrightness, adjustedBrightness, 0f, 1f } };
            var imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(Gamma, ColorAdjustType.Bitmap);
            var g = Graphics.FromImage(adjustedImage);
            g.DrawImage(originalimage, new System.Drawing.Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), 0, 0, originalimage.Width, originalimage.Height, GraphicsUnit.Pixel, imageAttributes);
            return adjustedImage;
        }


        /// <summary>
        /// Inverte as cores de uma imagem.
        /// </summary>
        /// <param name="original">A imagem original.</param>
        /// <returns>A imagem com as cores invertidas.</returns>
        public static Image InvertColors(this Image original)
        {
            if (original != null)
            {
                Bitmap bmp = new Bitmap(original.Width, original.Height);

                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        Color pixelColor = ((Bitmap)original).GetPixel(j, i);
                        Color newColor = Color.FromArgb(pixelColor.A,
                                                         255 - pixelColor.R,
                                                         255 - pixelColor.G,
                                                         255 - pixelColor.B);
                        bmp.SetPixel(j, i, newColor);
                    }
                }
                return bmp;
            }
            return null;
        }


        public static bool ContainsTransparency(this Image img)
        {
            // 4. Check if pixel format supports alpha
            if ((img.PixelFormat & PixelFormat.Alpha) == 0 &&
                (img.PixelFormat & PixelFormat.PAlpha) == 0 &&
                (img.PixelFormat & PixelFormat.Format32bppArgb) == 0 &&
                (img.PixelFormat & PixelFormat.Format64bppArgb) == 0)
            {
                return false;
            }

            // 5. Scan pixels for transparency
            using (var bmp = new Bitmap(img))
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        Color pixel = bmp.GetPixel(x, y);
                        if (pixel.A < 255) // Found transparency
                            return true;
                    }
                }
            }

            // No transparent pixels found
            return false;
        }


        public static RotateFlipType GetRotateFlip(this Image Img)
        {
            var rft = RotateFlipType.RotateNoneFlipNone;
            foreach (PropertyItem p in Img.PropertyItems)
            {
                if (p.Id == 274)
                {
                    short orientation = BitConverter.ToInt16(p.Value, 0);
                    switch (orientation)
                    {
                        case 1:
                            {
                                rft = RotateFlipType.RotateNoneFlipNone;
                                break;
                            }

                        case 3:
                            {
                                rft = RotateFlipType.Rotate180FlipNone;
                                break;
                            }

                        case 6:
                            {
                                rft = RotateFlipType.Rotate90FlipNone;
                                break;
                            }

                        case 8:
                            {
                                rft = RotateFlipType.Rotate270FlipNone;
                                break;
                            }
                    }
                }
            }

            return rft;
        }

        /// <summary>
        /// Pixeliza uma imagem
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="PixelateSize"></param>
        /// <returns></returns>
        public static Image Pixelate(this Image Image, int PixelateSize = 1)
        {
            if (Image == null) return null;
            var rectangle = new System.Drawing.Rectangle(0, 0, Image.Width, Image.Height);
            PixelateSize++;
            var pixelated = new Bitmap(Image.Width, Image.Height);
            using (var graphics = Graphics.FromImage(pixelated))
            {
                graphics.DrawImage(Image, new System.Drawing.Rectangle(0, 0, Image.Width, Image.Height), new System.Drawing.Rectangle(0, 0, Image.Width, Image.Height), GraphicsUnit.Pixel);
            }

            int xx = rectangle.X;
            while (xx < rectangle.X + rectangle.Width && xx < Image.Width)
            {
                int yy = rectangle.Y;
                while (yy < rectangle.Y + rectangle.Height && yy < Image.Height)
                {
                    int offsetX = (int)Math.Round(PixelateSize / 2d);
                    int offsetY = (int)Math.Round(PixelateSize / 2d);
                    while (xx + offsetX >= Image.Width)
                        offsetX -= 1;
                    while (yy + offsetY >= Image.Height)
                        offsetY -= 1;
                    var pixel = pixelated.GetPixel(xx + offsetX, yy + offsetY);
                    int x = xx;
                    while (x < xx + PixelateSize && x < Image.Width)
                    {
                        int y = yy;
                        while (y < yy + PixelateSize && y < Image.Height)
                        {
                            pixelated.SetPixel(x, y, pixel);
                            y += 1;
                        }

                        x += 1;
                    }

                    yy += PixelateSize;
                }

                xx += PixelateSize;
            }

            return pixelated;
        }

        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="Size">Tamanho</param>
        /// <param name="OnlyResizeIfWider">
        /// Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada
        /// </param>
        /// <returns></returns>
        public static Image Resize(this Image Original, Size Size, bool OnlyResizeIfWider = true) => Original.Resize(Size.Width, Size.Height, OnlyResizeIfWider);
        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="NewWidth">Nova Largura</param>
        /// <param name="MaxHeight">Altura máxima</param>
        /// <param name="OnlyResizeIfWider">
        /// Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada
        /// </param>
        /// <returns></returns>
        public static Image Resize(this Image Original, int NewWidth, int MaxHeight, bool OnlyResizeIfWider = true)
        {
            if (Original == null)
            {
                return null;
            }

            Image fullsizeImage = new Bitmap(Original);
            if (OnlyResizeIfWider)
            {
                if (fullsizeImage.Width <= NewWidth)
                {
                    NewWidth = fullsizeImage.Width;
                }
            }

            int newHeight = (int)Math.Round(fullsizeImage.Height * NewWidth / (double)fullsizeImage.Width);
            if (newHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = (int)Math.Round(fullsizeImage.Width * MaxHeight / (double)fullsizeImage.Height);
                newHeight = MaxHeight;
            }

            fullsizeImage = fullsizeImage.GetThumbnailImage(NewWidth, newHeight, null, IntPtr.Zero);
            fullsizeImage.RotateFlip(Original.GetRotateFlip());
            return fullsizeImage;
        }

        /// <summary>
        /// redimensiona e Cropa uma imagem, aproveitando a maior parte dela
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static Image ResizeCrop(this Image Image, int Width, int Height) => Image.Resize(Width, Height, false).Crop(Width, Height);

        /// <summary>
        /// redimensiona e Cropa uma imagem, aproveitando a maior parte dela
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static Image ResizeCrop(this Image Image, int Width, int Height, bool OnlyResizeIfWider) => Image.Resize(Width, Height, OnlyResizeIfWider).Crop(Width, Height);

        /// <summary>
        /// Redimensiona uma imagem para o tamanho definido por uma porcentagem
        /// </summary>
        /// <param name="Original"></param>
        /// <param name="Percent">Porcentagem ( no formato '30% 'ou '20% x 10%')</param>
        /// <param name="OnlyResizeIfWider"></param>
        /// <returns></returns>
        public static Image ResizePercent(this Image Original, string Percent, bool OnlyResizeIfWider = true)
        {
            var size = new Size();
            if (Percent.Contains("x"))
            {
                var parts = Percent.Split("x");
                if (parts[0].TrimBetween().EndsWith("%"))
                {
                    parts[0] = parts[0].TrimBetween().CalculateValueFromPercent(Original.Width).RoundDecimal().ToString();
                }

                if (parts[1].TrimBetween().EndsWith("%"))
                {
                    parts[1] = parts[1].TrimBetween().CalculateValueFromPercent(Original.Height).RoundDecimal().ToString();
                }

                size = new Size(parts[0].ToInt(), parts[1].ToInt());
            }
            else
            {
                if (Percent.TrimBetween().EndsWith("%"))
                {
                    Percent = Percent.Trim('%').TrimBetween();
                }

                if (Percent.IsNumber())
                {
                    size.Width = Convert.ToInt32(Percent.ToInt().CalculateValueFromPercent(Original.Width).RoundDecimal().ToString());
                    size.Height = Convert.ToInt32(Percent.ToInt().CalculateValueFromPercent(Original.Height).RoundDecimal().ToString());
                }
            }

            return Original.Resize(size, OnlyResizeIfWider);
        }

        /// <summary>
        /// Resizes the image by the specified percentage, optionally only if the image is wider than the target size.
        /// </summary>
        /// <param name="Original">The source image to be resized.</param>
        /// <param name="Percent">The percentage by which to resize the image. Must be greater than 0.</param>
        /// <param name="OnlyResizeIfWider">If <see langword="true"/>, resizing occurs only if the image is wider than the target size; otherwise, the
        /// image is always resized.</param>
        /// <returns>A new <see cref="Image"/> instance representing the resized image.</returns>
        public static Image ResizePercent(this Image Original, decimal Percent, bool OnlyResizeIfWider = true) => Original.ResizePercent(Percent.ToPercentString(), OnlyResizeIfWider);

        /// <summary>
        /// Resizes the image to fit within the specified aspect ratio, preserving the original proportions and cropping
        /// as necessary.
        /// </summary>
        /// <remarks>This method maintains the original image's aspect ratio and crops the image if needed
        /// to fit the target aspect ratio. The resulting image will not be stretched or distorted.</remarks>
        /// <param name="Original">The source image to be resized. Cannot be null.</param>
        /// <param name="AspectRatio">The target aspect ratio, specified as a <see cref="Size"/> where the width and height define the desired
        /// proportions.</param>
        /// <returns>A new <see cref="Image"/> instance resized to fit within the specified aspect ratio. The returned image may
        /// be cropped to maintain the original proportions.</returns>
        public static Image ResizeAspect(this Image Original, Size AspectRatio)
        {
            var aspectRatio = Original.Width / (double)Original.Height;
            var targetAspectRatio = AspectRatio.Width / (double)AspectRatio.Height;
            int newWidth;
            int newHeight;
            if (aspectRatio > targetAspectRatio)
            {
                newWidth = AspectRatio.Width;
                newHeight = (int)(AspectRatio.Width / aspectRatio);
            }
            else
            {
                newHeight = AspectRatio.Height;
                newWidth = (int)(AspectRatio.Height * aspectRatio);
            }
            return Original.ResizeCrop(newWidth, newHeight);
        }
        /// <summary>
        /// Resizes the image to fit within the specified aspect ratio, preserving the original proportions.
        /// </summary>
        /// <remarks>The resized image will be scaled so that it fits entirely within the target aspect
        /// ratio, without cropping or distorting the original image. If the aspect ratio is invalid or cannot be
        /// parsed, an exception may be thrown.</remarks>
        /// <param name="Original">The source image to be resized.</param>
        /// <param name="AspectRatio">A string representing the desired aspect ratio, in the format "width:height" (for example, "16:9").</param>
        /// <returns>An image resized to fit within the specified aspect ratio while maintaining its original proportions.</returns>
        public static Image ResizeAspect(this Image Original, string AspectRatio)   => Original.ResizeAspect(AspectRatio.ParseSize());
       
        /// <summary>
        /// Converts the specified image to a Base64-encoded string representation using the provided image format.
        /// </summary>
        /// <remarks>The returned string can be used for embedding images in text-based formats such as
        /// JSON or HTML. The image format affects the output; ensure the format is compatible with the intended
        /// use.</remarks>
        /// <param name="OriginalImage">The image to convert to a Base64 string. If null, the method returns null.</param>
        /// <param name="OriginalImageFormat">The format to use when encoding the image. If null, the method uses the image's format if available;
        /// otherwise, it defaults to PNG.</param>
        /// <returns>A Base64-encoded string representing the image data, or null if the input image is null.</returns>
        public static string ToBase64(this Image OriginalImage, ImageFormat OriginalImageFormat = null)
        {
            if (OriginalImage != null)
                using (var ms = new MemoryStream())
                {
                    OriginalImage.Save(ms, OriginalImageFormat ?? OriginalImage.GetImageFormat() ?? ImageFormat.Png);
                    var imageBytes = ms.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            return null;
        }

        /// <summary>
        /// Retorna uma <see cref="Bitmap"/> a partir de um Image
        /// </summary>
        /// <param name="Image"></param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this Image Image) => new Bitmap(Image);


        /// <summary>
        /// Transforma uma imagem em array de bytes
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns></returns>
        public static byte[] ToBytes(this Image Image, ImageFormat Format = null)
        {
            using (var ms = Image.ToStream(Format))
            {
                return ms.ToBytes();
            }
        }
        /// <summary>
        /// Transforma uma imagem em um stream
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns></returns>
        public static Stream ToStream(this Image Image, ImageFormat Format = null)
        {
            Format = Format ?? ImageFormat.Png;
            Stream s = new MemoryStream();
            Image.Save(s, Format);
            s.Position = 0L;
            return s;
        }

        /// <summary>
        /// Remove os excessos de uma cor de fundo de uma imagem deixando apenas seu conteudo
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="Color"></param>
        /// <returns></returns>
        public static Image Trim(this Image Img, Color? Color = default)
        {
            var bitmap = new Bitmap(Img);
            Color = Color ?? bitmap.GetPixel(0, 0);
            int w = bitmap.Width;
            int h = bitmap.Height;
            bool IsAllWhiteRow(int row)
            {
                for (int i = 0, loopTo = w - 1; i <= loopTo; i++)
                {
                    if (bitmap.GetPixel(i, row).ToArgb() != Color.Value.ToArgb())
                    {
                        return false;
                    }
                }

                return true;
            }
            bool IsAllWhiteColumn(int col)
            {
                for (int i = 0, loopTo = h - 1; i <= loopTo; i++)
                {
                    if (bitmap.GetPixel(col, i).ToArgb() != Color.Value.ToArgb())
                    {
                        return false;
                    }
                }

                return true;
            }
            int leftMost = 0;
            for (int col = 0, loopTo = w - 1; col <= loopTo; col++)
            {
                if (IsAllWhiteColumn(col))
                {
                    leftMost = col + 1;
                }
                else
                {
                    break;
                }
            }

            int rightMost = w - 1;
            for (int col = rightMost; col >= 1; col -= 1)
            {
                if (IsAllWhiteColumn(col))
                {
                    rightMost = col - 1;
                }
                else
                {
                    break;
                }
            }

            int topMost = 0;
            for (int row = 0, loopTo1 = h - 1; row <= loopTo1; row++)
            {
                if (IsAllWhiteRow(row))
                {
                    topMost = row + 1;
                }
                else
                {
                    break;
                }
            }

            int bottomMost = h - 1;
            for (int row = bottomMost; row >= 1; row -= 1)
            {
                if (IsAllWhiteRow(row))
                {
                    bottomMost = row - 1;
                }
                else
                {
                    break;
                }
            }

            if (rightMost == 0 && bottomMost == 0 && leftMost == w && topMost == h)
            {
                return bitmap;
            }

            int croppedWidth = rightMost - leftMost + 1;
            int croppedHeight = bottomMost - topMost + 1;
            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (var g = Graphics.FromImage(target))
                {
                    g.DrawImage(bitmap, new RectangleF(0f, 0f, croppedWidth, croppedHeight), new RectangleF(leftMost, topMost, croppedWidth, croppedHeight), GraphicsUnit.Pixel);
                }

                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Values are top={0} bottom={1} left={2} right={3}", topMost, bottomMost, leftMost, rightMost), ex);
            }
        }

        /// <summary>
        /// Insere uma imagem de marca d'água na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="WaterMarkImage">Imagem de Marca d'água</param>
        /// <param name="X">Posição X</param>
        /// <param name="Y">Posição Y</param>
        /// <returns></returns>
        public static Image Watermark(this Image Image, Image WaterMarkImage, int X = -1, int Y = -1, Color? transparentColor = null)
        {
            // a imagem onde iremos aplicar a marca d'água
            var bm_Resultado = new Bitmap(Image);

            // a imagem que será usada como marca d'agua
            var bm_marcaDagua = new Bitmap(WaterMarkImage.Resize(bm_Resultado.Width - 5, bm_Resultado.Height - 5));
            if (X < 0)
                X = (bm_Resultado.Width - bm_marcaDagua.Width) / 2;   // centraliza a marca d'agua
            if (Y < 0)
                Y = (bm_Resultado.Height - bm_marcaDagua.Height) / 2;   // centraliza a marca d'agua
            const byte ALPHA = 128;
            // Define o componente Alpha do pixel
            Color clr;
            for (int py = 0, loopTo = bm_marcaDagua.Height - 1; py <= loopTo; py++)
            {
                for (int px = 0, loopTo1 = bm_marcaDagua.Width - 1; px <= loopTo1; px++)
                {
                    clr = bm_marcaDagua.GetPixel(px, py);
                    bm_marcaDagua.SetPixel(px, py, Color.FromArgb(ALPHA, clr.R, clr.G, clr.B));
                }
            }
            // Define a marca d'agua como transparente
            bm_marcaDagua.MakeTransparent(transparentColor ?? bm_marcaDagua.GetPixel(0, 0));
            // Copia o resultado na imagem
            var gr = Graphics.FromImage(bm_Resultado);
            gr.DrawImage(bm_marcaDagua, X, Y);
            return bm_Resultado;
        }

        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="ResizeExpression">uma string contendo uma expressão de tamanho</param>
        /// <param name="OnlyResizeIfWider">
        /// Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada
        /// </param>
        /// <returns></returns>
        public static Image Resize(this Image Original, string ResizeExpression, bool OnlyResizeIfWider = true)
        {
            if (ResizeExpression.Contains("%"))
            {
                return Original.ResizePercent(ResizeExpression, OnlyResizeIfWider);
            }
            else
            {
                var s = ResizeExpression.ParseSize();
                return Original.Resize(s, OnlyResizeIfWider);
            }
        }


        public static Image Translate(this Image img, Color Color, float Alpha = 0f) => img.Translate(Color.R, Color.G, Color.B, Alpha);

        public static Image Translate(this Image img, float Red, float Green, float Blue, float Alpha = 0f)
        {
            float sr, sg, sb, sa;
            var copia = new Bitmap(img);
            // normalize the color components to 1
            sr = Red / 255f;
            sg = Green / 255f;
            sb = Blue / 255f;
            sa = Alpha / 255f;

            // create the color matrix
            var cm = new ColorMatrix(new float[][] { new float[] { 1f, 0f, 0f, 0f, 0f }, new float[] { 0f, 1f, 0f, 0f, 0f }, new float[] { 0f, 0f, 1f, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { sr, sg, sb, sa, 1f } });

            // apply the matrix to the image
            return ApplyColorMatrix(copia, cm);
        }

        /// <summary>
        /// Transforma uma imagem em uma URL Util
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>Uma DataURI em string</returns>
        public static string ToDataURL(this Image Image) => $"data:{Image.GetMimeTypes().First().ToLowerInvariant().Replace("application/octet-stream", GetFileType(".png").GetMimeTypesOrDefault().First())};base64,{Image.ToBase64()}";

        /// <summary>
        /// Converte uma imagem para DataURI trocando o MIME TEntity
        /// </summary>
        /// <param name="OriginalImage">Imagem</param>
        /// <param name="OriginalImageFormat">Formato da Imagem</param>
        /// <returns>Uma data URI com a imagem convertida</returns>
        public static string ToDataURL(this Image OriginalImage, ImageFormat OriginalImageFormat) => OriginalImage.ToBase64(OriginalImageFormat).Base64ToImage().ToDataURL();


        /// <summary>
        /// Rotaciona uma imagem para sua posição original caso ela já tenha sido rotacionada (EXIF)
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <returns>TRUE caso a imagem ja tenha sido rotacionada</returns>
        public static bool TestAndRotate(this Image Img)
        {
            var rft = Img.GetRotateFlip();
            if (rft != RotateFlipType.RotateNoneFlipNone)
            {
                Img.RotateFlip(rft);
                return true;
            }

            return false;
        }

        public static Image MakeLighter(this Image img, float percent = 50f)
        {
            var lockedBitmap = new Bitmap(img);
            for (int y = 0, loopTo = lockedBitmap.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = lockedBitmap.Width - 1; x <= loopTo1; x++)
                {
                    var oldColor = lockedBitmap.GetPixel(x, y);
                    if (!oldColor.CompareARGB(true, Color.Transparent, Color.Black, Color.White))
                    {
                        var newColor = oldColor.MakeLighter(percent);
                        lockedBitmap.SetPixel(x, y, newColor);
                    }
                }
            }

            return lockedBitmap;
        }


        public static Image MakeDarker(this Image img, float percent = 50f)
        {
            var lockedBitmap = new Bitmap(img);
            for (int y = 0, loopTo = lockedBitmap.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = lockedBitmap.Width - 1; x <= loopTo1; x++)
                {
                    var oldColor = lockedBitmap.GetPixel(x, y);
                    if (!oldColor.CompareARGB(true, Color.Transparent, Color.Black, Color.White))
                    {
                        var newColor = oldColor.MakeDarker(percent);
                        lockedBitmap.SetPixel(x, y, newColor);
                    }
                }
            }

            return lockedBitmap;
        }


        /// <summary>
        /// Insere uma imagem em outra imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="InsertedImage">Imagem de Marca Dagua</param>
        /// <param name="X">Posição X</param>
        /// <param name="Y">Posição Y</param>
        /// <returns></returns>
        public static Image Insert(this Image Image, Image InsertedImage, int X = -1, int Y = -1)
        {
            var bm_Resultado = new Bitmap(Image);
            var bm_marcaDagua = new Bitmap(InsertedImage.Resize(bm_Resultado.Width - 5, bm_Resultado.Height - 5));
            if (X < 0)
                X = (bm_Resultado.Width - bm_marcaDagua.Width) / 2;
            if (Y < 0)
                Y = (bm_Resultado.Height - bm_marcaDagua.Height) / 2;
            var gr = Graphics.FromImage(bm_Resultado);
            gr.DrawImage(bm_marcaDagua, X, Y);
            return bm_Resultado;
        }

        /// <summary>
        /// Inverte as cores de uma imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <returns></returns>
        public static Image Negative(this Image img)
        {
            var copia = new Bitmap(img);
            var cm = new ColorMatrix(new float[][] { new float[] { -1, 0f, 0f, 0f, 0f }, new float[] { 0f, -1, 0f, 0f, 0f }, new float[] { 0f, 0f, -1, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });
            return ApplyColorMatrix(copia, cm);
        }


        public static Image Monochrome(this Image Image, Color Color, float Alpha = 0f) => Image.Grayscale().Translate(Color.R, Color.G, Color.B, Alpha);


        /// <summary>
        /// Converte uma Imagem para Escala de cinza
        /// </summary>
        /// <param name="img">imagem original</param>
        /// <returns></returns>
        public static Image Grayscale(this Image img)
        {
            var copia = new Bitmap(img);
            var cm = new ColorMatrix(new float[][] { new float[] { 0.299f, 0.299f, 0.299f, 0f, 0f }, new float[] { 0.587f, 0.587f, 0.587f, 0f, 0f }, new float[] { 0.114f, 0.114f, 0.114f, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });
            return ApplyColorMatrix(copia, cm);
        }

        /// <summary>
        /// Redimensiona a imagem proporcionalmente para caber dentro das dimensões especificadas,
        /// considerando um padding horizontal e vertical.
        /// </summary>
        /// <param name="image">Imagem original.</param>
        /// <param name="maxWidth">Largura máxima total (incluindo padding).</param>
        /// <param name="maxHeight">Altura máxima total (incluindo padding).</param>
        /// <param name="paddingX">Espaço horizontal (esquerda + direita).</param>
        /// <param name="paddingY">Espaço vertical (topo + base).</param>
        /// <returns>Nova imagem redimensionada.</returns>
        public static Image ResizeToFit(this Image image, int maxWidth, int maxHeight, int paddingX = 0, int paddingY = 0)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            // Área útil disponível para a imagem (descontando o padding)
            int availableWidth = Math.Max(1, maxWidth - paddingX);
            int availableHeight = Math.Max(1, maxHeight - paddingY);

            // Calcula a proporção
            double ratioX = (double)availableWidth / image.Width;
            double ratioY = (double)availableHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            // Calcula novas dimensões
            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            // Cria nova imagem redimensionada
            var newImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }
    }
}

