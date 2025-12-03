using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;

namespace AsadorMoron.Models
{
    public class AmigosModel
    {
        public int id { get; set; }
        public int idCliente { get; set; }
        public int idAmigo { get; set; }
        public double saldoCliente { get; set; }
        public double saldoAmigo { get; set; }
        public int idPueblo { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool canjeado { get; set; }
        public int idPromo { get; set; }
    }
}
