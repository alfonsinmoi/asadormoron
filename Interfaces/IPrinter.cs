// TODO: Reimplementar para MAUI - ESC_POS_USB_NET no compatible
namespace AsadorMoron.Interfaces
{
    // Enums temporales para compilación
    public enum Fonts { FontA, FontB, FontC }
    public enum QrCodeSize { Small, Medium, Large }
    public enum Positions { None, Above, Below, Both }
    // TipoImpresion moved to ManagerImpresora

    internal interface IPrinter
    {
        int ColsNomal { get; }
        int ColsCondensed { get; }
        int ColsExpanded { get; }
        void PrintDocument();
        void Append(string value);
        void Append(byte[] value);
        void AppendWithoutLf(string value);
        void NewLine();
        void NewLines(int lines);
        void Clear();
        void Separator(char speratorChar = '-');
        void AutoTest();
        void ImprimirTicketPedido(int altura);
        void Font(string value, Fonts state);
        void BoldMode(string value);
        void BoldMode(int state);
        void UnderlineMode(string value);
        void UnderlineMode(int state);
        void ExpandedMode(string value);
        void ExpandedMode(int state);
        void CondensedMode(string value);
        void CondensedMode(int state);
        void NormalWidth();
        void DoubleWidth2();
        void DoubleWidth3();
        void NormalLineHeight();
        void SetLineHeight(byte height);
        void AlignLeft();
        void AlignRight();
        void AlignCenter();
        void FullPaperCut();
        void PartialPaperCut();
        void OpenDrawer();
        void Image(byte[] imageBytes);
        void QrCode(string qrData);
        void QrCode(string qrData, QrCodeSize qrCodeSize);
        void Code128(string code, Positions positions);
        void Code39(string code, Positions positions);
        void Ean13(string code, Positions positions);
        void InitializePrint();
    }
}
