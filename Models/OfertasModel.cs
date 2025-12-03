using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class OfertasModel
    {
        [PrimaryKey]
        public int id { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool pueblo { get; set; }
        public string idPueblo { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool establecimiento { get; set; }
        public string idEstablecimiento { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool producto { get; set; }
        public string idProducto { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool categoria { get; set; }
        public string idCategoria { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool gastos { get; set; }
        public int tipoOferta { get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool lunes { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool martes { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool miercoles { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool jueves { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool viernes { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool sabado { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool domingo { get; set; }
        public int creador { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool dosPorUno { get; set; }
        public string imagen { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool estado { get; set; }
        public string nombre { get; set; }
        public int idGrupo { get; set; }
        public double valor { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool limitado  { get; set; }
        public int cantidad { get; set; }
    }
}

