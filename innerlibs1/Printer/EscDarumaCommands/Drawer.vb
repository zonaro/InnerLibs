Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscDarumaCommands

    Friend Class Drawer
        Implements IDrawer

        Public Function Open() As Byte() Implements IDrawer.Open
            Return New Byte() {27, "p"c.ToByte()}
        End Function

    End Class

End Namespace