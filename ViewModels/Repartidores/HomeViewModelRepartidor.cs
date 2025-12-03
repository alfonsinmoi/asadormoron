using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Interfaces;
// 
// 
using Plugin.Maui.Audio;
using Mopups.Services;
using AsadorMoron.Interfaces;
using AsadorMoron.Messages;
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.Utils;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.ViewModels.Clientes;
using AsadorMoron.Views.Establecimientos;
using AsadorMoron.Views.Administrador;
using System.Diagnostics;

namespace AsadorMoron.ViewModels.Repartidores
{
    public class HomeViewModelRepartidor : ViewModelBase
    {
        #region Propiedades
        bool borra = false;
        public List<CabeceraPedido> ListPedidosTemp;
        IAudioPlayer player = null; // Initialized when needed
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
        private ObservableCollection<MensajesRepartidorModel> mensajes;
        public ObservableCollection<MensajesRepartidorModel> Mensajes
        {
            get { return mensajes; }
            set
            {
                if (mensajes != value)
                {
                    mensajes = value;
                    OnPropertyChanged(nameof(Mensajes));
                }
            }
        }
        private MensajesRepartidorModel mensaje;
        public MensajesRepartidorModel Mensaje
        {
            get { return mensaje; }
            set
            {
                if (mensaje != value)
                {
                    mensaje = value;
                    OnPropertyChanged(nameof(Mensaje));
                }
            }
        }
        private bool mostrarMensaje;
        public bool MostrarMensaje
        {
            get { return mostrarMensaje; }
            set
            {
                if (mostrarMensaje != value)
                {
                    mostrarMensaje = value;
                    OnPropertyChanged(nameof(MostrarMensaje));
                }
            }
        }
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
        #endregion
        public HomeViewModelRepartidor()
        {
            
        }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DeviceDisplay.KeepScreenOn = true;
                });
                Preferences.Set("idGrupo", 1);
                Preferences.Set("idPueblo", 1);
                if (App.userdialog == null)
                {
                    try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
                App.DAUtil.homeRep = this;
                App.DAUtil.EstoyenHome = true;
                App.DAUtil.EnTimer = true;
                ResponseServiceWS.getConfiguracionAdmin();
                await initTimer();
                if (DeviceInfo.Platform.ToString().Equals("WinUI"))
                {

                    Device.StartTimer(new TimeSpan(0, 0, 20), () =>
                    {
                        // do something every 60 seconds
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await actualizaPedidos();
                        });

                        return App.DAUtil.EnTimer; // runs again, or false to stop
                    });
                }
                if (DeviceInfo.Platform.ToString() != "WinUI")
                {
                    App.DAUtil.Usuario.platform = DeviceInfo.Platform.ToString().ToLower();
                    App.DAUtil.Usuario.token = App.DAUtil.InstallId.ToString();
                    if (DeviceInfo.Platform.ToString() == "iOS")
                        App.DAUtil.Usuario.version = App.DAUtil.versioniOS;
                    else if (DeviceInfo.Platform.ToString() == "Android")
                        App.DAUtil.Usuario.version = App.DAUtil.versionAndroid;


                    App.ResponseWS.RegistraTokenFCM(App.DAUtil.Usuario);
                }



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
                }
                else
                {
                    App.userdialog.HideLoading();
                }
            }
        }
        #region Comandos
        public ICommand RefreshCommand
        {
            get
            {
                return new Command(() =>
                {
                    IsRefreshing = true;
                    RefreshData();
                    IsRefreshing = false;
                });
            }
        }
        
        public ICommand cmdAddNotaGasto { get { return new Command(AddNotaGasto); } }
        public ICommand InfoUsuarioPedidoCommand { get { return new Command(InfoUsuarioPedido); } }
        public ICommand ComandoCoger { get { return new Command((parametro) => BotonCoger(parametro)); } }
        public ICommand ComandoMapa { get { return new Command((parametro) => BotonMapa(parametro)); } }
        public ICommand cmdVerCliente { get { return new Command(VerClienteExe); } }
        public ICommand cmdSi { get { return new Command(SiExe); } }
        public ICommand cmdNo { get { return new Command(NoExe); } }
        #endregion
        #region Métodos
        private void VerClienteExe()
        {
            App.DAUtil.NavigationService.NavigateToAsyncMenu<CartaViewModel>(App.EstActual);
        }
        private void AddNotaGasto()
        {
            App.DAUtil.NavigationService.NavigateToAsync<GastoViewModel>();
        }
        private async void SiExe()
        {
            Mensaje.contestado = true;
            Mensaje.ok = true;
            await App.ResponseWS.ContestaMensaje(Mensaje);
            Mensajes.RemoveAt(0);
            if (Mensajes.Count > 0)
                Mensaje = Mensajes[0];
            MostrarMensaje = Mensajes.Count > 0;
            
        }
        private async void NoExe()
        {
            Mensaje.contestado = true;
            Mensaje.ok = false;
            await App.ResponseWS.ContestaMensaje(Mensaje);
            Mensajes.RemoveAt(0);
            if (Mensajes.Count > 0)
                Mensaje = Mensajes[0];
            MostrarMensaje = Mensajes.Count > 0;
        }
        private void RefreshData()
        {
            if (DeviceInfo.Platform.ToString() != "WinUI")
            {
                App.DAUtil.Usuario.platform = DeviceInfo.Platform.ToString().ToLower();
                App.DAUtil.Usuario.token = App.DAUtil.InstallId.ToString();
                if (DeviceInfo.Platform.ToString() == "iOS")
                    App.DAUtil.Usuario.version = App.DAUtil.versioniOS;
                else if (DeviceInfo.Platform.ToString() == "Android")
                    App.DAUtil.Usuario.version = App.DAUtil.versionAndroid;


                App.ResponseWS.RegistraTokenFCM(App.DAUtil.Usuario);
            }
        }

        private void InfoUsuarioPedido(object codigo)
        {
            try
            {
                if (MopupService.Instance.PopupStack.Count() == 0)
                {
                    string cod = (string)codigo;
                    CabeceraPedido c2 = ResponseServiceWS.TraePedidoPorCodigo(cod);
                    ZonaModel z = App.DAUtil.getZonas().Find(p => p.idZona == c2.idZona);
                    string nombreZona = string.Empty;
                    if (z != null && !string.IsNullOrEmpty(z.nombre))
                    {
                        nombreZona = z.nombre;
                        if(string.IsNullOrEmpty(c2.zona))
                            c2.zona = nombreZona;
                    }
                    MopupService.Instance.PushAsync(new PopupPageInfoUsuarioPedido(c2), true);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }

        public async Task actualizaPedidos(bool mensaje=false)
        {
            try
            {
                if (mensaje)
                {
                    Mensajes = ResponseServiceWS.TraeMensajeRepartidor();
                    if (Mensajes.Count > 0)
                        Mensaje = Mensajes[0];
                    MostrarMensaje = Mensajes.Count > 0;
                }
                else
                {
                    if (App.DAUtil.Usuario.Repartidor == null)
                        App.DAUtil.Usuario.Repartidor = ResponseServiceWS.GetRepartidorByIdUsuario(App.DAUtil.Usuario.idUsuario);
                    if (App.DAUtil.Usuario.Repartidor != null)
                    {
                        ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getListadoPedidosByIdRepartidor(App.DAUtil.Usuario.Repartidor.id));
                        if (Listado == null)
                            Listado = new ObservableCollection<CabeceraPedido>();

                        if (ListPedidosTemp != null)
                        {
                            //List<CabeceraPedido> toBeAdded = ListPedidosTemp.Where(c => c.idPedido == 0).ToList();
                            List<CabeceraPedido> toBeAdded = ListPedidosTemp.Where(c => !Listado.Any(d => c.idPedido == d.idPedido)).ToList();
                            List<CabeceraPedido> toBeDeleted = Listado.Where(c => !ListPedidosTemp.Any(d => c.idPedido == d.idPedido)).ToList();
                            List<CabeceraPedido> toBeUpdated = ListPedidosTemp.Where(c => Listado.Any(d => c.idPedido == d.idPedido && c.idEstadoPedido != d.idEstadoPedido)).ToList();


                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                if (toBeAdded != null)
                                {
                                    if (toBeAdded.Count > 0)
                                    {
                                        foreach (var item in toBeAdded)
                                        {
                                            switch (item.idEstadoPedido)
                                            {
                                                case (int)EstadoPedido.Nuevo:
                                                    item.ColorPedido = "pedidoporrecoger.png";
                                                    break;
                                                case (int)EstadoPedido.EnProceso:
                                                    item.ColorPedido = "pedidoporrecoger.png";
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

                                            if (item.idRepartidor == App.DAUtil.Usuario.Repartidor.id)
                                            {
                                                if (item.idEstadoPedido == 3)
                                                    item.imagenBoton = "recogido.png";
                                                else if (item.idEstadoPedido == 4)
                                                    item.imagenBoton = "entregado.png";
                                                else
                                                    item.imagenBoton = "desasociar.png";
                                            }
                                            else
                                                item.imagenBoton = "asociar.png";



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

                                                if (x.idRepartidor == App.DAUtil.Usuario.Repartidor.id)
                                                {
                                                    if (x.idEstadoPedido == 3)
                                                        x.imagenBoton = "recogido.png";
                                                    else if (x.idEstadoPedido == 4)
                                                        x.imagenBoton = "entregado.png";
                                                    else
                                                        x.imagenBoton = "desasociar.png";
                                                }
                                                else
                                                    x.imagenBoton = "asociar.png";
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
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task BotonExecute(string idPedido)
        {
            try
            {
                borra = false;
                bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, App.MensajesGlobal.Where(p => p.clave.Equals("pregunta_entregado")).FirstOrDefault<MensajesModel>().valor, AppResources.No, AppResources.Si);
                
                if (result)
                {
                    borra = true;
                    CabeceraPedido c = Listado.Where(p => p.codigoPedido.Equals(idPedido)).FirstOrDefault<CabeceraPedido>();
                    if (await App.ResponseWS.cambiaEstadoPedido(c.idPedido, 5))
                    {

                        List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(App.DAUtil.Usuario.idPueblo);
                        foreach (TokensModel to in tokens)
                            App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, "El pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " ha sido entregado", to.token);

                        List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(c.idEstablecimiento);
                        foreach (TokensModel to in tokens3)
                            App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, "El pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " ha sido entregado", to.token);


                        string tokenUser = App.ResponseWS.getTokenUsuario(c.idUsuario);
                        App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, "El pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " ha sido entregado", tokenUser);
                        Listado.Remove(c);
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "PolloAndaluz", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                // 
            }


        }
        private async Task BotonExecute2(string idPedido)
        {
            try
            {
                borra = false;
                bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaPedidoRecogido, AppResources.No, AppResources.Si);
                
                if (result)
                {
                    borra = true;
                    CabeceraPedido c = Listado.Where(p => p.codigoPedido.Equals(idPedido)).FirstOrDefault<CabeceraPedido>();
                    
                    if (await App.ResponseWS.cambiaEstadoPedido(c.idPedido, 4))
                    {
                        await App.ResponseWS.pedidoConRepartidor(c.codigoPedido, App.DAUtil.Usuario.Repartidor.id);
                        c.idEstadoPedido = 4;
                        c.ColorPedido = "pedidorecogido.png";
                        c.imagenBoton = "entregado.png";
                        c.FotoRepartidor = App.DAUtil.Usuario.Repartidor.foto;
                        c.idRepartidor = App.DAUtil.Usuario.Repartidor.id;
                        c.repartidor = 1;

                        List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(App.DAUtil.Usuario.idPueblo);
                        foreach (TokensModel to in tokens)
                            App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, "El pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " ha sido recogido", to.token);

                        tokens = App.ResponseWS.getTokenEstablecimiento(c.idEstablecimiento);
                        foreach (TokensModel to in tokens)
                            App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, "El pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " ha sido recogido", to.token);
                        string tokenUser = App.ResponseWS.getTokenUsuario(c.idUsuario);
                        App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, "El pedido " + c.codigoPedido + " ha sido recogido de " + c.nombreEstablecimiento, tokenUser);
                    }
                    else
                    {
                        borra = false;
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync("Se ha producido un errro al cambiar el estado del pedido. Inténtelo de nuevo", "PolloAndaluz", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                // 
            }


        }
        private async Task BotonCogerExecute(string idPedido)
        {
            try
            {
                borra = false;
                CabeceraPedido c = Listado.Where(p => p.codigoPedido == idPedido).FirstOrDefault<CabeceraPedido>();
                if (c.idEstadoPedido == 4)
                {
                    try
                    {
                        await Task.Run(() =>
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                if (c.idRepartidor==App.DAUtil.Usuario.Repartidor.id)
                                    await BotonExecute(idPedido);
                                else
                                {
                                    await App.ResponseWS.pedidoConRepartidor(c.codigoPedido, App.DAUtil.Usuario.Repartidor.id);
                                    c.idEstadoPedido = 4;
                                    c.ColorPedido = "pedidorecogido.png";
                                    c.imagenBoton = "entregado.png";
                                    c.FotoRepartidor = App.DAUtil.Usuario.Repartidor.foto;
                                    c.idRepartidor = App.DAUtil.Usuario.Repartidor.id;
                                    c.repartidor = 1;

                                    List<TokensModel> tokens = App.ResponseWS.getTokenRepartidores(c.idEstablecimiento);
                                    foreach (TokensModel to in tokens)
                                        App.ResponseWS.enviaNotificacion(c.nombreEstablecimiento, "El pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " ha sido recogido", to.token);

                                }
                            });
                        }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                        {
                            if (borra)
                            {
                                Listado.Remove(c);
                                TotalPedidos--;
                            }
                            App.userdialog.HideLoading();
                        }));
                    }
                    catch (Exception ex)
                    {
                        // 
                    }
                }
                else if (c.idEstadoPedido == 2 || c.idEstadoPedido == 3)
                {
                    try
                    {
                        await Task.Run( () =>
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await BotonExecute2(idPedido);
                            });
                        }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                        {
                            if (borra)
                            {
                                c.idEstadoPedido = 4;
                                c.ColorPedido = "pedidorecogido.png";
                                c.imagenBoton = "entregado.png";
                                c.FotoRepartidor = App.DAUtil.Usuario.Repartidor.foto;
                                c.idRepartidor = App.DAUtil.Usuario.Repartidor.id;
                                c.repartidor = 1;
                            }
                            App.userdialog.HideLoading();
                        }));
                    }
                    catch (Exception ex)
                    {
                        // 
                    }
                }
            }
            catch (Exception ex)
            {
                // 
                App.userdialog.HideLoading();
                Console.WriteLine(ex.Message);
            }

        }
        private void BotonCoger(object accion)
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Procesando); } catch (Exception) { }
                string idPedido = (string)accion;
                Task.Run( () =>
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await BotonCogerExecute(idPedido);
                    }); 
                }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                {
                    
                    App.userdialog.HideLoading();
                }));
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async void BotonMapa(object accion)
        {
            try
            {
                string idPedido = (string)accion;
                CabeceraPedido c2 = Listado.Where(p => p.codigoPedido.Equals(idPedido)).FirstOrDefault();
                var placemark = new Placemark
                {
                    CountryName = "España",
                    AdminArea = c2.provinciaUsuario,
                    Thoroughfare = c2.direccionUsuario,
                    Locality = c2.localidadUsuario
                };
                var options = new MapLaunchOptions { Name = c2.nombreUsuario, NavigationMode = NavigationMode.Driving };
                await Map.OpenAsync(placemark, options);
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private async Task initTimer()
        {
            try
            {
                App.DAUtil.pedidoNuevo = false;
                if (App.DAUtil.Usuario.Repartidor == null)
                    App.DAUtil.Usuario.Repartidor = ResponseServiceWS.GetRepartidorByIdUsuario(App.DAUtil.Usuario.idUsuario);
                if (App.DAUtil.Usuario.Repartidor != null)
                {
                    Mensajes = ResponseServiceWS.TraeMensajeRepartidor();
                    if (Mensajes.Count > 0)
                        Mensaje = Mensajes[0];
                    MostrarMensaje = Mensajes.Count > 0;

                    ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getListadoPedidosByIdRepartidor(App.DAUtil.Usuario.Repartidor.id));
                    TotalPedidos = 0;

                    if (ListPedidosTemp != null && ListPedidosTemp.Count > 0)
                    {
                        foreach (CabeceraPedido item in ListPedidosTemp)
                        {
                            switch (item.idEstadoPedido)
                            {
                                case (int)EstadoPedido.Nuevo:
                                    item.ColorPedido = "pedidoporrecoger.png";
                                    break;
                                case (int)EstadoPedido.EnProceso:
                                    item.ColorPedido = "pedidoporrecoger.png";
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
                            if (item.idRepartidor == App.DAUtil.Usuario.Repartidor.id)
                            {
                                if (item.idEstadoPedido == 3)
                                    item.imagenBoton = "recogido.png";
                                else if (item.idEstadoPedido == 4)
                                    item.imagenBoton = "entregado.png";
                            }
                            else
                                item.imagenBoton = "recogido.png";
                        }
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            if (ListPedidosTemp.Count > 0)
                            {
                                Listado = new ObservableCollection<CabeceraPedido>(ListPedidosTemp);
                                TotalPedidos = ListPedidosTemp.Count();
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
        #endregion
        //TOdo: pregunta antierror con confirm --> App.MensajesGlobal.Where(p => p.clave.Equals("pregunta_entregado")).FirstOrDefault<MensajesModel>().valor
        //call cambiaEstadoPedido (idPedido,5)
        //enviar notificacion Listado = ResponseServiceWS.getListadoPedidosByIdRepartidor(App.DAUtil.Usuario.Repartidor.id);
    }
}
