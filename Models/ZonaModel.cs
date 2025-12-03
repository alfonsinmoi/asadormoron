using Newtonsoft.Json;
using SQLite;

namespace AsadorMoron.Models
{
    public class ZonaModel
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [JsonProperty("nombre")]
        public string nombre { get; set; }
        [JsonProperty("idZona")]
        public int idZona { get; set; }
        [JsonProperty("activo")]
        public int activo { get; set; }
        public double gastos { get; set; }
        public double pedidoMinimo { get; set; }
        public string color { get; set; }
        public int cambiaDireccion { get; set; }
        public string direccionEnvio { get; set; }
        public int idPueblo { get; set; }
        public int modificable { get; set; }
    }
}
