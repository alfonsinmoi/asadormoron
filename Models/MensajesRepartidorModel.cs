using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class MensajesRepartidorModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public int idRepartidor { get; set; }
        public string mensaje { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool ok { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool contestado { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool anulado { get; set; }
        public DateTime fechaEnvio { get; set; }
        public DateTime? fechaContestacion { get; set; }
        public int idSender { get; set; }
        public string sender { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool admin { get; set; }
    }
}
