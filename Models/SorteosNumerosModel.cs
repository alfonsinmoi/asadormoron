using System;
namespace AsadorMoron.Models
{
    public class SorteosNumerosModel
    {
        public int id { get; set; }
        public string numero { get; set; }
        public int idSorteo { get; set; }
        public int idCliente { get; set; }
        public DateTime fecha { get; set; }
    }
}
