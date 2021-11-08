using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic;



namespace InnerLibs
{
    /// <summary>
    /// Modulo de Imagem
    /// </summary>
    /// <remarks></remarks>
    public static class Images
    {

        /// <summary>
        /// Aplica um borrão a imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="BlurSize"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static Image Blur(this Image Img, int BlurSize = 5) => Blur(Img, BlurSize, new Rectangle(0, 0, Img.Width, Img.Height));


        /// <summary>
        /// Aplica um borrão a uma determinada parte da imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="BlurSize"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        private unsafe static Image Blur(this Image Img, int BlurSize, Rectangle rectangle)
        {
            Bitmap blurred = new Bitmap(Img.Width, Img.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(Img, new Rectangle(0, 0, Img.Width, Img.Height),
                    new Rectangle(0, 0, Img.Width, Img.Height), GraphicsUnit.Pixel);

            // Lock the bitmap's bits
            BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, Img.Width, Img.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);

            // Get bits per pixel for current PixelFormat
            int bitsPerPixel = Image.GetPixelFormatSize(blurred.PixelFormat);

            // Get pointer to first line
            byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + BlurSize && x < Img.Width); x++)
                    {
                        for (int y = yy; (y < yy + BlurSize && y < Img.Height); y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            avgB += data[0]; // Blue
                            avgG += data[1]; // Green
                            avgR += data[2]; // Red

                            blurPixelCount++;
                        }
                    }

                    avgR /= blurPixelCount;
                    avgG /= blurPixelCount;
                    avgB /= blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + BlurSize && x < Img.Width && x < rectangle.Width; x++)
                    {
                        for (int y = yy; y < yy + BlurSize && y < Img.Height && y < rectangle.Height; y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            // Change values
                            data[0] = (byte)avgB;
                            data[1] = (byte)avgG;
                            data[2] = (byte)avgR;
                        }
                    }
                }
            }

            // Unlock the bits
            blurred.UnlockBits(blurredData);

            return blurred;
        }

        /// <summary>
        /// Retorna uma <see cref="Bitmap"/> a partir de um Image
        /// </summary>
        /// <param name="Image"></param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this Image Image) => new Bitmap(Image);

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
        /// Insere uma imagem de marca d'água na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="WaterMarkImage">Imagem de Marca d'água</param>
        /// <param name="X">Posição X</param>
        /// <param name="Y">Posição Y</param>
        /// <returns></returns>
        public static Image Watermark(this Image Image, Image WaterMarkImage, int X = -1, int Y = -1)
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
            bm_marcaDagua.MakeTransparent(bm_marcaDagua.GetPixel(0, 0));
            // Copia o resultado na imagem
            var gr = Graphics.FromImage(bm_Resultado);
            gr.DrawImage(bm_marcaDagua, X, Y);
            return bm_Resultado;
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

        public static Image CreateSolidImage(this Color Color, int Width, int Height)
        {
            var Bmp = new Bitmap(Width, Height);
            using (var gfx = Graphics.FromImage(Bmp))
            {
                using (var brush = new SolidBrush(Color))
                {
                    gfx.FillRectangle(brush, 0, 0, Width, Height);
                }
            }

            return Bmp;
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
                    X = (int)Math.Round(bitmap.Width / 2d - tamanho.Width / 2f);
                }

                if (Y == -1)
                {
                    Y = (int)Math.Round(bitmap.Height / 2d - tamanho.Height / 2f);
                }

                Color = Color ?? bitmap.GetPixel(X, Y).GetContrastColor(50f);
                var B = new SolidBrush((Color)Color);
                graphics.DrawString(Text, Font, B, X, Y);
            }

            return bitmap;
        }

        public static Image Monochrome(this Image Image, Color Color, float Alpha = 0f) => Image.Grayscale().Translate(Color.R, Color.G, Color.B, Alpha);

        /// <summary>
        /// Inverte as cores de uma imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <returns></returns>
        public static Image Negative(this Image img)
        {
            var copia = new Bitmap(img);
            var cm = new ColorMatrix(new float[][] { new float[] { -1, 0f, 0f, 0f, 0f }, new float[] { 0f, -1, 0f, 0f, 0f }, new float[] { 0f, 0f, -1, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });
            draw_adjusted_image(copia, cm);
            return copia;
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
            draw_adjusted_image(copia, cm);
            return copia;
        }

        private static bool draw_adjusted_image(Image img, ColorMatrix cm)
        {
            try
            {
                var bmp = new Bitmap(img); // create a copy of the source image
                var imgattr = new ImageAttributes();
                var rc = new Rectangle(0, 0, img.Width, img.Height);
                var g = Graphics.FromImage(img);

                // associate the ColorMatrix object with an ImageAttributes object
                imgattr.SetColorMatrix(cm);

                // draw the copy of the source image back over the original image,
                // applying the ColorMatrix
                g.DrawImage(bmp, rc, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgattr);
                g.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converte uma Imagem para Escala de cinza
        /// </summary>
        /// <param name="img">imagem original</param>
        /// <returns></returns>
        public static Image Grayscale(this Image img)
        {
            var copia = new Bitmap(img);
            var cm = new ColorMatrix(new float[][] { new float[] { 0.299f, 0.299f, 0.299f, 0f, 0f }, new float[] { 0.587f, 0.587f, 0.587f, 0f, 0f }, new float[] { 0.114f, 0.114f, 0.114f, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });
            draw_adjusted_image(copia, cm);
            return copia;
        }

        public static bool CompareARGB(this Color Color1, Color Color2, bool IgnoreAlpha = true) => Color1.CompareARGB(IgnoreAlpha, Color2);

        public static bool CompareARGB(this Color Color1, bool IgnoreAlpha, params Color[] Colors) => (Colors = Colors ?? Array.Empty<Color>()).Any(Color2 => Color1.R == Color2.R && Color1.G == Color2.G && Color1.B == Color2.B && (IgnoreAlpha || Color1.A == Color2.A));

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
            g.DrawImage(originalimage, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), 0, 0, originalimage.Width, originalimage.Height, GraphicsUnit.Pixel, imageAttributes);
            return adjustedImage;
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
            Func<int, bool> IsAllWhiteRow = row =>
            {
                for (int i = 0, loopTo = w - 1; i <= loopTo; i++)
                {
                    if (bitmap.GetPixel(i, row).ToArgb() != Color.Value.ToArgb())
                    {
                        return false;
                    }
                }

                return true;
            };
            Func<int, bool> IsAllWhiteColumn = col =>
            {
                for (int i = 0, loopTo = h - 1; i <= loopTo; i++)
                {
                    if (bitmap.GetPixel(col, i).ToArgb() != Color.Value.ToArgb())
                    {
                        return false;
                    }
                }

                return true;
            };
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
        /// Cropa uma imagem a patir do centro
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="Size">Tamanho</param>
        /// <returns></returns>
        public static Image Crop(this Image Image, Size Size)
        {
            return Image.Crop(Size.Width, Size.Height);
        }

        /// <summary>
        /// Cropa uma imagem a patir do centro
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="maxWidth">Largura maxima</param>
        /// <param name="maxHeight">Altura maxima</param>
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
                g.DrawImage(Image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(left, top, srcWidth, srcHeight), GraphicsUnit.Pixel);
                finalImage = bitmap;
            }

            using (var encParams = new EncoderParameters(1))
            {
                encParams.Param[0] = new EncoderParameter(Encoder.Quality, 100);
                finalImage.RotateFlip(Image.GetRotateFlip());
                return finalImage;
            }
        }

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
                if (parts[0].AdjustBlankSpaces().EndsWith("%"))
                {
                    parts[0] = parts[0].AdjustBlankSpaces().CalculateValueFromPercent(Original.Width).RoundDecimal().ToString();
                }

                if (parts[1].AdjustBlankSpaces().EndsWith("%"))
                {
                    parts[1] = parts[1].AdjustBlankSpaces().CalculateValueFromPercent(Original.Height).RoundDecimal().ToString();
                }

                size = new Size(parts[0].ToInteger(), parts[1].ToInteger());
            }
            else
            {
                if (Percent.AdjustBlankSpaces().EndsWith("%"))
                {
                    Percent = Percent.Trim('%').AdjustBlankSpaces();
                }

                if (Percent.IsNumber())
                {
                    size.Width = Convert.ToInt32(Percent.ToInteger().CalculateValueFromPercent(Original.Width).RoundDecimal().ToString());
                    size.Height = Convert.ToInt32(Percent.ToInteger().CalculateValueFromPercent(Original.Height).RoundDecimal().ToString());
                }
            }

            return Original.Resize(size, OnlyResizeIfWider);
        }

        public static Image ResizePercent(this Image Original, decimal Percent, bool OnlyResizeIfWider = true)
        {
            return Original.ResizePercent(Percent.ToPercentString(), OnlyResizeIfWider);
        }

        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="ResizeExpression">uma string contendo uma expressão de tamanho</param>
        /// <param name="OnlyResizeIfWider">Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada</param>
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

        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="Size">Tamanho</param>
        /// <param name="OnlyResizeIfWider">Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada</param>
        /// <returns></returns>
        public static Image Resize(this Image Original, Size Size, bool OnlyResizeIfWider = true)
        {
            return Original.Resize(Size.Width, Size.Height, OnlyResizeIfWider);
        }

        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="NewWidth">Nova Largura</param>
        /// <param name="MaxHeight">Altura máxima</param>
        /// <param name="OnlyResizeIfWider">Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada</param>
        /// <returns></returns>
        public static Image Resize(this Image Original, int NewWidth, int MaxHeight, bool OnlyResizeIfWider = true)
        {
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
        /// Interpreta uma string de diversas formas e a transforma em um <see cref="Size"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static Size ParseSize(this string Text)
        {
            var s = new Size();
            Text = Text.ReplaceMany(" ", "px", " ", ";", ":").ToLower().Trim();
            Text = Text.Replace("largura", "width");
            Text = Text.Replace("altura", "height");
            Text = Text.Replace("l ", "w ");
            Text = Text.Replace("a ", "h ");
            try
            {
                switch (true)
                {
                    case object _ when Text.IsNumber():
                        {
                            s.Width = Convert.ToInt32(Text);
                            s.Height = s.Width;
                            break;
                        }

                    case object _ when Text.Like("width*") && !Text.Like("*height*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetAfter("width"));
                            s.Height = Convert.ToInt32(Text.GetAfter("width"));
                            break;
                        }

                    case object _ when Text.Like("height*") && !Text.Like("*width*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetAfter("height"));
                            s.Height = Convert.ToInt32(Text.GetAfter("height"));
                            break;
                        }

                    case object _ when Text.Like("w*") && !Text.Like("*h*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetAfter("w"));
                            s.Height = Convert.ToInt32(Text.GetAfter("w"));
                            break;
                        }

                    case object _ when Text.Like("h*") && !Text.Like("*w*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetAfter("h"));
                            s.Height = Convert.ToInt32(Text.GetAfter("h"));
                            break;
                        }

                    case object _ when Text.Like("width*height*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetBetween("width", "height"));
                            s.Height = Convert.ToInt32(Text.GetAfter("height"));
                            break;
                        }

                    case object _ when Text.Like("height*width*"):
                        {
                            s.Height = Convert.ToInt32(Text.GetBetween("height", "width"));
                            s.Width = Convert.ToInt32(Text.GetAfter("width"));
                            break;
                        }

                    case object _ when Text.Like("w*h*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetBetween("w", "h"));
                            s.Height = Convert.ToInt32(Text.GetAfter("h"));
                            break;
                        }

                    case object _ when Text.Like("h*w*"):
                        {
                            s.Height = Convert.ToInt32(Text.GetBetween("h", "w"));
                            s.Width = Convert.ToInt32(Text.GetAfter("w"));
                            break;
                        }

                    case object _ when Text.Like("*x*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "x" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "x" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    case object _ when Text.Like("*by*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "by" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "by" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    case object _ when Text.Like("*por*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "por" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "por" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    case object _ when Text.Like("*,*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    case object _ when Text.Like("*-*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    case object _ when Text.Like("*_*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    default:
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }
                }
            }
            catch
            {
            }

            return s;
        }

        /// <summary>
        /// Lista com todos os formatos de imagem
        /// </summary>
        /// <returns></returns>
        public static ImageFormat[] ImageTypes { get; private set; } = new[] { ImageFormat.Bmp, ImageFormat.Emf, ImageFormat.Exif, ImageFormat.Gif, ImageFormat.Icon, ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Tiff, ImageFormat.Wmf };

        /// <summary>
        /// Retorna o formato da imagem correspondente a aquela imagem
        /// </summary>
        /// <param name="OriginalImage"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(this Image OriginalImage)
        {
            return ImageTypes.Where(p => p.Guid == OriginalImage.RawFormat.Guid).FirstOr(ImageFormat.Png);
        }

        /// <summary>
        /// Pega o encoder a partir de um formato de imagem
        /// </summary>
        /// <param name="RawFormat">Image format</param>
        /// <returns>image codec info.</returns>
        public static ImageCodecInfo GetEncoderInfo(this ImageFormat RawFormat)
        {
            return ImageCodecInfo.GetImageDecoders().Where(c => c.FormatID == RawFormat.Guid).FirstOr(ImageCodecInfo.GetImageDecoders().Where(c => c.FormatID == ImageFormat.Png.Guid).First());
        }

        /// <summary>
        /// Combina 2 ou mais imagens em uma única imagem
        /// </summary>
        /// <param name="Images">Lista de Imagens para combinar</param>
        /// <param name="VerticalFlow">Se TRUE, combina as Imagens verticalmente (Uma em baixo da outra), caso contrario as imagens serão combinadas horizontalmente (Uma do lado da outra da esquerda para a direita)</param>
        /// <returns>Um Bitmap com a combinaçao de todas as imagens da Lista</returns>
        public static Bitmap CombineImages(bool VerticalFlow, params Image[] Images)
        {
            return Images.CombineImages(VerticalFlow);
        }

        /// <summary>
        /// Combina 2 ou mais imagens em uma única imagem
        /// </summary>
        /// <param name="Images">Array de Imagens para combinar</param>
        /// <param name="VerticalFlow">Se TRUE, combina as Imagens verticalmente (Uma em baixo da outra), caso contrario as imagens serão combinadas horizontalmente (Uma do lado da outra da esquerda para a direita)</param>
        /// <returns>Um Bitmap com a combinaçao de todas as imagens do Array</returns>
        public static Bitmap CombineImages(this IEnumerable<Image> Images, bool VerticalFlow = false)
        {
            Bitmap imagemFinal = null;
            int width = 0;
            int height = 0;
            foreach (Image image in Images)
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
            }

            // cria um bitmap para tratar a imagem combinada
            imagemFinal = new Bitmap(width, height);

            // Obtem o objeto gráfico da imagem
            using (var g = Graphics.FromImage(imagemFinal))
            {
                // define a cor de fundo
                g.Clear(Color.White);

                // percorre imagem por imagem e gera uma unica imagem final
                int offset = 0;
                foreach (Bitmap image in Images)
                {
                    if (VerticalFlow)
                    {
                        g.DrawImage(image, new Rectangle(0, offset, image.Width, image.Height));
                        offset += image.Height;
                    }
                    else
                    {
                        g.DrawImage(image, new Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }
            }

            return imagemFinal;
        }

        /// <summary>
        /// Retorna uma lista com as N cores mais utilizadas na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>uma lista de Color</returns>
        public static IEnumerable<HSVColor> GetMostUsedColors(this Image Image, int Count)
        {
            return new Bitmap(Image).GetMostUsedColors().Take(Count);
        }

        /// <summary>
        /// Retorna uma lista com as cores utilizadas na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>uma lista de Color</returns>
        public static IEnumerable<HSVColor> GetMostUsedColors(this Image Image)
        {
            return Image.ColorPallette().Keys;
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
                            if (dctColorIncidence.Keys.Contains(pixelColor))
                            {
                                dctColorIncidence[pixelColor] = dctColorIncidence[pixelColor] + 1;
                            }
                            else
                            {
                                dctColorIncidence.Add(pixelColor, 1);
                            }

                            linha = linha + 1;
                        }

                        coluna = coluna + 1;
                    }
                }

            return dctColorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => new HSVColor(Color.FromArgb(x.Key)), x => x.Value);
        }

        /// <summary>
        /// Pixeliza uma imagem
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="PixelateSize"></param>
        /// <returns></returns>
        public static Image Pixelate(this Image Image, int PixelateSize = 1)
        {
            var rectangle = new Rectangle(0, 0, Image.Width, Image.Height);
            PixelateSize = PixelateSize + 1;
            var pixelated = new Bitmap(Image.Width, Image.Height);
            using (var graphics = Graphics.FromImage(pixelated))
            {
                graphics.DrawImage(Image, new Rectangle(0, 0, Image.Width, Image.Height), new Rectangle(0, 0, Image.Width, Image.Height), GraphicsUnit.Pixel);
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
        /// Transforma uma imagem em um stream
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns></returns>
        public static Stream ToStream(this Image Image, ImageFormat Format = null)
        {
            Stream s = new MemoryStream();
            Image.Save(s, Format ?? ImageFormat.Png);
            s.Position = 0L;
            return s;
        }

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
        /// redimensiona e Cropa uma imagem, aproveitando a maior parte dela
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static Image ResizeCrop(this Image Image, int Width, int Height)
        {
            return Image.Resize(Width, Height, false).Crop(Width, Height);
        }

        /// <summary>
        /// redimensiona e Cropa uma imagem, aproveitando a maior parte dela
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static Image ResizeCrop(this Image Image, int Width, int Height, bool OnlyResizeIfWider)
        {
            return Image.Resize(Width, Height, OnlyResizeIfWider).Crop(Width, Height);
        }
    }
}