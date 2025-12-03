using AsadorMoron.Models;
using Mopups.Pages;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Administrador
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupPageInfoUsuarioPedido : PopupPage
    {
        public PopupPageInfoUsuarioPedido()
        {
            InitializeComponent();
        }

        public PopupPageInfoUsuarioPedido(CabeceraPedido cabeceraPedido)
        {
            InitializeComponent();
            lista.ItemsSource = cabeceraPedido.lineasPedidos;
            txtNombre.Text = cabeceraPedido.nombreUsuario;
            txtDireccion.Text = cabeceraPedido.direccionUsuario;
            txtTelefono.Text = cabeceraPedido.telefonoUsuario;
            txtEmail.Text = cabeceraPedido.emailUsuario;
            txtPago.Text = cabeceraPedido.tipoPago;
            if (string.IsNullOrEmpty(cabeceraPedido.zona))
            {
                txtZona.IsVisible = false;
                lblZona.IsVisible = false;
            }
            else
            {
                txtZona.IsVisible = true;
                lblZona.IsVisible = true;
                txtZona.Text = cabeceraPedido.zona;
            }
        }
    }
}