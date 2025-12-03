using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
// 
using System.Collections.ObjectModel;
using AsadorMoron.Recursos;
// using Xamarin.Forms.OpenWhatsApp; // TODO: Reimplementar WhatsApp
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using AsadorMoron.Services;
using CommunityToolkit.Mvvm.Input;

namespace AsadorMoron.ViewModels.Clientes
{

    public class DetalleEstablecimientoParaClienteViewModel : ViewModelBase
    {
        public DetalleEstablecimientoParaClienteViewModel()
        {
            if (App.DAUtil.NotificacionPantalla.Equals(""))
            {
                if (App.userdialog == null)
                {
                    try { App.userdialog?.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
            }
        }
        public ICommand VerProductos { get { return new Command(VerProductosExecute); } }
        private void VerProductosExecute()
        {
            try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                MainThread.BeginInvokeOnMainThread(async() =>
                {
                    await App.DAUtil.NavigationService.NavigateToAsync<CartaViewModel>(_establecimiento);
                });
        }

        public override Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                if (navigationData != null)
                {
                    _establecimiento = (Establecimiento)navigationData;
                    App.EstActual = _establecimiento;
                    Nombre = _establecimiento.nombre;
                    Direccion = _establecimiento.direccion.ToString();
                    Poblacion = _establecimiento.poblacion;
                    Provincia = _establecimiento.provincia;
                    CodPostal = _establecimiento.codPostal;
                    Imagen = _establecimiento.imagen;
                    Latitud = _establecimiento.latitud;
                    Tipo = _establecimiento.tipo;
                    Longitud = _establecimiento.longitud;
                    NumeroCategorias = _establecimiento.numeroCategorias;
                    Productos = _establecimiento.numeroProductos;
                    Telefono = _establecimiento.telefono;
                    Telefono2 = _establecimiento.telefono2;
                    WhatsApp = _establecimiento.whatsapp;
                    Web = _establecimiento.web;
                    TieneEmail = !string.IsNullOrEmpty(_establecimiento.emailContacto);
                    TieneTelefono = !string.IsNullOrEmpty(_establecimiento.telefono);
                    TieneTelefono2 = !string.IsNullOrEmpty(_establecimiento.telefono2);
                    TieneWhatsApp = !string.IsNullOrEmpty(_establecimiento.whatsapp);
                    TieneWeb = !string.IsNullOrEmpty(_establecimiento.web);
                    Logo = _establecimiento.logo;
                    Email = _establecimiento.emailContacto;
                    if (App.EstActual.configuracion == null)
                        App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(_establecimiento.idEstablecimiento);
                    TextoDetalle = App.EstActual.configuracion.textoDetalle;
                    SistemaPuntos = App.EstActual.configuracion.sistemaPuntos;
                    TextoPuntos = App.EstActual.configuracion.textoPuntos + Environment.NewLine;
                    if (App.EstActual.configuracion.puntosPorPedido > 0)
                        TextoPuntos += $"Por cada pedido, usted recibirá {App.EstActual.configuracion.puntosPorPedido} puntos";
                    else if (App.EstActual.configuracion.puntosPorEuro > 0)
                        TextoPuntos += $"Por cada euro, usted recibirá {App.EstActual.configuracion.puntosPorEuro} puntos";
                    Pins = new ObservableCollection<PinModel>();
                    PinModel pin = new PinModel();
                    pin.Address = _establecimiento.direccion;
                    pin.Descripcion = "";
                    pin.Latitude = _establecimiento.latitud;
                    pin.Longitude = _establecimiento.longitud;
                    pin.Nombre = _establecimiento.nombre;
                    pin.Phone = _establecimiento.telefono;
                    pin.Title = _establecimiento.nombre;
                    Pins.Add(pin);
                }

                return base.InitializeAsync(navigationData);

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
                    App.userdialog?.HideLoading();
                }
            }
            return base.InitializeAsync(navigationData);
        }

        public ICommand LlamaTelefono { get { return new Command(Llamar); } }
        public ICommand LlamaTelefono2 { get { return new Command(Llamar2); } }
        private void Llamar()
        {
            try
            {
                PhoneDialer.Open(_establecimiento.telefono.Replace(" ", ""));
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void Llamar2()
        {
            try
            {
                PhoneDialer.Open(_establecimiento.telefono2.Replace(" ", ""));
            }
            catch (Exception ex)
            {
                // 
            }
        }
        public ICommand LlamaWhatsApp { get { return new Command(AbreWhatsApp); } }
        private async void AbreWhatsApp()
        {
            try
            {
                await Launcher.Default.OpenAsync(new Uri($"https://wa.me/34{_establecimiento.whatsapp.Replace(" ", "")}"));
            }
            catch (Exception ex)
            {
                //
            }
        }
        public ICommand /*IAsyncRelayCommand*/ LlamaWeb { get { return new AsyncRelayCommand(AbreWeb); } }
        private async Task AbreWeb()
        {
            try
            {
                await Browser.OpenAsync(new Uri(Web), BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                // 
            }
        }
        public ICommand LlamaEmail { get { return new Command(AbreEmail); } }
        private async void AbreEmail()
        {
            try
            {
                await Launcher.Default.OpenAsync(new Uri("mailto:" + _establecimiento.emailContacto));
            }
            catch (Exception ex)
            {
                //
            }
        }


        private Establecimiento _establecimiento;
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
        private string textoDetalle;
        public string TextoDetalle
        {
            get { return textoDetalle; }
            set
            {
                if (textoDetalle != value)
                {
                    textoDetalle = value;
                    OnPropertyChanged(nameof(TextoDetalle));
                }
            }
        }
        private string whatsapp;
        public string WhatsApp
        {
            get
            {
                return whatsapp;
            }
            set
            {
                if (whatsapp != value)
                {
                    whatsapp = value;
                    OnPropertyChanged(nameof(WhatsApp));
                }
            }
        }
        private string web;
        public string Web
        {
            get
            {
                return web;
            }
            set
            {
                if (web != value)
                {
                    web = value;
                    OnPropertyChanged(nameof(Web));
                }
            }
        }
        private string logo;
        public string Logo
        {
            get { return logo; }
            set
            {
                if (logo != value)
                {
                    logo = value;
                    OnPropertyChanged(nameof(Logo));
                }
            }
        }
        private string imagen = "logo_producto.png";
        public string Imagen
        {
            get { return imagen; }
            set
            {
                if (imagen != value)
                {
                    imagen = value;
                    OnPropertyChanged(nameof(Imagen));
                }
            }
        }
        private string poblacion;
        public string Poblacion
        {
            get { return poblacion; }
            set
            {
                if (poblacion != value)
                {
                    poblacion = value;
                    OnPropertyChanged(nameof(Poblacion));
                }
            }
        }
        private int numeroCategorias = 0;
        public int NumeroCategorias
        {
            get { return numeroCategorias; }
            set
            {
                if (numeroCategorias != value)
                {
                    numeroCategorias = value;
                    OnPropertyChanged(nameof(NumeroCategorias));
                }
            }
        }
        private int numeroProductos = 0;
        public int Productos
        {
            get { return numeroProductos; }
            set
            {
                if (numeroProductos != value)
                {
                    numeroProductos = value;
                    OnPropertyChanged(nameof(Productos));
                }
            }
        }
        private ObservableCollection<PinModel> _pins;
        public ObservableCollection<PinModel> Pins
        {
            get { return _pins; }
            set
            {
                _pins = value;
                OnPropertyChanged(nameof(Pins));
            }
        }
        private string provincia;
        public string Provincia
        {
            get { return provincia; }
            set
            {
                if (provincia != value)
                {
                    provincia = value;
                    OnPropertyChanged(nameof(Provincia));
                }
            }
        }
        private string direccion;
        public string Direccion
        {
            get { return direccion; }
            set
            {
                if (direccion != value)
                {
                    direccion = value;
                    OnPropertyChanged(nameof(Direccion));
                }
            }
        }
        private string tipo;
        public string Tipo
        {
            get { return tipo; }
            set
            {
                if (tipo != value)
                {
                    tipo = value;
                    OnPropertyChanged(nameof(Tipo));
                }
            }
        }
        private string codPostal;
        public string CodPostal
        {
            get { return codPostal; }
            set
            {
                if (codPostal != value)
                {
                    codPostal = value;
                    OnPropertyChanged(nameof(CodPostal));
                }
            }
        }
        private double latitud;
        public double Latitud
        {
            get { return latitud; }
            set
            {
                if (latitud != value)
                {
                    latitud = value;
                    OnPropertyChanged(nameof(Latitud));
                }
            }
        }
        private double longitud;
        public double Longitud
        {
            get { return longitud; }
            set
            {
                if (longitud != value)
                {
                    longitud = value;
                    OnPropertyChanged(nameof(Longitud));
                }
            }
        }
        private string telefono;
        public string Telefono
        {
            get { return telefono; }
            set
            {
                if (telefono != value)
                {
                    telefono = value;
                    OnPropertyChanged(nameof(Telefono));
                }
            }
        }
        private string textoPuntos;
        public string TextoPuntos
        {
            get { return textoPuntos; }
            set
            {
                if (textoPuntos != value)
                {
                    textoPuntos = value;
                    OnPropertyChanged(nameof(TextoPuntos));
                }
            }
        }
        private bool sistemaPuntos;
        public bool SistemaPuntos
        {
            get { return sistemaPuntos; }
            set
            {
                if (sistemaPuntos != value)
                {
                    sistemaPuntos = value;
                    OnPropertyChanged(nameof(SistemaPuntos));
                }
            }
        }
        private string telefono2;
        public string Telefono2
        {
            get
            {
                return telefono2;
            }
            set
            {
                if (telefono2 != value)
                {
                    telefono2 = value;
                    OnPropertyChanged(nameof(Telefono2));
                }
            }
        }
        private bool tieneTelefono;
        public bool TieneTelefono
        {
            get
            {
                return tieneTelefono;
            }
            set
            {
                if (tieneTelefono != value)
                {
                    tieneTelefono = value;
                    OnPropertyChanged(nameof(TieneTelefono));
                }
            }
        }
        private bool tieneTelefono2;
        public bool TieneTelefono2
        {
            get
            {
                return tieneTelefono2;
            }
            set
            {
                if (tieneTelefono2 != value)
                {
                    tieneTelefono2 = value;
                    OnPropertyChanged(nameof(TieneTelefono2));
                }
            }
        }
        private bool tieneWhatsApp;
        public bool TieneWhatsApp
        {
            get
            {
                return tieneWhatsApp;
            }
            set
            {
                if (tieneWhatsApp != value)
                {
                    tieneWhatsApp = value;
                    OnPropertyChanged(nameof(TieneWhatsApp));
                }
            }
        }
        private bool tieneWeb;
        public bool TieneWeb
        {
            get
            {
                return tieneWeb;
            }
            set
            {
                if (tieneWeb != value)
                {
                    tieneWeb = value;
                    OnPropertyChanged(nameof(TieneWeb));
                }
            }
        }
        private bool tieneEmail;
        public bool TieneEmail
        {
            get
            {
                return tieneEmail;
            }
            set
            {
                if (tieneEmail != value)
                {
                    tieneEmail = value;
                    OnPropertyChanged(nameof(TieneEmail));
                }
            }
        }
        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }
    }
}
