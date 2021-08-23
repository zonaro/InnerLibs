Imports InnerLibs.Printer.Command
Imports Interfaces.Command

Namespace EscDarumaCommands
    Friend Class FontWidth
        Implements IFontWidth

        Public Function DoubleWidth2() As Byte() Implements IFontWidth.DoubleWidth2
            Return New Byte() {27, 14, 0}
        End Function

        Public Function DoubleWidth3() As Byte() Implements IFontWidth.DoubleWidth3
            Return New Byte() {27, 14, 0}
        End Function

        Public Function Normal() As Byte() Implements IFontWidth.Normal
            Return New Byte() {20}
        End Function
    End Class
End Namespace
