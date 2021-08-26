Imports System.Drawing
Imports System.Text

Namespace Printer.Command

    Public Interface IPrintCommand

        Property Encoding As Encoding

        ReadOnly Property ColsNomal As Integer
        ReadOnly Property ColsCondensed As Integer
        ReadOnly Property ColsExpanded As Integer

        Function AutoTest() As Byte()

        Function Left() As Byte()

        Function Right() As Byte()

        Function Center() As Byte()

        Function Code128(ByVal code As String) As Byte()

        Function Code39(ByVal code As String) As Byte()

        Function Ean13(ByVal code As String) As Byte()

        Function OpenDrawer() As Byte()

        Function Italic(ByVal state As Boolean) As Byte()

        Function Bold(ByVal state As Boolean) As Byte()

        Function Underline(ByVal state As Boolean) As Byte()

        Function Expanded(ByVal state As Boolean) As Byte()

        Function Condensed(ByVal state As Boolean) As Byte()

        Function NormalWidth() As Byte()

        Function DoubleWidth2() As Byte()

        Function DoubleWidth3() As Byte()

        Function PrintImage(ByVal image As Image, ByVal highDensity As Boolean) As Byte()

        Function Initialize() As Byte()

        Function FullCut() As Byte()

        Function PartialCut() As Byte()

        Function PrintQrData(ByVal qrData As String) As Byte()

        Function PrintQrData(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As Byte()

    End Interface

End Namespace