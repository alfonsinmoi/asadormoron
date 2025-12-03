using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;

namespace AsadorMoron.Models
{
    public class CuponesSQLModel
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string sentencia { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool usuario { get; set; }
        public int pueblo { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool estado { get; set; }
        public int valorSentencia { get; set; }
        public int tipoDescuento { get; set; }
        public double descuento { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool gastosEnvio { get; set; }
    }
}
