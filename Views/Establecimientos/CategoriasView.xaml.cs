using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Establecimientos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoriasView : ContentPage
    {
        public CategoriasView()
        {
            InitializeComponent();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
