using AsadorMoron.ViewModels.Repartidores;
using BarcodeScanning;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AsadorMoron.Views.Repartidores
{
    public partial class EscanearQRView : ContentPage
    {
        // Evita procesar múltiples detecciones del mismo frame.
        private bool _procesado = false;

        public EscanearQRView()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _procesado = false;

            // El scanner nativo requiere permiso de cámara en runtime.
            var ok = await Methods.AskForRequiredPermissionAsync();
            if (!ok)
            {
                await DisplayAlert("Cámara", "Sin permiso de cámara no se puede escanear.", "OK");
                await Navigation.PopAsync();
                return;
            }
            Reader.CameraEnabled = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            try { Reader.CameraEnabled = false; } catch { }
        }

        private void OnDetectionFinished(object sender, OnDetectionFinishedEventArg e)
        {
            if (_procesado) return;
            if (e?.BarcodeResults == null) return;

            var result = e.BarcodeResults.FirstOrDefault();
            var codigo = result?.RawValue ?? result?.DisplayValue;
            if (string.IsNullOrWhiteSpace(codigo)) return;

            _procesado = true;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    Reader.CameraEnabled = false;
                    var cb = EscanearQRViewModel.OnCodigoLeido;
                    EscanearQRViewModel.OnCodigoLeido = null; // single-use
                    await Navigation.PopAsync();
                    cb?.Invoke(codigo.Trim());
                }
                catch { /* ignore */ }
            });
        }

        private async void OnCerrarTapped(object sender, EventArgs e)
        {
            EscanearQRViewModel.OnCodigoLeido = null;
            try { Reader.CameraEnabled = false; } catch { }
            await Navigation.PopAsync();
        }
    }
}
