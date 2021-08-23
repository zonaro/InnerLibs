Imports InnerLibs.Printer.Command
Imports Interfaces.Command

Namespace EscPosCommands
    Friend Class Drawer
        Implements IDrawer

        Public Function Open() As Byte() Implements IDrawer.Open
            Return New Byte() {27, 112, 0, 60, 120}
        End Function
    End Class
End Namespace
