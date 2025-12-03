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
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Establecimientos
{
    public class AutoPedidoMinViewModel : ViewModelBase
    {
        public AutoPedidoMinViewModel()
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
                TiposPago = new ObservableCollection<string>();
                TiposPago.Add(AppResources.Efectivo);
                TiposPago.Add("Datafono");
                TipoPago = TiposPago[0];
                try
                {
                    MisEstablecimientos = new ObservableCollection<Establecimiento>(App.DAUtil.Usuario.establecimientos);
                }
                catch (Exception ex)
                {
                    MisEstablecimientos = new ObservableCollection<Establecimiento>();
                    MisEstablecimientos.Add(App.EstActual);
                }
                MiEstablecimiento = App.EstActual;
                VisibleEstablecimiento = MisEstablecimientos.Count > 1;

                if (MiEstablecimiento.configuracion == null)
                    MiEstablecimiento.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(MiEstablecimiento.idEstablecimiento);
                VisibleFuera = MiEstablecimiento.visibleFuera;
                if (VisibleFuera)
                {
                    Pueblos = new ObservableCollection<PueblosModel>(App.DAUtil.GetPueblosSQLite().Where(p => p.idGrupo == MiEstablecimiento.idGrupo));
                    PuebloSeleccionado = Pueblos[0];
                }
                else
                {
                    Zonas = App.DAUtil.getZonas();
                    ZonaSeleccionada = Zonas.Where(p => p.idZona == MiEstablecimiento.configuracion.idZonaAutoPedido).FirstOrDefault();
                }
                Importe = "";
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
        private bool visibleFuera=false;
        public bool VisibleFuera
        {
            get { return visibleFuera; }
            set
            {
                visibleFuera = value;
                OnPropertyChanged(nameof(VisibleFuera));
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
        private Establecimiento miEstablecimiento;
        public Establecimiento MiEstablecimiento
        {
            get { return miEstablecimiento; }
            set
            {
                miEstablecimiento = value;
                OnPropertyChanged(nameof(MiEstablecimiento));
                if (MiEstablecimiento.configuracion == null)
                    MiEstablecimiento.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(MiEstablecimiento.idEstablecimiento);
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
                    {
                        Zonas = App.ResponseWS.getZonas(PuebloSeleccionado.id);
                        ZonaSeleccionada = Zonas[0];
                    }
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
                if (double.Parse(Importe.Replace(".", ",")) <= 0)
                    await App.customDialog.ShowDialogAsync(AppResources.ImporteMayor0, AppResources.App, AppResources.Cerrar);
                /*else if (double.Parse(Importe.Replace(".", ",")) < MiEstablecimiento.configuracion.pedidoMinimo)
                    await App.customDialog.ShowDialogAsync(AppResources.ImporteMenorMinimo, AppResources.App, AppResources.Cerrar);*/
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
                                double imp = 0;
                                try
                                {
                                    imp = double.Parse(Importe.Replace(".", ","));
                                }
                                catch (Exception)
                                {
                                    double.Parse(Importe);
                                }

                                AutoPedidoModel auto = new AutoPedidoModel();
                                auto.apellidos = "";
                                auto.codigo = App.DAUtil.GetCodigo();
                                auto.direccion = "";
                                auto.hora = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                auto.idEstablecimiento = MiEstablecimiento.idEstablecimiento;
                                auto.idZona = App.EstActual.idZona;
                                auto.importe = imp;
                                auto.nombre = "Auto Pedido";
                                auto.telefono = "";
                                auto.tipoPago = TipoPago;
                                if (VisibleFuera)
                                {
                                    auto.idPueblo = PuebloSeleccionado.id;
                                    auto.codPostal = PuebloSeleccionado.codPostal;
                                    auto.poblacion = PuebloSeleccionado.nombre;
                                    auto.provincia = PuebloSeleccionado.Provincia;
                                    auto.importeZona += App.ResponseWS.getGastosEnvioOtroPueblo(PuebloSeleccionado.id, MiEstablecimiento.idPueblo);
                                }
                                else
                                {
                                    auto.idPueblo = MiEstablecimiento.idPueblo;
                                    auto.codPostal = MiEstablecimiento.codPostal;
                                    auto.poblacion = MiEstablecimiento.poblacion;
                                    auto.provincia = MiEstablecimiento.provincia;
                                }
                                //ZonaSeleccionada = Zonas.Find(p => p.idZona == App.EstActual.idZona);
                                auto.importeZona = ZonaSeleccionada.gastos;
                                int idCodigoPedido = ResponseServiceWS.NuevoAutoPedido(auto,MiEstablecimiento.configuracion.estadoAutoPedido);
                                if (idCodigoPedido > 0)
                                {
                                    string pedido = string.Empty;
                                    List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(MiEstablecimiento.idEstablecimiento);
                                    foreach (TokensModel to in tokens3)
                                        App.ResponseWS.enviaNotificacion(MiEstablecimiento.nombre, "Nuevo Pedido: " + auto.codigo, to.token);

                                    List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(MiEstablecimiento.idPueblo);
                                    foreach (TokensModel to in tokens)
                                        App.ResponseWS.enviaNotificacion(MiEstablecimiento.nombre, "Nuevo Pedido para " + MiEstablecimiento.nombre + ": " + auto.codigo, to.token);

                                    List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(MiEstablecimiento.idEstablecimiento);
                                    foreach (TokensModel to in tokens2)
                                        App.ResponseWS.enviaNotificacion(MiEstablecimiento.nombre, "Nuevo Pedido para " + MiEstablecimiento.nombre + ": " + auto.codigo, to.token);
                                    ok = true;
                                }
                                else
                                {
                                    await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
                                }
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
