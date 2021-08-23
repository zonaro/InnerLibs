Namespace Printer.Command
    Public Interface IAlignment
        Property Encoding As Encoding

        Function Left() As Byte()
        Function Right() As Byte()
        Function Center() As Byte()
    End Interface
End Namespace
