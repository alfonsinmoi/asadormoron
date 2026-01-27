using System;
using System.Collections.Generic;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Views.Clientes
{
    public partial class CartaProductosView : Microsoft.Maui.Controls.ContentPage
    {
        public CartaProductosView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateNavigationButton();
        }

        private void UpdateNavigationButton()
        {
            bool isRootPage = Navigation.NavigationStack.Count <= 1;
            BtnMenu.IsVisible = isRootPage;
            BtnBack.IsVisible = !isRootPage;
        }

        private void OnMenuTapped(object sender, EventArgs e)
        {
            if (Application.Current?.MainPage is FlyoutPage flyoutPage)
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
