using System.Collections.Generic;

namespace AsadorMoron.Models
{
    public class CuentaAdminModelAgrupado
    {
        public int idCuenta { get; set; }
        public int mesa { get; set; }
        public int idPedido { get; set; }
        public string codigoPedido { get; set; }
        public double total { get; set; }
        public List<CuentaAdminModel> lineasCuenta { get; set; }
    }
}
