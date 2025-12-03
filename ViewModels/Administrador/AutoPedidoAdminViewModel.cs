using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.ViewModels.Clientes;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Administrador
{
    public class AutoPedidoAdminViewModel : ViewModelBase
    {
        public AutoPedidoAdminViewModel()
        {
        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                Logo = "logomorado.png";
                Nombre = "";
                Apellidos = "";
                Direccion = "";
                Telefono = "";
                Importe = "0";

                TiposPago = new ObservableCollection<string>();
                TiposPago.Add(AppResources.Efectivo);
                TiposPago.Add("Datafono");
                TipoPago = TiposPago[0];
                Zonas = new List<ZonaModel>(App.DAUtil.getZonas());
                ZonaSeleccionada = Zonas.Where(p => p.idZona == App.EstActual.configuracion.idZonaAutoPedido).FirstOrDefault();
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }
            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Propiedades
        private ObservableCollection<string> tiposPago;
        public ObservableCollection<string> TiposPago
        {
            get
            {
                return tiposPago;
            }
            set
            {
                if (tiposPago != value)
                {
                    tiposPago = value;
                    OnPropertyChanged(nameof(TiposPago));

                }
            }
        }
        private string tipoPago;
        public string TipoPago
        {
            get
            {
                return tipoPago;
            }
            set
            {
                if (tipoPago != value)
                {
                    tipoPago = value;
                    OnPropertyChanged(nameof(TipoPago));
                }
            }
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
        private string logo;
        public string Logo
        {
            get
            {
                return logo;
            }
            set
            {
                if (logo != value)
                {
                    logo = value;
                    OnPropertyChanged(nameof(Logo));
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
        private string apellidos;
        public string Apellidos
        {
            get
            {
                return apellidos;
            }
            set
            {
                if (apellidos != value)
                {
                    apellidos = value;
                    OnPropertyChanged(nameof(Apellidos));
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
        private string telefono;
        public string Telefono
        {
            get
            {
                return telefono;
            }
            set
            {
                if (telefono != value)
                {
                    telefono = value;
                    OnPropertyChanged(nameof(Telefono));
                }
            }
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
                }
            }
        }
        private List<ZonaModel> zonas;
        public List<ZonaModel> Zonas
        {
            get
            {
                return zonas;
            }
            set
            {
                if (zonas != value)
                {
                    zonas = value;
                    OnPropertyChanged(nameof(Zonas));
                }
            }
        }
        private TimeSpan hora;
        public TimeSpan Hora
        {
            get
            {
                return hora;
            }
            set
            {
                if (hora != value)
                {
                    hora = value;
                    OnPropertyChanged(nameof(Hora));
                }
            }
        }
        private string importe;
        public string Importe
        {
            get
            {
                return importe;
            }
            set
            {
                if (importe != value)
                {
                    importe = value;
                    OnPropertyChanged(nameof(Importe));
                }
            }
        }
        #endregion
        #region Comandos
        public ICommand cmdGenerar { get { return new Command(Generar); } }
        #endregion
        #region Eventos
        private async void Generar()
        {
            try
            {
                if (string.IsNullOrEmpty(Nombre) || string.IsNullOrEmpty(Direccion) || string.IsNullOrEmpty(Telefono) || ZonaSeleccionada==null)
                    await App.customDialog.ShowDialogAsync(AppResources.TodosCamposObligatorios, AppResources.App, AppResources.Cerrar);
               else if (Telefono.Length<9)
                    await App.customDialog.ShowDialogAsync(AppResources.TelefonoIncorrecto, AppResources.App, AppResources.Cerrar);
                else
                {
                    bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaHacerAutoPedido, AppResources.No, AppResources.Si);
               
                    if (result)
                    {

                        try { App.userdialog.ShowLoading(AppResources.Grabando); } catch (Exception) { }
                        bool ok = false;
                        await Task.Run(() =>
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                App.autoPedidoAdmin = new AutoPedidoModel();
                                App.autoPedidoAdmin.apellidos = "";
                                App.autoPedidoAdmin.codigo = App.DAUtil.GetCodigo();
                                App.autoPedidoAdmin.direccion = Direccion;
                                App.autoPedidoAdmin.hora = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, Hora.Hours, Hora.Minutes, Hora.Seconds);
                                App.autoPedidoAdmin.idEstablecimiento = App.EstActual.idEstablecimiento;
                                App.autoPedidoAdmin.idZona = ZonaSeleccionada.idZona;
                                App.autoPedidoAdmin.importe = 0;
                                App.autoPedidoAdmin.nombre = Nombre;
                                App.autoPedidoAdmin.telefono = Telefono;
                                App.autoPedidoAdmin.importeZona = ZonaSeleccionada.gastos;
                                App.autoPedidoAdmin.tipoPago = TipoPago;
                                
                                App.autoPedidoAdmin.idPueblo = App.EstActual.idPueblo;
                                App.autoPedidoAdmin.codPostal = App.EstActual.codPostal;
                                App.autoPedidoAdmin.poblacion = App.EstActual.poblacion;
                                App.autoPedidoAdmin.provincia = App.EstActual.provincia;

                                await App.DAUtil.NavigationService.NavigateToAsyncMenu<CartaViewModel>(App.EstActual);
                            });
                        }).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                        {

                            App.userdialog.HideLoading();
                            if (ok)
                                App.DAUtil.NavigationService.InitializeAsync();
                        }));
                        App.userdialog.HideLoading();
                    }
                }
            }
            catch (Exception ex)
            {
                // 
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
            }
        }
        #endregion
    }
}
