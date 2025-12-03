// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Administrador
{
    public class ZonasViewModel : ViewModelBase
    {
        public ZonasViewModel()
        {

        }
        private ZonaModel zonaSeleccionada;
        public ZonaModel ZonaSeleccionada
        {
            get
            {
                return zonaSeleccionada;
            }
            set
            {
                if (zonaSeleccionada != value)
                {
                    zonaSeleccionada = value;
                    OnPropertyChanged(nameof(ZonaSeleccionada));
                    try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        App.DAUtil.Idioma = "ES";
                        await App.DAUtil.NavigationService.NavigateToAsync<DetalleZonaViewModel>(ZonaSeleccionada);
                    });

                }
            }
        }
        private List<ZonaModel> listadoZonas;

        public List<ZonaModel> ListadoZonas
        {
            get { return listadoZonas; }
            set
            {
                listadoZonas = value;
                OnPropertyChanged(nameof(ListadoZonas));
            }
        }

        public ICommand NuevaZonaCommand { get { return new Command((parametro) => NuevaZonaCommandExecute(parametro)); } }

        private void NuevaZonaCommandExecute(object parametro)
        {
            try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                App.DAUtil.Idioma = "ES";
                await App.DAUtil.NavigationService.NavigateToAsync<DetalleZonaViewModel>(PuebloSeleccionado);
            });

        }
        private bool esMultiAdmin;
        public bool EsMultiAdmin
        {
            get
            {
                return esMultiAdmin;
            }
            set
            {
                if (esMultiAdmin != value)
                {
                    esMultiAdmin = value;
                    OnPropertyChanged(nameof(EsMultiAdmin));
                }
            }
        }
        private ObservableCollection<PueblosModel> pueblos;
        public ObservableCollection<PueblosModel> Pueblos
        {
            get
            {
                return pueblos;
            }
            set
            {
                if (pueblos != value)
                {
                    pueblos = value;
                    OnPropertyChanged(nameof(Pueblos));

                }
            }
        }
        private PueblosModel puebloSeleccionado;
        public PueblosModel PuebloSeleccionado
        {
            get
            {
                return puebloSeleccionado;
            }
            set
            {
                if (puebloSeleccionado != value)
                {
                    puebloSeleccionado = value;
                    OnPropertyChanged(nameof(PuebloSeleccionado));
                    if (PuebloSeleccionado != null)
                        ListadoZonas = App.ResponseWS.getListadoZonas(PuebloSeleccionado.id);
                }
            }
        }
        private void CargarPueblos()
        {
            try
            {

                Pueblos = new ObservableCollection<PueblosModel>(ResponseServiceWS.getPueblosAdministrador());
                EsMultiAdmin = Pueblos.Count > 1;
                PuebloSeleccionado = Pueblos[0];
            }
            catch (Exception ex)
            {
                // 
            }
        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                CargarPueblos();
                
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
    }
}
