Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Printer

    Friend Class RawPrinterHelper
        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
        Public Class DOCINFOA
            <MarshalAs(UnmanagedType.LPStr)>
            Public pDocName As String
            <MarshalAs(UnmanagedType.LPStr)>
            Public pOutputFile As String
            <MarshalAs(UnmanagedType.LPStr)>
            Public pDataType As String
        End Class

#Region "Declaration Dll"

        <DllImport("winspool.Drv", EntryPoint:="OpenPrinterA", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
        Public Shared Function OpenPrinter(
        <MarshalAs(UnmanagedType.LPStr)> ByVal szPrinter As String, <Out> ByRef hPrinter As IntPtr, ByVal pd As IntPtr) As Boolean
        End Function

        <DllImport("winspool.Drv", EntryPoint:="ClosePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
        Public Shared Function ClosePrinter(ByVal hPrinter As IntPtr) As Boolean
        End Function

        <DllImport("winspool.Drv", EntryPoint:="StartDocPrinterA", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
        Public Shared Function StartDocPrinter(ByVal hPrinter As IntPtr, ByVal level As Integer,
        <[In]>
        <MarshalAs(UnmanagedType.LPStruct)> ByVal di As DOCINFOA) As Boolean
        End Function

        <DllImport("winspool.Drv", EntryPoint:="EndDocPrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
        Public Shared Function EndDocPrinter(ByVal hPrinter As IntPtr) As Boolean
        End Function

        <DllImport("winspool.Drv", EntryPoint:="StartPagePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
        Public Shared Function StartPagePrinter(ByVal hPrinter As IntPtr) As Boolean
        End Function

        <DllImport("winspool.Drv", EntryPoint:="EndPagePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
        Public Shared Function EndPagePrinter(ByVal hPrinter As IntPtr) As Boolean
        End Function

        <DllImport("winspool.Drv", EntryPoint:="WritePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
        Public Shared Function WritePrinter(ByVal hPrinter As IntPtr, ByVal pBytes As IntPtr, ByVal dwCount As Integer, <Out> ByRef dwWritten As Integer) As Boolean
        End Function

#End Region

#Region "Methods"

        ' SendBytesToPrinter()
        ' When the function is given a printer name and an unmanaged array
        ' of bytes, the function sends those bytes to the printer queue.
        ' Returns true on success, false on failure.
        Public Shared Function SendBytesToPrinter(ByVal szPrinterName As String, ByVal pBytes As IntPtr, ByVal dwCount As Integer) As Boolean
            Dim dwError = 0, dwWritten = 0
            Dim hPrinter = New IntPtr(0)
            Dim di = New DOCINFOA()
            Dim bSuccess = False ' Assume failure unless you specifically succeed.
            di.pDocName = "InnerLibs RAW Printer Document"
            di.pDataType = "RAW"

            ' Open the printer.
            If OpenPrinter(szPrinterName.Normalize(), hPrinter, IntPtr.Zero) Then
                ' Start a document.
                If StartDocPrinter(hPrinter, 1, di) Then
                    ' Start a page.
                    If StartPagePrinter(hPrinter) Then
                        ' Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, dwWritten)
                        EndPagePrinter(hPrinter)
                    End If

                    EndDocPrinter(hPrinter)
                End If

                ClosePrinter(hPrinter)
            End If
            ' If you did not succeed, GetLastError may give more information
            ' about why not.
            If bSuccess = False Then dwError = Marshal.GetLastWin32Error()
            Return bSuccess
        End Function

        Public Shared Function SendFileToPrinter(ByVal szPrinterName As String, ByVal szFileName As String) As Boolean
            ' Open the file.
            Dim fs = New FileStream(szFileName, FileMode.Open)

            ' Create a BinaryReader on the file.
            Dim br = New BinaryReader(fs)

            ' Dim an array of bytes big enough to hold the file's contents.
            Dim bytes = New Byte(fs.Length - 1) {}
            Dim bSuccess = False

            ' Your unmanaged pointer.
            Dim pUnmanagedBytes = New IntPtr(0)
            Dim nLength = Convert.ToInt32(fs.Length)

            ' Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength)
            ' Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength)
            ' Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength)
            ' Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength)
            ' Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes)
            Return bSuccess
        End Function

        Public Shared Function SendBytesToPrinter(ByVal szPrinterName As String, ByVal data As Byte()) As Boolean
            Dim pUnmanagedBytes = Marshal.AllocCoTaskMem(data.Length) ' Allocate unmanaged memory
            Marshal.Copy(data, 0, pUnmanagedBytes, data.Length) ' copy bytes into unmanaged memory
            Dim retval = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, data.Length)
            Marshal.FreeCoTaskMem(pUnmanagedBytes) ' Free the allocated unmanaged memory
            Return retval
        End Function

#End Region
    End Class



    Friend Enum Justifications
        Left
        Right
        Center
    End Enum

    Public Enum QrCodeSize
        Size0
        Size1
        Size2
    End Enum

    Friend Module PrinterByteExtensions

        Friend Function SharedImagePrinter(image As Image, ByVal highDensity As Boolean) As Byte()
            Dim list = New List(Of Byte)()
            Dim bmp = New Bitmap(image)

            ' Set character line spacing to n dotlines
            Dim send = "" & Microsoft.VisualBasic.ChrW(27) & Microsoft.VisualBasic.ChrW(51) & Microsoft.VisualBasic.ChrW(0)
            Dim data = New Byte(send.Length - 1) {}

            For i = 0 To send.Length - 1
                data(i) = Microsoft.VisualBasic.AscW(send(i))
            Next

            list.AddRange(data)
            data(0) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
            data(1) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
            data(2) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0)) ' Clear

            ' ESC * m nL nH d1…dk   Select bitmap mode
            Dim escBmp As Byte() = {&H1B, &H2A, &H0, &H0, &H0}
            escBmp(2) = Microsoft.VisualBasic.AscW("!"c)
            'nL, nH
            escBmp(3) = CByte(bmp.Width Mod 256)
            escBmp(4) = CByte(bmp.Width / 256)

            ' Cycle picture pixel print
            ' High cycle
            For i = 0 To bmp.Height / 24 + 1 - 1
                ' Set the bitmap mode
                list.AddRange(escBmp)

                ' Width
                For j = 0 To bmp.Width - 1

                    For k = 0 To 24 - 1

                        If i * 24 + k < bmp.Height Then ' if within the BMP size
                            Dim pixelColor = bmp.GetPixel(j, i * 24 + k)
                            If Not (pixelColor.R > 160 AndAlso pixelColor.G > 160 AndAlso pixelColor.B > 160) Then data(k / 8) += CByte(128 >> k Mod 8)
                            If highDensity Then Continue For
                            If pixelColor.R = 0 Then data(k / 8) += CByte(128 >> k Mod 8)
                        End If
                    Next

                    ' Write data，24dots
                    list.AddRange(data)
                    data(0) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
                    data(1) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
                    data(2) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0)) ' Clear
                Next

                Dim data2 As Byte() = {&HA}
                list.AddRange(data2)
            Next

            list.AddRange(New Byte() {27, "@"c.ToByte()})
            Return list.ToArray()
        End Function

        <Extension()>
        Public Function ToByte(ByVal c As Char) As Byte
            Return Microsoft.VisualBasic.AscW(c)
        End Function

        <Extension()>
        Public Function ToByte(ByVal c As [Enum]) As Byte
            Return Convert.ToInt16(c)
        End Function

        <Extension()>
        Public Function ToByte(ByVal c As Short) As Byte
            Return c
        End Function

        <Extension()>
        Public Function ToByte(ByVal c As Integer) As Byte
            Return c
        End Function

        <Extension()>
        Public Function AddBytes(ByVal bytes As Byte(), ByVal pAddBytes As Byte()) As Byte()
            If pAddBytes Is Nothing Then Return bytes
            Dim list = New List(Of Byte)()
            list.AddRange(bytes)
            list.AddRange(pAddBytes)
            Return list.ToArray()
        End Function

        <Extension()>
        Public Function AddTextBytes(ByVal bytes As Byte(), ByVal value As String, Encoding As Encoding) As Byte()
            If value.IsBlank Then Return bytes
            Dim list = New List(Of Byte)()
            list.AddRange(bytes)
            list.AddRange(TextBytes(value, Encoding))
            Return list.ToArray()
        End Function

        <Extension> Public Function TextBytes(Value As String, Encoding As Encoding) As Byte()
            Return If(Encoding, Encoding.Default).GetBytes(Value)
        End Function

        <Extension()>
        Public Function AddLF(ByVal bytes As Byte(), Optional Encoding As Encoding = Nothing) As Byte()
            Return bytes.AddTextBytes(Microsoft.VisualBasic.Constants.vbLf, Encoding)
        End Function

        <Extension()>
        Public Function AddCrLF(ByVal bytes As Byte(), Optional Encoding As Encoding = Nothing) As Byte()
            Return bytes.AddTextBytes(Microsoft.VisualBasic.Constants.vbCrLf, Encoding)
        End Function

        <Extension()>
        Public Function IsNullOrEmpty(ByVal value As String) As Boolean
            Return String.IsNullOrEmpty(value)
        End Function

    End Module
End Namespace