Imports System.Text
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscBemaCommands

    Public Class EscBema
        Implements IPrintCommand

#Region "Properties"

        Public ReadOnly Property ColsNomal As Integer Implements IPrintCommand.ColsNomal
            Get
                Return 50
            End Get
        End Property

        Public ReadOnly Property ColsCondensed As Integer Implements IPrintCommand.ColsCondensed
            Get
                Return 67
            End Get
        End Property

        Public ReadOnly Property ColsExpanded As Integer Implements IPrintCommand.ColsExpanded
            Get
                Return 25
            End Get
        End Property

        Public Property Encoding As Encoding = Encoding.GetEncoding(850) Implements IPrintCommand.Encoding

#End Region



#Region "Methods"



        Public Function AutoTest() As Byte() Implements IPrintCommand.AutoTest
            Return New Byte() {&H1D, &HF9, &H29, &H30}
        End Function



        Private Shared Function StoreQr(ByVal qrData As String, ByVal size As QrCodeSize) As IEnumerable(Of Byte)
            Dim length = qrData.Length
            Dim b = CByte(length Mod 256)
            Dim b2 = CByte(length / 256)
            Return New Byte() {29, 107, 81}.AddBytes({size.ToByte()}).AddBytes((New Byte() {6, 0, 1})).AddBytes({b}).AddBytes({b2})
        End Function

        Public Function PrintQrData(ByVal qrData As String) As Byte() Implements IPrintCommand.PrintQrData
            Return PrintQrData(qrData, QrCodeSize.Size0)
        End Function

        Public Function PrintQrData(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As Byte() Implements IPrintCommand.PrintQrData
            Dim list = New List(Of Byte)()
            list.AddRange(StoreQr(qrData, qrCodeSize))
            list.AddRange(Encoding.GetBytes(qrData))
            Return list.ToArray()
        End Function

        Public Function FullCut() As Byte() Implements IPrintCommand.FullCut
            Return New Byte() {27, "w"c.ToByte()}
        End Function

        Public Function PartialCut() As Byte() Implements IPrintCommand.PartialCut
            Return New Byte() {27, "m"c.ToByte()}
        End Function

        Public Function Initialize() As Byte() Implements IPrintCommand.Initialize
            Return New Byte() {27, "@"c.ToByte()}.AddBytes(SetEscBema()).AddBytes(SetLineSpace3())
        End Function

        Private Shared Function SetEscBema() As Byte()
            Return New Byte() {29, 249, 32, 0}
        End Function

        Private Shared Function SetLineSpace3(ByVal Optional range As Byte = 20) As Byte()
            Return New Byte() {27, 51, range}
        End Function

        Public Function NormalFont() As Byte() Implements IPrintCommand.NormalFont
            Return New Byte() {27, "W"c.ToByte(), 0, 27, "d"c.ToByte(), 0}
        End Function

        Public Function LargeFont() As Byte() Implements IPrintCommand.LargeFont
            Return New Byte() {27, "W"c.ToByte(), 1, 27, "d"c.ToByte(), 1}
        End Function

        Public Function LargerFont() As Byte() Implements IPrintCommand.LargerFont
            Return New Byte() {29, "!"c.ToByte(), 32}
        End Function



        Public Function Italic(ByVal state As Boolean) As Byte() Implements IPrintCommand.Italic
            Return If(state = True, New Byte() {27, "4"c.ToByte()}, New Byte() {27, "5"c.ToByte()})
        End Function



        Public Function Bold(ByVal state As Boolean) As Byte() Implements IPrintCommand.Bold
            Return If(state = True, New Byte() {27, "E"c.ToByte()}, New Byte() {27, "F"c.ToByte()})
        End Function



        Public Function Underline(ByVal state As Boolean) As Byte() Implements IPrintCommand.Underline
            Return If(state = True, New Byte() {27, "-"c.ToByte(), 1}, New Byte() {27, "-"c.ToByte(), 0})
        End Function



        Public Function Expanded(ByVal state As Boolean) As Byte() Implements IPrintCommand.Expanded
            Return New Byte() {27, "W"c.ToByte(), If(state = True, "1"c.ToByte(), "0"c.ToByte())}
        End Function



        Public Function Condensed(ByVal state As Boolean) As Byte() Implements IPrintCommand.Condensed
            Return If(state = True, New Byte() {27, 15}, New Byte() {27, "P"c.ToByte()})
        End Function

        Public Function OpenDrawer() As Byte() Implements IPrintCommand.OpenDrawer
            Return New Byte() {27, "v"c.ToByte(), 140}
        End Function

        Public Function Code128(ByVal code As String) As Byte() Implements IPrintCommand.Code128
            Return New Byte() {29, 119, 2}.AddBytes((New Byte() {29, 104, 50})).AddBytes((New Byte() {29, 102, 1})).AddBytes((New Byte() {29, 72, 0})).AddBytes((New Byte() {29, 107, 73})).AddBytes({CByte(code.Length + 2)}).AddBytes({"{"c.ToByte, "C"c.ToByte}).AddTextBytes(code, Encoding) ' Width
            ' Height
            ' font hri character
            ' If print code informed
            ' printCode
        End Function

        Public Function Code39(ByVal code As String) As Byte() Implements IPrintCommand.Code39
            Return New Byte() {29, 119, 2}.AddBytes((New Byte() {29, 104, 50})).AddBytes((New Byte() {29, 102, 0})).AddBytes((New Byte() {29, 72, 0})).AddBytes((New Byte() {29, 107, 4})).AddTextBytes(code, Encoding).AddBytes(New Byte() {0}) ' Width
            ' Height
            ' font hri character
            ' If print code informed
        End Function

        Public Function Ean13(ByVal code As String) As Byte() Implements IPrintCommand.Ean13
            If code.Trim().Length <> 13 Then Return New Byte(-1) {}
            Return New Byte() {29, 119, 40}.AddBytes((New Byte() {29, 104, 50})).AddBytes((New Byte() {29, 72, 0})).AddBytes((New Byte() {29, 107, 67, 12})).AddTextBytes(code.Substring(0, 12), Encoding) ' Width
            ' Height
            ' If print code informed
        End Function

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

        Public Function Left() As Byte() Implements IPrintCommand.Left
            Return Align(Justifications.Left)
        End Function

        Public Function Right() As Byte() Implements IPrintCommand.Right
            Return Align(Justifications.Right)
        End Function

        Public Function Center() As Byte() Implements IPrintCommand.Center
            Return Align(Justifications.Center)
        End Function

        Public Function PrintImage(image As Drawing.Image, highDensity As Boolean) As Byte() Implements IPrintCommand.PrintImage
            Return SharedImagePrinter(image, highDensity)
        End Function

#End Region

    End Class

End Namespace