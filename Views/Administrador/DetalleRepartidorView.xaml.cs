using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Administrador
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetalleRepartidorView : ContentPage
    {
        public DetalleRepartidorView()
        {
            InitializeComponent();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
