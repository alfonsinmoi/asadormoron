using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using AsadorMoron.ViewModels.Clientes;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using System.Diagnostics;
using AsadorMoron.Models.PayComet;
using Newtonsoft.Json;
using AsadorMoron.Models;
using AsadorMoron.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.Devices;
using System;
using System.Linq;
using AsadorMoron.Recursos;
using CommunityToolkit.Mvvm.Messaging;

namespace AsadorMoron.Views.Clientes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCreditCardPage : ContentPage
    {
        public ObservableCollection<TarjetaBindableModel> _listTarjetas = new ObservableCollection<TarjetaBindableModel>();

        public AddCreditCardPage()
        {
            InitializeComponent();
            BindingContext = new AddCreditCardPageViewModel();
            webView.Source = App.DAUtil.miURL + "paycomet.php";
            webView.Navigated += WebView_Navigated2;
            if (DeviceInfo.Platform.ToString() == "Android")
            {
                webView.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().EnableZoomControls(true);
                webView.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().DisplayZoomControls(true);
            }
        }

        public AddCreditCardPage(ref ObservableCollection<TarjetaBindableModel> tarjetasBindables)
        {
            InitializeComponent();

            _listTarjetas = tarjetasBindables;
            if (_listTarjetas == null)
                _listTarjetas = new ObservableCollection<TarjetaBindableModel>();
            BindingContext = new AddCreditCardPageViewModel();
            webView.Source = App.DAUtil.miURL + "paycomet.php";
            webView.Navigated += WebView_Navigated;
            if (DeviceInfo.Platform.ToString() == "Android")
            {
                webView.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().EnableZoomControls(true);
                webView.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().DisplayZoomControls(true);
            }
        }

        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            string resultado = await webView.EvaluateJavaScriptAsync("document.body.innerHTML");
            if (resultado.StartsWith("Proceso correcto"))
            {
                ResponseAddCardModel info = JsonConvert.DeserializeObject<ResponseAddCardModel>(resultado.Replace("Proceso correcto", "").Replace("\\", ""));
                TarjetaModel tarjeta = await App.ResponseWS.infoTarjetaPaycomet(info.DS_IDUSER, info.DS_TOKEN_USER);
                App.TarjetaSeleccionada = tarjeta;
                Preferences.Set("TarjetaSeleccionada", App.TarjetaSeleccionada.pan);
                if (tarjeta != null)
                {
                    bool tarjetaRepetida = false;
                    foreach (var item in _listTarjetas)
                    {
                        if (item.tarjeta.pan == tarjeta.pan)
                        {
                            tarjetaRepetida = true;
                            await App.customDialog.ShowDialogAsync(AppResources.TarjetaEnUso, AppResources.App, AppResources.Cerrar);
                            break;
                        }
                    }

                    if (!tarjetaRepetida)
                    {
                        await App.ResponseWS.nuevaTarjeta(tarjeta);
                        TarjetaBindableModel tar = new TarjetaBindableModel();
                        tar.tarjeta = tarjeta;
                        tar.fondo = "#000000";
                        tar.Seleccionada = true;
                        foreach (var c in _listTarjetas)
                        {
                            c.Seleccionada = false;
                        }
                        _listTarjetas?.Add(tar);
                        WeakReferenceMessenger.Default.Send(new Messages.AddCardMessage(tarjeta));
                    }
                }
                await Navigation.PopAsync(true);
            }
            else if (resultado.StartsWith("Proceso Incorrecto") || resultado.StartsWith("Error, no se ha obtenido token"))
            {
                webView.Reload();
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
                await Navigation.PopAsync(true);
            }
        }

        private async void WebView_Navigated2(object sender, WebNavigatedEventArgs e)
        {
            string resultado = await webView.EvaluateJavaScriptAsync("document.body.innerHTML");
            if (resultado.StartsWith("Proceso correcto"))
            {
                ResponseAddCardModel info = JsonConvert.DeserializeObject<ResponseAddCardModel>(resultado.Replace("Proceso correcto", "").Replace("\\", ""));
                TarjetaModel tarjeta = await App.ResponseWS.infoTarjetaPaycomet(info.DS_IDUSER, info.DS_TOKEN_USER);
                App.TarjetaSeleccionada = tarjeta;
                Preferences.Set("TarjetaSeleccionada", App.TarjetaSeleccionada.pan);
                if (tarjeta != null)
                {
                    await App.ResponseWS.nuevaTarjeta(tarjeta);
                    WeakReferenceMessenger.Default.Send(new Messages.AddCardMessage(tarjeta));
                }
                await Navigation.PopAsync(true);
            }
            else if (resultado.StartsWith("Proceso Incorrecto") || resultado.StartsWith("Error, no se ha obtenido token"))
            {
                webView.Reload();
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
                await Navigation.PopAsync(true);
            }
        }
    }
}
