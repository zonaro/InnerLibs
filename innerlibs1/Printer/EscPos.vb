Imports System.Drawing
Imports System.Text
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscPosCommands

    Public Class EscPos
        Implements IPrintCommand

#Region "Properties"

        Public ReadOnly Property ColsNomal As Integer Implements IPrintCommand.ColsNomal
            Get
                Return 48
            End Get
        End Property

        Public ReadOnly Property ColsCondensed As Integer Implements IPrintCommand.ColsCondensed
            Get
                Return 64
            End Get
        End Property

        Public ReadOnly Property ColsExpanded As Integer Implements IPrintCommand.ColsExpanded
            Get
                Return 24
            End Get
        End Property

        'Encoding.GetEncoding("IBM860") 
        Public Property Encoding As Encoding Implements IPrintCommand.Encoding

#End Region



#Region "Methods"



        Public Function AutoTest() As Byte() Implements IPrintCommand.AutoTest
            Return New Byte() {29, 40, 65, 2, 0, 0, 2}
        End Function

        Public Function Left() As Byte() Implements IPrintCommand.Left
            Return New Byte() {27, "a"c.ToByte(), 0}
        End Function

        Public Function Right() As Byte() Implements IPrintCommand.Right
            Return New Byte() {27, "a"c.ToByte(), 2}
        End Function

        Public Function Center() As Byte() Implements IPrintCommand.Center
            Return New Byte() {27, "a"c.ToByte(), 1}
        End Function

        Public Function Code128(ByVal code As String) As Byte() Implements IPrintCommand.Code128
            Return New Byte() {29, 119, 2}.AddBytes((New Byte() {29, 104, 50})).AddBytes((New Byte() {29, 102, 1})).AddBytes((New Byte() {29, 72, 0})).AddBytes((New Byte() {29, 107, 73})).AddBytes({CByte(code.Length + 2)}).AddBytes((New Byte() {123, 66})).AddTextBytes(code, Encoding) ' Width
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
            Return New Byte() {29, 119, 2}.AddBytes((New Byte() {29, 104, 50})).AddBytes((New Byte() {29, 72, 0})).AddBytes((New Byte() {29, 107, 67, 12})).AddTextBytes(code.Substring(0, 12), Encoding) ' Width
            ' Height
            ' If print code informed
        End Function

        Public Function OpenDrawer() As Byte() Implements IPrintCommand.OpenDrawer
            Return New Byte() {27, 112, 0, 60, 120}
        End Function



        Public Function Italic(ByVal state As Boolean) As Byte() Implements IPrintCommand.Italic
            Return If(state = True, New Byte() {27, "4"c.ToByte()}, New Byte() {27, "5"c.ToByte()})
        End Function



        Public Function Bold(ByVal state As Boolean) As Byte() Implements IPrintCommand.Bold
            Return If(state = True, New Byte() {27, "E"c.ToByte(), 1}, New Byte() {27, "E"c.ToByte(), 0})
        End Function



        Public Function Underline(ByVal state As Boolean) As Byte() Implements IPrintCommand.Underline
            Return If(state = True, New Byte() {27, "-"c.ToByte(), 1}, New Byte() {27, "-"c.ToByte(), 0})
        End Function



        Public Function Expanded(ByVal state As Boolean) As Byte() Implements IPrintCommand.Expanded
            Return If(state = True, New Byte() {29, "!"c.ToByte(), 16}, New Byte() {29, "!"c.ToByte(), 0})
        End Function



        Public Function Condensed(ByVal state As Boolean) As Byte() Implements IPrintCommand.Condensed
            Return If(state = True, New Byte() {27, "!"c.ToByte(), 1}, New Byte() {27, "!"c.ToByte(), 0})
        End Function

        Public Function NormalFont() As Byte() Implements IPrintCommand.NormalFont
            Return New Byte() {27, "!"c.ToByte(), 0}
        End Function

        Public Function LargeFont() As Byte() Implements IPrintCommand.LargeFont
            Return New Byte() {29, "!"c.ToByte(), 16}
        End Function

        Public Function LargerFont() As Byte() Implements IPrintCommand.LargerFont
            Return New Byte() {29, "!"c.ToByte(), 32}
        End Function

        Public Function Initialize() As Byte() Implements IPrintCommand.Initialize
            Return New Byte() {27, "@"c.ToByte()}
        End Function

        Public Function PrintImage(ByVal image As Drawing.Image, ByVal highDensity As Boolean) As Byte() Implements IPrintCommand.PrintImage
            Return SharedImagePrinter(image, highDensity)
        End Function

        Public Function FullCut() As Byte() Implements IPrintCommand.FullCut
            Return New Byte() {29, "V"c.ToByte(), 65, 3}
        End Function

        Public Function PartialCut() As Byte() Implements IPrintCommand.PartialCut
            Return New Byte() {29, "V"c.ToByte(), 65, 3}
        End Function

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

        Public Function PrintQrData(ByVal qrData As String) As Byte() Implements IPrintCommand.PrintQrData
            Return PrintQrData(qrData, QrCodeSize.Size0)
        End Function

        Public Function PrintQrData(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As Byte() Implements IPrintCommand.PrintQrData
            Dim list = New List(Of Byte)()
            list.AddRange(ModelQr())
            list.AddRange(Size(qrCodeSize))
            list.AddRange(ErrorQr())
            list.AddRange(StoreQr(qrData))
            list.AddRange(Encoding.GetBytes(qrData))
            list.AddRange(PrintQr())
            Return list.ToArray()
        End Function



#End Region





    End Class

End Namespace