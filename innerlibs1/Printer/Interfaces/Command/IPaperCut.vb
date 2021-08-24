Imports System.Text

Namespace Printer.Command
    Public Interface IPaperCut


        Function Full() As Byte()
        Function [Partial]() As Byte()
    End Interface
End Namespace
