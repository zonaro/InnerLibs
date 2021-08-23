Namespace Printer.Command
    Public Interface IPaperCut
        Property Encoding As Encoding

        Function Full() As Byte()
        Function [Partial]() As Byte()
    End Interface
End Namespace
