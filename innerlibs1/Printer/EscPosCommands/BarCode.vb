Imports Extensions
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command
Imports Interfaces.Command

Namespace EscPosCommands
    Friend Class BarCode
        Implements IBarCode

        Public Function Code128(ByVal code As String) As Byte() Implements IBarCode.Code128
            Return New Byte() {29, 119, 2}.AddBytes((New Byte() {29, 104, 50})).AddBytes((New Byte() {29, 102, 1})).AddBytes((New Byte() {29, 72, 0})).AddBytes((New Byte() {29, 107, 73})).AddBytes({CByte(code.Length + 2)}).AddBytes((New Byte() {123, 66})).AddTextBytes(code) ' Width
            ' Height
            ' font hri character
            ' If print code informed
            ' printCode
        End Function

        Public Function Code39(ByVal code As String) As Byte() Implements IBarCode.Code39
            Return New Byte() {29, 119, 2}.AddBytes((New Byte() {29, 104, 50})).AddBytes((New Byte() {29, 102, 0})).AddBytes((New Byte() {29, 72, 0})).AddBytes((New Byte() {29, 107, 4})).AddTextBytes(code).AddBytes(New Byte() {0}) ' Width
            ' Height
            ' font hri character
            ' If print code informed
        End Function

        Public Function Ean13(ByVal code As String) As Byte() Implements IBarCode.Ean13
            If code.Trim().Length <> 13 Then Return New Byte(-1) {}
            Return New Byte() {29, 119, 2}.AddBytes((New Byte() {29, 104, 50})).AddBytes((New Byte() {29, 72, 0})).AddBytes((New Byte() {29, 107, 67, 12})).AddTextBytes(code.Substring(0, 12)) ' Width
            ' Height
            ' If print code informed
        End Function
    End Class
End Namespace
