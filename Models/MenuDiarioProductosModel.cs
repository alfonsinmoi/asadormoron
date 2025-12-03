using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;

namespace AsadorMoron.Models
{
    public class MenuDiarioProductosModel
    {
        public int id { get; set; }
        public int idMenu { get; set; }
        public string imagen { get; set; }
        public string nombre { get; set; }
        public int tipo { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activo { get; set; }
    }
}
