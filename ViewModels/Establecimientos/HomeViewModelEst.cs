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
using Mopups.Services;
// 
using Plugin.Maui.Audio;
using AsadorMoron.Utils;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using AsadorMoron.ViewModels.Clientes;
using AsadorMoron.Views.Administrador;
// using AsadorMoron.Print; // TODO: Reimplementar
using System.Diagnostics;
using System.Timers;
using AsadorMoron.Recursos;
using AsadorMoron.Managers;
using AsadorMoron.Interfaces;
using CommunityToolkit.Mvvm.Input;
using static AsadorMoron.Managers.ManagerImpresora;
using AsadorMoron.Views.Repartidores;
using AsadorMoron.ViewModels.Administrador;
using AsadorMoron.Models.PayComet;

namespace AsadorMoron.ViewModels.Establecimientos
{
    public class HomeViewModelEst : ViewModelBase
    {
        public List<RepartidorModel> repartidores;
        public HomeViewModelEst()
        {
            if (App.DAUtil.NotificacionPantalla.Equals(""))
            {
                if (App.userdialog == null)
                {
                    try { dialogHomeAdmin.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
            }
            //Listado = ResponseServiceWS.getPedidosPendientesTraido();
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
                    nombreImpresora = es.configuracion.nombreImpresora;
                    nombreImpresora2 = es.configuracion.nombreImpresora2;
                    nombreImpresora3 = es.configuracion.nombreImpresora3;
                    nombreImpresora4 = es.configuracion.nombreImpresora4;
                    nombreImpresora5 = es.configuracion.nombreImpresora5;
                    nombreImpresora6 = es.configuracion.nombreImpresora6;
                    nombreImpresora7 = es.configuracion.nombreImpresora7;
                    nombreImpresora8 = es.configuracion.nombreImpresora8;
                    nombreImpresora9 = es.configuracion.nombreImpresora9;
                    nombreImpresora10 = es.configuracion.nombreImpresora10;
                    alturaLinea = es.configuracion.alturaLineaImpresora;

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
                App.DAUtil.EstoyenHome = true;
                App.DAUtil.EnTimer = true;
                ListadoRepartidores = new ObservableCollection<RepartidorModel>(App.DAUtil.GetRepartidores().Where(p => p.activo == 1 && p.idPueblo == App.DAUtil.Usuario.idPueblo).ToList());
                IsVisibleRepartidores = ListadoRepartidores.Count > 0;
                await initTimer();
                App.timer = new System.Timers.Timer();
                App.timer.Interval = 8000;
                App.timer.Elapsed += _timer_Elapsed;
                App.timer.Start();

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
        private string nombreImpresora;
        private string nombreImpresora2;
        private string nombreImpresora3;
        private string nombreImpresora4;
        private string nombreImpresora5;
        private string nombreImpresora6;
        private string nombreImpresora7;
        private string nombreImpresora8;
        private string nombreImpresora9;
        private string nombreImpresora10;
        private int alturaLinea;
        private IUserDialogs dialogHomeAdmin = App.userdialog;
        public List<CabeceraPedido> ListPedidosTemp;
        IAudioPlayer player = null; // Initialized when needed
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
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await actualizaPedidos();
                Console.WriteLine(DateTime.Now);
            });

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
        private void VerClienteExe()
        {
            App.DAUtil.NavigationService.NavigateToAsyncMenu<CartaViewModel>();
            
        }
        public async Task actualizaPedidos()
        {
            try
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
                    List<CabeceraPedido> toBeUpdated = ListPedidosTemp.Where(c => !c.tipoVenta.Equals("Local") && Listado.Any(d => c.idPedido == d.idPedido && (c.idEstadoPedido != d.idEstadoPedido))).ToList();


                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (toBeAdded != null)
                        {
                            if (toBeAdded.Count > 0)
                            {
                                foreach (var item in toBeAdded)
                                {
                                    item.ColorPedido = "pedidoporrecoger.png";
                                    item.imagenBoton = "recogido.png";
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
                                    if (item.idEstadoPedido == 2)
                                    {
                                        await Print(item.codigoPedido, TipoImpresion.Normal);
                                        App.ResponseWS.cambiaEstadoPedido(item.idPedido, 3);
                                        if (item.idRepartidor != 0)
                                        {
                                            string tokenUser = App.ResponseWS.getTokenRepartidor(item.idRepartidor);
                                            App.ResponseWS.enviaNotificacion(item.nombreEstablecimiento, "Pedido En proceso: " + item.codigoPedido, tokenUser);
                                        }
                                        else
                                        {
                                            List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(item.idEstablecimiento);
                                            foreach (TokensModel to in tokens2)
                                                App.ResponseWS.enviaNotificacion(item.nombreEstablecimiento, "Pedido en proceso: " + item.codigoPedido, to.token);
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
                                            case 2:
                                                item.ColorPedido = "pedidoporrecoger.png";
                                                item.imagenBoton = "recogido.png";
                                                Print(item.codigoPedido, TipoImpresion.Normal);
                                                App.ResponseWS.cambiaEstadoPedido(item.idPedido, 3);
                                                if (item.idRepartidor != 0)
                                                {
                                                    string tokenUser = App.ResponseWS.getTokenRepartidor(item.idRepartidor);
                                                    App.ResponseWS.enviaNotificacion(item.nombreEstablecimiento, "Pedido En proceso: " + item.codigoPedido, tokenUser);
                                                }
                                                else
                                                {
                                                    List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(item.idEstablecimiento);
                                                    foreach (TokensModel to in tokens2)
                                                        App.ResponseWS.enviaNotificacion(item.nombreEstablecimiento, "Pedido en proceso: " + item.codigoPedido, to.token);
                                                }
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

                        if (toBeDeleted.Count > 0 || toBeUpdated.Count > 0)
                        {

                            TotalPedidos = ListPedidosTemp.Count();
                            if (toBeDeleted.Count > 0 )
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
                            else if (toBeUpdated.Count > 0 )
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
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task EnvioNotificacionesPedido(string mensajeAdmin, string mensajeUsuario, string mensajeCamarero, string mensajeRepartidor, CabeceraPedido c)
        {
            if (!string.IsNullOrEmpty(mensajeAdmin))
            {
                List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(App.EstActual.idPueblo);
                foreach (TokensModel to in tokens)
                   await App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, mensajeAdmin, to.token);
            }
            if (!string.IsNullOrEmpty(mensajeUsuario))
            {
                string tokenUser = App.ResponseWS.getTokenUsuario(c.idUsuario);
                await App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, mensajeUsuario, tokenUser);
            }
            if (!string.IsNullOrEmpty(mensajeRepartidor))
            {
                if (c.idRepartidor != 0)
                {
                    string tokenUser = App.ResponseWS.getTokenRepartidor(c.idRepartidor);
                    await App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, mensajeRepartidor, tokenUser);
                }
                else
                {
                    List<TokensModel> tokens2 =  App.ResponseWS.getTokenRepartidores(c.idEstablecimiento);
                    foreach (TokensModel to in tokens2)
                        await App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, mensajeRepartidor, to.token); ;
                }
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
                            case 2:
                                item.ColorPedido = "pedidoporrecoger.png";
                                item.imagenBoton = "recogido.png";
                                await Print(item.codigoPedido, TipoImpresion.Normal);
                                App.ResponseWS.cambiaEstadoPedido(item.idPedido, 3);
                                if (item.idRepartidor != 0)
                                {
                                    string tokenUser = App.ResponseWS.getTokenRepartidor(item.idRepartidor);
                                    App.ResponseWS.enviaNotificacion(item.nombreEstablecimiento, "Pedido En proceso: " + item.codigoPedido, tokenUser);
                                }
                                else
                                {
                                    List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(item.idEstablecimiento);
                                    foreach (TokensModel to in tokens2)
                                        App.ResponseWS.enviaNotificacion(item.nombreEstablecimiento, "Pedido en proceso: " + item.codigoPedido, to.token);
                                }
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
        private async Task PrePrint(string codigo)
        {
            var c = Listado.Where(p => p.codigoPedido == codigo).FirstOrDefault();
            if(c != null)
                await Print(codigo, TipoImpresion.Normal);
            else
                await Print(codigo, TipoImpresion.Mesa);
        }
        private async Task Print(string codigo, TipoImpresion tipo, int veces = 1, bool mostrarMensaje = true)
        {
            try
            {
                    if (!string.IsNullOrEmpty(nombreImpresora))
                        ManagerImpresora.ImprimirTicket(codigo, nombreImpresora, alturaLinea, veces, tipo, 1);
                    if (!string.IsNullOrEmpty(nombreImpresora2))
                        ManagerImpresora.ImprimirTicket(codigo, nombreImpresora2, alturaLinea, veces, tipo, 2);
                    if (!string.IsNullOrEmpty(nombreImpresora3))
                        ManagerImpresora.ImprimirTicket(codigo, nombreImpresora3, alturaLinea, veces, tipo, 3);
                    if (!string.IsNullOrEmpty(nombreImpresora4))
                        ManagerImpresora.ImprimirTicket(codigo, nombreImpresora4, alturaLinea, veces, tipo, 4);
                    if (!string.IsNullOrEmpty(nombreImpresora5))
                        ManagerImpresora.ImprimirTicket(codigo, nombreImpresora5, alturaLinea, veces, tipo, 5);
                    if (!string.IsNullOrEmpty(nombreImpresora6))
                        ManagerImpresora.ImprimirTicket(codigo, nombreImpresora6, alturaLinea, veces, tipo, 6);
                    if (!string.IsNullOrEmpty(nombreImpresora7))
                        ManagerImpresora.ImprimirTicket(codigo, nombreImpresora7, alturaLinea, veces, tipo, 7);
                    if (!string.IsNullOrEmpty(nombreImpresora8))
                        ManagerImpresora.ImprimirTicket(codigo, nombreImpresora8, alturaLinea, veces, tipo, 8);
                    if (!string.IsNullOrEmpty(nombreImpresora9))
                        ManagerImpresora.ImprimirTicket(codigo, nombreImpresora9, alturaLinea, veces, tipo, 9);
                    if (!string.IsNullOrEmpty(nombreImpresora10))
                        ManagerImpresora.ImprimirTicket(codigo, nombreImpresora10, alturaLinea, veces, tipo, 10);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Print UWP:" + ex.Message);
                // 
            }
        }
        private async void VerAutoPedidoExe()
        {
            try
            {
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
        #endregion
        #region Comandos
        public ICommand InfoUsuarioPedidoCommand => new Command(InfoUsuarioPedido);
        public ICommand /*IAsyncRelayCommand*/ commandCerrarTodo { get { return new AsyncRelayCommand(CerrarTodo); } }

        public ICommand cmdVerCliente => new Command(VerClienteExe);
        public ICommand cmdVerRepartidor { get { return new Command((parametro) => VerRepartidor(parametro)); } }
        public ICommand PrintCommand => new AsyncRelayCommand<string>(async (codigo) => await PrePrint(codigo));
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
                    await App.ResponseWS.cambiaEstadoPedido(c.idPedido, 5);
                }
                App.userdialog.HideLoading();
                await actualizaPedidos();
            }
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
                    await App.customDialog.ShowDialogAsync("Se ha producido un errro al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
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
                    await App.customDialog.ShowDialogAsync("Se ha producido un errro al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
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
