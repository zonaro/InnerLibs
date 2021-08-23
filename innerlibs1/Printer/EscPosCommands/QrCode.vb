Imports System.Text
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscPosCommands

    Friend Class QrCode
        Implements IQrCode

        Private Shared Function Size(ByVal pSize As QrCodeSize) As Byte()
            Return New Byte() {29, 40, 107, 3, 0, 49, 67}.AddBytes({(pSize + 3).ToByte()})
        End Function

        Private Function ModelQr() As IEnumerable(Of Byte)
            Return New Byte() {29, 40, 107, 4, 0, 49, 65, 50, 0}
        End Function

        Private Function ErrorQr() As IEnumerable(Of Byte)
            Return New Byte() {29, 40, 107, 3, 0, 49, 69, 48}
        End Function

        Private Shared Function StoreQr(ByVal qrData As String) As IEnumerable(Of Byte)
            Dim length = qrData.Length + 3
            Dim b = CByte(length Mod 256)
            Dim b2 = CByte(length / 256)
            Return New Byte() {29, 40, 107}.AddBytes({b}).AddBytes({b2}).AddBytes(New Byte() {49, 80, 48})
        End Function

        Private Function PrintQr() As IEnumerable(Of Byte)
            Return New Byte() {29, 40, 107, 3, 0, 49, 81, 48}
        End Function

        Public Function Print(ByVal qrData As String) As Byte() Implements IQrCode.Print
            Return Print(qrData, QrCodeSize.Size0)
        End Function

        Public Function Print(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As Byte() Implements IQrCode.Print
            Dim list = New List(Of Byte)()
            list.AddRange(ModelQr())
            list.AddRange(Size(qrCodeSize))
            list.AddRange(ErrorQr())
            list.AddRange(StoreQr(qrData))
            list.AddRange(Encoding.UTF8.GetBytes(qrData))
            list.AddRange(PrintQr())
            Return list.ToArray()
        End Function

    End Class

End Namespace