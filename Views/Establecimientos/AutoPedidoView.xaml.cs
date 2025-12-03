using AsadorMoron.Recursos;
using AsadorMoron.ViewModels.Establecimientos;
using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Establecimientos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AutoPedidoView : Microsoft.Maui.Controls.ContentPage
    {
        private bool isBack = false;
        public AutoPedidoView()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            isBack = true;
            return base.OnBackButtonPressed();
        }

        protected override void OnDisappearing()
        {
            if (DeviceInfo.Platform.ToString().Equals("WinUI"))
            {
                if (isBack)
                {
                    App.DAUtil.NavigationService.InitializeAsync();
                }
                else
                {
                    base.OnDisappearing();
                }
            }
            else
            {
                base.OnDisappearing();
            }
        }
    }
}
