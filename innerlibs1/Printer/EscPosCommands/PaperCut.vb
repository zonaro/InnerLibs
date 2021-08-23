Imports Extensions
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command
Imports Interfaces.Command

Namespace EscPosCommands
    Friend Class PaperCut
        Implements IPaperCut

        Public Function Full() As Byte() Implements IPaperCut.Full
            Return New Byte() {29, "V"c.ToByte(), 65, 3}
        End Function

        Public Function [Partial]() As Byte() Implements IPaperCut.Partial
            Return New Byte() {29, "V"c.ToByte(), 65, 3}
        End Function
    End Class
End Namespace
