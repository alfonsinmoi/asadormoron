using System;
namespace AsadorMoron.Models
{
    public class ValoracionPedido
    {
        public int idUsuario { get; set; }
        public int idPedido { get; set; }
        public double valoracionServicio { get; set; }
        public double valoracionPuntualidad { get; set; }
        public double valoracionEstablecimiento { get; set; }
        public double valoracionRepartidor { get; set; }
        public string comentario { get; set; }
        public DateTime fecha { get; set; }
    }
}
