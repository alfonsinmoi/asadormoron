using System;
using System.Threading.Tasks;
using System.Windows.Input;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Informes
{
    public class InformesViewModel:ViewModelBase
    {
        public InformesViewModel()
        {
        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.ConFoto = false;
                App.DAUtil.ExportandoExcel = false;
                if (App.DAUtil.Usuario.rol == (int)RolesEnum.Marketing)
                    InformeVisibleByRol = false;
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Comandos
        public ICommand IrBeneficiosCommand { get { return new Command(IrBeneficio); } }
        public ICommand IrTicketMedioCommand { get { return new Command(IrTicketMedio); } }
        public ICommand IrProductosMasVendidosCommand { get { return new Command(IrProductosMasVendidos); } }
        public ICommand IrMejoresClientesCommand { get { return new Command(IrMejoresClientes); } }
        public ICommand IrClientesInactivosCommand { get { return new Command(IrClientesInsactivos); } }
        #endregion

        #region Métodos
        private void IrBeneficio()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                    MainThread.BeginInvokeOnMainThread(async() =>
                    {
                       await App.DAUtil.NavigationService.NavigateToAsync<InformeBeneficiosViewModel>();
                    });
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje +ex.Message,AppResources.SoloError, AppResources.Cerrar);
                // 
            }
        }
        private void IrTicketMedio()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                    MainThread.BeginInvokeOnMainThread(async() =>
                    {
                        await App.DAUtil.NavigationService.NavigateToAsync<InformeTicketMedioViewModel>();
                    });
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje +ex.Message,AppResources.SoloError, AppResources.Cerrar);
                // 
            }
        }
        private void IrProductosMasVendidos()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.DAUtil.NavigationService.NavigateToAsync<ProductosMasVendidosViewModel>();
                    });
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje +ex.Message,AppResources.SoloError, AppResources.Cerrar);
                // 
            }
        }
        private void IrMejoresClientes()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                    MainThread.BeginInvokeOnMainThread(async() =>
                    {
                        await App.DAUtil.NavigationService.NavigateToAsync<MejoresClientesViewModel>();
                    });
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje +ex.Message,AppResources.SoloError, AppResources.Cerrar);
                // 
            }
        }
        private void IrClientesInsactivos()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.DAUtil.NavigationService.NavigateToAsync<ClientesInactivosViewModel>();
                    });
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje +ex.Message,AppResources.SoloError, AppResources.Cerrar);
                // 
            }
        }
        #endregion

        private bool informeVisibleByRol = true;

        public bool InformeVisibleByRol
        {
            get { return informeVisibleByRol; }
            set 
            { 
                informeVisibleByRol = value;
                OnPropertyChanged();
            }
        }
    }
}
