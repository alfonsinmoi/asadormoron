using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.Interfaces;
using Mopups.Services;
using System.Text;
// 
using Plugin.Maui.Audio;
// 
using AsadorMoron.Utils;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using AsadorMoron.ViewModels.Clientes;
using AsadorMoron.Views.Administrador;
using AsadorMoron.Recursos;
using System.Diagnostics;
using AsadorMoron.Views.Repartidores;
using AsadorMoron.ViewModels.Administrador;
using CommunityToolkit.Mvvm.Input;

namespace AsadorMoron.ViewModels.Establecimientos
{
    public class HomeViewModelEstMobile : ViewModelBase
    {
        public List<RepartidorModel> repartidores;
        public HomeViewModelEstMobile()
        {
            if (App.DAUtil.NotificacionPantalla.Equals(""))
            {
                if (App.userdialog == null)
                {
                    try { dialogHomeAdmin.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
            }
        }
        public override async Task InitializeAsync(object navigationData)
        {

            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DeviceDisplay.KeepScreenOn = true;
                });
                AceptaEncargos = false;
                VerSoloHoy = false;
                TieneLocal = false;
                foreach (Establecimiento es in App.DAUtil.Usuario.establecimientos)
                {
                    if (es.configuracion == null)
                        es.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(es.idEstablecimiento);
                    if (es.configuracion.aceptaEncargos)
                        AceptaEncargos = true;

                    if (AceptaEncargos)
                        VerSoloHoy = Preferences.Get("VerSoloHoy", true);
                    else
                        if (es.esComercio)
                        VerSoloHoy = true;
                    if (es.local==1)
                        TieneLocal = true;

                    if (es.configuracion.visibilidadHoras == 0)
                    {
                        VisibleFechaEntrega = true;
                        VisibleFechaPedido = false;
                        visibleDosFechas = false;
                    } else if (es.configuracion.visibilidadHoras == 1)
                    {
                        VisibleFechaEntrega = false;
                        VisibleFechaPedido = true;
                        visibleDosFechas = false;
                    } else if (es.configuracion.visibilidadHoras == 2)
                    {
                        VisibleFechaEntrega = false;
                        VisibleFechaPedido = false;
                        visibleDosFechas = true;
                    }
                }

                VisibleMulti = App.DAUtil.Usuario.establecimientos.Count > 1;
                if (VisibleMulti)
                    TextoMulti = "Establecimiento activo: " + App.EstActual.textoMulti;


                Preferences.Set("idGrupo", 1);
                Preferences.Set("idPueblo", 1);
                if (App.userdialog == null)
                {
                    try { dialogHomeAdmin.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
                App.EstActual = App.MiEst;
                if (App.EstActual.configuracion == null)
                    App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);
                App.DAUtil.homeEst = this;
                App.DAUtil.EstoyenHome = true;
                App.DAUtil.EnTimer = true;
                ListadoRepartidores = new ObservableCollection<RepartidorModel>(App.DAUtil.GetRepartidores().Where(p => p.activo == 1 && p.idPueblo==App.DAUtil.Usuario.idPueblo).ToList());
                IsVisibleRepartidores = ListadoRepartidores.Count > 0;
                await initTimer();
                App.DAUtil.Usuario.platform = DeviceInfo.Platform.ToString().ToLower();
                App.DAUtil.Usuario.token = App.DAUtil.InstallId.ToString();
                if (DeviceInfo.Platform.ToString() == "iOS")
                    App.DAUtil.Usuario.version = App.DAUtil.versioniOS;
                else if (DeviceInfo.Platform.ToString() == "Android")
                    App.DAUtil.Usuario.version = App.DAUtil.versionAndroid;


                App.ResponseWS.RegistraTokenFCM(App.DAUtil.Usuario);
                
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
                    dialogHomeAdmin = null;
                }
                else
                {
                    App.userdialog.HideLoading();
                    dialogHomeAdmin = null;
                }
            }
        }

        #region Propiedades
        int resultadoEstadoImpresora;
        private IUserDialogs dialogHomeAdmin = App.userdialog;
        public List<CabeceraPedido> ListPedidosTemp;
        IAudioPlayer player = null; // Initialized when needed
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        private bool aceptaEncargos;
        public bool AceptaEncargos
        {
            get
            {
                return aceptaEncargos;
            }
            set
            {
                if (aceptaEncargos != value)
                {
                    aceptaEncargos = value;
                    OnPropertyChanged(nameof(AceptaEncargos));
                }
            }
        }
        private bool visibleMulti;
        public bool VisibleMulti
        {
            get
            {
                return visibleMulti;
            }
            set
            {
                if (visibleMulti != value)
                {
                    visibleMulti = value;
                    OnPropertyChanged(nameof(VisibleMulti));
                }
            }
        }
        private string textoMulti;
        public string TextoMulti
        {
            get
            {
                return textoMulti;
            }
            set
            {
                if (textoMulti != value)
                {
                    textoMulti = value;
                    OnPropertyChanged(nameof(TextoMulti));
                }
            }
        }
        private bool verSoloHoy;
        public bool VerSoloHoy
        {
            get
            {
                return verSoloHoy;
            }
            set
            {
                if (verSoloHoy != value)
                {
                    verSoloHoy = value;
                    OnPropertyChanged(nameof(VerSoloHoy));
                    Preferences.Set("VerSoloHoy", VerSoloHoy);
                    MainThread.BeginInvokeOnMainThread(async () => await initTimer());
                }
            }
        }
        private bool visibleFechaPedido;
        public bool VisibleFechaPedido
        {
            get
            {
                return visibleFechaPedido;
            }
            set
            {
                if (visibleFechaPedido != value)
                {
                    visibleFechaPedido = value;
                    OnPropertyChanged(nameof(VisibleFechaPedido));
                }
            }
        }
        private bool visibleFechaEntrega;
        public bool VisibleFechaEntrega
        {
            get
            {
                return visibleFechaEntrega;
            }
            set
            {
                if (visibleFechaEntrega != value)
                {
                    visibleFechaEntrega = value;
                    OnPropertyChanged(nameof(VisibleFechaEntrega));
                }
            }
        }
        private bool visibleDosFechas;
        public bool VisibleDosFechas
        {
            get
            {
                return visibleDosFechas;
            }
            set
            {
                if (visibleDosFechas != value)
                {
                    visibleDosFechas = value;
                    OnPropertyChanged(nameof(VisibleDosFechas));
                }
            }
        }
        private bool esComercio;
        public bool EsComercio
        {
            get
            {
                return esComercio;
            }
            set
            {
                if (esComercio != value)
                {
                    esComercio = value;
                    OnPropertyChanged(nameof(EsComercio));
                }
            }
        }
        private bool isVisibleRepartidores;

        public bool IsVisibleRepartidores
        {
            get { return isVisibleRepartidores; }
            set
            {
                isVisibleRepartidores = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<RepartidorModel> listadoRepartidores;
        public ObservableCollection<RepartidorModel> ListadoRepartidores
        {
            get { return listadoRepartidores; }
            set
            {
                if (listadoRepartidores != value)
                {
                    listadoRepartidores = value;
                    OnPropertyChanged(nameof(ListadoRepartidores));
                }
            }
        }
        private int totalPedidos;
        public int TotalPedidos
        {
            get
            {
                return totalPedidos;
            }
            set
            {
                if (totalPedidos != value)
                {
                    totalPedidos = value;
                    OnPropertyChanged(nameof(TotalPedidos));
                }
            }
        }
        private ObservableCollection<CabeceraPedido> listado;
        public ObservableCollection<CabeceraPedido> Listado
        {
            get { return listado; }
            set
            {
                if (listado != value)
                {
                    listado = value;
                    OnPropertyChanged(nameof(Listado));
                }
            }
        }
        private ObservableCollection<CabeceraPedido> listadoLocal;
        public ObservableCollection<CabeceraPedido> ListadoLocal
        {
            get { return listadoLocal; }
            set
            {
                if (listadoLocal != value)
                {
                    listadoLocal = value;
                    OnPropertyChanged(nameof(ListadoLocal));
                }
            }
        }
        private CabeceraPedido cabeceraPedido;

        public CabeceraPedido CabeceraPedido
        {
            get { return cabeceraPedido; }
            set
            {
                cabeceraPedido = value;
                OnPropertyChanged(nameof(CabeceraPedido));
            }
        }
        private bool tieneLocal;
        public bool TieneLocal
        {
            get
            {
                return tieneLocal;
            }
            set
            {
                if (tieneLocal != value)
                {
                    tieneLocal = value;
                    OnPropertyChanged(nameof(TieneLocal));
                }
            }
        }
        #endregion
        #region Funciones
        private void RefreshData()
        {
            try
            {
                App.DAUtil.Usuario.platform = DeviceInfo.Platform.ToString().ToLower();
                App.DAUtil.Usuario.token = App.DAUtil.InstallId.ToString();
                if (DeviceInfo.Platform.ToString() == "iOS")
                    App.DAUtil.Usuario.version = App.DAUtil.versioniOS;
                else if (DeviceInfo.Platform.ToString() == "Android")
                    App.DAUtil.Usuario.version = App.DAUtil.versionAndroid;


                App.ResponseWS.RegistraTokenFCM(App.DAUtil.Usuario);
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void VerRepartidor(object repartidor)
        {

            try
            {
                if (MopupService.Instance.PopupStack.Count() == 0)
                {
                    RepartidorModel r = (RepartidorModel)repartidor;
                    MopupService.Instance.PushAsync(new PopupPageInfoRepartidor(r), true);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void InfoUsuarioPedido(object obj)
        {
            try
            {
                if (MopupService.Instance.PopupStack.Count() == 0)
                {
                    string cod = (string)obj;
                    CabeceraPedido c2 = ResponseServiceWS.TraePedidoPorCodigo(cod);
                    ZonaModel z = App.DAUtil.getZonas().Find(p => p.idZona == c2.idZona);
                    if (z != null)
                        c2.zona = z.nombre;
                    else
                        c2.zona = c2.poblacion;
                    MopupService.Instance.PushAsync(new PopupPageInfoUsuarioPedido(c2), true);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async void VerAutoPedidoExe()
        {
            try {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.DAUtil.NavigationService.NavigateToAsync<CartaViewModel>();
                });
            }
            catch (Exception ex)
            {
                // 
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
            }
            finally
            {
                App.userdialog.HideLoading();
            }
        }
        public async Task actualizaPedidos(bool esMensaje = false, string mensaje = "")
        {
            try
            {
                if (esMensaje)
                {
                    await App.customDialog.ShowDialogAsync(mensaje, "PolloAndaluz", "Cerrar");
                }
                else
                {
                    if (VerSoloHoy)
                        ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendiente()).FindAll(p => ((DateTime)p.fechaEntrega).ToString("dd/MM/yyyy").Equals(DateTime.Today.ToString("dd/MM/yyyy")));
                    else
                        ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendiente());
                    if (Listado == null)
                        Listado = new ObservableCollection<CabeceraPedido>();

                    if (ListPedidosTemp != null)
                    {
                        //List<CabeceraPedido> toBeAdded = ListPedidosTemp.Where(c => c.idPedido == 0).ToList();
                        List<CabeceraPedido> toBeAdded = ListPedidosTemp.Where(c => !c.tipoVenta.Equals("Local") && !Listado.Any(d => c.idPedido == d.idPedido)).ToList();
                        List<CabeceraPedido> toBeDeleted = Listado.Where(c => !c.tipoVenta.Equals("Local") && !ListPedidosTemp.Any(d => c.idPedido == d.idPedido)).ToList();
                        List<CabeceraPedido> toBeUpdated = ListPedidosTemp.Where(c => !c.tipoVenta.Equals("Local") && Listado.Any(d => c.idPedido == d.idPedido && (c.idEstadoPedido != d.idEstadoPedido || c.idRepartidor != d.idRepartidor))).ToList();

                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            if (toBeAdded != null)
                            {
                                if (toBeAdded.Count > 0)
                                {
                                    foreach (var item in toBeAdded)
                                    {
                                        bool ok = false;
                                        switch (item.idEstadoPedido)
                                        {
                                            case (int)EstadoPedido.Nuevo:
                                                item.ColorPedido = "pedidovisto.png";
                                                item.imagenBoton = "minutes.png";
                                                ok = true;
                                                break;
                                            case (int)EstadoPedido.EnProceso:
                                                item.ColorPedido = "pedidovisto.png";
                                                item.imagenBoton = "minutes.png";
                                                ok = true;
                                                break;
                                            case (int)EstadoPedido.PorRecoger:
                                                item.ColorPedido = "pedidoporrecoger.png";
                                                item.imagenBoton = "recogido.png";
                                                ok = true;
                                                break;
                                            case (int)EstadoPedido.Recogido:
                                                ok = false;
                                                break;
                                            case (int)EstadoPedido.Entregado:
                                                ok = false;
                                                break;
                                        }

                                        if (ok)
                                        {
                                            int i = 0;
                                            bool insertado = false;
                                            foreach (CabeceraPedido cab in Listado)
                                            {
                                                if (TimeSpan.Parse(cab.horaEntrega.Substring(11)) > TimeSpan.Parse(item.horaEntrega.Substring(11)))
                                                    insertado = true;
                                                else
                                                    i++;
                                            }

                                            if (Listado.Count(p => p.codigoPedido.Equals(item.codigoPedido)) == 0)
                                            {
                                                if (!insertado)
                                                    Listado.Add(item);
                                                else
                                                    Listado.Insert(i, item);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                                toBeAdded = new List<CabeceraPedido>();

                            if (toBeDeleted != null)
                            {
                                if (toBeDeleted.Count > 0)
                                {
                                    foreach (var item in toBeDeleted)
                                    {
                                        Listado.Remove(item);
                                    }
                                }
                            }
                            else
                                toBeDeleted = new List<CabeceraPedido>();

                            if (toBeUpdated != null)
                            {
                                if (toBeUpdated.Count > 0)
                                {
                                    foreach (var item in toBeUpdated)
                                    {
                                        Listado.Where(p => p.idPedido == item.idPedido).ToList().ForEach(x =>
                                        {
                                            x.idEstadoPedido = item.idEstadoPedido;
                                            x.estadoPedido = item.estadoPedido;
                                            x.idRepartidor = item.idRepartidor;
                                            x.repartidor = item.repartidor;
                                            x.FotoRepartidor = item.FotoRepartidor;
                                            switch (item.idEstadoPedido)
                                            {
                                                case (int)EstadoPedido.Nuevo:
                                                    x.ColorPedido = "pedidonuevo.png";
                                                    x.imagenBoton = "visto.png";
                                                    break;
                                                case (int)EstadoPedido.EnProceso:
                                                    x.ColorPedido = "pedidovisto.png";
                                                    x.imagenBoton = "minutes.png";
                                                    break;
                                                case (int)EstadoPedido.PorRecoger:
                                                    x.ColorPedido = "pedidoporrecoger.png";
                                                    x.imagenBoton = "recogido.png";
                                                    break;
                                                case (int)EstadoPedido.Recogido:
                                                    x.ColorPedido = "pedidorecogido.png";
                                                    x.imagenBoton = "entrega.png";
                                                    break;
                                            }
                                        });
                                    }
                                }
                            }
                            else
                                toBeUpdated = new List<CabeceraPedido>();

                            if (toBeAdded.Count > 0 || toBeDeleted.Count > 0 || toBeUpdated.Count > 0 )
                            {

                                TotalPedidos = ListPedidosTemp.Count();
                                if (toBeAdded.Count > 0)
                                {
                                    if (toBeAdded.Count > 0)
                                    {
                                        try
                                        {
                                            // TODO: Fix audio - player.Load("Bip.mp3");
                                            // TODO: Fix audio - player.Play();
                                        }
                                        catch (Exception)
                                        {
                                            if (DeviceInfo.Platform.ToString() != "WinUI")
                                            {
                                                DependencyService.Get<IAppAudio>().PlayAudioFile("Bip.mp3");
                                            }
                                        }
                                    }
                                }
                                else if (toBeDeleted.Count > 0)
                                {
                                    try
                                    {
                                        // TODO: Fix audio - player.Load("quitado.mp3");
                                        // TODO: Fix audio - player.Play();
                                    }
                                    catch (Exception)
                                    {
                                        if (DeviceInfo.Platform.ToString() != "WinUI")
                                            DependencyService.Get<IAppAudio>().PlayAudioFile("quitado.mp3");
                                    }
                                }
                                else if (toBeUpdated.Count > 0)
                                {
                                    try
                                    {
                                        // TODO: Fix audio - player.Load("otros.mp3");
                                        // TODO: Fix audio - player.Play();
                                    }
                                    catch (Exception)
                                    {
                                        if (DeviceInfo.Platform.ToString() != "WinUI")
                                            DependencyService.Get<IAppAudio>().PlayAudioFile("otros.mp3");
                                    }
                                }
                            }

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task<bool> BotonExecute(int idPedido)
        {
            int linea = 0;
            try
            {
                string mensajeAdmin = "";
                string mensajeUsuario = "";
                string mensajeCamarero = "";
                string mensajeRepartidor = "";
                linea = 1;
                CabeceraPedido c = Listado.Where(p => p.idPedido == idPedido).FirstOrDefault<CabeceraPedido>();
                linea = 2;
                if (c == null)
                {
                    c = ListadoLocal.Where(p => p.idPedido == idPedido).FirstOrDefault<CabeceraPedido>();
                    linea = 3;
                }
                if (c != null)
                {
                    linea = 4;
                    if (c.imagenBoton.Equals("visto.png"))
                    {
                        linea = 5;
                        bool ok = false;
                        if (c.tipoVenta.Equals("Local"))
                            ok=await App.ResponseWS.cambiaEstadoPedido(idPedido, 3);
                        else
                            ok=await App.ResponseWS.cambiaEstadoPedido(idPedido, 2);
                        if (ok)
                        {
                            linea = 6;
                            mensajeUsuario = "El pedido " + c.codigoPedido + " ha sido visto por " + c.nombreEstablecimiento;
                            linea = 7;
                            if (c.tipoVenta.StartsWith("Envío"))
                                mensajeRepartidor = "El pedido " + c.codigoPedido + " ha sido visto por " + c.nombreEstablecimiento;
                            try
                            {
                                if (!c.tipoVenta.Equals("Local"))
                                {
                                    mensajeAdmin = "El pedido " + c.codigoPedido + " ha sido visto por " + c.nombreEstablecimiento;
                                    linea = 11;
                                    Establecimiento es = App.DAUtil.Usuario.establecimientos.Find(p => p.idEstablecimiento == c.idEstablecimiento);
                                    if (es.configuracion == null)
                                        es.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(es.idEstablecimiento);
                                    
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "PolloAndaluz", "OK");
                            return false;
                        }
                    }
                    else if (c.imagenBoton.Equals("minutes.png"))
                    {
                        linea = 12;
                        if (await App.ResponseWS.cambiaEstadoPedido(idPedido, 3))
                        {
                            linea = 13;
                            if (!c.tipoVenta.StartsWith("Recogida"))
                            {
                                mensajeAdmin = "En 10 minutos el cliente pasará a recoger el pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento;
                                mensajeUsuario = "Su pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " está terminando, en 10 minutos puede pasar a recogerlo.";
                            }
                            if (c.tipoVenta.StartsWith("Envío") || c.tipoVenta.Equals("Reparto Propio"))
                            {
                                linea = 14;
                                mensajeUsuario = "Su pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " está en camino, en breve lo recibirá.";
                                if (c.tipoVenta.StartsWith("Envío"))
                                {
                                    linea = 15;
                                    mensajeAdmin = "En 10 minutos puede pasar a recoger el pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento;
                                    mensajeRepartidor = "En 10 minutos puede pasar a recoger el pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento;
                                    linea = 16;
                                }
                            }
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "PolloAndaluz", "OK");
                            return false;
                        }
                    }
                    else if (c.imagenBoton.Equals("recogido.png"))
                    {
                        linea = 17;
                        bool ok = false;
                        if (!c.tipoVenta.StartsWith("Recogida"))
                            ok=await App.ResponseWS.cambiaEstadoPedido(idPedido, 4);
                        else
                           ok=await App.ResponseWS.cambiaEstadoPedido(idPedido, 5);
                        if (ok)
                        {
                            linea = 18;
                            mensajeUsuario = "En 10 minutos su pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " será recogido por el repartidor.";
                            if (c.tipoVenta.StartsWith("Envío"))
                            {
                                mensajeAdmin = "El pedido " + c.codigoPedido + " ha sido recogido de " + c.nombreEstablecimiento;
                                mensajeRepartidor = "El pedido " + c.codigoPedido + " ha sido recogido de " + c.nombreEstablecimiento;
                            }
                            else if (c.tipoVenta.Equals("Local"))
                            {
                                linea = 19;
                                Establecimiento es = App.DAUtil.Usuario.establecimientos.Where(p => p.idEstablecimiento == c.idEstablecimiento).FirstOrDefault();
                                if (es.llevaAMesa)
                                {
                                    mensajeCamarero = "El pedido " + c.codigoPedido + " para la mesa " + c.mesa + " de la zona " + c.zonaEstablecimiento + " ya puede ser recogido.";
                                    mensajeUsuario = "Su pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " ya está preparado, en breve el camarero se lo llevará a su mesa.";
                                    linea = 20;
                                }
                                else if (es.recogeEnBarra)
                                {
                                    mensajeUsuario = "Su pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " ya está preparado, puede recogerlo cuando quiera.";
                                    linea = 21;
                                }
                            }
                            else
                            {
                                linea = 22;
                                mensajeAdmin = "El pedido " + c.codigoPedido + " ha sido recogido de " + c.nombreEstablecimiento;
                                mensajeUsuario = "Ha recogido su pedido con código, " + c.codigoPedido + ", de " + c.nombreEstablecimiento + "." + Environment.NewLine + "Buen Provecho.";
                            }
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "PolloAndaluz", "OK");
                            return false;
                        }
                    }
                    else if (c.imagenBoton.Equals("entrega.png"))
                    {
                        linea = 23;
                        if (await App.ResponseWS.cambiaEstadoPedido(idPedido, 5))
                            mensajeUsuario = "Su pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " está en camino.";
                        else
                        {
                            await App.customDialog.ShowDialogAsync("Se ha producido un errro al cambiar el estado del pedido. Inténtelo de nuevo", "PolloAndaluz", "OK");
                            return false;
                        }
                    linea = 24;
                    }
                    if (!string.IsNullOrEmpty(mensajeAdmin))
                    {
                        linea = 25;
                        List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(App.EstActual.idPueblo);
                        foreach (TokensModel to in tokens)
                            App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, mensajeAdmin, to.token);
                    }
                    if (!string.IsNullOrEmpty(mensajeUsuario))
                    {
                        linea = 26;
                        string tokenUser = App.ResponseWS.getTokenUsuario(c.idUsuario);
                        App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, mensajeUsuario, tokenUser);
                    }
                    if (!string.IsNullOrEmpty(mensajeRepartidor))
                    {
                        List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(c.idEstablecimiento);
                        foreach (TokensModel to in tokens2)
                            App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, mensajeRepartidor, to.token); ;
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.Error + ex.Message + " en la linea " + linea, AppResources.App, AppResources.Cancelar);
                // 
                return false;
            }
        }
        private async Task<bool> BotonAtrasExecute(int idPedido)
        {
            int linea = 0;
            try
            {
                string mensajeAdmin = "";
                string mensajeCamarero = "";
                string mensajeRepartidor = "";
                linea = 1;
                CabeceraPedido c = Listado.Where(p => p.idPedido == idPedido).FirstOrDefault<CabeceraPedido>();
                linea = 2;
                if (c == null)
                {
                    c = ListadoLocal.Where(p => p.idPedido == idPedido).FirstOrDefault<CabeceraPedido>();
                    linea = 3;
                }
                if (c != null)
                {
                    linea = 4;
                    if (c.imagenBoton.Equals("recogido.png") || c.imagenBoton.Equals("entrega.png"))
                    {
                        bool ok = false;
                        if (c.imagenBoton.Equals("recogido.png"))
                        {
                            if (c.tipoVenta.Equals("Local"))
                                ok=await App.ResponseWS.cambiaEstadoPedido(idPedido, 3);
                            else
                                ok=await App.ResponseWS.cambiaEstadoPedido(idPedido, 2);
                        }
                        else
                            ok=await App.ResponseWS.cambiaEstadoPedido(idPedido, 3);
                        if (ok)
                        {
                            if (c.tipoVenta.StartsWith("Envío"))
                            {
                                linea = 8;
                                mensajeAdmin = "El pedido " + c.codigoPedido + " ha pasado, de nuevo, a visto por " + c.nombreEstablecimiento;
                                linea = 9;
                                mensajeRepartidor = "El pedido " + c.codigoPedido + " ha psado, de nuevo, a visto por " + c.nombreEstablecimiento;
                                linea = 10;
                            }
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "PolloAndaluz", "OK");
                        }
                    }
                    if (!string.IsNullOrEmpty(mensajeAdmin))
                    {
                        linea = 25;
                        List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(App.EstActual.idPueblo);
                        foreach (TokensModel to in tokens)
                            App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, mensajeAdmin, to.token);
                    }
                    if (!string.IsNullOrEmpty(mensajeRepartidor))
                    {
                        linea = 27;
                        if (c.idRepartidor != 0)
                        {
                            string tokenUser = App.ResponseWS.getTokenRepartidor(c.idRepartidor);
                            App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, mensajeRepartidor, tokenUser);
                        }
                        else
                        {
                            List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(c.idEstablecimiento);
                            foreach (TokensModel to in tokens2)
                                App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, mensajeRepartidor, to.token); ;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.Error + ex.Message + " en la linea " + linea, AppResources.App, AppResources.Cancelar);
                // 
                return false;
            }
        }
        private async Task initTimer()
        {
            try
            {
                App.DAUtil.pedidoNuevo = false;
                if (VerSoloHoy)
                    ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendiente()).FindAll(p => ((DateTime)p.fechaEntrega).ToString("dd/MM/yyyy").Equals(DateTime.Today.ToString("dd/MM/yyyy")));
                else
                    ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendiente());
                TotalPedidos = 0;

                if (ListPedidosTemp != null && ListPedidosTemp.Count > 0)
                {
                    foreach (CabeceraPedido item in ListPedidosTemp)
                    {
                        switch (item.idEstadoPedido)
                        {
                            case (int)EstadoPedido.Nuevo:
                                item.ColorPedido = "pedidoporrecoger.png";
                                item.imagenBoton = "recogido.png";
                                break;
                            case (int)EstadoPedido.EnProceso:
                                item.ColorPedido = "pedidovisto.png";
                                item.imagenBoton = "minutes.png";
                                break;
                            case (int)EstadoPedido.PorRecoger:
                                item.ColorPedido = "pedidoporrecoger.png";
                                item.imagenBoton = "recogido.png";
                                break;
                            case (int)EstadoPedido.Recogido:
                                item.ColorPedido = "pedidorecogido.png";
                                item.imagenBoton = "entrega.png";
                                break;
                        }
                    }
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (ListPedidosTemp.Count > 0)
                        {
                            Listado = new ObservableCollection<CabeceraPedido>(ListPedidosTemp.Where(p => !p.tipoVenta.Equals("Local")));
                            ListadoLocal = new ObservableCollection<CabeceraPedido>(ListPedidosTemp.Where(p => p.tipoVenta.Equals("Local")));
                            TotalPedidos = ListPedidosTemp.Count();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        #endregion
        #region Comandos
        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    RefreshData();

                    IsRefreshing = false;
                });
            }
        }
        public ICommand cmdVerRepartidor { get { return new Command((parametro) => VerRepartidor(parametro)); } }
        public ICommand /*IAsyncRelayCommand*/ cmdVerCliente { get { return new AsyncRelayCommand(VerCliente); } }
        public ICommand /*IAsyncRelayCommand*/ commandCerrarTodo { get { return new AsyncRelayCommand(CerrarTodo); } }
        public ICommand InfoUsuarioPedidoCommand { get { return new Command(InfoUsuarioPedido); } }
        public ICommand cmdAutoPedido { get { return new Command(VerAutoPedidoExe); } }
        public ICommand cerrarCommand { get { return new Command((parametro) => CerrarPedido(parametro)); } }
        public ICommand anularCommand { get { return new Command((parametro) => AnularPedido(parametro)); } }
        private async Task CerrarTodo()
        {
            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, "¿Está seguro de cerrar todos los pedidos?", AppResources.No, AppResources.Si);
            if (result)
            {
                App.userdialog.ShowLoading("Cerrando...", MaskType.Black);
                foreach (CabeceraPedido c in Listado)
                {
                    await CerrarPedidoexe(c);
                }
                App.userdialog.HideLoading();
                await actualizaPedidos();
            }
        }
        private async Task VerCliente()
        {
            App.userdialog.ShowLoading("Cargando...", MaskType.Black);
            await App.DAUtil.NavigationService.NavigateToAsyncMenu <CartaViewModel>();
        }
        private void CerrarPedido(object accion)
        {
            string idPedido = (string)accion;
            CabeceraPedido c = ResponseServiceWS.TraePedidoPorCodigo(idPedido);
            c.opciones = false;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaCerrarPedido, AppResources.No, AppResources.Si);

                if (result)
                {
                    await CerrarPedidoexe(c);
                    await actualizaPedidos();
                }

            });
        }
        private void AnularPedido(object accion)
        {
            string idPedido = (string)accion;
            CabeceraPedido c = ResponseServiceWS.TraePedidoPorCodigo(idPedido);
            c.opciones = false;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaAnularPedido, AppResources.No, AppResources.Si);

                if (result)
                {
                    await AnularPedidoexe(c);
                    await actualizaPedidos();
                }

            });
        }
        private async Task CerrarPedidoexe(CabeceraPedido c)
        {
            try
            {

                if (await App.ResponseWS.cambiaEstadoPedido(c.idPedido, 5))
                {

                    List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(c.idEstablecimiento);
                    foreach (TokensModel to in tokens3)
                        App.ResponseWS.enviaNotificacion(AppResources.App, "Pedido Cancelado: " + c.codigoPedido, to.token);

                    if (c.idRepartidor != 0)
                    {
                        string tokenUser = App.ResponseWS.getTokenRepartidor(c.idRepartidor);
                        App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, "Pedido Cancelado: " + c.codigoPedido, tokenUser);
                    }
                    else
                    {
                        List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(c.idEstablecimiento);
                        foreach (TokensModel to in tokens2)
                            App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, "Pedido Cancelado: " + c.codigoPedido, to.token);
                    }
                }
                else
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync("Se ha producido un errro al cambiar el estado del pedido. Inténtelo de nuevo", "PolloAndaluz", "OK");
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task AnularPedidoexe(CabeceraPedido c)
        {
            try
            {
                if (await App.ResponseWS.cambiaEstadoPedido(c.idPedido, 99))
                {

                    List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(c.idEstablecimiento);
                    foreach (TokensModel to in tokens3)
                        App.ResponseWS.enviaNotificacion(AppResources.App, "Pedido Anulado: " + c.codigoPedido, to.token);

                    if (c.idRepartidor != 0)
                    {
                        string tokenUser = App.ResponseWS.getTokenRepartidor(c.idRepartidor);
                        App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, "Pedido Anulado: " + c.codigoPedido, tokenUser);
                    }
                    else
                    {
                        List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(c.idEstablecimiento);
                        foreach (TokensModel to in tokens2)
                            App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, "Pedido Anulado: " + c.codigoPedido, to.token);
                    }
                }
                else
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync("Se ha producido un errro al cambiar el estado del pedido. Inténtelo de nuevo", "PolloAndaluz", "OK");
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        #endregion

    }


}
