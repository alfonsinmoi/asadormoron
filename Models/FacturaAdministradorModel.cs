using System;
using SQLite;

namespace AsadorMoron.Models
{
    public class FacturaAdministradorModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public string ruta { get; set; }
        public string nombre { get; set; }
        public string numero { get; set; }
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }
        public int idPueblo { get; set; }
        public string nombreAdministrador { get; set; }
        public double total { get; set; }
    }
}
