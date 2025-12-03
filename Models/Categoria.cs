using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class Categoria
    {
        [JsonProperty("id")]
        [PrimaryKey]
        public int id { get; set; }
        [JsonProperty("nombre")]
        public string nombre { get; set; }
        [JsonProperty("nombre_eng")]
        public string nombre_eng { get; set; }
        [JsonProperty("nombre_fr")]
        public string nombre_fr { get; set; }
        [JsonProperty("nombre_ger")]
        public string nombre_ger { get; set; }
        [JsonProperty("idEstablecimiento")]
        public int idEstablecimiento { get; set; }
        [JsonProperty("tipo")]
        public string tipo { get; set; }
        [JsonProperty("estado")]
        public int estado { get; set; }
        [JsonProperty("idTipo")]
        public int idTipo { get; set; }
        [JsonProperty("numeroProductos")]
        public int numeroProductos { get; set; }
        [JsonProperty("color")]
        public string color { get; set; }
        public int idGrupo { get; set; }
        public int numeroImpresora { get; set; }
        public string imagen { get; set; }
        public int orden { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool esPuntos { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool navidad { get; set; }
    }
}
