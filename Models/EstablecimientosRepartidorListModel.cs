using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class EstablecimientosRepartidorListModel : BindableObject
    {
        public int id { get; set; }
        public int idRepartidor { get; set; }
        public int idEstablecimiento { get; set; }
        public string nombre { get; set; }
        private bool Seleccionado;
        public bool seleccionado
        {
            get
            {
                return Seleccionado;
            }
            set
            {
                if (Seleccionado != value)
                {
                    Seleccionado = value;
                    OnPropertyChanged(nameof(seleccionado));
                }
            }
        }
    }
}
