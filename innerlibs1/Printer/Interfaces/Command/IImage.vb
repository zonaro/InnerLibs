Imports System.Drawing
Imports System.Text

Namespace Printer.Command
    Public Interface IImage

        Function Print(ByVal image As Image, ByVal highDensity As Boolean) As Byte()
    End Interface
End Namespace
