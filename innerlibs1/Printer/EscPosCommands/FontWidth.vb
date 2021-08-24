Imports System.Text
Imports Extensions
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command
Imports Interfaces.Command

Namespace EscPosCommands
    Friend Class FontWidth
        Implements IFontWidth

        Public Property Encoding As Encoding Implements IFontWidth.Encoding


        Public Function Normal() As Byte() Implements IFontWidth.Normal
            Return New Byte() {27, "!"c.ToByte(), 0}
        End Function

        Public Function DoubleWidth2() As Byte() Implements IFontWidth.DoubleWidth2
            Return New Byte() {29, "!"c.ToByte(), 16}
        End Function

        Public Function DoubleWidth3() As Byte() Implements IFontWidth.DoubleWidth3
            Return New Byte() {29, "!"c.ToByte(), 32}
        End Function
    End Class
End Namespace
