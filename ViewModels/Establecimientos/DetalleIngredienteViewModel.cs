using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
// 
using AsadorMoron.Recursos;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;

namespace AsadorMoron.ViewModels.Establecimientos
{

    public class DetalleIngredienteViewModel : ViewModelBase
    {
        public DetalleIngredienteViewModel() { }

        public override Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                if (navigationData != null)
                {
                    try
                    {
                        _Ingrediente = (IngredientesModel)navigationData;

                        Nombre = _Ingrediente.nombre;
                        Estado = _Ingrediente.estado == 1;
                        Puntos = _Ingrediente.puntos;
                        Precio = _Ingrediente.precio.ToString();
                    }
                    catch (Exception)
                    {

                    }

                }
                else
                {
                    Estado = true;
                }
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
            return base.InitializeAsync(navigationData);
        }

        #region Metodos
        private async void Guardar()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                await Task.Delay(200);
                await Task.Run(async () => { await initGuardar(); }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.DAUtil.NavigationService.NavigateBackAsync().ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
                }));
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void Cancelar()
        {
            App.DAUtil.NavigationService.NavigateBackAsync();
        }

        private async Task initGuardar()
        {
            try
            {
                if (_Ingrediente == null)
                {
                    _Ingrediente = new IngredientesModel();
                    _Ingrediente.nombre = Nombre;
                    try
                    {
                        _Ingrediente.puntos = Puntos;
                    }catch(Exception)
                    {
                        _Ingrediente.puntos = 0;
                    }
                    _Ingrediente.idEstablecimiento = App.EstActual.idEstablecimiento;
                    if (Estado)
                        _Ingrediente.estado = 1;
                    else
                        _Ingrediente.estado = 0;
                    _Ingrediente.precio = double.Parse(Precio.Replace(".", ","));

                    if (App.ResponseWS.nuevoIngrediente(_Ingrediente))
                    {

                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.IngredienteOK, AppResources.App, AppResources.Aceptar);
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                    }
                }
                else
                {
                    _Ingrediente.nombre = Nombre;
                    if (Estado)
                        _Ingrediente.estado = 1;
                    else
                        _Ingrediente.estado = 0;
                    try
                    {
                        _Ingrediente.puntos = Puntos;
                    }
                    catch (Exception)
                    {
                        _Ingrediente.puntos = 0;
                    }
                    _Ingrediente.precio = double.Parse(Precio.Replace(".", ","));
                    _Ingrediente.idEstablecimiento = App.EstActual.idEstablecimiento;
                    if (App.ResponseWS.actualizaIngrediente(_Ingrediente))
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.mIngredienteOK, AppResources.App, AppResources.Aceptar);
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                    }
                }

            }
            catch (Exception ex)
            {
                // 
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
            }
        }

        #endregion

        #region Comandos

        public ICommand CommandGuardar { get { return new Command(Guardar); } }
        public ICommand CommandCancelar { get { return new Command(Cancelar); } }

        #endregion

        #region Propiedades

        private IngredientesModel _Ingrediente;

        private string precio;
        public string Precio
        {
            get
            {
                return precio;
            }
            set
            {
                if (precio != value)
                {
                    precio = value;
                    OnPropertyChanged(nameof(Precio));
                }
            }
        }
        private int puntos;
        public int Puntos
        {
            get
            {
                return puntos;
            }
            set
            {
                if (puntos != value)
                {
                    puntos = value;
                    OnPropertyChanged(nameof(Puntos));
                }
            }
        }

        private bool estado;
        public bool Estado
        {
            get
            {
                return estado;
            }
            set
            {
                if (estado != value)
                {
                    estado = value;
                    OnPropertyChanged(nameof(Estado));
                }
            }
        }

        private string nombre;
        public string Nombre
        {
            get
            {
                return nombre;
            }
            set
            {
                if (nombre != value)
                {
                    nombre = value;
                    OnPropertyChanged(nameof(Nombre));
                }
            }
        }
        #endregion

    }
}
