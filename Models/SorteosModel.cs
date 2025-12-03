using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;

namespace AsadorMoron.Models
{
    public class SorteosModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public int numeros { get; set; }
        public int cantidad { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activo { get; set; }
        public double pedidoMinimo { get; set; }
    }
}
