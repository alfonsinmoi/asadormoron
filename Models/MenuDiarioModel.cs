using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class MenuDiarioModel
    {
        public int id { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int idEstablecimiento { get; set; }
        public double precio { get; set; }
        [Ignore]
        public MenuDiarioConfiguracionModel configuracion { get; set; }
        [Ignore]
        public List<MenuDiarioProductosModel> productos { get; set; }
    }
}
