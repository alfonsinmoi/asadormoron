using AsadorMoron.Print.Command;

namespace AsadorMoron.Print.ModelCommand
{
    public class InitializePrint : IInitializePrint
    {
        public byte[] Initialize()
        {
            return new byte[] { 27, '@'.ToByte() };
        }
    }
}
