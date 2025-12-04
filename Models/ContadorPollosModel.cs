using System;

namespace AsadorMoron.Models
{
    /// <summary>
    /// Modelo para el contador diario de pollos asados vendidos
    /// </summary>
    public class ContadorPollosModel
    {
        public int id { get; set; }
        public int idEstablecimiento { get; set; }
        public int cantidad { get; set; }
        public string fecha { get; set; }
        public string operacion { get; set; }
    }
}
