using System;
using SQLite;

namespace AsadorMoron.Models
{
    public class ReservaModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public int idEstablecimiento { get; set; }
        public string nombre { get; set; }
        public string logo { get; set; }
        public int idUsuario { get; set; }
        public DateTime fecha { get; set; }
        public int comensales { get; set; }
        public string observaciones { get; set; }
        public int estado { get; set; }
        public string textoEstado { get; set; }
        public string respuestaRestaurante { get; set; }
    }
}
