using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class IngredienteProductoBindableModel : BindableObject
    {
        public int id { get; set; }
        public int idProducto { get; set; }
        public string nombre { get; set; }
        public string nombre_eng { get; set; }
        public string nombre_ger { get; set; }
        public string nombre_fr { get; set; }
        public double precio { get; set; }
        public int puntos { get; set; }
        public bool visiblePuntos { get; set; }
        public int idIngrediente { get; set; }
        private int cantidad;
        public int Cantidad
        {
            get
            {
                return cantidad;
            }
            set
            {
                if (cantidad != value)
                {
                    cantidad = value;
                    OnPropertyChanged(nameof(Cantidad));
                }
            }
        }
    }
}