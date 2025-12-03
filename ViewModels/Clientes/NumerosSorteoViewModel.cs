using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
// 
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Clientes
{
    public class NumerosSorteoViewModel : ViewModelBase
    {
        private ObservableCollection<SorteosNumerosModel> numeros;
        public ObservableCollection<SorteosNumerosModel> Numeros
        {
            get { return numeros; }
            set
            {
                numeros = value;
                OnPropertyChanged(nameof(Numeros));
            }
        }
        public NumerosSorteoViewModel()
        {
        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                Numeros = new ObservableCollection<SorteosNumerosModel>(App.ResponseWS.getNumerosSorteo());
                await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.userdialog.HideLoading();
                }));
            }
            catch (Exception ex)
            {
                // 
            }
            finally
            {
                if (App.userdialog != null)
                {
                    App.userdialog.HideLoading();
                }
                else
                {
                    App.userdialog.HideLoading();
                }
            }

        }
    }
}
