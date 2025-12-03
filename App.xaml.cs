using System;
using System.Threading.Tasks;
using AsadorMoron.Interfaces;



using AsadorMoron.Models;
using AsadorMoron.Interfaces;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using AsadorMoron.Views;
using AsadorMoron.Services;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;


using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using AsadorMoron.ViewModels.Clientes;
using System.Net.Http;
using AsadorMoron.Recursos;
using Plugin.StoreReview;
using AsadorMoron.Messages;
using System.Text;
using AsadorMoron.Utils;
using AsadorMoron.Models.PayComet;
using System.Globalization;
// Device removed in MAUI
using System.Collections.ObjectModel;
// using AsadorMoron.Print; // TODO

[assembly: ExportFont("Syne-Bold.ttf", Alias = "Syne")]
[assembly: ExportFont("Syne-Medium.ttf", Alias = "Syne_Medium")]
[assembly: ExportFont("Syne-Regular.ttf", Alias = "Syne_Regular")]
[assembly: ExportFont("NunitoSans-Regular.ttf", Alias = "Nunito")]
[assembly: ExportFont("NunitoSans-Bold.ttf", Alias = "Nunito_bold")]

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AsadorMoron
{
    public partial class App : Application
    {
        public static bool esPorPuntos = false;
        public static List<ComboModel> combos;
        public static AutoPedidoModel autoPedidoAdmin = null;
        public static double saldoGastado = 0;
        public static AmigosModel amigos = null;
        public static bool PendientePromocion = false;
        public static PromocionAmigoModel promocionAmigo = null;
        public static double Descuento = 0;
        public static OnlineModel online;
        public static IList<object> establecimientosSeleccionados;
        public static bool okSeleccion = false;
        public static bool ViendoDocumento = false;
        public static bool puebloCambiado = false;
        public static string idioma = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();
        public static bool tieneShort = false;
        public static Establecimiento EstShortCode = null;
        public static ConfiguracionGlobalModel global;
        private static DataUltis dbUtils;
        public static bool entradoEnCarta = false;
        public static bool tengoConexion = false;
        public static Establecimiento EstActual = null;
        public static Establecimiento MiEst = null;
        private static ResponseServiceWS responseWS;
        public static IUserDialogs userdialog = new Services.UserDialogsService();
        public static IAlertDialogService customDialog = new Services.AlertDialogService.AlertDialogService();
        public static List<MensajesModel> MensajesGlobal;
        public static List<PredefinidosModel> MensajesPredefinidos;
        public static System.Timers.Timer timer;
        public static System.Timers.Timer timerPosiciones;
        public static TarjetaModel TarjetaSeleccionada;
        internal static List<ArticuloModel> listaProductos;
        public static HttpClient Client = new HttpClient(new HttpClientHandler());
        public const string LoggedInKey = "LoggedIn";
        public const string AppleUserIdKey = "AppleUserIdKey";
        public static List<CodeErrors> erroresPaycomet;
        public static string urlChallengue;
        public static CabeceraPedido pedidoEnCurso;
        public static List<CarritoModel> carritoEnCurso;
        public static List<string> pedidosARecoger = new List<string>();
        public App()
        {
            try
            {
                // Register SQLite service before anything else
                DependencyService.Register<ISQLite, SQLiteService>();

                InitializeComponent();
                DAUtil.ConFoto = false;

                if (DAUtil.DoIHaveInternet())
                    tengoConexion = true;
                else
                    tengoConexion = false;
                MainPage = new NavigationPage(new PaginaEspera())
                {
                    BarBackgroundColor = Color.FromArgb("#ffffff")
                };
                // TODO: Initialize OneSignal for MAUI
                // OneSignal initialization code removed - needs reimplementation
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                userdialog.HideLoading();
                
            }
        }
        public static int Usage
        {
            get => Preferences.Get(nameof(Usage), 0);
            set => Preferences.Set(nameof(Usage), value);
        }
        protected override async void OnStart()
        {
            //Task.Delay(100).ConfigureAwait(false); ; //NO QUITAR

            try
            {
                //ResponseServiceWS.EncryptaContraseñas();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                App.EstActual = ResponseServiceWS.getEstablecimiento(67);
                App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(67);
                combos = ResponseServiceWS.getCombos();
                
                /*try
                {
                    List<AppAction> acciones= new List<AppAction>();
                    foreach (FavoritosModel f in establecimientos)
                    {
                        acciones.Add(new AppAction(f.idEstablecimiento.ToString(), AppResources.PedidoA + f.nombreEstablecimiento));
                    }
                    if (acciones.Count>0)
                        await AppActions.SetAsync(acciones);
                }
                catch (FeatureNotSupportedException)
                {
                    Debug.WriteLine("App Actions not supported");
                }*/
                online = new OnlineModel();
                await InitNotification();
                await InitNavigation();
                Connectivity.ConnectivityChanged += HandleConnectivityChanged;
                var count = Usage;
                count++;
                if (count == 5)
                {
                   await CrossStoreReview.Current.RequestReview(false);
                }
                Usage = count;




                





                //string mo2 = Crypto.Decrypt("6Zz1sbFvnMUlvrZMIb48cP29ZoIUvhPSZUOGx15egjY=");
                //string moi = Crypto.Encrypt("admlaibense");
            }
            catch (Exception ex)
            {
                userdialog.HideLoading();
                
            }


            base.OnStart();
        }
        protected override void OnSleep()
        {
            if (App.DAUtil.Usuario != null)
            {
                if (App.DAUtil.Usuario.rol == (int)RolesEnum.Cliente)
                       ResponseServiceWS.GuardaOnline(2);
            }
        }
        protected override void OnResume()
        {
            try
            {
                if (DAUtil.Usuario != null) {
                    if (DAUtil.Usuario.rol == (int)RolesEnum.Cliente)
                    {
                        online = new OnlineModel();
                        ResponseServiceWS.GuardaOnline(1);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error OnResume: " + ex.ToString());
                userdialog.HideLoading();
                
            }
            base.OnResume();
        }

        public static DataUltis DAUtil
        {
            get
            {
                if (dbUtils == null)
                {
                    dbUtils = new DataUltis();
                }
                return dbUtils;
            }
        }
        public static ResponseServiceWS ResponseWS
        {
            get
            {
                try
                {
                    if (responseWS == null)
                    {
                        responseWS = new ResponseServiceWS();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error ResponseWS: " + ex.ToString());
                    throw ex;
                }

                return responseWS;
            }
        }

        // TODO: Implement OneSignal notification handlers for MAUI
        // private void OnHandleNotificationReceived(...) { }
        // private void OnHandleNotificationOpened(...) { }
        public async void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            try
            {
                if ((Current.MainPage.Title != null) && Current.MainPage.Title.Equals(AppResources.Acceso))
                {
                    if (DAUtil.DoIHaveInternet())
                    {
                        tengoConexion = true;
                        using (userdialog.Loading(AppResources.RestableciendoConexion, null, null, true, MaskType.Black))
                        {
                            if (IsLogin())
                            {
                                await InitNotification();
                                    try { userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { userdialog.HideLoading(); }
                                    MainThread.BeginInvokeOnMainThread(async () =>
                                    {
                                        await DAUtil.NavigationService.InitializeAsync();
                                    });
                            }
                        }
                    }
                    else
                    {
                        tengoConexion = false;
                    }
                }
                if (DAUtil.DoIHaveInternet())
                {
                    tengoConexion = true;
                }
                else
                {
                    tengoConexion = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error HandleConnectivityChanged: " + ex.ToString());
                Console.WriteLine(ex);
                
            }
        }

        private async Task InitNotification()
        {
            // TODO: Implement OneSignal for MAUI
            await Task.CompletedTask;
        }
        public static async Task<bool> InitNavigation()
        {
            try
            {
                DAUtil.CreaTablas();
                bool result;
                if (!Preferences.Get("Social", false))
                    result = IsLogin();
                else
                {
                    result = IsLoginSocial();
                }

                //TODO: Mejora tiempo
                await ResponseWS.getConfiguracionGeneralASync();
                MensajesGlobal = await ResponseWS.getMensajesAsync();
                MensajesPredefinidos = ResponseWS.getMensajesPredefinidos();
                var pueblos = ResponseWS.getPueblos();
                await ResponseWS.getZonasAsync();
               
                if (DAUtil.Usuario != null)
                {
                    int rol = DAUtil.Usuario.rol;
                    if (rol == (int)RolesEnum.Establecimiento || rol == (int)RolesEnum.Administrador || rol == (int)RolesEnum.SuperAdmin)
                    {
                        try
                        {
                            string ids = "";
                            if (pueblos != null && pueblos.Any())
                            {
                                foreach (PueblosModel item in pueblos)
                                {
                                    if (!ids.Equals(""))
                                        ids += ",";
                                    ids += item.id;
                                }
                            }
                            responseWS.ListadoRepartidoresMultiAdmin(ids);
                            if (DeviceInfo.Platform.ToString() != "WinUI")
                            {
                               if (App.DAUtil.Usuario.establecimientos != null)
                                    if (App.DAUtil.Usuario.establecimientos.Count > 0)
                                    {
                                        if (!string.IsNullOrEmpty(App.DAUtil.Usuario.establecimientos[0].nombreImpresoraBarra))
                                            Preferences.Set("defaultPrinter", App.DAUtil.Usuario.establecimientos[0].nombreImpresoraBarra);
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(string.Format("Error InitNavigation: {0}", ex.Message.ToString()));
                        }
                    }
                    else if (rol == (int)RolesEnum.Repartidor)
                    {
                        DAUtil.Usuario.Repartidor = ResponseServiceWS.GetRepartidorByIdUsuario(DAUtil.Usuario.idUsuario);
                    }
                    else if (rol == (int)RolesEnum.Cliente)
                    {
                        promocionAmigo = ResponseServiceWS.getPromocionAmigo();

                    }
                }

                ResponseWS.getConfiguracionGlobal();
                if ((DeviceInfo.Platform.ToString() == "Android" && Preferences.Get("VersionMinimaAndroid", 0) > DAUtil.versionAppAndroid) || (DeviceInfo.Platform.ToString() == "iOS" && Preferences.Get("VersionMinimaiOS", 0) > DAUtil.versionAppiOS))
                {
                    DAUtil.NavigationService.LogOutApp(typeof(VersionMinimaViewModel), null);
                }
                else
                {
                    Preferences.Set("PIN", "");
                    DAUtil.NavigationService.InitializeAsync();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error InitNavigation: {0}", ex.ToString()));
                userdialog.HideLoading();
                
                return false;
            }

            return true;
        }
        private static bool IsLogin()
        {
            string Username = string.Empty;
            string Pass = string.Empty;

            try
            {
                UsuarioModel persona = DAUtil.GetUsuarioSQLite();
                if (persona != null)
                {
                    DAUtil.Usuario = persona;
                    Username = persona.email;
                    Pass = persona.password;
                }
                if (tengoConexion)
                {
                    if (!string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Pass))
                    {
                        try
                        {
                            if (ResponseServiceWS.Login(Username, Pass))
                            {
                                return true;
                            }
                            else
                            {
                                
                                DAUtil.Usuario = null;
                                DAUtil.DeleteUsuarioSQLite();
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error isLogin: " + ex.ToString());
                            Console.WriteLine(ex);
                            
                            ex.ToString();
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error isLogin: " + ex.ToString());
                Console.WriteLine(ex);
                userdialog.HideLoading();
                
                return false;
            }
            return true;
        }
        private void IdsAvailable(string userID, string pushToken)
        {
            try
            {
                string plat = DeviceInfo.Platform.ToString().ToLower();
                DAUtil.InstallId = userID.ToString();

                if (DAUtil.Usuario != null)
                {
                    DAUtil.Usuario.platform = plat;
                    DAUtil.Usuario.token = userID.ToString();
                    if (DeviceInfo.Platform.ToString() == "iOS")
                        DAUtil.Usuario.version = DAUtil.versioniOS;
                    else if (DeviceInfo.Platform.ToString() == "Android")
                        DAUtil.Usuario.version = DAUtil.versionAndroid;

                    ResponseWS.RegistraTokenFCM(DAUtil.Usuario);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error IdsAvailable: " + ex.ToString());
                Console.WriteLine(ex);
                
            }
        }

        private static bool IsLoginSocial()
        {
            string Username = string.Empty;
            string Pass = string.Empty;

            try
            {
                UsuarioModel persona = DAUtil.GetUsuarioSQLite();
                if (persona != null)
                {
                    DAUtil.Usuario = persona;
                    Username = persona.email;
                    Pass = persona.idSocial;

                }
                if (tengoConexion)
                {
                    if (!string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Pass))
                    {
                        try
                        {
                            AuthNetworkData data = new AuthNetworkData();
                            data.Id = Pass;
                            data.Email = Username;
                            if (ResponseServiceWS.LoginSocial(data))
                            {
                                if (Preferences.Get("RedSocial", "").Equals("apple"))
                                {

                                }
                                return true;
                            }
                            else
                            {
                                
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error isLoginSocial: " + ex.ToString());
                            Console.WriteLine(ex);
                            
                            ex.ToString();
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error isLoginSocial: " + ex.ToString());
                Console.WriteLine(ex);
                userdialog.HideLoading();
                
                return false;
            }
            return true;
        }
        internal static double ParseaPrecio(string precio)
        {
            string miles = ",";
            string decimales = ".";
            if (CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator.Equals(","))
            {
                miles = ".";
                decimales = ",";
            }
            double valor = double.Parse(precio.Replace(miles, decimales), CultureInfo.InvariantCulture);
            string entero;
            string entero2;
            if (precio.Contains(","))
                entero = precio.Split(',')[0];
            else
                entero = precio.Split('.')[0];
            if (valor.ToString().Contains(","))
                entero2 = valor.ToString().Split(',')[0];
            else
                entero2 = valor.ToString().Split('.')[0];
            if (!entero.Equals(entero2))
                valor = double.Parse(precio.Replace(decimales, miles), CultureInfo.InvariantCulture);
            return valor;
        }
    }
}
