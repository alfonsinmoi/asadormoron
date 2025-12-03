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

namespace AsadorMoron.ViewModels.Administrador
{

    public class DetalleZonaViewModel : ViewModelBase
    {
        public DetalleZonaViewModel() { }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
               
                App.DAUtil.EnTimer = false;
                if (navigationData is ZonaModel)
                {
                    try
                    {
                        _Zona = (ZonaModel)navigationData;
                        
                        Nombre = _Zona.nombre;
                        if (_Zona.activo == 1)
                            Estado = AppResources.Activo.ToUpper();
                        else
                            Estado = AppResources.Inactivo.ToUpper();
                        Gastos = _Zona.gastos.ToString();
                        PedidoMinimo = _Zona.pedidoMinimo;
                        ValorEstado = _Zona.activo;
                        Modificable = _Zona.modificable == 1;
                        CambiaDireccion = _Zona.cambiaDireccion == 1;
                        if (CambiaDireccion)
                            CambiaDireccionTexto = AppResources.Activo.ToUpper();
                        else
                            CambiaDireccionTexto = AppResources.Inactivo.ToUpper();

                        Direccion = _Zona.direccionEnvio;

                        ColorP = new Microsoft.Maui.Graphics.Color();
                        ColorP = Microsoft.Maui.Graphics.Color.FromHex(_Zona.color);

                        ColorTexto = _Zona.color;
                        VisibleNormal = true;
                        VisibleEdicion = false;
                    }
                    catch (Exception)
                    {
                        VisibleNormal = false;
                        VisibleEdicion = true;
                    }

                }
                else
                {
                    VisibleNormal = false;
                    VisibleEdicion = true;
                }
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
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
        private void EditarZonaCommandExecute(object parametro)
        {
            try
            {
                Nombre = _Zona.nombre;

                if (_Zona.activo == 1)
                    Estado = AppResources.Activo.ToUpper();
                else
                    Estado = AppResources.Inactivo.ToUpper();
                ValorEstado = _Zona.activo;
                Gastos = _Zona.gastos.ToString();
                PedidoMinimo = _Zona.pedidoMinimo;
                CambiaDireccion = _Zona.cambiaDireccion == 1;
                if (CambiaDireccion)
                    CambiaDireccionTexto = AppResources.Activo.ToUpper();
                else
                    CambiaDireccionTexto = AppResources.Inactivo.ToUpper();

                Direccion = _Zona.direccionEnvio;
                ColorP = new Microsoft.Maui.Graphics.Color();
                ColorP = Microsoft.Maui.Graphics.Color.FromHex(_Zona.color);
                ColorTexto = _Zona.color;
                VisibleNormal = false;
                VisibleEdicion = true;
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task initGuardar()
        {
            try
            {
                if (_Zona == null)
                {
                    _Zona = new ZonaModel();
                    _Zona.nombre = Nombre;
                    _Zona.activo = ValorEstado;
                    _Zona.gastos = double.Parse(Gastos.Replace(".", ","));
                    _Zona.pedidoMinimo = PedidoMinimo;
                    _Zona.idPueblo = 1;
                    _Zona.color = ColorP.ToHex();
                    if (Modificable)
                        _Zona.modificable = 1;
                    else
                        _Zona.modificable = 0;
                    if (CambiaDireccion)
                        _Zona.cambiaDireccion = 1;
                    else
                        _Zona.cambiaDireccion = 0;
                    _Zona.direccionEnvio = Direccion;

                    if (App.ResponseWS.nuevaZona(_Zona))
                    {

                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.ZonaOK, AppResources.App, AppResources.Aceptar);
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                    }
                }
                else
                {
                    _Zona.nombre = Nombre;
                    _Zona.activo = ValorEstado;
                    _Zona.idPueblo = 1;
                    _Zona.gastos = double.Parse(Gastos.Replace(".", ","));
                    _Zona.pedidoMinimo = PedidoMinimo;
                    _Zona.color = ColorP.ToHex();
                    if (Modificable)
                        _Zona.modificable = 1;
                    else
                        _Zona.modificable = 0;
                    if (CambiaDireccion)
                        _Zona.cambiaDireccion = 1;
                    else
                        _Zona.cambiaDireccion = 0;
                    _Zona.direccionEnvio = Direccion;
                    if (App.ResponseWS.actualizaZona(_Zona))
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.mZonaOK, AppResources.App, AppResources.Aceptar);
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
        public ICommand EditarZonaCommand { get { return new Command((parametro) => EditarZonaCommandExecute(parametro)); } }
        #endregion

        #region Propiedades

        private string gastos;
        public string Gastos
        {
            get
            {
                return gastos;
            }
            set
            {
                if (gastos != value)
                {
                    gastos = value;
                    OnPropertyChanged(nameof(Gastos));
                }
            }
        }

        private double pedidoMinimo;
        public double PedidoMinimo
        {
            get
            {
                return pedidoMinimo;
            }
            set
            {
                if (pedidoMinimo != value)
                {
                    pedidoMinimo = value;
                    OnPropertyChanged(nameof(PedidoMinimo));
                }
            }
        }
        private bool modificable;
        public bool Modificable
        {
            get
            {
                return modificable;
            }
            set
            {
                if (modificable != value)
                {
                    modificable = value;
                    OnPropertyChanged(nameof(Modificable));
                }
            }
        }
        private bool cambiaDireccion;
        public bool CambiaDireccion
        {
            get
            {
                return cambiaDireccion;
            }
            set
            {
                if (cambiaDireccion != value)
                {
                    cambiaDireccion = value;
                    OnPropertyChanged(nameof(CambiaDireccion));
                }
            }
        }

        private Microsoft.Maui.Graphics.Color colorP;
        public Microsoft.Maui.Graphics.Color ColorP
        {
            get
            {
                return colorP;
            }
            set
            {
                if (colorP != value)
                {
                    colorP = value;
                    OnPropertyChanged(nameof(ColorP));
                }
            }
        }

        private string direccion;
        public string Direccion
        {
            get
            {
                return direccion;
            }
            set
            {
                if (direccion != value)
                {
                    direccion = value;
                    OnPropertyChanged(nameof(Direccion));
                }
            }
        }

        private string cambiaDireccionTexto;
        public string CambiaDireccionTexto
        {
            get
            {
                return cambiaDireccionTexto;
            }
            set
            {
                if (cambiaDireccionTexto != value)
                {
                    cambiaDireccionTexto = value;
                    OnPropertyChanged(nameof(CambiaDireccionTexto));
                }
            }
        }

        private string colorTexto;
        public string ColorTexto
        {
            get
            {
                return colorTexto;
            }
            set
            {
                if (colorTexto != value)
                {
                    colorTexto = value;
                    OnPropertyChanged(nameof(ColorTexto));
                }
            }
        }

        private ZonaModel _Zona;

        private string nombre;
        public string Nombre
        {
            get { return nombre; }
            set
            {
                if (nombre != value)
                {
                    nombre = value;
                    OnPropertyChanged(nameof(Nombre));
                }
            }
        }

        private string estado;
        public string Estado
        {
            get { return estado; }
            set
            {
                if (estado != value)
                {
                    estado = value;
                    OnPropertyChanged(nameof(Estado));
                }
            }
        }

        private int valorEstado = 1;
        public int ValorEstado
        {
            get { return valorEstado; }
            set
            {
                if (valorEstado != value)
                {
                    valorEstado = value;
                    OnPropertyChanged(nameof(ValorEstado));
                }
            }
        }

        private bool visibleNormal;
        public bool VisibleNormal
        {
            get { return visibleNormal; }
            set
            {
                if (visibleNormal != value)
                {
                    visibleNormal = value;
                    OnPropertyChanged(nameof(VisibleNormal));
                }
            }
        }

        private bool visibleEdicion;
        public bool VisibleEdicion
        {
            get { return visibleEdicion; }
            set
            {
                if (visibleEdicion != value)
                {
                    visibleEdicion = value;
                    OnPropertyChanged(nameof(VisibleEdicion));
                }
            }
        }
        #endregion

    }
}
