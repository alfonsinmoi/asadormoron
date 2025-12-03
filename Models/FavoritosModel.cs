using System;
using SQLite;

namespace AsadorMoron.Models
{
    public class FavoritosModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public int idEstablecimiento { get; set; }
        public string nombreEstablecimiento { get; set; }
        public int idUsuario { get; set; }
        public int activo { get; set; }
        public DateTime fechaActivacion { get; set; }
        public DateTime? fechaDesactivacion { get; set; }
    }
}
