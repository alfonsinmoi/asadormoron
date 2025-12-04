using ESC_POS_USB_NET.Enums;

namespace AsadorMoron.Print.Command
{
    internal interface IQrCode
    {
        byte[] Print(string qrData);
        byte[] Print(string qrData, QrCodeSize qrCodeSize);
    }
}
