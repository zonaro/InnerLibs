Namespace Printer.Command
    Public Interface IInitializePrint
        Property Encoding As Encoding

        Function Initialize() As Byte()
    End Interface
End Namespace
