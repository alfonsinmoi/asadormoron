using AsadorMoron.Models;

namespace AsadorMoron.Messages
{
    public class AddCardMessage
    {
        public TarjetaModel Tarjeta { get; }

        public AddCardMessage(TarjetaModel tarjeta)
        {
            Tarjeta = tarjeta;
        }
    }
}
