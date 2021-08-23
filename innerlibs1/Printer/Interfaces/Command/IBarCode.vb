Namespace Printer.Command
    Public Interface IBarCode
        Function Code128(ByVal code As String) As Byte()
        Function Code39(ByVal code As String) As Byte()
        Function Ean13(ByVal code As String) As Byte()
    End Interface
End Namespace
