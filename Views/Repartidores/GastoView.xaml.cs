using System;
using System.Collections.Generic;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Views.Repartidores
{
    public partial class GastoView : ContentPage
    {
        public GastoView()
        {
            InitializeComponent();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}

