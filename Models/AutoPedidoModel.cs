using System;
namespace AsadorMoron.Models
{
    public class AutoPedidoModel
    {
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public int idZona { get; set; }
        public int idEstablecimiento { get; set; }
        public DateTime hora { get; set; }
        public double importe { get; set; }
        public string codigo { get; set; }
        public double importeZona { get; set; }
        public string tipoPago { get; set; }
        public int idPueblo { get; set; }
        public string codPostal { get; set; }
        public string poblacion { get; set; }
        public string provincia { get; set; }
        public string comentario { get; set; }
        public int idUsuario { get; set; }
    }
}
