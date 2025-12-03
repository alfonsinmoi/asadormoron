using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Establecimientos
{
    public class CategoriasViewModel : ViewModelBase
    {
        public CategoriasViewModel() { }

        public override async Task InitializeAsync(object navigationData)
        {
            App.DAUtil.EnTimer = false;
            _establecimiento = (Establecimiento)navigationData;
            ListadoCategorias = ResponseServiceWS.getListadoCategorias(_establecimiento.idEstablecimiento);

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Metodos
        private void NuevaCategoriaCommandExecute(object parametro)
        {
            try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                App.DAUtil.Idioma = "ES";
                await App.DAUtil.NavigationService.NavigateToAsync<DetalleCategoriaViewModel>(_establecimiento.idEstablecimiento);
            });
        }
        #endregion

        #region Comandos
        public ICommand NuevaCategoriaCommand { get { return new Command((parametro) => NuevaCategoriaCommandExecute(parametro)); } }
        #endregion

        #region Propiedades

        private Establecimiento _establecimiento;
        private Categoria categoriaSeleccionada;
        public Categoria CategoriaSeleccionada
        {
            get { return categoriaSeleccionada; }
            set
            {
                if (categoriaSeleccionada != value)
                {
                    categoriaSeleccionada = value;
                    OnPropertyChanged(nameof(CategoriaSeleccionada));
                    try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        App.DAUtil.Idioma = "ES";
                        await App.DAUtil.NavigationService.NavigateToAsync<DetalleCategoriaViewModel>(CategoriaSeleccionada);
                    });

                }
            }
        }

        private Categoria categoriaSeleccionadaEng;
        public Categoria CategoriaSeleccionadaEng
        {
            get { return categoriaSeleccionadaEng; }
            set
            {
                if (categoriaSeleccionadaEng != value)
                {
                    categoriaSeleccionadaEng = value;
                    OnPropertyChanged(nameof(CategoriaSeleccionadaEng));
                    App.DAUtil.Idioma = "ENG";
                    App.DAUtil.NavigationService.NavigateToAsync<DetalleCategoriaViewModel>(CategoriaSeleccionadaEng);
                }
            }
        }

        private Categoria categoriaSeleccionadaFr;
        public Categoria CategoriaSeleccionadaFr
        {
            get { return categoriaSeleccionadaFr; }
            set
            {
                if (categoriaSeleccionadaFr != value)
                {
                    categoriaSeleccionadaFr = value;
                    OnPropertyChanged(nameof(CategoriaSeleccionadaFr));
                    App.DAUtil.Idioma = "FR";
                    App.DAUtil.NavigationService.NavigateToAsync<DetalleCategoriaViewModel>(CategoriaSeleccionadaFr);
                }
            }
        }
        private Categoria categoriaSeleccionadaDe;
        public Categoria CategoriaSeleccionadaDe
        {
            get { return categoriaSeleccionadaDe; }
            set
            {
                if (categoriaSeleccionadaDe != value)
                {
                    categoriaSeleccionadaDe = value;
                    OnPropertyChanged(nameof(CategoriaSeleccionadaDe));
                    App.DAUtil.Idioma = "GER";
                    App.DAUtil.NavigationService.NavigateToAsync<DetalleCategoriaViewModel>(CategoriaSeleccionadaDe);
                }
            }
        }

        private List<Categoria> listadoCategorias;
        public List<Categoria> ListadoCategorias
        {
            get { return listadoCategorias; }
            set
            {
                listadoCategorias = value;
                OnPropertyChanged(nameof(ListadoCategorias));
            }
        }

        #endregion

    }
}
