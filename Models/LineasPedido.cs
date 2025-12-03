using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class LineasPedido : BindableObject
    {
        public int idProducto { get; set; }
        public string nombreProducto { get; set; }
        public int cantidad { get; set; }
        public double precio { get; set; }
        public double importe { get; set; }
        public string desripcionProducto { get; set; }
        public string imagenProducto { get; set; }
        public int estadoProducto { get; set; }
        public int tipoComida { get; set; }
        public int pagadoConPuntos { get; set; }
        public bool IsLatest { get; set; }
        public int numeroImpresora { get; set; }
        public string tipoVenta { get; set; }
        public string comentario { get; set; }
        private string colorPedido;
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
