using System;
namespace AsadorMoron.Models
{
    public class PreFacturaModel
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string tipoVenta { get; set; }
        public string tipoPago { get; set; }
        public double precio { get; set; }
        public int tipo { get; set; }
        public int pagadoConPuntos { get; set; }
    }
}
