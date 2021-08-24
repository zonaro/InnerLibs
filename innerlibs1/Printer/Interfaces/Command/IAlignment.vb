Imports System.Text

Namespace Printer.Command
    Public Interface IAlignment


        Function Left() As Byte()
        Function Right() As Byte()
        Function Center() As Byte()
    End Interface
End Namespace
