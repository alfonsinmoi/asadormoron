using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;

namespace AsadorMoron.Models
{
    public class PromocionAmigoModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public int idPueblo { get; set; }
        public double saldoUsuario { get; set; }
        public double saldoAmigo { get; set; }
        public int personasAlcanzadas { get; set; }
        public double saldoRepartido { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activo { get; set; }
        public double pedidoMinimo { get; set; }
    }
}
