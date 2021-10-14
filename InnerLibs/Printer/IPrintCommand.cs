using System.Drawing;
using System.Text;

namespace InnerLibs.Printer.Command
{
    public interface IPrintCommand
    {
        Encoding Encoding { get; set; }
        int ColsNormal { get; }
        int ColsCondensed { get; }
        int ColsExpanded { get; }

        byte[] AutoTest();
        byte[] Left();
        byte[] Right();
        byte[] Center();
        byte[] Code128(string code);
        byte[] Code39(string code);
        byte[] Ean13(string code);
        byte[] OpenDrawer();
        byte[] Italic(bool state);
        byte[] Bold(bool state);
        byte[] Underline(bool state);
        byte[] Expanded(bool state);
        byte[] Condensed(bool state);
        byte[] NormalFont();
        byte[] LargeFont();
        byte[] LargerFont();
        byte[] PrintImage(Image image, bool highDensity);
        byte[] Initialize();
        byte[] FullCut();
        byte[] PartialCut();
        byte[] PrintQrData(string qrData);
        byte[] PrintQrData(string qrData, QrCodeSize qrCodeSize);
    }
}