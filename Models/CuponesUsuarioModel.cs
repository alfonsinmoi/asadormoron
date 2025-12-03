using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class CuponesUsuarioModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public int idCupon { get; set; }
        public int idUsuario { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool utilizado { get; set; }
        public DateTime fechaUtilizacion { get; set; }
        public DateTime fechaAnulacion { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool anulado { get; set; }
    }
}
