using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Establecimientos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeViewEst : ContentPage
    {
        public HomeViewEst()
        {
            InitializeComponent();
        }

        private void OnMenuTapped(object sender, EventArgs e)
        {
            if (Application.Current?.MainPage is FlyoutPage flyoutPage)
            {
                flyoutPage.IsPresented = true;
            }
        }

        protected override void OnDisappearing()
        {
            App.timer.Stop();
            App.DAUtil.EnTimer = false;
            App.DAUtil.EstoyenHome = false;
            base.OnDisappearing();
        }
    }
}
