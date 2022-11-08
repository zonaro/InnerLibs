using InnerLibs.Printer;
using InnerLibs.Printer.Command;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace InnerLibs.EscDarumaCommands
{
    public class EscDaruma : IPrintCommand
    {
        #region Properties

        public int ColsCondensed
        {
            get
            {
                return 57;
            }
        }

        public int ColsExpanded
        {
            get
            {
                return 25;
            }
        }

        public int ColsNormal
        {
            get
            {
                return 48;
            }
        }

        // Encoding.GetEncoding("IBM860")
        public Encoding Encoding { get; set; }

        #endregion Properties

        #region Methods

        private static IEnumerable<byte> StoreQr(string qrData, QrCodeSize size)
        {
            int length = qrData.Length + 3;
            byte b = (byte)(length % 255);
            byte b2 = (byte)Math.Round(length / 255d);
            return (new byte[] { 27, 106, 49 }).AddBytes(new byte[] { 27, 129 }).AddBytes(new[] { b }).AddBytes(new[] { b2 }).AddBytes(new[] { ((int)size + 3).ToByte() }).AddBytes(new[] { 'M'.ToByte() });
        }

        public byte[] AutoTest()
        {
            return new byte[] { 28, 'M'.ToByte(), 254, 0 };
        }

        public byte[] Bold(bool state)
        {
            return state == true ? (new byte[] { 27, 'E'.ToByte() }) : (new byte[] { 27, 'F'.ToByte() });
        }

        public byte[] Center()
        {
            return new byte[] { 27, 'j'.ToByte(), 1 };
        }

        public byte[] Code128(string code)
        {
            return (new byte[] { 27, 'b'.ToByte(), 5 }).AddBytes(new byte[] { 2 }).AddBytes(new byte[] { 50 }).AddBytes(new byte[] { 0 }).AddTextBytes(code, Encoding).AddBytes(new byte[] { 0 }); // Code Type
            // Witdh Height If print code informed (1 print, 0 dont print)
        }

        public byte[] Code39(string code)
        {
            return (new byte[] { 27, 'b'.ToByte(), 6 }).AddBytes(new byte[] { 2 }).AddBytes(new byte[] { 50 }).AddBytes(new byte[] { 0 }).AddTextBytes(code, Encoding).AddBytes(new byte[] { 0 }); // Code Type
            // Witdh Height If print code informed (1 print, 0 dont print)
        }

        public byte[] Condensed(bool state)
        {
            return state == true ? (new byte[] { 27, 15 }) : (new byte[] { 27, 18, 20 });
        }

        public byte[] Ean13(string code)
        {
            if (code.Trim().Length != 13)
                return new byte[0];
            return (new byte[] { 27, 'b'.ToByte(), 1 }).AddBytes(new byte[] { 2 }).AddBytes(new byte[] { 50 }).AddBytes(new byte[] { 0 }).AddTextBytes(code.Substring(0, 12), Encoding).AddBytes(new byte[] { 0 }); // Code Type
            // Witdh Height If print code informed (1 print, 0 dont print)
        }

        public byte[] Expanded(bool state)
        {
            return state == true ? (new byte[] { 27, 'w'.ToByte(), 1 }) : (new byte[] { 27, 'w'.ToByte(), 0 });
        }

        public byte[] FullCut()
        {
            return new byte[] { 27, 'm'.ToByte() };
        }

        public byte[] Initialize()
        {
            return new byte[] { 27, '@'.ToByte() };
        }

        public byte[] Italic(bool state)
        {
            return state == true ? (new byte[] { 27, '4'.ToByte(), 1 }) : (new byte[] { 27, '4'.ToByte(), 0 });
        }

        public byte[] LargeFont()
        {
            return new byte[] { 27, 14, 0 };
        }

        public byte[] LargerFont()
        {
            return new byte[] { 27, 14, 0 };
        }

        public byte[] Left()
        {
            return new byte[] { 27, 'j'.ToByte(), 0 };
        }

        public byte[] NormalFont()
        {
            return new byte[] { 20 };
        }

        public byte[] OpenDrawer()
        {
            return new byte[] { 27, 'p'.ToByte() };
        }

        public byte[] PartialCut()
        {
            return new byte[] { 27, 'm'.ToByte() };
        }

        public byte[] PrintImage(Image image, bool highDensity)
        {
            var list = new List<byte>();
            var bmp = new Bitmap(image);
            string send = InnerLibs.Text.Empty + '\u001b' + '3' + '\0';
            var data = new byte[send.Length];
            for (int i = 0, loopTo = send.Length - 1; i <= loopTo; i++)
                data[i] = Text.ToAscByte(send[i]);
            list.AddRange(data);
            data[0] = Text.ToAscByte('\0');
            data[1] = Text.ToAscByte('\0');
            data[2] = Text.ToAscByte('\0');
            var escBmp = new[] { (byte)0x1B, (byte)0x2A, (byte)0x0, (byte)0x0, (byte)0x0 };
            escBmp[2] = Text.ToAscByte('!');
            escBmp[3] = (byte)(bmp.Width % 256);
            escBmp[4] = (byte)Math.Round(bmp.Width / 256d);
            for (double i = 0d, loopTo1 = bmp.Height / 24d + 1d - 1d; i <= loopTo1; i++)
            {
                list.AddRange(escBmp);
                for (int j = 0, loopTo2 = bmp.Width - 1; j <= loopTo2; j++)
                {
                    for (int k = 0; k <= 24 - 1; k++)
                    {
                        if (i * 24d + k < bmp.Height)
                        {
                            var pixelColor = bmp.GetPixel(j, (int)Math.Round(i * 24d + k));
                            if (!(pixelColor.R > 160 && pixelColor.G > 160 && pixelColor.B > 160))
                                data[(int)Math.Round(k / 8d)] += (byte)(128 >> k % 8);
                            if (highDensity)
                                continue;
                            if (pixelColor.R == 0)
                                data[(int)Math.Round(k / 8d)] += (byte)(128 >> k % 8);
                        }
                    }

                    list.AddRange(data);
                    data[0] = Text.ToAscByte('\0');
                    data[1] = Text.ToAscByte('\0');
                    data[2] = Text.ToAscByte('\0');
                }
            }

            list.AddRange(new byte[] { 27, '@'.ToByte() });
            return list.ToArray();
        }

        public byte[] Printqrdata(string qrData) => ((IPrintCommand)this).PrintQrData(qrData);

        public byte[] PrintQrData(string qrData, QrCodeSize qrCodeSize)
        {
            var list = new List<byte>();
            list.AddRange(StoreQr(qrData, qrCodeSize));
            list.AddRange(Encoding.GetBytes(qrData));
            return list.ToArray();
        }

        public byte[] Right()
        {
            return new byte[] { 27, 'j'.ToByte(), 2 };
        }

        public byte[] Underline(bool state)
        {
            return state == true ? (new byte[] { 27, '-'.ToByte(), 1 }) : (new byte[] { 27, '-'.ToByte(), 0 });
        }

        byte[] IPrintCommand.PrintQrData(string qrData)
        {
            return PrintQrData(qrData, QrCodeSize.Size0);
        }

        #endregion Methods
    }
}