Imports System.Drawing

Namespace Printer.Command
    Public Interface IImage
        Property Encoding As Encoding

        Function Print(ByVal image As Image, ByVal highDensity As Boolean) As Byte()
    End Interface
End Namespace
