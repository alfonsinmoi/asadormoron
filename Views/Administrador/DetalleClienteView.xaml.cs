using Microsoft.Maui.Controls;

namespace AsadorMoron.Views.Administrador
{
    public partial class DetalleClienteView : ContentPage
    {
        public DetalleClienteView()
        {
            InitializeComponent();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
