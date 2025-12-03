using SQLite;
using System;

namespace AsadorMoron.Models
{
    public class PedidoModel
    {


        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string nombreUsuario { get; set; }
        public int idArticulo { get; set; }
        public int idEstablecimiento { get; set; }
        public int idEvento { get; set; }
        public int cantidad { get; set; }
        public string observaciones { get; set; }
        public int opcion { get; set; }
        public string comida { get; set; }
        public string textoMulti { get; set; }
        public double precio { get; set; }

        public string imagen { get; set; }

        public double precioTotal { get; set; }

        public string nombreCantidad { get; set; }

        public int idPedido { get; set; }

        public DateTime? fechaPedido { get; set; }

        public string nombreEstablecimiento { get; set; }
        public string horaEntrega { get; set; }

    }
}
