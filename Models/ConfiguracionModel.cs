using System;
using SQLite;

namespace AsadorMoron.Models
{
    public class ConfiguracionModel
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public int idEstablecimiento { get; set; }
        public int idZona { get; set; }
        public int mesa { get; set; }
        public DateTime fecha { get; set; }
        public string tipoPedido { get; set; }
        public int fila { get; set; }
        public int asiento { get; set; }
        public int idEvento { get; set; }
    }
}
