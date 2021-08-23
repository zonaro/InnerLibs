Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscBemaCommands

    Friend Class InitializePrint
        Implements IInitializePrint

        Public Function Initialize() As Byte() Implements IInitializePrint.Initialize
            Return New Byte() {27, "@"c.ToByte()}.AddBytes(SetEscBema()).AddBytes(SetLineSpace3())
        End Function

        Private Shared Function SetEscBema() As Byte()
            Return New Byte() {29, 249, 32, 0}
        End Function

        Private Shared Function SetLineSpace3(ByVal Optional range As Byte = 20) As Byte()
            Return New Byte() {27, 51, range}
        End Function

    End Class

End Namespace