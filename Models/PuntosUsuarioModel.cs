using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class PuntosUsuarioModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public int idUsuario { get; set; }
        public int idEstablecimiento { get; set; }
        public int puntos { get; set; }
        [Ignore]
        public string nombreEstablecimiento { get; set; }
    }
}

