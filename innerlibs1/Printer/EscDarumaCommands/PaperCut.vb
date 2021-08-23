Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscDarumaCommands

    Friend Class PaperCut
        Implements IPaperCut

        Public Function Full() As Byte() Implements IPaperCut.Full
            Return New Byte() {27, "m"c.ToByte()}
        End Function

        Public Function [Partial]() As Byte() Implements IPaperCut.Partial
            Return New Byte() {27, "m"c.ToByte()}
        End Function

    End Class

End Namespace