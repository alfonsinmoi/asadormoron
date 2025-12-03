using Newtonsoft.Json;

namespace AsadorMoron.Models
{
    public class TipoEstablecimientoModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("nombre")]
        public string Nombre { get; set; }
    }
}
