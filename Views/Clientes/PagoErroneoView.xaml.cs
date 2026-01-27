using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Views.Clientes
{
    public partial class PagoErroneoView : ContentPage
    {
        public PagoErroneoView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            bool isRootPage = Navigation.NavigationStack.Count <= 1;
            BtnMenu.IsVisible = isRootPage;
            BtnBack.IsVisible = !isRootPage;
        }

        private void OnMenuTapped(object sender, EventArgs e)
        {
            if (Microsoft.Maui.Controls.Application.Current?.MainPage is FlyoutPage flyoutPage)
            {
                flyoutPage.IsPresented = true;
            }
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
