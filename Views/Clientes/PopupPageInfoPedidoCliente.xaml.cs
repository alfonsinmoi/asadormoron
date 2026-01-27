using AsadorMoron.Models;
using Mopups.Pages;
using Mopups.Services;
using Microsoft.Maui.Controls.Xaml;
using System;

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

            // Configurar lista de productos con BindableLayout
            BindableLayout.SetItemsSource(ProductosList, cabeceraPedido.lineasPedidos);

            // Código de pedido en el header
            if (!string.IsNullOrEmpty(cabeceraPedido.codigoPedido))
            {
                LbCodigoPedido.Text = $"Pedido #{cabeceraPedido.codigoPedido}";
            }
            else
            {
                LbCodigoPedido.IsVisible = false;
            }

            // Establecimiento
            if (!string.IsNullOrEmpty(cabeceraPedido.nombreEstablecimiento))
            {
                EstablecimientoStack.IsVisible = true;
                Establecimiento.Text = cabeceraPedido.nombreEstablecimiento;
            }
            else
            {
                EstablecimientoStack.IsVisible = false;
            }

            // Dirección
            if (!string.IsNullOrEmpty(cabeceraPedido.direccionUsuario))
            {
                DireccionStack.IsVisible = true;
                Direccion.Text = cabeceraPedido.direccionUsuario;
            }
            else
            {
                DireccionStack.IsVisible = false;
            }

            // Zona
            if (!string.IsNullOrEmpty(cabeceraPedido.zona))
            {
                ZonaStack.IsVisible = true;
                Zona.Text = cabeceraPedido.zona;
            }
            else
            {
                ZonaStack.IsVisible = false;
            }

            // Tipo de Pago
            if (!string.IsNullOrEmpty(cabeceraPedido.tipoPago))
            {
                TipoPagoStack.IsVisible = true;
                TipoPago.Text = cabeceraPedido.tipoPago;
            }
            else
            {
                TipoPagoStack.IsVisible = false;
            }

            // Total
            if (cabeceraPedido.precioTotalPedido > 0)
            {
                TotalStack.IsVisible = true;
                Total.Text = string.Format("{0:N2} €", cabeceraPedido.precioTotalPedido);
            }
            else
            {
                TotalStack.IsVisible = false;
            }

            // Comentario/Observaciones
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

        private async void OnCloseTapped(object sender, EventArgs e)
        {
            await MopupService.Instance.PopAsync();
        }
    }
}
