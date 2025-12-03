using System;
namespace AsadorMoron.Models
{
    public class GastoModel
    {
        public int idRepartidor { get; set; }
        public string nombreRepartidor { get; set; }
        public double precio { get; set; }
        public string concepto { get; set; }
        public DateTime fecha { get; set; }
    }
}

