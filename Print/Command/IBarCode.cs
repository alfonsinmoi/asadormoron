using ESC_POS_USB_NET.Enums;

namespace AsadorMoron.Print.Command
{
    interface IBarCode
    {
        byte[] Code128(string code, Positions printString);
        byte[] Code39(string code, Positions printString);
        byte[] Ean13(string code, Positions printString);
    }
}
