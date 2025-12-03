using System;

namespace AsadorMoron.Models
{
    public class CuentaAdminModel
    {
        public int idCuenta { get; set; }
        public int cuentaPedida { get; set; }
        public DateTime? fechaPago { get; set; }
        public DateTime fecha { get; set; }
        public string codigoPedido { get; set; }
        public DateTime horaPedido { get; set; }
        public int idPedido { get; set; }
        public double precio { get; set; }
        public int cantidad { get; set; }
        public double importe { get; set; }
        public string producto { get; set; }
        public string imagen { get; set; }
        public int mesa { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public string fotoUsuario { get; set; }
        public string telefono { get; set; }
        public int idUsuario { get; set; }
    }
}
