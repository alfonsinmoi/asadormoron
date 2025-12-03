using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class PueblosModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public string nombre { get; set; }
        public string codPostal { get; set; }
        public string Provincia { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activo { get; set; }
        public int idUsuario { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public int cantidad { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool demo { get; set; }
        public int idGrupo { get; set; }
        public string textoPueblo { get; set; }
        public string colorPueblo { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool visibleListado { get; set; }
        public double pedidoMinimo { get; set; }
        public int minutosAntes { get; set; }
        public int radio { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool especial { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool entregaCasa { get; set; }
        public string direccionEntrega { get; set; }
    }
}
