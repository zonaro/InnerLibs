Imports InnerLibs.Printer

Namespace Printer.Command
    Friend Interface IQrCode
        Function Print(ByVal qrData As String) As Byte()
        Function Print(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As Byte()
    End Interface
End Namespace
