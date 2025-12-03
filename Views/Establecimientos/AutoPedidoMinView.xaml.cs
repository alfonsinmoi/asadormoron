using System;
using System.Collections.Generic;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Views.Establecimientos
{
    public partial class AutoPedidoMinView : Microsoft.Maui.Controls.ContentPage
    {
        public AutoPedidoMinView()
        {
            InitializeComponent();
            
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            txtMiPrecio.Focus();
        }
    }
}
