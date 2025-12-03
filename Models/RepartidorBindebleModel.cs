using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class RepartidorBindableModel:BindableObject
    {
        public int id { get; set; }
        public int idUsuario { get; set; }
        public string nombre { get; set; }
        public string foto { get; set; }
        public int activo { get; set; }
        public string pin { get; set; }
        public int idPueblo { get; set; }
        public int eliminado { get; set; }
        public int idGrupo { get; set; }
        public string telefono { get; set; }
        private int cantida;
        public int Cantidad
        {
            get { return cantida; }
            set {
                cantida = value;
                OnPropertyChanged();
            }
        }
    }
}
