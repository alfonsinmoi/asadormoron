using System;
using SQLite;

namespace AsadorMoron.Models
{
    public class Pedido
    {
        [PrimaryKey]
        public int id { get; set; }
        public int idPedido { get; set; }
        public string codigoPedido { get; set; }
        public int idEstablecimiento { get; set; }
        public int idUsuario { get; set; }
        public string nombreUsuario { get; set; }
        public string nombreUsuarioCompleto { get; set; }
        public int idDetalle { get; set; }
        public DateTime horaPedido { get; set; }
        public int idProducto { get; set; }
        public string colorZona { get; set; }
        public int repartidor { get; set; }
        public string tipoVenta { get; set; }
        public string nombreProducto { get; set; }
        public string comentarioProducto { get; set; }
        public int cantidad { get; set; }
        public string zona { get; set; }
        public int numeroImpresora { get; set; }
        public int idEvento { get; set; }
        public int idCuenta { get; set; }
        public string transaccion { get; set; }
        public double precio { get; set; }
        public string horaEntrega { get; set; }
        public double importe { get; set; }
        public string textoMulti { get; set; }
        public string desripcionProducto { get; set; }
        public string tipoPago { get; set; }
        public string comentario { get; set; }
        public string imagenProducto { get; set; }
        public int idZonaEstablecimiento { get; set; }
        public string zonaEstablecimiento { get; set; }
        public int valorado { get; set; }
        public string poblacion { get; set; }
        public string mesa { get; set; }
        public DateTime? fechaEntrega { get; set; }
        public int estadoDetalle { get; set; }
        public int idZona { get; set; }
        public string estadoPedido { get; set; }
        public int idEstadoPedido { get; set; }
        public int idTipoPedido { get; set; }
        public string tipoPedido { get; set; }
        public string formaPago { get; set; }
        public int pagado { get; set; }
        public int asiento { get; set; }
        public int fila { get; set; }
        public string colorPedido { get; set; }
        public string imagenLottie { get; set; }
        public int tipoProducto { get; set; }
        public string nombreEstablecimiento { get; set; }
        public double precioTotalPedido { get; set; }
        //Info usuario
        public string direccionUsuario { get; set; }
        public string emailUsuario { get; set; }
        public string telefonoUsuario { get; set; }
        public int idRepartidor { get; set; }
        public string fotoRepartidor { get; set; }
        public int tipo { get; set; }

    }
}
