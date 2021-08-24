Imports System.Text
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscBemaCommands

    Friend Class FontWidth
        Implements IFontWidth

        Public Property Encoding As Encoding Implements IFontWidth.Encoding


        Public Function Normal() As Byte() Implements IFontWidth.Normal
            Return New Byte() {27, "W"c.ToByte(), 0, 27, "d"c.ToByte(), 0}
        End Function

        Public Function DoubleWidth2() As Byte() Implements IFontWidth.DoubleWidth2
            Return New Byte() {27, "W"c.ToByte(), 1, 27, "d"c.ToByte(), 1}
        End Function

        Public Function DoubleWidth3() As Byte() Implements IFontWidth.DoubleWidth3
            Return New Byte() {29, "!"c.ToByte(), 32}
        End Function

    End Class

End Namespace