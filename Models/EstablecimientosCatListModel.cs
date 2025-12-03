using Newtonsoft.Json;
using AsadorMoron.Utils;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class EstablecimientosCatListModel : BindableObject
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string nombre_eng { get; set; }
        public string nombre_ger { get; set; }
        public string nombre_fr { get; set; }
        public string imagen { get; set; }
        public int estado { get; set; }
        public int idPueblo { get; set; }
        public int orden { get; set; }
        public int raiz { get; set; }
        public int idGrupo { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool proximamente { get; set; }
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
