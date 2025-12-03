using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Administrador
{
    public class CuponesViewModel : ViewModelBase
    {
        public CuponesViewModel()
        {
        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                ListadoCupones = new ObservableCollection<CuponesModel>(App.ResponseWS.getListadoCupones());
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }
            finally
            {
                App.userdialog.HideLoading();
            }
            await base.InitializeAsync(navigationData);
        }

        #region Propiedades
        private ObservableCollection<CuponesModel> listadoCupones;

        public ObservableCollection<CuponesModel> ListadoCupones
        {
            get { return listadoCupones; }
            set
            {
                listadoCupones = value;
                OnPropertyChanged(nameof(ListadoCupones));
            }
        }

        private CuponesModel cuponSeleccionado;

        public CuponesModel CuponSeleccionado
        {
            get { return cuponSeleccionado; }
            set
            {
                cuponSeleccionado = value;
                OnPropertyChanged(nameof(CuponSeleccionado));
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    App.DAUtil.Idioma = "ES";
                    App.establecimientosSeleccionados = new ObservableCollection<object>();
                    await App.DAUtil.NavigationService.NavigateToAsync<DetalleCuponViewModel>(CuponSeleccionado);
                });
            }
        }
        #endregion
        #region Comandos
        public ICommand NuevoCuponCommand { get { return new Command((parametro) => NuevoCuponCommandExecute(parametro)); } }

        #endregion
        #region Eventos
        private void NuevoCuponCommandExecute(object parametro)
        {
            try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                App.DAUtil.Idioma = "ES";
                App.establecimientosSeleccionados = new ObservableCollection<object>();
                await App.DAUtil.NavigationService.NavigateToAsync<DetalleCuponViewModel>();
            });

        }

        #endregion
    }
}

