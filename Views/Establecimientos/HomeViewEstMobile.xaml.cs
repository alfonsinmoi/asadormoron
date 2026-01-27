using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Establecimientos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeViewEstMobile : ContentPage
    {
        public HomeViewEstMobile()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            App.DAUtil.EnTimer = false;
            App.DAUtil.EstoyenHome = false;
            base.OnDisappearing();
        }

        private void OnMenuTapped(object sender, EventArgs e)
        {
            if (Application.Current?.MainPage is FlyoutPage flyoutPage)
            {
                flyoutPage.IsPresented = true;
            }
        }
    }
}
