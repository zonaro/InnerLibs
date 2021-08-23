Imports System.Drawing
Imports System.IO

Namespace Printer

    Friend Interface IPrinter(Of PrinterClass As IPrinter(Of PrinterClass))
        ReadOnly Property ColsNomal As Integer
        ReadOnly Property ColsCondensed As Integer
        ReadOnly Property ColsExpanded As Integer

        Function PrintDocument(Optional copies As Integer = 1) As PrinterClass

        Function Write(ByVal value As String) As PrinterClass

        Function Write(ByVal value As Byte()) As PrinterClass

        Function WriteLine(ByVal value As String) As PrinterClass

        Function NewLine() As PrinterClass

        Function NewLines(ByVal lines As Integer) As PrinterClass

        Function Clear() As PrinterClass

#Region "Commands"

        Function Separator() As PrinterClass

        Function AutoTest() As PrinterClass

        Function TestPrinter() As PrinterClass

#Region "FontMode"

        Function ItalicMode(ByVal value As String) As PrinterClass

        Function ItalicMode(ByVal state As PrinterModeState) As PrinterClass

        Function BoldMode(ByVal value As String) As PrinterClass

        Function BoldMode(ByVal state As PrinterModeState) As PrinterClass

        Function UnderlineMode(ByVal value As String) As PrinterClass

        Function UnderlineMode(ByVal state As PrinterModeState) As PrinterClass

        Function ExpandedMode(ByVal value As String) As PrinterClass

        Function ExpandedMode(ByVal state As PrinterModeState) As PrinterClass

        Function CondensedMode(ByVal value As String) As PrinterClass

        Function CondensedMode(ByVal state As PrinterModeState) As PrinterClass

#End Region

#Region "FontWidth"

        Function NormalWidth() As PrinterClass

        Function DoubleWidth2() As PrinterClass

        Function DoubleWidth3() As PrinterClass

#End Region

#Region "Alignment"

        Function AlignLeft() As PrinterClass

        Function AlignRight() As PrinterClass

        Function AlignCenter() As PrinterClass

#End Region

#Region "PaperCut"

        Function FullPaperCut() As PrinterClass

        Function FullPaperCut(ByVal predicate As Boolean) As PrinterClass

        Function PartialPaperCut() As PrinterClass

        Function PartialPaperCut(ByVal predicate As Boolean) As PrinterClass

#End Region

#Region "Drawer"

        Function OpenDrawer() As PrinterClass

#End Region

#Region "QrCode"

        Function QrCode(ByVal qrData As String) As PrinterClass

        Function QrCode(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As PrinterClass

#End Region

#Region "Image"

        Function Image(ByVal path As String, Optional highDensity As Boolean = True) As PrinterClass

        Function Image(ByVal stream As Stream, Optional highDensity As Boolean = True) As PrinterClass

        Function Image(ByVal bytes As Byte(), Optional highDensity As Boolean = True) As PrinterClass

        Function Image(ByVal img As Image, Optional highDensity As Boolean = True) As PrinterClass

#End Region

#Region "BarCode"

        Function Code128(ByVal code As String) As PrinterClass

        Function Code39(ByVal code As String) As PrinterClass

        Function Ean13(ByVal code As String) As PrinterClass

#End Region

#Region "InitializePrint"

        Function InitializePrint() As PrinterClass

#End Region

#End Region

    End Interface

End Namespace