using System.Collections.ObjectModel;
using System.ComponentModel;
using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class ArticuloModel : INotifyPropertyChanged
    {

        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [JsonProperty("idArticulo")]
        public int idArticulo { get; set; }
        [JsonProperty("nombre")]
        public string nombre { get; set; }
        [JsonProperty("nombre_eng")]
        public string nombre_eng { get; set; }
        [JsonProperty("nombre_ger")]
        public string nombre_ger { get; set; }
        [JsonProperty("nombre_fr")]
        public string nombre_fr { get; set; }
        [JsonProperty("idCategoria")]
        public int idCategoria { get; set; }
        [JsonProperty("imagen")]
        public string imagen { get; set; }
        [JsonProperty("estado")]
        public int estado { get; set; }
        [JsonProperty("precio")]
        public double precio { get; set; }
        public double precioLocal { get; set; }
        public int vistaEnvios { get; set; }
        public int vistaLocal { get; set; }
        public string comentario { get; set; }
        [JsonProperty("descripcion")]
        public string descripcion { get; set; }
        [JsonProperty("descripcion_eng")]
        public string descripcion_eng { get; set; }
        [JsonProperty("descripcion_ger")]
        public string descripcion_ger { get; set; }
        [JsonProperty("descripcion_fr")]
        public string descripcion_fr { get; set; }
        [JsonProperty("opciones")]
        public string opciones { get; set; }
        [JsonProperty("idTipoCategoria")]
        public int idTipoCategoria { get; set; }
        [JsonProperty("ingredientes")]
        public string ingredientes { get; set; }
        public int fuerzaIngredientes { get; set; }
        [JsonProperty("alergenos")]
        public string alergenos { get; set; }
        public int eliminado { get; set; }
        [Ignore]
        public ObservableCollection<OpcionesModel> listadoOpciones { get; set; }
        [Ignore]
        public ObservableCollection<IngredienteProductoModel> listadoIngredientes { get; set; }
        [Ignore]
        public ObservableCollection<AlergenosModel> listadoAlergenos { get; set; }

        [JsonProperty("categoria")]
        public string categoria { get; set; }
        [JsonProperty("categoria_eng")]
        public string categoria_eng { get; set; }
        [JsonProperty("categoria_ger")]
        public string categoria_ger { get; set; }
        [JsonProperty("categoria_fr")]
        public string categoria_fr { get; set; }
        [JsonProperty("idEstablecimiento")]
        public int idEstablecimiento { get; set; }
        public int estadoCategoria { get; set; }
        public string nombreEstablecimiento { get; set; }
        public int numeroIngredientes { get; set; }
        public int puntos { get; set; }
        [Ignore]
        public bool visiblePuntos
        {
            get { return VisiblePuntos; }
            set
            {
                VisiblePuntos = value;
                OnPropertyChanged(nameof(visiblePuntos));
            }
        }
        private int cantidad;
        private bool VisiblePuntos { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool porEncargo { get; set; }
        [Ignore]
        public int Cantidad
        {
            get { return cantidad; }
            set
            {
                cantidad = value;
                OnPropertyChanged(nameof(Cantidad));
            }
        }
        private bool Seleccionado;
        [Ignore]
        public bool seleccionado
        {
            get
            {
                return Seleccionado;
            }
            set
            {
                if (value != Seleccionado)
                {
                    Seleccionado = value;
                    OnPropertyChanged(nameof(seleccionado));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
