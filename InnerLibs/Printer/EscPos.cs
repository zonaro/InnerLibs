using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using InnerLibs.Printer;
using InnerLibs.Printer.Command;

namespace InnerLibs.EscPosCommands
{
    public class EscPos : IPrintCommand
    {

        #region Properties

        public int ColsNomal
        {
            get
            {
                return 48;
            }
        }

        public int ColsCondensed
        {
            get
            {
                return 64;
            }
        }

        public int ColsExpanded
        {
            get
            {
                return 24;
            }
        }

        // Encoding.GetEncoding("IBM860") 
        public Encoding Encoding { get; set; }

        #endregion



        #region Methods



        public byte[] AutoTest()
        {
            return new byte[] { 29, 40, 65, 2, 0, 0, 2 };
        }

        public byte[] Left()
        {
            return new byte[] { 27, 'a'.ToByte(), 0 };
        }

        public byte[] Right()
        {
            return new byte[] { 27, 'a'.ToByte(), 2 };
        }

        public byte[] Center()
        {
            return new byte[] { 27, 'a'.ToByte(), 1 };
        }

        public byte[] Code128(string code)
        {
            return (new byte[] { 29, 119, 2 }).AddBytes(new byte[] { 29, 104, 50 }).AddBytes(new byte[] { 29, 102, 1 }).AddBytes(new byte[] { 29, 72, 0 }).AddBytes(new byte[] { 29, 107, 73 }).AddBytes(new[] { (byte)(code.Length + 2) }).AddBytes(new byte[] { 123, 66 }).AddTextBytes(code, Encoding); // Width
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
            return (new byte[] { 29, 119, 2 }).AddBytes(new byte[] { 29, 104, 50 }).AddBytes(new byte[] { 29, 72, 0 }).AddBytes(new byte[] { 29, 107, 67, 12 }).AddTextBytes(code.Substring(0, 12), Encoding); // Width
            // Height
            // If print code informed
        }

        public byte[] OpenDrawer()
        {
            return new byte[] { 27, 112, 0, 60, 120 };
        }

        public byte[] Italic(bool state)
        {
            return state == true ? (new byte[] { 27, '4'.ToByte() }) : (new byte[] { 27, '5'.ToByte() });
        }

        public byte[] Bold(bool state)
        {
            return state == true ? (new byte[] { 27, 'E'.ToByte(), 1 }) : (new byte[] { 27, 'E'.ToByte(), 0 });
        }

        public byte[] Underline(bool state)
        {
            return state == true ? (new byte[] { 27, '-'.ToByte(), 1 }) : (new byte[] { 27, '-'.ToByte(), 0 });
        }

        public byte[] Expanded(bool state)
        {
            return state == true ? (new byte[] { 29, '!'.ToByte(), 16 }) : (new byte[] { 29, '!'.ToByte(), 0 });
        }

        public byte[] Condensed(bool state)
        {
            return state == true ? (new byte[] { 27, '!'.ToByte(), 1 }) : (new byte[] { 27, '!'.ToByte(), 0 });
        }

        public byte[] NormalFont()
        {
            return new byte[] { 27, '!'.ToByte(), 0 };
        }

        public byte[] LargeFont()
        {
            return new byte[] { 29, '!'.ToByte(), 16 };
        }

        public byte[] LargerFont()
        {
            return new byte[] { 29, '!'.ToByte(), 32 };
        }

        public byte[] Initialize()
        {
            return new byte[] { 27, '@'.ToByte() };
        }

        public byte[] PrintImage(Image image, bool highDensity)
        {
            return PrinterByteExtensions.SharedImagePrinter(image, highDensity);
        }

        public byte[] FullCut()
        {
            return new byte[] { 29, 'V'.ToByte(), 65, 3 };
        }

        public byte[] PartialCut()
        {
            return new byte[] { 29, 'V'.ToByte(), 65, 3 };
        }

        private static byte[] Size(QrCodeSize pSize)
        {
            return new byte[] { 29, 40, 107, 3, 0, 49, 67 }.AddBytes(new[] { ((int)pSize + 3).ToByte() });
        }

        private IEnumerable<byte> ModelQr()
        {
            return new byte[] { 29, 40, 107, 4, 0, 49, 65, 50, 0 };
        }

        private IEnumerable<byte> ErrorQr()
        {
            return new byte[] { 29, 40, 107, 3, 0, 49, 69, 48 };
        }

        private static IEnumerable<byte> StoreQr(string qrData)
        {
            int length = qrData.Length + 3;
            byte b = (byte)(length % 256);
            byte b2 = (byte)Math.Round(length / 256d);
            return (new byte[] { 29, 40, 107 }).AddBytes(new[] { b }).AddBytes(new[] { b2 }).AddBytes(new byte[] { 49, 80, 48 });
        }

        private IEnumerable<byte> PrintQr()
        {
            return new byte[] { 29, 40, 107, 3, 0, 49, 81, 48 };
        }

        public byte[] PrintQrData(string qrData)
        {
            return PrintQrData(qrData, QrCodeSize.Size0);
        }

        public byte[] PrintQrData(string qrData, QrCodeSize qrCodeSize)
        {
            var list = new List<byte>();
            list.AddRange(ModelQr());
            list.AddRange(Size(qrCodeSize));
            list.AddRange(ErrorQr());
            list.AddRange(StoreQr(qrData));
            list.AddRange(Encoding.GetBytes(qrData));
            list.AddRange(PrintQr());
            return list.ToArray();
        }



        #endregion





    }
}