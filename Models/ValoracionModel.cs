using System;

namespace AsadorMoron.Models
{
    public class ValoracionModel
    {
        public int idUsuario { get; set; }
        public int rechazada { get; set; }
        public double valoracion { get; set; }
        public string comentario { get; set; }
        public DateTime fecha { get; set; }
        public string codigoPedido { get; set; }
        public int tipoValoracion { get; set; }
        public int idValoracion { get; set; }
    }
}
