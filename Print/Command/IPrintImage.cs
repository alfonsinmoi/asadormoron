using System.Drawing;

namespace AsadorMoron.Print.Command
{
    public interface IPrintImage
    {
        byte[] Print(Bitmap image);
    }
}
