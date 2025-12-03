using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class ConfiguracionGlobalModel
    {
        [PrimaryKey]
        public int id { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool pedidoenmesa { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool redessociales { get; set; }
        public int longitudCodigo { get; set; }
        public string mensajeRegistro { get; set; }
        public string apiPaycomet { get; set; }
        public int terminalPaycomet { get; set; }
        public int numeroFactura { get; set; }
        public int distanciaLocal { get; set; }
    }
}
