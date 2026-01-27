using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Establecimientos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetalleCategoriaView : ContentPage
    {
        public DetalleCategoriaView()
        {
            InitializeComponent();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
