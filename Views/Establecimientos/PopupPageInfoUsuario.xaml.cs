using Mopups.Pages;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Establecimientos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupPageInfoUsuario : PopupPage
    {
        public PopupPageInfoUsuario()
        {
            InitializeComponent();

        }

        public PopupPageInfoUsuario(string nombreUsuario, string email, string telefono, string direccion, string zona)
        {
            InitializeComponent();
            txtNombre.Text = nombreUsuario;
            txtDireccion.Text = direccion;
            txtTelefono.Text = telefono;
            txtEmail.Text = email;
            txtZona.Text = zona;
        }
    }
}