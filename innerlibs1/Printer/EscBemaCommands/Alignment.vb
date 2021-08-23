Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscBemaCommands

    Friend Class Alignment
        Implements IAlignment

        Private Shared Function Align(ByVal justification As Justifications) As Byte()
            Dim lAlign As Byte

            Select Case justification
                Case Justifications.Right
                    lAlign = "2"c.ToByte()
                Case Justifications.Center
                    lAlign = "1"c.ToByte()
                Case Else
                    lAlign = "0"c.ToByte()
            End Select

            Return New Byte() {27, "a"c.ToByte(), lAlign}
        End Function

        Public Function Left() As Byte() Implements IAlignment.Left
            Return Align(Justifications.Left)
        End Function

        Public Function Right() As Byte() Implements IAlignment.Right
            Return Align(Justifications.Right)
        End Function

        Public Function Center() As Byte() Implements IAlignment.Center
            Return Align(Justifications.Center)
        End Function

    End Class

End Namespace