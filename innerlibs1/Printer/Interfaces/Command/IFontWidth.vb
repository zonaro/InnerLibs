Namespace Printer.Command
    Public Interface IFontWidth
        Property Encoding As Encoding

        Function Normal() As Byte()
        Function DoubleWidth2() As Byte()
        Function DoubleWidth3() As Byte()
    End Interface
End Namespace
