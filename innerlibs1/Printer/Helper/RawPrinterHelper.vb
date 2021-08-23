Imports System
Imports System.IO
Imports System.Runtime.InteropServices

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
            di.pDocName = "VIP RAW PrinterDocument"
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
End Namespace
