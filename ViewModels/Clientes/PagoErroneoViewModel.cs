using System;
using System.Threading.Tasks;
using System.Windows.Input;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Clientes
{
    public class PagoErroneoViewModel : ViewModelBase
    {
        public PagoErroneoViewModel() { }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;

                Texto = AppResources.PagoKO;

                await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.userdialog.HideLoading();
                }));
            }
            catch (Exception ex)
            {
                // 
            }
        }

        public ICommand VolverCommand { get { return new Command(Volver); } }

        void Volver()
        {
            App.DAUtil.NavigationService.InitializeAsync();
        }

        private string texto;
        public string Texto
        {
            get
            {
                return texto;
            }
            set
            {
                if (texto != value)
                {
                    texto = value;
                    OnPropertyChanged(nameof(Texto));
                }
            }
        }
    }
}
