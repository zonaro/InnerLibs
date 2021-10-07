using System;
using System.Collections.Generic;
using System.Text;
using InnerLibs.Printer;
using InnerLibs.Printer.Command;

namespace InnerLibs.EscBemaCommands
{
    public class EscBema : IPrintCommand
    {

        #region Properties

        public int ColsNomal
        {
            get
            {
                return 50;
            }
        }

        public int ColsCondensed
        {
            get
            {
                return 67;
            }
        }

        public int ColsExpanded
        {
            get
            {
                return 25;
            }
        }

        public Encoding Encoding { get; set; } = Encoding.GetEncoding(850);

        #endregion



        #region Methods



        public byte[] AutoTest()
        {
            return new byte[] { 0x1D, 0xF9, 0x29, 0x30 };
        }

        private static IEnumerable<byte> StoreQr(string qrData, QrCodeSize size)
        {
            int length = qrData.Length;
            byte b = (byte)(length % 256);
            byte b2 = (byte)Math.Round(length / 256d);
            return (new byte[] { 29, 107, 81 }).AddBytes(new[] { size.ToByte() }).AddBytes(new byte[] { 6, 0, 1 }).AddBytes(new[] { b }).AddBytes(new[] { b2 });
        }

        public byte[] PrintQrData(string qrData)
        {
            return PrintQrData(qrData, QrCodeSize.Size0);
        }

        public byte[] PrintQrData(string qrData, QrCodeSize qrCodeSize)
        {
            var list = new List<byte>();
            list.AddRange(StoreQr(qrData, qrCodeSize));
            list.AddRange(Encoding.GetBytes(qrData));
            return list.ToArray();
        }

        public byte[] FullCut()
        {
            return new byte[] { 27, 'w'.ToByte() };
        }

        public byte[] PartialCut()
        {
            return new byte[] { 27, 'm'.ToByte() };
        }

        public byte[] Initialize()
        {
            return (new byte[] { 27, '@'.ToByte() }).AddBytes(SetEscBema()).AddBytes(SetLineSpace3());
        }

        private static byte[] SetEscBema()
        {
            return new byte[] { 29, 249, 32, 0 };
        }

        private static byte[] SetLineSpace3(byte range = 20)
        {
            return new byte[] { 27, 51, range };
        }

        public byte[] NormalFont()
        {
            return new byte[] { 27, 'W'.ToByte(), 0, 27, 'd'.ToByte(), 0 };
        }

        public byte[] LargeFont()
        {
            return new byte[] { 27, 'W'.ToByte(), 1, 27, 'd'.ToByte(), 1 };
        }

        public byte[] LargerFont()
        {
            return new byte[] { 29, '!'.ToByte(), 32 };
        }

        public byte[] Italic(bool state)
        {
            return state == true ? (new byte[] { 27, '4'.ToByte() }) : (new byte[] { 27, '5'.ToByte() });
        }

        public byte[] Bold(bool state)
        {
            return state == true ? (new byte[] { 27, 'E'.ToByte() }) : (new byte[] { 27, 'F'.ToByte() });
        }

        public byte[] Underline(bool state)
        {
            return state == true ? (new byte[] { 27, '-'.ToByte(), 1 }) : (new byte[] { 27, '-'.ToByte(), 0 });
        }

        public byte[] Expanded(bool state)
        {
            return new byte[] { 27, 'W'.ToByte(), state == true ? '1'.ToByte() : '0'.ToByte() };
        }

        public byte[] Condensed(bool state)
        {
            return state == true ? (new byte[] { 27, 15 }) : (new byte[] { 27, 'P'.ToByte() });
        }

        public byte[] OpenDrawer()
        {
            return new byte[] { 27, 'v'.ToByte(), 140 };
        }

        public byte[] Code128(string code)
        {
            return (new byte[] { 29, 119, 2 }).AddBytes(new byte[] { 29, 104, 50 }).AddBytes(new byte[] { 29, 102, 1 }).AddBytes(new byte[] { 29, 72, 0 }).AddBytes(new byte[] { 29, 107, 73 }).AddBytes(new[] { (byte)(code.Length + 2) }).AddBytes(new[] { '{'.ToByte(), 'C'.ToByte() }).AddTextBytes(code, Encoding); // Width
            // Height
            // font hri character
            // If print code informed
            // printCode
        }

        public byte[] Code39(string code)
        {
            return (new byte[] { 29, 119, 2 }).AddBytes(new byte[] { 29, 104, 50 }).AddBytes(new byte[] { 29, 102, 0 }).AddBytes(new byte[] { 29, 72, 0 }).AddBytes(new byte[] { 29, 107, 4 }).AddTextBytes(code, Encoding).AddBytes(new byte[] { 0 }); // Width
            // Height
            // font hri character
            // If print code informed
        }

        public byte[] Ean13(string code)
        {
            if (code.Trim().Length != 13)
                return new byte[0];
            return (new byte[] { 29, 119, 40 }).AddBytes(new byte[] { 29, 104, 50 }).AddBytes(new byte[] { 29, 72, 0 }).AddBytes(new byte[] { 29, 107, 67, 12 }).AddTextBytes(code.Substring(0, 12), Encoding); // Width
            // Height
            // If print code informed
        }

        private static byte[] Align(Justifications justification)
        {
            byte lAlign;
            switch (justification)
            {
                case Justifications.Right:
                    {
                        lAlign = '2'.ToByte();
                        break;
                    }

                case Justifications.Center:
                    {
                        lAlign = '1'.ToByte();
                        break;
                    }

                default:
                    {
                        lAlign = '0'.ToByte();
                        break;
                    }
            }

            return new byte[] { 27, 'a'.ToByte(), lAlign };
        }

        public byte[] Left()
        {
            return Align(Justifications.Left);
        }

        public byte[] Right()
        {
            return Align(Justifications.Right);
        }

        public byte[] Center()
        {
            return Align(Justifications.Center);
        }

        public byte[] PrintImage(System.Drawing.Image image, bool highDensity)
        {
            return PrinterByteExtensions.SharedImagePrinter(image, highDensity);
        }

        #endregion

    }
}