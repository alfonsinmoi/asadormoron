using AsadorMoron.Models;
using Mopups.Pages;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Clientes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupPageInfoPedidoCliente : PopupPage
    {
        public PopupPageInfoPedidoCliente()
        {
            InitializeComponent();
        }
        public PopupPageInfoPedidoCliente(CabeceraPedido cabeceraPedido)
        {
            InitializeComponent();
            lista.ItemsSource = cabeceraPedido.lineasPedidos;
            if (!string.IsNullOrEmpty(cabeceraPedido.codigoPedido)) 
            {
                CodigoPedidoStack.IsVisible = true;
                CodigoPedido.Text = cabeceraPedido.codigoPedido;
            }
            else
            {
                CodigoPedidoStack.IsVisible = false;
            }

            if (!string.IsNullOrEmpty(cabeceraPedido.zona))
            {
                ZonaStack.IsVisible = true;
                Zona.Text = cabeceraPedido.zona;
            }
            else
            {
                ZonaStack.IsVisible = false;
            }

            if (!string.IsNullOrEmpty(cabeceraPedido.nombreEstablecimiento))
            {
                EstablecimientoStack.IsVisible = true;
                Establecimiento.Text = cabeceraPedido.nombreEstablecimiento;
            }
            else
            {
                Establecimiento.IsVisible = false;
            }

            if (!string.IsNullOrEmpty(cabeceraPedido.tipoPago))
            {
                TipoPagoStack.IsVisible = true;
                TipoPago.Text = cabeceraPedido.tipoPago;
            }
            else
            {
                TipoPagoStack.IsVisible = false;
            }

            if (cabeceraPedido.precioTotalPedido > 0)
            {
                TotalStack.IsVisible = true;
                Total.Text = string.Format("{0:N2} €",cabeceraPedido.precioTotalPedido);
            }
            else
            {
                TotalStack.IsVisible = false;
            }

            if (!string.IsNullOrEmpty(cabeceraPedido.direccionUsuario))
            {
                DireccionStack.IsVisible = true;
                Direccion.Text = cabeceraPedido.direccionUsuario;
            }
            else
            {
                DireccionStack.IsVisible = false;
            }

            if (!string.IsNullOrEmpty(cabeceraPedido.comentario))
            {
                ComentarioStack.IsVisible = true;
                Comentario.Text = cabeceraPedido.comentario;
            }
            else
            {
                ComentarioStack.IsVisible = false;
            }
        }
    }
}