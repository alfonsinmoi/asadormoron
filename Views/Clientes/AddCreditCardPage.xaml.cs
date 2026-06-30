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
using System.Threading.Tasks;
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
            if (Microsoft.Maui.Controls.Application.Current?.MainPage is FlyoutPage flyoutPage)
            {
                flyoutPage.IsPresented = true;
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

        // Android devuelve el resultado de EvaluateJavaScriptAsync envuelto en comillas y
        // con las comillas internas escapadas: "Proceso correcto{\"DS_IDUSER\":...}".
        // Normalizamos a texto plano para que StartsWith/parse funcione igual que en iOS.
        private static string NormalizarResultadoJs(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "";
            string r = raw.Trim();
            if (r.StartsWith("\"") && r.EndsWith("\"") && r.Length >= 2)
                r = r.Substring(1, r.Length - 2);
            // Unescape de comillas y barras
            r = r.Replace("\\\"", "\"").Replace("\\/", "/").Replace("\\\\", "\\");
            return r;
        }

        // Extrae el objeto JSON ({...}) que viene tras "Proceso correcto".
        private static string ExtraerJson(string resultado)
        {
            int ini = resultado.IndexOf('{');
            int fin = resultado.LastIndexOf('}');
            if (ini >= 0 && fin > ini) return resultado.Substring(ini, fin - ini + 1);
            return "{}";
        }

        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            string resultado = NormalizarResultadoJs(await webView.EvaluateJavaScriptAsync("document.body.innerHTML"));
            if (resultado.Contains("Proceso correcto"))
            {
                ResponseAddCardModel info = JsonConvert.DeserializeObject<ResponseAddCardModel>(ExtraerJson(resultado));
                TarjetaModel tarjeta = await App.ResponseWS.infoTarjetaPaycomet(info.DS_IDUSER, info.DS_TOKEN_USER);
                if (tarjeta != null)
                {
                    App.TarjetaSeleccionada = tarjeta;
                    Preferences.Set("TarjetaSeleccionada", tarjeta.pan ?? "");

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
                else
                {
                    await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
                }
                await ClosePage();
            }
            else if (resultado.Contains("Proceso Incorrecto") || resultado.Contains("Error, no se ha obtenido token"))
            {
                webView.Reload();
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
                await ClosePage();
            }
        }

        private async void WebView_Navigated2(object sender, WebNavigatedEventArgs e)
        {
            string resultado = NormalizarResultadoJs(await webView.EvaluateJavaScriptAsync("document.body.innerHTML"));
            if (resultado.Contains("Proceso correcto"))
            {
                ResponseAddCardModel info = JsonConvert.DeserializeObject<ResponseAddCardModel>(ExtraerJson(resultado));
                TarjetaModel tarjeta = await App.ResponseWS.infoTarjetaPaycomet(info.DS_IDUSER, info.DS_TOKEN_USER);
                if (tarjeta != null)
                {
                    App.TarjetaSeleccionada = tarjeta;
                    Preferences.Set("TarjetaSeleccionada", tarjeta.pan ?? "");
                    await App.ResponseWS.nuevaTarjeta(tarjeta);
                    WeakReferenceMessenger.Default.Send(new Messages.AddCardMessage(tarjeta));
                }
                else
                {
                    await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
                }
                await ClosePage();
            }
            else if (resultado.Contains("Proceso Incorrecto") || resultado.Contains("Error, no se ha obtenido token"))
            {
                webView.Reload();
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
                await ClosePage();
            }
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await ClosePage();
        }

        private async Task ClosePage()
        {
            try
            {
                // Intentar navegación normal primero
                if (Navigation.NavigationStack.Count > 1)
                {
                    await Navigation.PopAsync();
                }
                else if (Navigation.ModalStack.Count > 0)
                {
                    await Navigation.PopModalAsync();
                }
            }
            catch
            {
                // Fallback
                try { await Navigation.PopModalAsync(); } catch { }
            }
        }
    }
}
