Imports System.Text
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscDarumaCommands

    Friend Class BarCode
        Implements IBarCode

        Public Property Encoding As Encoding Implements IBarCode.Encoding


        Public Function Code128(ByVal code As String) As Byte() Implements IBarCode.Code128
            Return New Byte() {27, "b"c.ToByte, 5}.AddBytes((New Byte() {2})).AddBytes((New Byte() {50})).AddBytes((New Byte() {0})).AddTextBytes(code, Encoding).AddBytes(New Byte() {0}) ' Code Type
            ' Witdh
            ' Height
            ' If print code informed (1 print, 0 dont print)
        End Function

        Public Function Code39(ByVal code As String) As Byte() Implements IBarCode.Code39
            Return New Byte() {27, "b"c.ToByte, 6}.AddBytes((New Byte() {2})).AddBytes((New Byte() {50})).AddBytes((New Byte() {0})).AddTextBytes(code, Encoding).AddBytes(New Byte() {0}) ' Code Type
            ' Witdh
            ' Height
            ' If print code informed (1 print, 0 dont print)
        End Function

        Public Function Ean13(ByVal code As String) As Byte() Implements IBarCode.Ean13
            If code.Trim().Length <> 13 Then Return New Byte(-1) {}
            Return New Byte() {27, "b"c.ToByte(), 1}.AddBytes((New Byte() {2})).AddBytes((New Byte() {50})).AddBytes((New Byte() {0})).AddTextBytes(code.Substring(0, 12), Encoding).AddBytes(New Byte() {0}) ' Code Type
            ' Witdh
            ' Height
            ' If print code informed (1 print, 0 dont print)
        End Function

    End Class

End Namespace