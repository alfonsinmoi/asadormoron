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
using AsadorMoron.Print;
using AsadorMoron.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using AsadorMoron.Recursos;
using System.Timers;
using AsadorMoron.ViewModels.Clientes;
using AsadorMoron.Views.Administrador;
// TODO: Reimplementar Print para MAUI
// //  // TODO: Reimplementar
using System.Diagnostics;

using CommunityToolkit.Mvvm.Input;
using AsadorMoron.Views.Repartidores;
using AsadorMoron.Managers;
using AsadorMoron.ViewModels.Establecimientos;
using static AsadorMoron.Managers.ManagerImpresora;

namespace AsadorMoron.ViewModels.Administrador
{
    public class HomeViewModelAdmin : ViewModelBase
    {
        public List<RepartidorModel> repartidores;
        public HomeViewModelAdmin()
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
                    if (es.local == 1)
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

                // Cargar contador de pollos asados
                _ = CargarContadorPollosAsync();

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

        // Contador de pollos asados del día
        private int contadorPollos = 0;
        public int ContadorPollos
        {
            get { return contadorPollos; }
            set
            {
                if (contadorPollos != value)
                {
                    contadorPollos = value;
                    OnPropertyChanged(nameof(ContadorPollos));
                }
            }
        }
        #endregion

        #region Funciones

        private async Task CargarContadorPollosAsync()
        {
            try
            {
                if (App.EstActual == null) return;

                var contador = await ResponseServiceWS.GetContadorPollosAsync(App.EstActual.idEstablecimiento);
                if (contador != null)
                {
                    ContadorPollos = contador.cantidad;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomeViewModelAdmin] Error cargando contador pollos: {ex.Message}");
            }
        }

        private async Task SumarPolloAsync()
        {
            try
            {
                if (App.EstActual == null) return;

                var resultado = await ResponseServiceWS.SumarPollosAsync(App.EstActual.idEstablecimiento, 1);
                if (resultado != null)
                {
                    ContadorPollos = resultado.cantidad;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomeViewModelAdmin] Error sumando pollo: {ex.Message}");
            }
        }

        private async Task RestarPolloAsync()
        {
            try
            {
                if (App.EstActual == null || ContadorPollos <= 0) return;

                var resultado = await ResponseServiceWS.RestarPollosAsync(App.EstActual.idEstablecimiento, 1);
                if (resultado != null)
                {
                    ContadorPollos = resultado.cantidad;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomeViewModelAdmin] Error restando pollo: {ex.Message}");
            }
        }

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
                                    await Print(item.codigoPedido, TipoImpresion.Normal);
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

                        if (toBeDeleted.Count > 0 || toBeUpdated.Count > 0)
                        {

                            TotalPedidos = ListPedidosTemp.Count();
                            if (toBeDeleted.Count > 0)
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
                        var resultBeep = 0;
                        resultBeep = Listado.Where(p => p.estadoPedido.Equals(EstadoPedido.Nuevo.ToString())).Count();
                        if (resultBeep > 0)
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
                    });
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
                                            case (int)EstadoPedido.PorRecoger:
                                                item.ColorPedido = "pedidoporrecoger.png";
                                                item.imagenBoton = "recogido.png";
                                                await Print(item.codigoPedido, TipoImpresion.Normal);
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
                App.userdialog.ShowLoading(AppResources.Cargando);
                await Task.Delay(200);
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
                            ok = await App.ResponseWS.cambiaEstadoPedido(idPedido, 3);
                        else
                            ok = await App.ResponseWS.cambiaEstadoPedido(idPedido, 2);
                        linea = 6;
                        if (ok)
                        {
                            mensajeUsuario = "El pedido " + c.codigoPedido + " ha sido visto por " + c.nombreEstablecimiento;
                            linea = 7;
                            if (c.tipoVenta.StartsWith("Envío"))
                            {
                                mensajeRepartidor = "El pedido " + c.codigoPedido + " ha sido visto por " + c.nombreEstablecimiento;
                            }
                            try
                            {
                                if (!c.tipoVenta.Equals("Local"))
                                {
                                    linea = 11;
                                    mensajeAdmin = "El pedido " + c.codigoPedido + " ha sido visto por " + c.nombreEstablecimiento;
                                    Establecimiento es = App.DAUtil.Usuario.establecimientos.Find(p => p.idEstablecimiento == c.idEstablecimiento);
                                    if (es.configuracion == null)
                                        es.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(es.idEstablecimiento);
                                    await Print(c.codigoPedido, TipoImpresion.Normal, es.configuracion.numImpresion);
                                }
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
                            await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
                            return false;
                        }
                    }
                    else if (c.imagenBoton.Equals("entrega.png"))
                    {
                        linea = 23;
                        if (await App.ResponseWS.cambiaEstadoPedido(idPedido, 5))
                        {
                            mensajeUsuario = "Su pedido " + c.codigoPedido + " de " + c.nombreEstablecimiento + " está en camino.";
                            linea = 24;
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
                            return false;
                        }
                    }

                    await EnvioNotificacionesPedido(mensajeAdmin, mensajeUsuario, mensajeCamarero, mensajeRepartidor, c);

                }
                await actualizaPedidos();
                return true;
            }
            catch (Exception ex)
            {
                await App.customDialog.ShowDialogAsync(AppResources.Error + ex.Message + " en la linea " + linea, AppResources.App, AppResources.Cancelar);
                
                return false;
            }
            finally
            {
                App.userdialog.HideLoading();
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
                App.userdialog.ShowLoading(AppResources.Cargando);
                await Task.Delay(200);
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
                        linea = 5;
                        bool ok = false;
                        if (c.imagenBoton.Equals("recogido.png"))
                        {
                            if (c.tipoVenta.Equals("Local"))
                                ok = await App.ResponseWS.cambiaEstadoPedido(idPedido, 3);
                            else
                                ok = await App.ResponseWS.cambiaEstadoPedido(idPedido, 2);
                        }
                        else
                            ok = await App.ResponseWS.cambiaEstadoPedido(idPedido, 3);

                        if (ok)
                        {
                            linea = 6;
                            linea = 7;
                            if (c.tipoVenta.StartsWith("Envío"))
                            {
                                linea = 8;
                                mensajeAdmin = "El pedido " + c.codigoPedido + " ha pasado, de nuevo, a visto por " + c.nombreEstablecimiento;
                                linea = 9;
                                mensajeRepartidor = "El pedido " + c.codigoPedido + " ha pasado, de nuevo, a visto por " + c.nombreEstablecimiento;
                                linea = 10;
                            }
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync("Se ha producido un error al cambiar el estado del pedido. Inténtelo de nuevo", "AsadorMoron", "OK");
                        }
                    }
                    await EnvioNotificacionesPedido(mensajeAdmin, "", mensajeCamarero, mensajeRepartidor, c);

                }
                await actualizaPedidos();
                return true;
            }
            catch (Exception ex)
            {
                await App.customDialog.ShowDialogAsync(AppResources.Error + ex.Message + " en la linea " + linea, AppResources.App, AppResources.Cancelar);
                
                return false;
            }
            finally
            {
                App.userdialog.HideLoading();
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
                    List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(c.idEstablecimiento);
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
                            case (int)EstadoPedido.PorRecoger:
                                item.ColorPedido = "pedidoporrecoger.png";
                                item.imagenBoton = "recogido.png";
                                await Print(item.codigoPedido, TipoImpresion.Normal);
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
        private async Task PrePrint(string codigo)
        {
            var c = Listado.Where(p => p.codigoPedido == codigo).FirstOrDefault();
            if (c != null)
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
        #endregion
        #region Comandos
        public ICommand InfoUsuarioPedidoCommand => new Command(InfoUsuarioPedido);
        public ICommand cmdVerRepartidor { get { return new Command((parametro) => VerRepartidor(parametro)); } }
        public ICommand SumarPolloCommand { get { return new Command(async () => await SumarPolloAsync()); } }
        public ICommand RestarPolloCommand { get { return new Command(async () => await RestarPolloAsync()); } }
        public ICommand PrintCommand => new Command<string>(async (codigo) => await PrePrint(codigo));
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

/* 
 public HomeViewModelAdmin()
 {
     if (App.DAUtil.NotificacionPantalla.Equals(""))
     {
         if (App.userdialog == null)
         {
             try { dialogHomeAdmin.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
         }
     }
 }
 private async void GetNumeroUsuarios()
 {
     UsuariosOnline = await App.ResponseWS.getUsuariosOnline();
 }
 public override async Task InitializeAsync(object navigationData)
 {

     try
     {
         ListadoRepartidores = new ObservableCollection<RepartidorBindableModel>();
         App.autoPedidoAdmin = null;

         ListadoRepartidores = new ObservableCollection<RepartidorBindableModel>(App.DAUtil.GetRepartidoresBindables().Where(t => t.activo == 1).ToList());
         CambiaPueblo = false;

         GetNumeroUsuarios();
         await Inicia();
         await base.InitializeAsync(navigationData);
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
 private string nombreImpresora;
 private int alturaLinea;
 private IUserDialogs dialogHomeAdmin = App.userdialog;
 private List<CabeceraPedido> ListPedidosTemp;
 private IAudioPlayer player = null; // Initialized when needed
 public List<RepartidorModel> repartidores;
 public bool l_checkEstado = false;
 public List<CustomPin> listCustomPins = new List<CustomPin>();

 private CustomMap miMapa;
 public CustomMap MiMapa
 {
     get
     {
         return miMapa;
     }
     set
     {
         if (miMapa != value)
         {
             miMapa = value;
             OnPropertyChanged(nameof(MiMapa));
         }
     }
 }
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
 private CustomMap customMap;

 public CustomMap CustomMap
 {
     get { return customMap; }
     set
     {
         customMap = value;
         OnPropertyChanged(nameof(CustomMap));
     }
 }


 private ObservableCollection<CustomPin> customPins = new ObservableCollection<CustomPin>();
 public ObservableCollection<CustomPin> CustomPins
 {
     get { return customPins; }
     set
     {
         customPins = value;
         OnPropertyChanged(nameof(CustomPins));
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
 private bool verMapa = true;
 public bool VerMapa
 {
     get
     {
         return verMapa;
     }
     set
     {
         if (verMapa != value)
         {
             verMapa = value;
             OnPropertyChanged(nameof(VerMapa));
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


 private List<RepartidorBindableModel> repartidoresOrigin;
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
 public ICommand cmdVerMapa { get { return new Command(VerMapaExe); } }
 public ICommand cmdVerCliente { get { return new Command(VerClienteExe); } }
 public ICommand InfoUsuarioPedidoCommand { get { return new Command(InfoUsuarioPedido); } }
 public ICommand PrintCommand { get { return new Command<string>(async(parametro) => await Print(parametro)); } }
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
 private async Task Inicia()
 {

     if (App.userdialog == null)
     {
         try { dialogHomeAdmin.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
     }
     App.EstActual = App.MiEst;
     App.DAUtil.homeAdmin = this;
     App.DAUtil.EstoyenHome = true;
     App.DAUtil.EnTimer = true;
     VerSoloHoy = Preferences.Get("VerSoloHoy", true);
     await initTimer();

     App.timer = new System.Timers.Timer();
     App.timer.Interval = 8000;
     App.timer.Elapsed += _timer_Elapsed;
     App.timer.Start();
     if (App.EstActual != null)
     {
         if (App.EstActual.configuracion == null)
             App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);
         nombreImpresora = App.EstActual.configuracion.nombreImpresora;
         alturaLinea = App.EstActual.configuracion.alturaLineaImpresora;
     }
     //repartidoresOrigin = App.DAUtil.GetRepartidoresBindables().Where(p => p.activo == 1).ToList();
     //ListadoRepartidores = new  ObservableCollection<RepartidorBindableModel>(repartidoresOrigin);
     ActualizarRepartidores();
 }
 private void _timer_Elapsed(object sender, ElapsedEventArgs e)
 {
     MainThread.BeginInvokeOnMainThread(async () =>
     {
         await actualizaPedidos();
         Console.WriteLine(DateTime.Now);
     });

 }
 private void VerMapaExe()
 {
     VerMapa = !VerMapa;
     Preferences.Set("VerMapa", VerMapa);
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
     App.DAUtil.NavigationService.NavigateToAsyncMenu<CartaViewModel>();

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
 public async Task actualizaPosicion()
 {
     try
     {
         bool flag = false;

         List<CustomPin> l_customPins = new List<CustomPin>();
         foreach (CustomPin customPin in MiMapa.ItemsSource)
         {
             l_customPins.Add(customPin);
         }
         if (repartidores.Count > 0)
         {
             foreach (RepartidorModel repartidor in repartidores.OrderBy(p => p.id))
             {
                 foreach (CustomPin customPin in MiMapa.ItemsSource)
                 {
                     if (repartidor.nombre == customPin.Name)
                     {
                         Position posicionRepartidor = ResponseServiceWS.getPosicionRepartidor(repartidor.id);
                         string direccion = "";
                         var placemarks = await Geocoding.GetPlacemarksAsync(posicionRepartidor.Latitude, posicionRepartidor.Longitude);
                         var placemark = placemarks?.FirstOrDefault();
                         if (placemark != null)
                         {
                             if (!string.IsNullOrEmpty(placemark.Thoroughfare))
                             {
                                 direccion = placemark.Thoroughfare;
                             }
                         }

                         if (customPin.Posicion != posicionRepartidor || customPin.Direccion != direccion)
                         {
                             flag = true;
                             customPin.Posicion = posicionRepartidor;
                             customPin.Position = posicionRepartidor;
                             customPin.Direccion = direccion;
                             customPin.Address = direccion;

                         }
                     }
                     if (!l_customPins.Contains(customPin))
                         l_customPins.Add(customPin);
                 }
             }

             if (flag && l_customPins.Count > 0)
             {
                 MiMapa.ItemsSource = new ObservableCollection<CustomPin>(l_customPins);
                 MiMapa.CustomPins = new ObservableCollection<CustomPin>(l_customPins);
                 //MiMapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(l_customPins[0].Posicion.Latitude, l_customPins[0].Posicion.Longitude), Distance.FromMiles(0.3)));
                 if (!DeviceInfo.Platform.ToString().Equals("WinUI"))
                 {
                     if (listCustomPins != null)
                     {
                         MiMapa.CustomPins.Clear();
                         MiMapa.Pins.Clear();
                         foreach (CustomPin c in listCustomPins)
                         {
                             MiMapa.CustomPins.Add(c);
                             MiMapa.Pins.Add(c);
                         }
                     }
                 }
             }
         }
     }
     catch (Exception ex)
     {
         string a = ex.Message;
     }
 }
 public async Task actualizaPedidos()
 {
     try
     {
         ListPedidosTemp = new List<CabeceraPedido>();
         ListadoRepartidores = new ObservableCollection<RepartidorBindableModel>(App.DAUtil.GetRepartidoresBindables().Where(t => t.activo == 1).ToList());
         if (VerSoloHoy)
             ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendienteMultiAdmin(App.DAUtil.Usuario.idPueblo.ToString())).FindAll(p => ((DateTime)p.fechaEntrega).ToString("dd/MM/yyyy").Equals(DateTime.Today.ToString("dd/MM/yyyy")));
         else
             ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendienteMultiAdmin(App.DAUtil.Usuario.idPueblo.ToString()));
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
                             item.ColorPedido = "pedidonuevo.png";

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
                         if (toBeAdded.Count > 0)
                         {
                             try
                             {
                                 foreach (var item in toBeAdded)
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
                                     });
                                 }
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

             ActualizarRepartidores();
         }
         else
             




     }
     catch (Exception ex)
     {
         
     }
 }

 private void ActualizarRepartidores()
 {

     List<RepartidorBindableModel> repartidores = new List<RepartidorBindableModel>(ListadoRepartidores);

     if (Listado != null && Listado.Count() > 0)
     {
         foreach (RepartidorBindableModel r in repartidores)
         {
             r.Cantidad = Listado.Where(p2 => p2.idRepartidor == r.id).Count();
         }

         bool isEqual = Enumerable.SequenceEqual(ListadoRepartidores, repartidores.OrderByDescending(r => r.Cantidad).ToList());

         if (!isEqual)
         {
             foreach (RepartidorBindableModel r in repartidores)
             {
                 r.Cantidad = Listado.Where(p2 => p2.idRepartidor == r.id).Count();
             }
             ListadoRepartidores = new ObservableCollection<RepartidorBindableModel>(repartidores.OrderByDescending(r => r.Cantidad).ToList());
         }
     }

     IsVisibleRepartidores = true;
 }

 private async Task initTimer()
 {
     try
     {
         ListPedidosTemp = new List<CabeceraPedido>();
         List<CabeceraPedido> cab;

         App.DAUtil.pedidoNuevo = false;
         if (VerSoloHoy)
             ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendienteMultiAdmin(App.DAUtil.Usuario.idPueblo.ToString())).FindAll(p => ((DateTime)p.fechaEntrega).ToString("dd/MM/yyyy").Equals(DateTime.Today.ToString("dd/MM/yyyy")));
         else
             ListPedidosTemp = new List<CabeceraPedido>(await ResponseServiceWS.getPedidoPendienteMultiAdmin(App.DAUtil.Usuario.idPueblo.ToString()));

         TotalPedidos = 0;
         GetNumeroUsuarios();
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
                 Listado = new ObservableCollection<CabeceraPedido>(ListPedidosTemp);
                 TotalPedidos = ListPedidosTemp.Count();
             });
         }
     }
     catch (Exception ex)
     {
         
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
             c2.opciones = false;
             MopupService.Instance.PushAsync(new PopupPageInfoUsuarioPedido(c2), true);
         }
     }
     catch (Exception ex)
     {
         
     }
 }
 private async Task Print(string codigo)
 {
     try
     {
         if (string.IsNullOrEmpty(nombreImpresora))
             await App.customDialog.ShowDialogAsync(AppResources.ImpresoraNoConfigurada, AppResources.App, AppResources.Cerrar);
         else
         {
             Printer printer = new Printer(nombreImpresora, codigo, "ISO-8859-1");
             printer.ImprimirTicketPedido(alturaLinea);
             printer.PrintDocument();
         }
     }
     catch (Exception ex)
     {
         Debug.WriteLine("Error Print UWP:" + ex.Message);
         
     }
 }

 #endregion
}
}
*/