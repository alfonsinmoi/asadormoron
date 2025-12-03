using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class PredefinidosModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public string texto { get; set; }
        public string textoCorto { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool establecimiento { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool administrador { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool estado { get; set; }
    }
}
