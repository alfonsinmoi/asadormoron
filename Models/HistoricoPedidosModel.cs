using System;
using SQLite;

namespace AsadorMoron.Models
{
    public class HistoricoPedidosModel
    {
        [PrimaryKey, AutoIncrement]
        public int idPedido { get; set; }
        public string codigoPedido { get; set; }
        public int idEstablecimiento { get; set; }
        public DateTime? horaPedido { get; set; }
        public int idTipoPedido { get; set; }
        public string tipoPedido { get; set; }
        public string formaPago { get; set; }
        public int pagado { get; set; }
        public string zona { get; set; }
        public int asiento { get; set; }
        public int fila { get; set; }
        public int idEstadio { get; set; }
        public string estadio { get; set; }
        public int idEvento { get; set; }
        public string evento { get; set; }
        public DateTime horaEvento { get; set; }
        public double total { get; set; }
        public string equipo1 { get; set; }
        public string equipo2 { get; set; }

    }
}
