using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class MensajesCamareroModel : BindableObject
    {
        public string nombreUsuario { get; set; }
        public string codigoPedido { get; set; }
        public int idUsuario { get; set; }
        public DateTime horaPedido { get; set; }
        public int idEstadoPedido { get; set; }
        public string estadoPedido { get; set; }
        public string mensaje { get; set; }
        public int mesa { get; set; }
        public int idZona { get; set; }
        public int idEstablecimiento { get; set; }
        public string zona { get; set; }
        private string colorPedido;
        public int idCamarero { get; set; }
        public string ColorPedido
        {
            get { return colorPedido; }
            set
            {
                if (colorPedido != value)
                {
                    colorPedido = value;
                    OnPropertyChanged(nameof(ColorPedido));
                }
            }
        }
    }
}
