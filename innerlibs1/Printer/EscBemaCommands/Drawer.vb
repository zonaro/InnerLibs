Imports Extensions
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command
Imports Interfaces.Command

Namespace EscBemaCommands
    Friend Class Drawer
        Implements IDrawer

        Public Function Open() As Byte() Implements IDrawer.Open
            Return New Byte() {27, "v"c.ToByte(), 140}
        End Function
    End Class
End Namespace
