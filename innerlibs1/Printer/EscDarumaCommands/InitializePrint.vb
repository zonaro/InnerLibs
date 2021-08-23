Imports Extensions
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command
Imports Interfaces.Command

Namespace EscDarumaCommands
    Friend Class InitializePrint
        Implements IInitializePrint

        Public Function Initialize() As Byte() Implements IInitializePrint.Initialize
            Return New Byte() {27, "@"c.ToByte()}
        End Function
    End Class
End Namespace
