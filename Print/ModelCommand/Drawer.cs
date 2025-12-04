using AsadorMoron.Print.Command;

namespace AsadorMoron.Print.ModelCommand
{
    public class Drawer : IDrawer
    {
        public byte[] Open()
        {
            return new byte[] { 27, 112, 0, 60, 120 };
        }
    }
}
