using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class PublicidadModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public int idGrupo { get; set; }
        public int idPueblo { get; set; }
        public int linkIdEstablecimiento { get; set; }
        public string linkWeb { get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public int numeroVisualizaciones { get; set; }
        public int visualizaciones { get; set; }
        public int apariciones { get; set; }
        public int links { get; set; }
        public int preferencia { get; set; }
        public string imagen { get; set; }
        public string nombre { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool estado { get; set; }
    }
}
