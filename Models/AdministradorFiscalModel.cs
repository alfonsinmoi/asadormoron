using System;
using SQLite;

namespace AsadorMoron.Models
{
    public class AdministradorFiscalModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public string razonSocial { get; set; }
        public string direccion { get; set; }
        public string cp { get; set; }
        public string poblacion { get; set; }
        public string provincia { get; set; }
        public string telefono { get; set; }
        public string cif { get; set; }
        public string iban { get; set; }
        public int idGrupo { get; set; }
    }
}
