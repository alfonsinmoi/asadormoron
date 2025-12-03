using System;
namespace AsadorMoron.Models
{
    public class BeneficiosModel
    {
        public double pedidos { get; set; }
        public double gastos { get; set; }
        public string formato { get; set; }
        public double total { get; set; }
        public DateTime fecha { get; set; }
        public DateTime fechaHasta { get; set; }
    }
}
