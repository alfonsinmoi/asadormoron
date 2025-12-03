using System;
using System.Collections.Generic;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Views.Administrador
{
    public partial class HomeAdminMobileView : Microsoft.Maui.Controls.ContentPage
    {
        public HomeAdminMobileView()
        {
            InitializeComponent();
        }
        protected override void OnDisappearing()
        {
            App.DAUtil.EnTimer = false;
            App.DAUtil.EstoyenHome = false;
            base.OnDisappearing();
        }
    }
}
