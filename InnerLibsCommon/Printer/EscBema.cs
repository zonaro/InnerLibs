using System;
using System.Collections.Generic;
using System.Text;
using Printers;
using Printers.Command;

namespace EscBemaCommands
{
    public class EscBema : IPrintCommand
    {
        #region Properties

        public int ColsCondensed => 67;

        public int ColsExpanded => 25;

        public int ColsNormal => 50;

        public Encoding Encoding { get; set; } = Encoding.GetEncoding(850);

        #endregion Properties

        #region Methods

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

        private static byte[] SetEscBema() => new byte[] { 29, 249, 32, 0 };

        private static byte[] SetLineSpace3(byte range = 20) => new byte[] { 27, 51, range };

        private static IEnumerable<byte> StoreQr(string qrData, QrCodeSize size)
        {
            int length = qrData.Length;
            byte b = (byte)(length % 256);
            byte b2 = (byte)Math.Round(length / 256d);
            return (new byte[] { 29, 107, 81 }).AddBytes(new[] { size.ToByte() }).AddBytes(new byte[] { 6, 0, 1 }).AddBytes(new[] { b }).AddBytes(new[] { b2 });
        }

        public byte[] AutoTest() => new byte[] { 0x1D, 0xF9, 0x29, 0x30 };

        public byte[] Bold(bool state) => state == true ? (new byte[] { 27, 'E'.ToByte() }) : (new byte[] { 27, 'F'.ToByte() });

        public byte[] Center() => Align(Justifications.Center);

        public byte[] Code128(string code) => (new byte[] { 29, 119, 2 }).AddBytes(new byte[] { 29, 104, 50 }).AddBytes(new byte[] { 29, 102, 1 }).AddBytes(new byte[] { 29, 72, 0 }).AddBytes(new byte[] { 29, 107, 73 }).AddBytes(new[] { (byte)(code.Length + 2) }).AddBytes(new[] { '{'.ToByte(), 'C'.ToByte() }).AddTextBytes(code, Encoding); // Width// Height font hri character If print code informed printCode

        public byte[] Code39(string code) => (new byte[] { 29, 119, 2 }).AddBytes(new byte[] { 29, 104, 50 }).AddBytes(new byte[] { 29, 102, 0 }).AddBytes(new byte[] { 29, 72, 0 }).AddBytes(new byte[] { 29, 107, 4 }).AddTextBytes(code, Encoding).AddBytes(new byte[] { 0 }); // Width// Height font hri character If print code informed

        public byte[] Condensed(bool state) => state == true ? (new byte[] { 27, 15 }) : (new byte[] { 27, 'P'.ToByte() });

        public byte[] Ean13(string code)
        {
            if (code.Trim().Length != 13)
                return new byte[0];
            return (new byte[] { 29, 119, 40 }).AddBytes(new byte[] { 29, 104, 50 }).AddBytes(new byte[] { 29, 72, 0 }).AddBytes(new byte[] { 29, 107, 67, 12 }).AddTextBytes(code.Substring(0, 12), Encoding); // Width
            // Height If print code informed
        }

        public byte[] Expanded(bool state) => new byte[] { 27, 'W'.ToByte(), state == true ? '1'.ToByte() : '0'.ToByte() };

        public byte[] FullCut() => new byte[] { 27, 'w'.ToByte() };

        public byte[] Initialize() => (new byte[] { 27, '@'.ToByte() }).AddBytes(SetEscBema()).AddBytes(SetLineSpace3());

        public byte[] Italic(bool state) => state == true ? (new byte[] { 27, '4'.ToByte() }) : (new byte[] { 27, '5'.ToByte() });

        public byte[] LargeFont() => new byte[] { 27, 'W'.ToByte(), 1, 27, 'd'.ToByte(), 1 };

        public byte[] LargerFont() => new byte[] { 29, '!'.ToByte(), 32 };

        public byte[] Left() => Align(Justifications.Left);

        public byte[] NormalFont() => new byte[] { 27, 'W'.ToByte(), 0, 27, 'd'.ToByte(), 0 };

        public byte[] OpenDrawer() => new byte[] { 27, 'v'.ToByte(), 140 };

        public byte[] PartialCut() => new byte[] { 27, 'm'.ToByte() };

        public byte[] PrintImage(System.Drawing.Image image, bool highDensity) => PrinterByteExtensions.SharedImagePrinter(image, highDensity);

        public byte[] PrintQrData(string qrData) => PrintQrData(qrData, QrCodeSize.Size0);

        public byte[] PrintQrData(string qrData, QrCodeSize qrCodeSize)
        {
            var list = new List<byte>();
            list.AddRange(StoreQr(qrData, qrCodeSize));
            list.AddRange(Encoding.GetBytes(qrData));
            return list.ToArray();
        }

        public byte[] Right() => Align(Justifications.Right);

        public byte[] Underline(bool state) => state == true ? (new byte[] { 27, '-'.ToByte(), 1 }) : (new byte[] { 27, '-'.ToByte(), 0 });

        #endregion Methods
    }
}