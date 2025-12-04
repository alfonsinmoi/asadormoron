using AsadorMoron.Print.Command;

namespace AsadorMoron.Print.ModelCommand
{
    class LineHeight : ILineHeight
    {
        public byte[] Normal()
        {
            return new byte[] { 27, '3'.ToByte(), 30 };
        }

        // Line Height may vary from 24 dots (3mm) to 8128 dots (1016mm)
        public byte[] SetLineHeight(byte height)
        {
            return new byte[] { 27, '3'.ToByte(), height };
        }
    }
}
