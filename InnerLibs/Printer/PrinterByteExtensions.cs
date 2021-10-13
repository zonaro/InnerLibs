﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;

namespace InnerLibs.Printer
{
    internal class RawPrinterHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        #region Declaration Dll

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);
        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In][MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);
        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        #endregion

        #region Methods

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the printer queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, int dwCount)
        {
            int dwError = 0;
            int dwWritten = 0;
            var hPrinter = new IntPtr(0);
            var di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.
            di.pDocName = "RAW Printer Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }

                    EndDocPrinter(hPrinter);
                }

                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
                dwError = Marshal.GetLastWin32Error();
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            var fs = new FileStream(szFileName, FileMode.Open);

            // Create a BinaryReader on the file.
            var br = new BinaryReader(fs);

            // Dim an array of bytes big enough to hold the file's contents.
            var bytes = new byte[(int)(fs.Length - 1L + 1)];
            bool bSuccess = false;

            // Your unmanaged pointer.
            var pUnmanagedBytes = new IntPtr(0);
            int nLength = Convert.ToInt32(fs.Length);

            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        public static bool SendBytesToPrinter(string szPrinterName, byte[] data)
        {
            var pUnmanagedBytes = Marshal.AllocCoTaskMem(data.Length); // Allocate unmanaged memory
            Marshal.Copy(data, 0, pUnmanagedBytes, data.Length); // copy bytes into unmanaged memory
            bool retval = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, data.Length);
            Marshal.FreeCoTaskMem(pUnmanagedBytes); // Free the allocated unmanaged memory
            return retval;
        }

        #endregion
    }

    internal enum Justifications
    {
        Left,
        Right,
        Center
    }

    public enum QrCodeSize
    {
        Size0,
        Size1,
        Size2
    }

    internal static class PrinterByteExtensions
    {
        internal static byte[] SharedImagePrinter(Image image, bool highDensity)
        {
            var list = new List<byte>();
            var bmp = new Bitmap(image);

            // Set character line spacing to n dotlines
            string send = "" + '\u001b' + '3' + '\0';
            var data = new byte[send.Length];
            for (int i = 0, loopTo = send.Length - 1; i <= loopTo; i++)
                data[i] = (byte)Strings.AscW(send[i]);
            list.AddRange(data);
            data[0] = (byte)Strings.AscW('\0');
            data[1] = (byte)Strings.AscW('\0');
            data[2] = (byte)Strings.AscW('\0'); // Clear

            // ESC * m nL nH d1…dk   Select bitmap mode
            var escBmp = new[] { (byte)0x1B, (byte)0x2A, (byte)0x0, (byte)0x0, (byte)0x0 };
            escBmp[2] = (byte)Strings.AscW('!');
            // nL, nH
            escBmp[3] = (byte)(bmp.Width % 256);
            escBmp[4] = (byte)Math.Round(bmp.Width / 256d);

            // Cycle picture pixel print
            // High cycle
            for (double i = 0d, loopTo1 = bmp.Height / 24d + 1d - 1d; i <= loopTo1; i++)
            {
                // Set the bitmap mode
                list.AddRange(escBmp);

                // Width
                for (int j = 0, loopTo2 = bmp.Width - 1; j <= loopTo2; j++)
                {
                    for (int k = 0; k <= 24 - 1; k++)
                    {
                        if (i * 24d + k < bmp.Height) // if within the BMP size
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

                    // Write data，24dots
                    list.AddRange(data);
                    data[0] = (byte)Strings.AscW('\0');
                    data[1] = (byte)Strings.AscW('\0');
                    data[2] = (byte)Strings.AscW('\0'); // Clear
                }

                var data2 = new[] { (byte)0xA };
                list.AddRange(data2);
            }

            list.AddRange(new byte[] { 27, '@'.ToByte() });
            return list.ToArray();
        }

        public static byte ToByte(this char c)
        {
            return (byte)Strings.AscW(c);
        }

        public static byte ToByte(this Enum c)
        {
            return (byte)Convert.ToInt16(c);
        }

        public static byte ToByte(this short c)
        {
            return (byte)c;
        }

        public static byte ToByte(this int c)
        {
            return (byte)c;
        }

        public static byte[] AddBytes(this byte[] bytes, byte[] pAddBytes)
        {
            if (pAddBytes is null)
                return bytes;
            var list = new List<byte>();
            list.AddRange(bytes);
            list.AddRange(pAddBytes);
            return list.ToArray();
        }

        public static byte[] AddTextBytes(this byte[] bytes, string value, Encoding Encoding)
        {
            if (value.IsBlank())
                return bytes;
            var list = new List<byte>();
            list.AddRange(bytes);
            list.AddRange(value.TextBytes(Encoding));
            return list.ToArray();
        }

        public static byte[] TextBytes(this string Value, Encoding Encoding)
        {
            return (Encoding ?? Encoding.Default).GetBytes(Value);
        }

        public static byte[] AddLF(this byte[] bytes, Encoding Encoding = null)
        {
            return bytes.AddTextBytes(Constants.vbLf, Encoding);
        }

        public static byte[] AddCrLF(this byte[] bytes, Encoding Encoding = null)
        {
            return bytes.AddTextBytes(Constants.vbCrLf, Encoding);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}