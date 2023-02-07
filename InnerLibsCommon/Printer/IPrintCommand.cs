using System.Drawing;
using System.Text;

namespace InnerLibs.Printer.Command
{
    public interface IPrintCommand
    {
        #region Public Properties

        int ColsCondensed { get; }
        int ColsExpanded { get; }
        int ColsNormal { get; }
        Encoding Encoding { get; set; }

        #endregion Public Properties

        #region Public Methods

        byte[] AutoTest();

        byte[] Bold(bool state);

        byte[] Center();

        byte[] Code128(string code);

        byte[] Code39(string code);

        byte[] Condensed(bool state);

        byte[] Ean13(string code);

        byte[] Expanded(bool state);

        byte[] FullCut();

        byte[] Initialize();

        byte[] Italic(bool state);

        byte[] LargeFont();

        byte[] LargerFont();

        byte[] Left();

        byte[] NormalFont();

        byte[] OpenDrawer();

        byte[] PartialCut();

        byte[] PrintImage(Image image, bool highDensity);

        byte[] PrintQrData(string qrData);

        byte[] PrintQrData(string qrData, QrCodeSize qrCodeSize);

        byte[] Right();

        byte[] Underline(bool state);

        #endregion Public Methods
    }
}