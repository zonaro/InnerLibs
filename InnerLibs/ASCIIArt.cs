using System;
using System.Drawing;
using System.Text;

namespace InnerLibs
{
    public static partial class Util
    {
        #region Private Fields

        private static readonly string[] asciiChars = new[] { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", " " };

        #endregion Private Fields

        #region Public Methods

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
                    red = (int)Math.Round((pixelColor.R.ToInt() + pixelColor.G.ToInt() + pixelColor.B.ToInt()) / 3d);
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

        #endregion Public Methods
    }
}