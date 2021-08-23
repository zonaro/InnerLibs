Imports System.Text

Namespace Printer.Command
    Public Interface IPrintCommand


        ReadOnly Property Encoding As Encoding
        ReadOnly Property ColsNomal As Integer
        ReadOnly Property ColsCondensed As Integer
        ReadOnly Property ColsExpanded As Integer
        Property FontMode As IFontMode
        Property FontWidth As IFontWidth
        Property Alignment As IAlignment
        Property PaperCut As IPaperCut
        Property Drawer As IDrawer
        Property QrCode As IQrCode
        Property Image As IImage
        Property BarCode As IBarCode
        Property InitializePrint As IInitializePrint
        Function Separator() As Byte()
        Function AutoTest() As Byte()
    End Interface
End Namespace
