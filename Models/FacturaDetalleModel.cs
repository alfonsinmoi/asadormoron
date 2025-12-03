using System;
namespace AsadorMoron.Models
{
    public class FacturaDetalleModel
    {
        public int id { get; set; }
        public int idFactura { get; set; }
        public int cantidad { get; set; }
        public double precio { get; set; }
        public double total { get; set; }
        public double baseImponible { get; set; }
        public double iva { get; set; }
        public string concepto { get; set; }
    }
}
