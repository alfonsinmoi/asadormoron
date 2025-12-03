using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class IngredientesListModel : BindableObject
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int estado { get; set; }
        private string Precio;
        public int puntos { get; set; }
        public int idIngPro { get; set; }
        public string precio
        {
            get
            {
                return Precio;
            }
            set
            {
                if (Precio != value)
                {
                    Precio = value;
                    OnPropertyChanged(nameof(precio));
                }
            }
        }
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
