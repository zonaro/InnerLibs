using System.Drawing;
using System.Drawing.Imaging;


namespace Extensions
{

public partial class Util
{



    /// <summary>
    /// Aplica um borrão a imagem
    /// </summary>
    /// <param name="Img"></param>
    /// <param name="BlurSize"></param>
    /// <returns></returns>
    public static Image Blur(this Image Img, int BlurSize = 5) => Blur(Img, BlurSize, new System.Drawing.Rectangle(0, 0, Img.Width, Img.Height));

    /// <summary>
    /// Aplica um borrão a uma determinada parte da imagem
    /// </summary>
    /// <param name="Img"></param>
    /// <param name="BlurSize"></param>
    /// <param name="rectangle"></param>
    /// <returns></returns>
    public static unsafe Image Blur(this Image Img, int BlurSize, System.Drawing.Rectangle rectangle)
    {
        Bitmap blurred = new Bitmap(Img.Width, Img.Height);

        // make an exact copy of the bitmap provided
        using (Graphics graphics = Graphics.FromImage(blurred))
            graphics.DrawImage(Img, new System.Drawing.Rectangle(0, 0, Img.Width, Img.Height),
                new System.Drawing.Rectangle(0, 0, Img.Width, Img.Height), GraphicsUnit.Pixel);

        // Lock the bitmap's bits
        BitmapData blurredData = blurred.LockBits(new System.Drawing.Rectangle(0, 0, Img.Width, Img.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);

        // GetCliente bits per pixel for current PixelFormat
        int bitsPerPixel = Image.GetPixelFormatSize(blurred.PixelFormat);

        // GetCliente pointer to first line
        byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

        // look at every pixel in the blur rectangle
        for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
        {
            for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
            {
                int avgR = 0, avgG = 0, avgB = 0;
                int blurPixelCount = 0;

                // average the color of the red, green and blue for each pixel in the blur size
                // while making sure you don't go outside the image bounds
                for (int x = xx; (x < xx + BlurSize && x < Img.Width); x++)
                {
                    for (int y = yy; (y < yy + BlurSize && y < Img.Height); y++)
                    {
                        // GetCliente pointer to RGB
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
                        // GetCliente pointer to RGB
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
}
}