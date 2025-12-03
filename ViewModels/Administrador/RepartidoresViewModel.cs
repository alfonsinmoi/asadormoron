using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System;
// 
using AsadorMoron.Recursos;
using AsadorMoron.Interfaces;
using AsadorMoron.Services;
using System.Linq;

namespace AsadorMoron.ViewModels.Administrador
{
    public class RepartidoresViewModel : ViewModelBase
    {
        public RepartidoresViewModel()
        {

        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                ListadoRepartidores = new ObservableCollection<RepartidorModel>(App.DAUtil.GetRepartidores());
                Task.Run(async () => { recarga = App.ResponseWS.listadoRepartidores(); }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (recarga.Count > 0)
                    {
                        ListadoRepartidores = new ObservableCollection<RepartidorModel>(App.DAUtil.GetRepartidores());
                    }
                }));
                await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
            }
            catch (Exception ex)
            {
                // 
            }
            finally
            {
                App.userdialog.HideLoading();
            }
        }

        #region Propiedades

        List<RepartidorModel> recarga;

        private RepartidorModel repartidorSeleccionado;
        public RepartidorModel RepartidorSeleccionado
        {
            get
            {
                return repartidorSeleccionado;
            }
            set
            {
                if (repartidorSeleccionado != value)
                {
                    repartidorSeleccionado = value;
                    OnPropertyChanged(nameof(RepartidorSeleccionado));
                    try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
               
                        MainThread.BeginInvokeOnMainThread(async() =>
                        {
                           await App.DAUtil.NavigationService.NavigateToAsync<DetalleRepartidorViewModel>(RepartidorSeleccionado);
                        });

                }
            }
        }
        private ObservableCollection<RepartidorModel> listadoRepartidores;

        public ObservableCollection<RepartidorModel> ListadoRepartidores
        {
            get { return listadoRepartidores; }
            set
            {
                listadoRepartidores = value;
                OnPropertyChanged(nameof(ListadoRepartidores));
            }
        }

        #endregion

        #region Comandos

        public ICommand NuevoRepartidorCommand { get { return new Command((parametro) => NuevoRepartidorCommandExecute(parametro)); } }
        public ICommand EliminarRepartidorCommand { get { return new Command((parametro) => EliminarRepartidorCommandExecute(parametro)); } }

        #endregion

        #region Metodos

        private void NuevoRepartidorCommandExecute(object parametro)
        {
            try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
            
                MainThread.BeginInvokeOnMainThread(async() =>
                {
                   await App.DAUtil.NavigationService.NavigateToAsync<DetalleRepartidorViewModel>();
                });
        }

        private async void EliminarRepartidorCommandExecute(object parametro)
        {
            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaEliminarRepartidor, AppResources.No, AppResources.Si);
            if (result)
            {
                RepartidorModel r = (RepartidorModel)parametro;
                r.eliminado = 1;
                if (App.ResponseWS.actualizaRepartidor(r,""))
                {
                    await App.customDialog.ShowDialogAsync(AppResources.RepartidorEliminadoOK, AppResources.App, AppResources.Cerrar);
                    r.eliminado = 0;
                    ListadoRepartidores.Remove(r);
                }
                else
                    await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
            }
        }

        #endregion
    }
}
