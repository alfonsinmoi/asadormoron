using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using AsadorMoron.Controls;
using AsadorMoron.Models;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Administrador;
using Microsoft.Maui.Devices;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Administrador
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeViewAdmin : Microsoft.Maui.Controls.ContentPage
    {
        public HomeViewAdmin()
        {
            InitializeComponent();
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
