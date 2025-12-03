using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class CabeceraPedido : BindableObject
    {
        public int idPedido { get; set; }
        public string codigoPedido { get; set; }
        public int idEstablecimiento { get; set; }
        public int idUsuario { get; set; }
        public int idDetalle { get; set; }
        public DateTime horaPedido { get; set; }
        public string comentario { get; set; }
        public DateTime? fechaEntrega { get; set; }
        public int estadoDetalle { get; set; }
        public string estadoPedido { get; set; }
        public int tieneComentario { get; set; }
        public string transaccion { get; set; }
        public int pagado { get; set; }
        public int idCuenta { get; set; }
        public string textoMulti { get; set; }
        public string colorMulti { get; set; }
        public string tipoPago { get; set; }
        private string TipoVenta { get; set; }
        public string tipoVenta
        {
            get { return TipoVenta; }
            set
            {
                if (TipoVenta != value)
                {
                    TipoVenta = value;
                    OnPropertyChanged(nameof(tipoVenta));
                }
            }
        }
        public string textoPueblo { get; set; }
        public string colorPueblo { get; set; }
        //Info usuario
        public string nombreUsuario { get; set; }
        public string direccionUsuario { get; set; }
        public string poblacion { get; set; }
        public string provinciaUsuario { get; set; }
        public string localidadUsuario { get; set; }
        public string emailUsuario { get; set; }
        public int cuentaPedida { get; set; }
        public string telefonoUsuario { get; set; }
        public int idZonaEstablecimiento { get; set; }
        public string zonaEstablecimiento { get; set; }
        public int valorado { get; set; }
        public string mesa { get; set; }
        public string horaEntrega { get; set; }
        public DateTime horaEntrega2 { get; set; }
        public ObservableCollection<LineasPedido> lineasPedidos { get; set; }
        public ObservableCollection<LineasPedido> lineasPedidosAdd { get; set; }
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
        private bool Opciones;
        public bool opciones
        {
            get { return Opciones; }
            set
            {
                if (Opciones != value)
                {
                    Opciones = value;
                    OnPropertyChanged(nameof(opciones));
                }
            }
        }
        private int Repartidor;
        public int repartidor
        {
            get { return Repartidor; }
            set
            {
                if (Repartidor != value)
                {
                    Repartidor = value;
                    OnPropertyChanged(nameof(repartidor));
                }
            }
        }
        bool _isDetailVisible;
        public bool IsDetailVisible
        {
            get { return _isDetailVisible; }
            set
            {
                Task.Run(async () =>
                {
                    await Task.Delay(value ? 0 : 250);
                    _isDetailVisible = value;
                    OnPropertyChanged();
                });
            }
        }
        public int idZona { get; set; }
        public string zona { get; set; }
        public string colorZona { get; set; }
        public string nombreEstablecimiento { get; set; }
        public double precioTotalPedido { get; set; }
        private string imgBoton;
        public string imagenBoton
        {
            get
            {
                return imgBoton;
            }
            set
            {
                if (imgBoton != value)
                {
                    imgBoton = value;
                    OnPropertyChanged(nameof(imagenBoton));
                }
            }
        }
        public int idRepartidor { get; set; }

        private string fotoRepartidor;
        public string FotoRepartidor
        {
            get { return fotoRepartidor; }
            set
            {
                if (fotoRepartidor != value)
                {
                    fotoRepartidor = value;
                    OnPropertyChanged(nameof(FotoRepartidor));
                }
            }
        }
        public int tipo { get; set; }
        private int IdEstadoPedido;
        public int idEstadoPedido
        {
            get
            {
                return IdEstadoPedido;
            }
            set
            {
                if (IdEstadoPedido != value)
                {
                    IdEstadoPedido = value;
                    OnPropertyChanged(nameof(idEstadoPedido));
                }
            }
        }
    }
}
