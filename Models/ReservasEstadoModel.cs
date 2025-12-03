using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class ReservasEstadoModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public int idReserva { get; set; }
        public string comentario { get; set; }
        public DateTime fecha { get; set; }
        public int estado { get; set; }
        public int idEstablecimiento { get; set; }
        public DateTime stampi { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activo { get; set; }
    }
}
