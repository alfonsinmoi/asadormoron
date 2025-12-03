using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using AsadorMoron.Interfaces;
using System;
using System.Linq;
// 
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using AsadorMoron.Recursos;

namespace AsadorMoron.ViewModels.Establecimientos
{
    public class IngredientesViewModel : ViewModelBase
    {
        public IngredientesViewModel()
        {

        }
        private ObservableCollection<Establecimiento> misEstablecimientos;
        public ObservableCollection<Establecimiento> MisEstablecimientos
        {
            get { return misEstablecimientos; }
            set
            {
                misEstablecimientos = value;
                OnPropertyChanged(nameof(MisEstablecimientos));
            }
        }
        private Establecimiento miEstablecimiento;
        public Establecimiento MiEstablecimiento
        {
            get { return miEstablecimiento; }
            set
            {
                miEstablecimiento = value;
                OnPropertyChanged(nameof(MiEstablecimiento));
                App.EstActual = MiEstablecimiento;
                CargaIngredientes();
            }
        }
        private bool visibleEstablecimiento = false;
        public bool VisibleEstablecimiento
        {
            get { return visibleEstablecimiento; }
            set
            {
                visibleEstablecimiento = value;
                OnPropertyChanged(nameof(VisibleEstablecimiento));
            }
        }
        private IngredientesModel ingredienteSeleccionado;
        public IngredientesModel IngredienteSeleccioando
        {
            get
            {
                return ingredienteSeleccionado;
            }
            set
            {
                if (ingredienteSeleccionado != value)
                {
                    ingredienteSeleccionado = value;
                    OnPropertyChanged(nameof(IngredienteSeleccioando));
                    try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await App.DAUtil.NavigationService.NavigateToAsync<DetalleIngredienteViewModel>(IngredienteSeleccioando);
                        });

                }
            }
        }
        private ObservableCollection<IngredientesModel> listadoIngredientes;

        public ObservableCollection<IngredientesModel> ListadoIngredientes
        {
            get { return listadoIngredientes; }
            set
            {
                listadoIngredientes = value;
                OnPropertyChanged(nameof(ListadoIngredientes));
            }
        }

        public ICommand NuevoIngredienteCommand { get { return new Command((parametro) => NuevoIngredienteCommandExecute(parametro)); } }

        private void NuevoIngredienteCommandExecute(object parametro)
        {
            try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
            
                MainThread.BeginInvokeOnMainThread(async() =>
                {
                   await App.DAUtil.NavigationService.NavigateToAsync<DetalleIngredienteViewModel>();
                });
        }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                if (App.userdialog == null)
                {
                    try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
                App.DAUtil.EnTimer = false;
                try
                {
                    MisEstablecimientos = new ObservableCollection<Establecimiento>(App.DAUtil.Usuario.establecimientos);
                }
                catch (Exception ex)
                {
                    MisEstablecimientos = new ObservableCollection<Establecimiento>();
                    MisEstablecimientos.Add(App.EstActual);
                }
                if (App.EstActual == null)
                    App.EstActual = MisEstablecimientos[0];
                MiEstablecimiento = App.EstActual;
                VisibleEstablecimiento = MisEstablecimientos.Count > 1;
                
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
        private void CargaIngredientes()
        {
            ListadoIngredientes = new ObservableCollection<IngredientesModel>(App.ResponseWS.listadoIngredientes(MiEstablecimiento.idEstablecimiento).OrderBy(p => p.nombre));
        }
    }
}
