Imports System.Text
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscDarumaCommands

    Friend Class QrCode
        Implements IQrCode

        Public Property Encoding As Encoding Implements IQrCode.Encoding


        Private Shared Function Size(ByVal pSize As QrCodeSize) As Byte()
            Return {(pSize + 3).ToByte()}
        End Function

        Private Shared Function StoreQr(ByVal qrData As String, ByVal size As QrCodeSize) As IEnumerable(Of Byte)
            Dim length = qrData.Length + 3
            Dim b = CByte(length Mod 255)
            Dim b2 = CByte(length / 255)
            Return New Byte() {27, 106, 49}.AddBytes((New Byte() {27, 129})).AddBytes({b}).AddBytes({b2}).AddBytes(QrCode.Size(size)).AddBytes({"M"c.ToByte()})
        End Function

        Public Function Print(ByVal qrData As String) As Byte() Implements IQrCode.Print
            Return Print(qrData, QrCodeSize.Size0)
        End Function

        Public Function Print(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As Byte() Implements IQrCode.Print
            Dim list = New List(Of Byte)()
            list.AddRange(StoreQr(qrData, qrCodeSize))
            list.AddRange(Encoding.GetBytes(qrData))
            Return list.ToArray()
        End Function

    End Class

End Namespace