Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscPosCommands

    Friend Class Alignment
        Implements IAlignment

        Public Function Left() As Byte() Implements IAlignment.Left
            Return New Byte() {27, "a"c.ToByte(), 0}
        End Function

        Public Function Right() As Byte() Implements IAlignment.Right
            Return New Byte() {27, "a"c.ToByte(), 2}
        End Function

        Public Function Center() As Byte() Implements IAlignment.Center
            Return New Byte() {27, "a"c.ToByte(), 1}
        End Function

    End Class

End Namespace