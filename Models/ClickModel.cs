using System;
namespace AsadorMoron.Models
{
    public class ClickModel
    {
        public string pantalla { get; set; }
        public string filtro { get; set; }
        public int idEstablecimiento { get; set; }
        public int idCategoria { get; set; }
        public int idUsuario { get; set; }
        public DateTime fecha { get; set; }
    }
}
