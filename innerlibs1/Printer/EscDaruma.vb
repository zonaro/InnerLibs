Imports System.Drawing
Imports System.Text
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscDarumaCommands

    Public Class EscDaruma
        Implements IPrintCommand

#Region "Properties"

        Public ReadOnly Property ColsNomal As Integer Implements IPrintCommand.ColsNomal
            Get
                Return 48
            End Get
        End Property

        Public ReadOnly Property ColsCondensed As Integer Implements IPrintCommand.ColsCondensed
            Get
                Return 57
            End Get
        End Property

        Public ReadOnly Property ColsExpanded As Integer Implements IPrintCommand.ColsExpanded
            Get
                Return 25
            End Get
        End Property

        'Encoding.GetEncoding("IBM860")
        Public Property Encoding As Encoding Implements IPrintCommand.Encoding

#End Region



#Region "Methods"

        Public Function Separator() As Byte() Implements IPrintCommand.Separator
            Return Condensed(True).AddTextBytes(New String("-"c, ColsCondensed), Encoding).AddBytes(Condensed(False)).AddLF()
        End Function

        Public Function AutoTest() As Byte() Implements IPrintCommand.AutoTest
            Return New Byte() {28, "M"c.ToByte(), 254, 0}
        End Function

        Private Shared Function StoreQr(ByVal qrData As String, ByVal size As QrCodeSize) As IEnumerable(Of Byte)
            Dim length = qrData.Length + 3
            Dim b = CByte(length Mod 255)
            Dim b2 = CByte(length / 255)
            Return New Byte() {27, 106, 49}.AddBytes((New Byte() {27, 129})).AddBytes({b}).AddBytes({b2}).AddBytes({(size + 3).ToByte()}).AddBytes({"M"c.ToByte()})
        End Function

        Public Function Printqrdata(ByVal qrData As String) As Byte() Implements IPrintCommand.PrintQrData
            Return PrintQrData(qrData, QrCodeSize.Size0)
        End Function

        Public Function PrintQrData(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As Byte() Implements IPrintCommand.PrintQrData
            Dim list = New List(Of Byte)()
            list.AddRange(StoreQr(qrData, qrCodeSize))
            list.AddRange(Encoding.GetBytes(qrData))
            Return list.ToArray()
        End Function

        Public Function FullCut() As Byte() Implements IPrintCommand.FullCut
            Return New Byte() {27, "m"c.ToByte()}
        End Function

        Public Function PartialCut() As Byte() Implements IPrintCommand.PartialCut
            Return New Byte() {27, "m"c.ToByte()}
        End Function

        Public Function Initialize() As Byte() Implements IPrintCommand.Initialize
            Return New Byte() {27, "@"c.ToByte()}
        End Function

        Public Function PrintImage(ByVal image As Drawing.Image, ByVal highDensity As Boolean) As Byte() Implements IPrintCommand.PrintImage
            Dim list = New List(Of Byte)()
            Dim bmp = New Bitmap(image)
            Dim send = "" & Microsoft.VisualBasic.ChrW(27) & Microsoft.VisualBasic.ChrW(51) & Microsoft.VisualBasic.ChrW(0)
            Dim data = New Byte(send.Length - 1) {}

            For i = 0 To send.Length - 1
                data(i) = Microsoft.VisualBasic.AscW(send(i))
            Next

            list.AddRange(data)
            data(0) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
            data(1) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
            data(2) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
            Dim escBmp As Byte() = {&H1B, &H2A, &H0, &H0, &H0}
            escBmp(2) = Microsoft.VisualBasic.AscW("!"c)
            escBmp(3) = CByte(bmp.Width Mod 256)
            escBmp(4) = CByte(bmp.Width / 256)

            For i = 0 To bmp.Height / 24 + 1 - 1
                list.AddRange(escBmp)

                For j = 0 To bmp.Width - 1

                    For k = 0 To 24 - 1

                        If i * 24 + k < bmp.Height Then
                            Dim pixelColor = bmp.GetPixel(j, i * 24 + k)
                            If Not (pixelColor.R > 160 AndAlso pixelColor.G > 160 AndAlso pixelColor.B > 160) Then data(k / 8) += CByte(128 >> k Mod 8)
                            If highDensity Then Continue For
                            If pixelColor.R = 0 Then data(k / 8) += CByte(128 >> k Mod 8)
                        End If
                    Next

                    list.AddRange(data)
                    data(0) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
                    data(1) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
                    data(2) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
                Next
            Next

            list.AddRange(New Byte() {27, "@"c.ToByte()})
            Return list.ToArray()
        End Function

        Public Function DoubleWidth2() As Byte() Implements IPrintCommand.DoubleWidth2
            Return New Byte() {27, 14, 0}
        End Function

        Public Function DoubleWidth3() As Byte() Implements IPrintCommand.DoubleWidth3
            Return New Byte() {27, 14, 0}
        End Function

        Public Function NormalWidth() As Byte() Implements IPrintCommand.NormalWidth
            Return New Byte() {20}
        End Function

        Public Function Bold(ByVal value As String) As Byte() Implements IPrintCommand.Bold
            Return Bold(True).AddTextBytes(value, Encoding).AddBytes(Bold(False)).AddLF()
        End Function

        Public Function Bold(ByVal state As Boolean) As Byte() Implements IPrintCommand.Bold
            Return If(state = True, New Byte() {27, "E"c.ToByte()}, New Byte() {27, "F"c.ToByte()})
        End Function

        Public Function Condensed(ByVal value As String) As Byte() Implements IPrintCommand.Condensed
            Return Condensed(True).AddTextBytes(value, Encoding).AddBytes(Condensed(False)).AddLF()
        End Function

        Public Function Condensed(ByVal state As Boolean) As Byte() Implements IPrintCommand.Condensed
            Return If(state = True, New Byte() {27, 15}, New Byte() {27, 18, 20})
        End Function

        Public Function Expanded(ByVal value As String) As Byte() Implements IPrintCommand.Expanded
            Return Expanded(True).AddTextBytes(value, Encoding).AddBytes(Expanded(False)).AddLF()
        End Function

        Public Function Expanded(ByVal state As Boolean) As Byte() Implements IPrintCommand.Expanded
            Return If(state = True, New Byte() {27, "w"c.ToByte(), 1}, New Byte() {27, "w"c.ToByte(), 0})
        End Function

        Public Function Italic(ByVal value As String) As Byte() Implements IPrintCommand.Italic
            Return Italic(True).AddTextBytes(value, Encoding).AddBytes(Italic(False)).AddLF()
        End Function

        Public Function Italic(ByVal state As Boolean) As Byte() Implements IPrintCommand.Italic
            Return If(state = True, New Byte() {27, "4"c.ToByte(), 1}, New Byte() {27, "4"c.ToByte(), 0})
        End Function

        Public Function Underline(ByVal value As String) As Byte() Implements IPrintCommand.Underline
            Return Underline(True).AddTextBytes(value, Encoding).AddBytes(Underline(False)).AddLF()
        End Function

        Public Function Underline(ByVal state As Boolean) As Byte() Implements IPrintCommand.Underline
            Return If(state = True, New Byte() {27, "-"c.ToByte(), 1}, New Byte() {27, "-"c.ToByte(), 0})
        End Function

        Public Function OpenDrawer() As Byte() Implements IPrintCommand.OpenDrawer
            Return New Byte() {27, "p"c.ToByte()}
        End Function

        Public Function Code128(ByVal code As String) As Byte() Implements IPrintCommand.Code128
            Return New Byte() {27, "b"c.ToByte, 5}.AddBytes((New Byte() {2})).AddBytes((New Byte() {50})).AddBytes((New Byte() {0})).AddTextBytes(code, Encoding).AddBytes(New Byte() {0}) ' Code Type
            ' Witdh
            ' Height
            ' If print code informed (1 print, 0 dont print)
        End Function

        Public Function Code39(ByVal code As String) As Byte() Implements IPrintCommand.Code39
            Return New Byte() {27, "b"c.ToByte, 6}.AddBytes((New Byte() {2})).AddBytes((New Byte() {50})).AddBytes((New Byte() {0})).AddTextBytes(code, Encoding).AddBytes(New Byte() {0}) ' Code Type
            ' Witdh
            ' Height
            ' If print code informed (1 print, 0 dont print)
        End Function

        Public Function Ean13(ByVal code As String) As Byte() Implements IPrintCommand.Ean13
            If code.Trim().Length <> 13 Then Return New Byte(-1) {}
            Return New Byte() {27, "b"c.ToByte(), 1}.AddBytes((New Byte() {2})).AddBytes((New Byte() {50})).AddBytes((New Byte() {0})).AddTextBytes(code.Substring(0, 12), Encoding).AddBytes(New Byte() {0}) ' Code Type
            ' Witdh
            ' Height
            ' If print code informed (1 print, 0 dont print)
        End Function

        Public Function Center() As Byte() Implements IPrintCommand.Center
            Return New Byte() {27, "j"c.ToByte(), 1}
        End Function

        Public Function Left() As Byte() Implements IPrintCommand.Left
            Return New Byte() {27, "j"c.ToByte(), 0}
        End Function

        Public Function Right() As Byte() Implements IPrintCommand.Right
            Return New Byte() {27, "j"c.ToByte(), 2}
        End Function

#End Region

    End Class

End Namespace