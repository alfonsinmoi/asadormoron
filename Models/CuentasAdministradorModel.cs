using System;
namespace AsadorMoron.Models
{
    public class CuentasAdministradorModel
    {
        public double totalVentas { get; set; }
        public double totalVentasSinGastos { get; set; }
        public int totalPedidos { get; set; }
        public double pagosAEstablecimientos { get; set; }
        public double totalVentasTarjeta { get; set; }
        public double fijoTarjeta { get; set; }
        public double variableTarjeta { get; set; }
        public double totalVentasDatafono { get; set; }
        public double comisionDatafono { get; set; }
        public double comision { get; set; }
        public int idPueblo { get; set; }
    }
}
