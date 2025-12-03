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
using Microsoft.Maui.Controls;
using System.Text;
using AsadorMoron.Interfaces;
using Mopups.Services;

using Plugin.Maui.Audio;

using AsadorMoron.Utils;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using AsadorMoron.Recursos;
using AsadorMoron.ViewModels.Clientes;
using AsadorMoron.Views.Administrador;
using AsadorMoron.Views.Establecimientos;

using System.Diagnostics;
using AsadorMoron.Views.Repartidores;
using AsadorMoron.ViewModels.Establecimientos;

namespace AsadorMoron.ViewModels.Administrador
{
    public class HomeViewModelAdminMobile : ViewModelBase
    {
        public List<RepartidorModel> repartidores;
        public HomeViewModelAdminMobile()
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

                    if (es.configuracion.visibilidadHoras == 0)
                    {
                        VisibleFechaEntrega = true;
                        VisibleFechaPedido = false;
                        visibleDosFechas = false;
                    }
                    else if (es.configuracion.visibilidadHoras == 1)
                    {
                        VisibleFechaEntrega = false;
                        VisibleFechaPedido = true;
                        visibleDosFechas = false;
                    }
                    else if (es.configuracion.visibilidadHoras == 2)
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
                App.DAUtil.homeAdminMobile = this;
                App.DAUtil.EstoyenHome = true;
                App.DAUtil.EnTimer = true;
                ListadoRepartidores = new ObservableCollection<RepartidorModel>(App.DAUtil.GetRepartidores().Where(p => p.activo == 1 && p.idPueblo == App.DAUtil.Usuario.idPueblo).ToList());
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
                
            }
        }
        private async void VerAutoPedidoExe()
        {
            if (App.EstActual.configuracion.tipoAutoPedido == 1)
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.DAUtil.NavigationService.NavigateToAsync<AutoPedidoViewModel>();
                });
            }
            else if (App.EstActual.configuracion.tipoAutoPedido == 2)
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.DAUtil.NavigationService.NavigateToAsync<AutoPedidoMinViewModel>();
                });
            }
            else if (App.EstActual.configuracion.tipoAutoPedido == 3)
            {
                //App.DAUtil.NavigationService.NavigateToAsync<AutoPedidoViewModel>();
                try
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
                                AutoPedidoModel auto = new AutoPedidoModel();
                                auto.apellidos = "";
                                auto.codigo = App.DAUtil.GetCodigo();
                                auto.direccion = "";
                                auto.hora = DateTime.Now;
                                auto.idEstablecimiento = App.EstActual.idEstablecimiento;
                                auto.idZona = App.EstActual.idZona;
                                auto.importe = 20;
                                auto.nombre = "Auto Pedido";
                                auto.telefono = "";
                                auto.importeZona = 0;
                                int idCodigoPedido = ResponseServiceWS.NuevoAutoPedido(auto, App.EstActual.configuracion.estadoAutoPedido);
                                if (idCodigoPedido > 0)
                                {
                                    string pedido = string.Empty;
                                    List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(App.EstActual.idEstablecimiento);
                                    foreach (TokensModel to in tokens3)
                                        App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido: " + auto.codigo, to.token);

                                    List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(App.EstActual.idPueblo);
                                    foreach (TokensModel to in tokens)
                                        App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + auto.codigo, to.token);

                                    List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(App.EstActual.idEstablecimiento);
                                    foreach (TokensModel to in tokens2)
                                        App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + auto.codigo, to.token);
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
                catch (Exception ex)
                {
                    
                    await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
                }
                finally
                {
                    App.userdialog.HideLoading();
                }
            }
        }
        public async Task actualizaPedidos(bool esMensaje = false, string mensaje = "")
        {
            try
            {
                if (esMensaje)
                {
                    await App.customDialog.ShowDialogAsync(mensaje, "AsadorMoron", "Cerrar");
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

                            if (toBeAdded.Count > 0 || toBeDeleted.Count > 0 || toBeUpdated.Count > 0)
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
                if (c != null)
                {
                    linea = 4;
                    if (c.imagenBoton.Equals("visto.png"))
                    {
                        linea = 5;
                        bool ok = false;
                        ok = await App.ResponseWS.cambiaEstadoPedido(idPedido, 2);
                        if (ok)
                        {
                            linea = 6;
                            mensajeUsuario = "El pedido " + c.codigoPedido + " ha sido visto por " + c.nombreEstablecimiento;
                            linea = 7;
                            if (c.tipoVenta.StartsWith("Envío"))
                                mensajeRepartidor = "El pedido " + c.codigoPedido + " ha sido visto por " + c.nombreEstablecimiento;
                            try
                            {
                                mensajeAdmin = "El pedido " + c.codigoPedido + " ha sido visto por " + c.nombreEstablecimiento;
                                    linea = 11;
                                    Establecimiento es = App.DAUtil.Usuario.establecimientos.Find(p => p.idEstablecimiento == c.idEstablecimiento);
                                    if (es.configuracion == null)
                                        es.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(es.idEstablecimiento);
                                    
                            }
                            catch (Exception)
                            {

                            }
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
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
                            await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
                            return false;
                        }
                    }
                    else if (c.imagenBoton.Equals("recogido.png"))
                    {
                        linea = 17;
                        bool ok = false;
                        if (!c.tipoVenta.StartsWith("Recogida"))
                            ok = await App.ResponseWS.cambiaEstadoPedido(idPedido, 4);
                        else
                            ok = await App.ResponseWS.cambiaEstadoPedido(idPedido, 5);
                        if (ok)
                        {
                            linea = 18;
                            mensajeUsuario = "En 10 minutos su pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " será recogido por el repartidor.";
                            if (c.tipoVenta.StartsWith("Envío"))
                            {
                                mensajeAdmin = "El pedido " + c.codigoPedido + " ha sido recogido de " + c.nombreEstablecimiento;
                                mensajeRepartidor = "El pedido " + c.codigoPedido + " ha sido recogido de " + c.nombreEstablecimiento;
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
                            await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
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
                            await App.customDialog.ShowDialogAsync("Se ha producido un errro al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
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
                
                return false;
            }
        }
        private async Task<bool> BotonAtrasExecute(int idPedido)
        {
            int linea = 0;
            try
            {
                string mensajeAdmin = "";
                string mensajeRepartidor = "";
                linea = 1;
                CabeceraPedido c = Listado.Where(p => p.idPedido == idPedido).FirstOrDefault<CabeceraPedido>();
                if (c != null)
                {
                    linea = 4;
                    if (c.imagenBoton.Equals("recogido.png") || c.imagenBoton.Equals("entrega.png"))
                    {
                        bool ok = false;
                        if (c.imagenBoton.Equals("recogido.png"))
                        {
                            ok = await App.ResponseWS.cambiaEstadoPedido(idPedido, 2);
                        }
                        else
                            ok = await App.ResponseWS.cambiaEstadoPedido(idPedido, 3);
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
                            await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
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
                            TotalPedidos = ListPedidosTemp.Count();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                
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
        public ICommand InfoUsuarioPedidoCommand { get { return new Command(InfoUsuarioPedido); } }
        public ICommand cmdAutoPedido { get { return new Command(VerAutoPedidoExe); } }
        public ICommand cerrarCommand { get { return new Command((parametro) => CerrarPedido(parametro)); } }
        public ICommand anularCommand { get { return new Command((parametro) => AnularPedido(parametro)); } }
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
                    await App.customDialog.ShowDialogAsync("Se ha producido un errro al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
                }
            }
            catch (Exception ex)
            {
                
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
                    await App.customDialog.ShowDialogAsync("Se ha producido un errro al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        #endregion

    }


}

/*int resultadoEstadoImpresora;
private IUserDialogs dialogHomeAdmin = App.userdialog;
private List<CabeceraPedido> ListPedidosTemp;
private IAudioPlayer player = null; // Initialized when needed

public bool l_checkEstado = false;

private bool cambiaPueblo;
public bool CambiaPueblo
{
    get
    {
        return cambiaPueblo;
    }
    set
    {
        if (cambiaPueblo != value)
        {
            cambiaPueblo = value;
            OnPropertyChanged(nameof(CambiaPueblo));
        }
    }
}
private ObservableCollection<PuebloBindableModel> listaPueblos;
public ObservableCollection<PuebloBindableModel> ListaPueblos
{
    get
    {
        return listaPueblos;
    }
    set
    {
        if (listaPueblos != value)
        {
            listaPueblos = value;
            OnPropertyChanged(nameof(ListaPueblos));
        }
    }
}

private PueblosModel puebloSeleccionado;

public PueblosModel PuebloSeleccionado
{
    get { return puebloSeleccionado; }
    set
    {
        if (value != null)
        {
            puebloSeleccionado = value;
            OnPropertyChanged(nameof(PuebloSeleccionado));
            App.DAUtil.Usuario.idPueblo = PuebloSeleccionado.id;
            Preferences.Set("idPueblo", PuebloSeleccionado.id);
            App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black);
            Task.Run(async () =>
            {
                await Inicia().ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                {
                    CambiaPueblo = false;
                    App.userdialog.HideLoading();
                }));

            });

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
            if (iniciado)
                MainThread.BeginInvokeOnMainThread(async () => await initTimer());
        }
    }
}

public HomeViewModelAdminMobile()
{
    iniciado = false;
    if (App.DAUtil.NotificacionPantalla.Equals(""))
    {
        if (App.userdialog == null)
        {
            try { dialogHomeAdmin.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
        }
    }
}
private async Task Inicia()
{
    if (App.DAUtil.impresora == null)
        App.GetConnectedDevices();
    if (App.userdialog == null)
    {
        try { dialogHomeAdmin.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
    }
    App.EstActual = App.MiEst;
    App.DAUtil.homeAdminMobile = this;
    App.DAUtil.EstoyenHome = true;
    App.DAUtil.EnTimer = true;
    VerSoloHoy = Preferences.Get("VerSoloHoy", true);
    await initTimer();



    App.DAUtil.Usuario.platform = DeviceInfo.Platform.ToString().ToLower();
    App.DAUtil.Usuario.token = App.DAUtil.InstallId.ToString();
    if (DeviceInfo.Platform.ToString() == "iOS")
        App.DAUtil.Usuario.version = App.DAUtil.versioniOS;
    else if (DeviceInfo.Platform.ToString() == "Android")
        App.DAUtil.Usuario.version = App.DAUtil.versionAndroid;


    App.ResponseWS.RegistraTokenFCM(App.DAUtil.Usuario);
    try
    {
        if (DeviceInfo.Platform.ToString() != "WinUI")
        {
            if (App.DAUtil.impresora != null)
            {
                if (!App.DAUtil.impresora.IsConnected())
                {
                    if (App._centralManager.IsScanning)
                        App._centralManager.StopScan();
                    App.GetDeviceList();
                }
            }
            else
            {
                if (App._centralManager.IsScanning)
                    App._centralManager.StopScan();
                App.GetDeviceList();
            }
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine(ex.Message);
    }
}
private async void GetNumeroUsuarios()
{
    UsuariosOnline=await App.ResponseWS.getUsuariosOnline();
}
public bool iniciado = false;
public override async Task InitializeAsync(object navigationData)
{

    try
    {
        App.autoPedidoAdmin = null;
        iniciado = false;
        if (App.DAUtil.homeAdminMobile!=null)
            App.DAUtil.homeAdminMobile.iniciado = false;
        GetNumeroUsuarios();
        CambiaPueblo = false;
        var temp= new ObservableCollection<PuebloBindableModel>();
        if (App.listaOriginalPueblos != null && App.listaOriginalPueblos.Any())
        {
            if (App.listaOriginalPueblos.Count() > 1)
                IsMultiAdmin = true;
            else
                IsMultiAdmin = false;

            foreach (PueblosModel p in App.listaOriginalPueblos)
            {
                PuebloBindableModel p2 = new PuebloBindableModel();
                p2.pueblo = p;
                p2.seleccionado = Preferences.Get(p.textoPueblo, true);

                temp.Add(p2);
            }
        }

        ListaPueblos = temp;
        ListadoRepartidores = new ObservableCollection<RepartidorBindableModel>(App.DAUtil.GetRepartidoresBindables().Where(t => t.activo == 1 && ListaPueblos.Any(p => p.pueblo.id == t.idPueblo && p.seleccionado)).ToList());

        await Inicia().ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
        {
            App.userdialog.HideLoading();
            iniciado = true;
            if (App.DAUtil.homeAdminMobile != null)
                App.DAUtil.homeAdminMobile.iniciado = true;
        }));

        await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
        {
            App.userdialog.HideLoading();
        }));
    }
    catch (Exception ex)
    {
        
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

private bool isMultiAdmin;

public bool IsMultiAdmin
{
    get { return isMultiAdmin; }
    set { 
        isMultiAdmin = value;
        OnPropertyChanged();
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
private int usuariosOnline;
public int UsuariosOnline
{
    get { return usuariosOnline; }
    set
    {
        if (usuariosOnline != value)
        {
            usuariosOnline = value;
            OnPropertyChanged(nameof(UsuariosOnline));
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


private ObservableCollection<RepartidorBindableModel> listadoRepartidores;
public ObservableCollection<RepartidorBindableModel> ListadoRepartidores
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
private ObservableCollection<LineasPedido> lineaPedido;
public ObservableCollection<LineasPedido> LineaPedido
{
    get { return lineaPedido; }
    set
    {
        lineaPedido = value;
        OnPropertyChanged(nameof(LineaPedido));
    }
}
private decimal itemsSummary;
public decimal ItemsSummary
{
    get { return itemsSummary; }
    set
    {
        itemsSummary = value;
        OnPropertyChanged(nameof(ItemsSummary));
    }
}
private ObservableCollection<LineasPedido> lineaPedidoAdd;
public ObservableCollection<LineasPedido> LineaPedidoAdd
{
    get { return lineaPedidoAdd; }
    set
    {
        lineaPedidoAdd = value;
        OnPropertyChanged(nameof(LineaPedidoAdd));
    }
}
private Color btnColorProceso;
public Color BtnColorProceso
{
    get { return btnColorProceso; }
    set
    {
        btnColorProceso = value;
        OnPropertyChanged(nameof(BtnColorProceso));
    }
}
private Color btnColorEnviado;
public Color BtnColorEnviado
{
    get { return btnColorEnviado; }
    set
    {
        btnColorEnviado = value;
        OnPropertyChanged(nameof(BtnColorEnviado));
    }
}
private Color btnColorPendiente;
public Color BtnColorPendiente
{
    get { return btnColorPendiente; }
    set
    {
        btnColorPendiente = value;
        OnPropertyChanged(nameof(BtnColorPendiente));
    }
}
private Color bgPedidoSeleccionado;
public Color BgPedidoSeleccionado
{
    get { return bgPedidoSeleccionado; }
    set
    {
        bgPedidoSeleccionado = value;
        OnPropertyChanged(nameof(BgPedidoSeleccionado));
    }
}
private string textoBusquedaPueblo = "";
public string TextoBusquedaPueblo
{
    get { return textoBusquedaPueblo; }
    set
    {
        if (textoBusquedaPueblo != value)
        {
            textoBusquedaPueblo = value;
            OnPropertyChanged(nameof(TextoBusquedaPueblo));
            //FiltrarPueblo().ConfigureAwait(false);
        }
    }
}
#endregion

#region comandos
public ICommand RepartidorCommand { get { return new Command((parametro) => Boton(parametro)); } }
public ICommand cmdVerRepartidor { get { return new Command((parametro) => VerRepartidor(parametro)); } }
public ICommand cmdVerCliente { get { return new Command(VerClienteExe); } }
public ICommand cmdCambiaPueblo { get { return new Command(CambiaPuebloExe); } }
public ICommand InfoUsuarioCommand { get { return new Command(InfoUsuario); } }
public ICommand InfoPedidoCommand { get { return new Command(InfoPedido); } }
// public ICommand BtnFiltrarPueblo { get { return new DelegateCommandAsync(FiltrarPueblo); } }
public ICommand InfoUsuarioPedidoCommand { get { return new Command(InfoUsuarioPedido); } }
public ICommand PrintCommand { get { return new Command(Print); } }
public ICommand cerrarCommand { get { return new Command((parametro) => CerrarPedido(parametro)); } }
public ICommand anularCommand { get { return new Command((parametro) => AnularPedido(parametro)); } }
public ICommand opcionesCommand { get { return new Command((parametro) => OpcionesPedido(parametro)); } }
public ICommand cmdAutoPedido { get { return new Command(VerAutoPedidoExe); } }
#endregion

#region Metodos
private async void VerAutoPedidoExe()
{
    try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await App.DAUtil.NavigationService.NavigateToAsync<AutoPedidoAdminViewModel>();
        });

}
//public async Task FiltrarPueblo()
//{
//    try
//    {
//        await Task.Run(() =>
//        {
//            try
//            {
//                MainThread.BeginInvokeOnMainThread(() =>
//                {
//                    string paraBuscar = "";
//                    if (!string.IsNullOrEmpty(TextoBusquedaPueblo) && listaOriginalPueblos.Count > 0)
//                    {
//                        paraBuscar = TextoBusquedaPueblo.ToUpper();
//                        ListaPueblos = new ObservableCollection<PuebloBindableModel>(listaOriginalPueblos.Where(p => p.nombre.ToUpper().Contains(paraBuscar)));
//                    }
//                    else
//                        ListaPueblos = new ObservableCollection<PuebloBindableModel>(listaOriginalPueblos);
//                });
//            }
//            catch (Exception)
//            {
//                App.userdialog.HideLoading();
//            }
//        });
//    }
//    catch (Exception ex)
//    {
//        
//    }
//}
private void InfoUsuarioPedido(object codigo)
{
    try
    {
        if (MopupService.Instance.PopupStack.Count() == 0)
        {
            string cod = (string)codigo;
            CabeceraPedido c2 = ResponseServiceWS.TraePedidoPorCodigo(cod);
            c2.opciones = false;
            MopupService.Instance.PushAsync(new PopupPageInfoUsuarioPedido(c2), true);
        }
    }
    catch (Exception ex)
    {
        
    }
}
private void VerRepartidor(object repartidor)
{

    try
    {
        if (MopupService.Instance.PopupStack.Count() == 0)
        {
            RepartidorBindableModel r = (RepartidorBindableModel)repartidor;
            RepartidorModel r2 = App.DAUtil.GetRepartidores().Find(p => p.id == r.id);
            MopupService.Instance.PushAsync(new PopupPageInfoRepartidor(r2), true);
        }
    }
    catch (Exception ex)
    {
        
    }
}
private void VerClienteExe()
{
    App.DAUtil.NavigationService.NavigateToAsyncMenu<CartaViewModel>(App.Establecimiento);

}
private void CambiaPuebloExe()
{
    if (ListaPueblos.Count>1)
        CambiaPueblo = !CambiaPueblo;
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
private void OpcionesPedido(object accion)
{
    CabeceraPedido c = (CabeceraPedido)accion;
    c.opciones = !c.opciones;
}

private void Boton(object accion)
{
    try
    {
        if (MopupService.Instance.PopupStack.Count() == 0)
        {
            try { App.userdialog.ShowLoading(AppResources.Procesando); } catch (Exception) { }
            string idPedido = (string)accion;
            CabeceraPedido pedido = Listado.Where(p => p.codigoPedido.Equals(idPedido)).FirstOrDefault<CabeceraPedido>();

            MopupService.Instance.PushAsync(new PopupPageRepartidores(pedido)).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
            {
                CabeceraPedido c = Listado.Where(p => p.codigoPedido.Equals(idPedido)).FirstOrDefault<CabeceraPedido>();

                if (c != null)
                {
                    c.repartidor = 1;
                    c.opciones = false;
                    ActualizarRepartidores();
                }
                App.userdialog.HideLoading();
            }));
        }
    }
    catch (Exception ex)
    {
        
    }
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
            await App.customDialog.ShowDialogAsync("Se ha producido un errro al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
        }
    }
    catch (Exception ex)
    {
        
    }
}
private async Task AnularPedidoexe(CabeceraPedido c)
{
    try
    {
        if (await App.ResponseWS.cambiaEstadoPedido(c.idPedido, 99)) {

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
            await App.customDialog.ShowDialogAsync("Se ha producido un errro al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
        }
    }
    catch (Exception ex)
    {
        
    }
}
public async Task actualizaPedidos(bool esMensaje = false, string mensaje = "")
{
    try
    {
        GetNumeroUsuarios();
        if (esMensaje)
        {
            await App.customDialog.ShowDialogAsync(mensaje, "AsadorMoron", "Cerrar");
        }
        else
        {
            ListPedidosTemp = new List<CabeceraPedido>();
            string ids = "";
            foreach (PuebloBindableModel p in ListaPueblos)
            {
                if (p.seleccionado)
                {
                    if (!ids.Equals(""))
                        ids += ",";
                    ids += p.pueblo.id;
                }
            }
            ListadoRepartidores = new ObservableCollection<RepartidorBindableModel>(App.DAUtil.GetRepartidoresBindables().Where(t => t.activo == 1  && ListaPueblos.Any(p => p.pueblo.id == t.idPueblo && p.seleccionado)).ToList());

            if (VerSoloHoy)
                ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendienteMultiAdmin(ids)).FindAll(p => ((DateTime)p.fechaEntrega).ToString("dd/MM/yyyy").Equals(DateTime.Today.ToString("dd/MM/yyyy")));
            else
                ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendienteMultiAdmin(ids));
            if (Listado == null)
                Listado = new ObservableCollection<CabeceraPedido>();
            if (ListPedidosTemp != null)
            {
                List<CabeceraPedido> toBeAdded = ListPedidosTemp.Where(c => !Listado.Any(d => c.idPedido == d.idPedido)).ToList();
                List<CabeceraPedido> toBeDeleted = Listado.Where(c => !ListPedidosTemp.Any(d => c.idPedido == d.idPedido)).ToList();
                List<CabeceraPedido> toBeUpdated = ListPedidosTemp.Where(c => Listado.Any(d => c.idPedido == d.idPedido && (c.idEstadoPedido != d.idEstadoPedido || c.idRepartidor != d.idRepartidor))).ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (toBeAdded != null)
                    {
                        if (toBeAdded.Count > 0)
                        {
                            foreach (var item in toBeAdded)
                            {
                                if (Listado.Where(p => p.codigoPedido.Equals(item.codigoPedido)).ToList().Count == 0)
                                {
                                    switch (item.idEstadoPedido)
                                    {
                                        case (int)EstadoPedido.Nuevo:
                                            item.ColorPedido = "pedidonuevo.png";
                                            break;
                                        case (int)EstadoPedido.EnProceso:
                                            item.ColorPedido = "pedidovisto.png";
                                            break;
                                        case (int)EstadoPedido.PorRecoger:
                                            item.ColorPedido = "pedidoporrecoger.png";
                                            break;
                                        case (int)EstadoPedido.Recogido:
                                            item.ColorPedido = "pedidorecogido.png";
                                            break;
                                        case (int)EstadoPedido.Entregado:
                                            item.ColorPedido = "pedidoentregado.png";
                                            break;
                                    }
                                //item.ColorPedido = "pedidonuevo.png";

                                //Listado.Add(item);
                                int i = 0;
                                    bool insertado = false;
                                    foreach (CabeceraPedido cab in Listado)
                                    {
                                        if (TimeSpan.Parse(cab.horaEntrega.Substring(11)) > TimeSpan.Parse(item.horaEntrega.Substring(11)))
                                            insertado = true;
                                        else
                                            i++;
                                    }

                                    if (!insertado)
                                        Listado.Add(item);
                                    else
                                        Listado.Insert(i, item);
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
                                            break;
                                        case (int)EstadoPedido.EnProceso:
                                            x.ColorPedido = "pedidovisto.png";
                                            break;
                                        case (int)EstadoPedido.PorRecoger:
                                            x.ColorPedido = "pedidoporrecoger.png";
                                            break;
                                        case (int)EstadoPedido.Recogido:
                                            x.ColorPedido = "pedidorecogido.png";
                                            break;
                                        case (int)EstadoPedido.Entregado:
                                            x.ColorPedido = "pedidoentregado.png";
                                            break;
                                    }
                                }); ;
                            }
                        }
                    }
                    else
                        toBeUpdated = new List<CabeceraPedido>();

                    if (toBeAdded.Count > 0 || toBeDeleted.Count > 0 || toBeUpdated.Count > 0)
                    {

                        TotalPedidos = ListPedidosTemp.Count();
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
                await ActualizarRepartidores();
            }
            else
                
        }
    }
    catch (Exception ex)
    {
        
    }
}
private async Task ActualizarRepartidores()
{
    try
    {
        if (Listado != null)
        {
            var list = Listado?.GroupBy(p => p.idRepartidor);
            if (list != null && list.Count() > 0)
            {
                foreach (RepartidorBindableModel r in ListadoRepartidores)
                {
                    r.Cantidad = Listado.Where(p2 => p2.idRepartidor == r.id).Count();
                }


                ListadoRepartidores = new ObservableCollection<RepartidorBindableModel>(ListadoRepartidores.OrderByDescending(r => r.Cantidad));

            }
        }
    }
    catch (Exception)
    {

    }
}

private async Task initTimer()
{
    try
    {
        App.DAUtil.pedidoNuevo = false;
        ListPedidosTemp = new List<CabeceraPedido>();
        List<CabeceraPedido> cab;
        string ids = "";
        foreach (PuebloBindableModel p in ListaPueblos)
        {
            if (p.seleccionado)
            {
                if (!ids.Equals(""))
                    ids += ",";
                ids += p.pueblo.id;
            }
        }
        if (VerSoloHoy)
            cab = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendienteMultiAdmin(ids)).FindAll(p => ((DateTime)p.fechaEntrega).ToString("dd/MM/yyyy").Equals(DateTime.Today.ToString("dd/MM/yyyy")));
        else
            cab = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendienteMultiAdmin(ids));

        foreach (CabeceraPedido c in cab)
        {
            if (ListPedidosTemp.Where(p => p.codigoPedido.Equals(c.codigoPedido)).ToList().Count == 0)
                ListPedidosTemp.Add(c);
        }

        TotalPedidos = 0;
        if (ListPedidosTemp != null)
        {
            foreach (CabeceraPedido item in ListPedidosTemp)
            {
                switch (item.idEstadoPedido)
                {
                    case (int)EstadoPedido.Nuevo:
                        item.ColorPedido = "pedidonuevo.png";
                        break;
                    case (int)EstadoPedido.EnProceso:
                        item.ColorPedido = "pedidovisto.png";
                        break;
                    case (int)EstadoPedido.PorRecoger:
                        item.ColorPedido = "pedidoporrecoger.png";
                        break;
                    case (int)EstadoPedido.Recogido:
                        item.ColorPedido = "pedidorecogido.png";
                        break;
                    case (int)EstadoPedido.Entregado:
                        item.ColorPedido = "pedidoentregado.png";
                        break;
                }

            }
            MainThread.BeginInvokeOnMainThread(() =>
            {
                //if (ListPedidosTemp.Count > 0)
                //{
                Listado = new ObservableCollection<CabeceraPedido>(ListPedidosTemp);
                TotalPedidos = ListPedidosTemp.Count();
                //}
                ActualizarRepartidores().ConfigureAwait(false);
            });

        }
    }
    catch (Exception ex)
    {
        
    }
}
private void InfoPedido(object codigo)
{
    try
    {
        if (MopupService.Instance.PopupStack.Count() == 0)
        {
            string cod = (string)codigo;
            CabeceraPedido c2 = ResponseServiceWS.TraePedidoPorCodigo(cod);
            c2.opciones = false;
            MopupService.Instance.PushAsync(new PopupPageInfoPedido(c2.lineasPedidos), true);
        }
    }
    catch (Exception ex)
    {
        
    }
}
private void InfoUsuario(object codigo)
{
    try
    {
        if (MopupService.Instance.PopupStack.Count() == 0)
        {
            string cod = (string)codigo;
            CabeceraPedido c2 = ResponseServiceWS.TraePedidoPorCodigo(cod);
            c2.opciones = false;
            ZonaModel z = App.DAUtil.getZonas().Find(p => p.idZona == c2.idZona);
            string nombreZona = "";
            if (z != null)
                nombreZona = z.nombre;
            MopupService.Instance.PushAsync(new PopupPageInfoUsuario(c2.nombreUsuario, c2.emailUsuario, c2.telefonoUsuario, c2.direccionUsuario, nombreZona), true);
        }
    }
    catch (Exception ex)
    {
        
    }
}
private void Activar(bool hacer)
{
    try
    {
        if (hacer)
        {
            App.DAUtil.NavigationService.NavigateToAsyncMenu<ConfigurarImpresoraViewModel>();
        }
        else
        {
            App.userdialog.HideLoading();
        }
    }
    catch (Exception ex)
    {
        
        App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
    }
}
private async void Print(Object codigo)
{
    try
    {
        if (CheckImpresora())
        {
            App.GetDeviceList();
            ConfiguracionAdmin cadmin = ResponseServiceWS.getConfiguracionAdmin();
            App.DAUtil.ImprimirBT(codigo, 1,cadmin.ticketSize);

        }
        else
        {
            bool result;
            switch (resultadoEstadoImpresora)
            {
                case 1:
                    result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaImpresoraNoConectada, AppResources.No, AppResources.Si);
                    Activar(result);
                    break;
                case 2:
                    await App.Current.MainPage.DisplayAlert(AppResources.Permisos, AppResources.EnciendeBT, AppResources.OK);
                    Activar(true);
                    break;
                case 3:
                    result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaImpresoraNoConectada, AppResources.No, AppResources.Si);
                    Activar(result);
                    break;
                default:
                    break;
            }
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error Imprimir ticket: {ex.ToString()}");
    }
}
private bool CheckImpresora()
{
    resultadoEstadoImpresora = 0;

    if (App.DAUtil?.impresora == null)
    {
        resultadoEstadoImpresora = 1; // -- Impresora no conectada
        return false;
    }

    if (App._centralManager.Status != AccessState.Available)
    {
        resultadoEstadoImpresora = 2; // -- Bluetooth no conectado
        return false;
    }

    if (!App.DAUtil.impresora.Status.Equals(ConnectionState.Connected))
    {
        resultadoEstadoImpresora = 3;
    }

    return true;
}
#endregion
}
}
*/