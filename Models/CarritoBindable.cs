using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class CarritoBindable : BindableObject
    {


        public int idArticulo { get; set; }
        public int idEstablecimiento { get; set; }
        public int idEvento { get; set; }
        public string comentario { get; set; }
        private int Cantidad;
        public int cantidad
        {
            get
            {
                return Cantidad;
            }
            set
            {
                if (Cantidad != value)
                {
                    Cantidad = value;
                    OnPropertyChanged(nameof(cantidad));
                }
            }
        }
        public string observaciones { get; set; }
        public int opcion { get; set; }
        public string comida { get; set; }
        public string comida_ger { get; set; }
        public string comida_eng { get; set; }
        public string comida_fr { get; set; }
        public double precio { get; set; }
        public bool porEncargo { get; set; }
        public string imagen { get; set; }
        public int porPuntos { get; set; }
        public int puntos { get; set; }
        private double PrecioTotal;
        public double precioTotal
        {
            get
            {
                return PrecioTotal;
            }
            set
            {
                if (PrecioTotal != value)
                {
                    PrecioTotal = value;
                    OnPropertyChanged(nameof(precioTotal));
                }
            }
        }

        public string nombreCantidad { get; set; }

    }
}
