using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;

namespace InnerLibs
{
    public static class AsciiArt
    {
        public static string ToAsciiArt(this Bitmap image, int ratio)
        {
            image = (Bitmap)image.Negative();
            bool toggle = false;
            var sb = new StringBuilder();
            int h = 0;
            while (h < image.Height)
            {
                int w = 0;
                while (w < image.Width)
                {
                    var pixelColor = image.GetPixel(w, h);
                    int red, green, blue;
                    red = (int)Math.Round((pixelColor.R.ToInteger() + pixelColor.G.ToInteger() + pixelColor.B.ToInteger()) / 3d);
                    green = red;
                    blue = green;
                    var grayColor = Color.FromArgb(red, green, blue);
                    if (!toggle)
                    {
                        int index = (int)Math.Round(grayColor.R * 10 / 255d);
                        sb.Append(asciiChars[index]);
                    }

                    w += ratio;
                }

                if (!toggle)
                {
                    sb.AppendLine();
                    toggle = true;
                }
                else
                {
                    toggle = false;
                }

                h += ratio;
            }

            return sb.ToString();
        }

        private static string[] asciiChars = new[] { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", " " };

        public static string ToAsciiArt(this Bitmap sourceBitmap, int pixelBlockSize, int colorCount = 0)
        {
            var sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var pixelBuffer = new byte[(sourceData.Stride * sourceData.Height)];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);
            var asciiArt = new StringBuilder();
            int avgBlue = 0;
            int avgGreen = 0;
            int avgRed = 0;
            int offset = 0;
            int rows = (int)Math.Round(sourceBitmap.Height / (double)pixelBlockSize);
            int columns = (int)Math.Round(sourceBitmap.Width / (double)pixelBlockSize);
            if (colorCount > 0)
            {
                colorCharacters = Generate.RandomWord(colorCount);
            }

            for (int y = 0, loopTo = rows - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = columns - 1; x <= loopTo1; x++)
                {
                    avgBlue = 0;
                    avgGreen = 0;
                    avgRed = 0;
                    for (int pY = 0, loopTo2 = pixelBlockSize - 1; pY <= loopTo2; pY++)
                    {
                        for (int pX = 0, loopTo3 = pixelBlockSize - 1; pX <= loopTo3; pX++)
                        {
                            offset = y * pixelBlockSize * sourceData.Stride + x * pixelBlockSize * 4;
                            offset += pY * sourceData.Stride;
                            offset += pX * 4;
                            try
                            {
                                avgBlue += pixelBuffer[offset];
                                avgGreen += pixelBuffer[offset + 1];
                                avgRed += pixelBuffer[offset + 2];
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }

                    avgBlue = (int)Math.Round(avgBlue / (double)(pixelBlockSize * pixelBlockSize));
                    avgGreen = (int)Math.Round(avgGreen / (double)(pixelBlockSize * pixelBlockSize));
                    avgRed = (int)Math.Round(avgRed / (double)(pixelBlockSize * pixelBlockSize));
                    asciiArt.Append(GetColorCharacter(avgBlue, avgGreen, avgRed));
                }

                asciiArt.Append(Constants.vbCrLf);
            }

            return asciiArt.ToString();
        }

        private static string colorCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static string GetColorCharacter(int blue, int green, int red)
        {
            string colorChar = "";
            int intensity = (int)Math.Round((blue + green + red) / 3d * (colorCharacters.Length - 1) / 255d);
            colorChar = colorCharacters.Substring(intensity, 1).ToUpper();
            colorChar += colorChar.ToLower();
            return colorChar;
        }
    }
}