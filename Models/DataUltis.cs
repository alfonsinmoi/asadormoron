using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
// 
using AsadorMoron.Interfaces;
using AsadorMoron.Models.PayComet;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Administrador;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.ViewModels.Clientes;
using AsadorMoron.ViewModels.Establecimientos;
using AsadorMoron.ViewModels.Repartidores;
using SQLite;
using Microsoft.Maui.Devices;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class DataUltis
    {
        public string versionAndroid = "1.1.0";
        public string versioniOS = "1.1.0";
        public string versionUWP = "1.1.0";
        public int versionAppAndroid = 40;
        public int versionAppiOS = 40;

        public DataUltis()
        {
            NavigationService = navigationService = ViewModelLocator.Instance.Resolve<INavigationService>();
            dbConn = Microsoft.Maui.Controls.DependencyService.Get<ISQLite>().GetConnection();

        }
        public HomeViewModelAdmin homeAdmin;
        public HomeViewModelAdminMobile homeAdminMobile;
        public HomeViewModelEstMobile homeEst;
        public HomeViewModelRepartidor homeRep;
        public bool EstoyenHome = false;
        public string Idioma = "";
        public ConfiguracionModel config = new ConfiguracionModel();

        public bool EnTimer = false;
        public List<Establecimiento> listadoEst;
        public bool pedidoNuevo;
        public UsuarioModel UsuarioNoVerificado;
        public string miURL = ResponseServiceWS.urlPro;
        private string notificacionPantalla = "";

        public string NotificacionPantalla
        {
            get { return notificacionPantalla; }
            set { notificacionPantalla = value; }
        }
        public bool ExportandoExcel = false;
        private Boolean conFoto;

        public Boolean ConFoto
        {
            get { return conFoto; }
            set { conFoto = value; }
        }
        internal bool EstaAbierto()
        {
            if (App.EstActual.configuracion == null)
                App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);

            ConfiguracionEstablecimiento configuracionAdmin = App.EstActual.configuracion;

            if (DateTime.Now.Hour < 17)
            {
                if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
                {
                    if (configuracionAdmin.activoViernesLocal && configuracionAdmin.inicioViernesLocal != null)
                        return configuracionAdmin.inicioViernesLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finViernesLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                {
                    if (configuracionAdmin.activoLunesLocal && configuracionAdmin.inicioLunesLocal != null)
                        return configuracionAdmin.inicioLunesLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finLunesLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                {
                    if (configuracionAdmin.activoSabadoLocal && configuracionAdmin.inicioSabadoLocal != null)
                        return configuracionAdmin.inicioSabadoLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finSabadoLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (configuracionAdmin.activoDomingoLocal && configuracionAdmin.inicioDomingoLocal != null)
                        return configuracionAdmin.inicioDomingoLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finDomingoLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday)
                {
                    if (configuracionAdmin.activoJuevesLocal && configuracionAdmin.inicioJuevesLocal != null)
                        return configuracionAdmin.inicioJuevesLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finJuevesLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                {
                    if (configuracionAdmin.activoMartesLocal && configuracionAdmin.inicioMartesLocal != null)
                        return configuracionAdmin.inicioMartesLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finMartesLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday)
                {
                    if (configuracionAdmin.activoMiercolesLocal && configuracionAdmin.inicioMiercolesLocal != null)
                        return configuracionAdmin.inicioMiercolesLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finMiercolesLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else
                    return false;
            }
            else
            {
                if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
                {
                    if (configuracionAdmin.activoViernesTardeLocal && configuracionAdmin.inicioViernesTardeLocal != null)
                        return configuracionAdmin.inicioViernesTardeLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finViernesTardeLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;

                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                {
                    if (configuracionAdmin.activoLunesTardeLocal && configuracionAdmin.inicioLunesTardeLocal != null)
                        return configuracionAdmin.inicioLunesTardeLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finLunesTardeLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                {
                    if (configuracionAdmin.activoSabadoTardeLocal && configuracionAdmin.inicioSabadoTardeLocal != null)
                        return configuracionAdmin.inicioSabadoTardeLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finSabadoTardeLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (configuracionAdmin.activoDomingoTardeLocal && configuracionAdmin.inicioDomingoTardeLocal != null)
                        return configuracionAdmin.inicioDomingoTardeLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finDomingoTardeLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday)
                {
                    if (configuracionAdmin.activoJuevesTardeLocal && configuracionAdmin.inicioJuevesTardeLocal != null)
                        return configuracionAdmin.inicioJuevesTardeLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finJuevesTardeLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                {
                    if (configuracionAdmin.activoMartesTardeLocal && configuracionAdmin.inicioMartesTardeLocal != null)
                        return configuracionAdmin.inicioMartesTardeLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finMartesTardeLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday)
                {
                    if (configuracionAdmin.activoMiercolesTardeLocal && configuracionAdmin.inicioMiercolesTardeLocal != null)
                        return configuracionAdmin.inicioMiercolesTardeLocal.Ticks <= DateTime.Now.TimeOfDay.Ticks && configuracionAdmin.finMiercolesTardeLocal.Ticks >= DateTime.Now.TimeOfDay.Ticks;
                }
                else
                    return false;
            }
            return false;
        }
        internal ConfiguracionModel GetConfiguracionSQLite()
        {
            try
            {
                ConfiguracionModel c = dbConn.Query<ConfiguracionModel>("Select * From [ConfiguracionModel]").FirstOrDefault();
                if (c != null)
                {
                    return c;
                }

                return new ConfiguracionModel();

            }
            catch (Exception)
            {
                return new ConfiguracionModel();

            }
        }

        internal void WriteLine(string view, string viewModel)
        {
            Debug.WriteLine(string.Format("********* http://View --> {0} http://ViewModel --> {1}   Hora: {2} *********", view, viewModel, DateTime.Now.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture)));
        }

        public void SaveToken(TokenResponse token)
        {
            try
            {
                dbConn.DeleteAll<TokenResponse>();
                dbConn.Insert(token);
                dbConn.Commit();
            }
            catch
            {
            }
        }
        public void CreaTablas()
        {
            try
            {

                if (GetInfo("UsuarioModel") != 32)
                {
                    UsuarioModel u = null;
                    if (GetInfo("UsuarioModel") != 0)
                    {
                        u = GetUsuarioSQLite();
                        if (u != null)
                        {
                            if (string.IsNullOrEmpty(u.codigo))
                            {
                                try
                                {
                                    u.codigo = u.nombre.Substring(0, 1) + u.apellidos.Substring(0, 1) + u.idUsuario.ToString().PadLeft(6, '0');
                                }
                                catch (Exception)
                                {
                                    u.codigo = u.nombre.Substring(0, 2) + u.idUsuario.ToString().PadLeft(6, '0');
                                }
                            }
                        }
                        dbConn.DropTable<UsuarioModel>();

                    }

                    dbConn.CreateTable<UsuarioModel>();
                    if (u != null)
                        dbConn.Insert(u);
                }
                if (GetInfo("MensajesRepartidorModel") != 9)
                {
                    if (GetInfo("MensajesRepartidorModel") != 0)
                        dbConn.DropTable<MensajesRepartidorModel>();

                    dbConn.CreateTable<MensajesRepartidorModel>();
                }
                if (GetInfo("Categoria") != 17)
                {
                    if (GetInfo("Categoria") != 0)
                        dbConn.DropTable<Categoria>();

                    dbConn.CreateTable<Categoria>();
                }
                if (GetInfo("ConfiguracionGlobalModel") != 10)
                {
                    if (GetInfo("ConfiguracionGlobalModel") != 0)
                        dbConn.DropTable<ConfiguracionGlobalModel>();

                    dbConn.CreateTable<ConfiguracionGlobalModel>();
                }
                if (GetInfo("PublicidadModel") != 15)
                {
                    if (GetInfo("PublicidadModel") != 0)
                        dbConn.DropTable<PublicidadModel>();

                    dbConn.CreateTable<PublicidadModel>();
                }
                if (GetInfo("AlergenosModel") != 3)
                {
                    if (GetInfo("AlergenosModel") != 0)
                        dbConn.DropTable<AlergenosModel>();

                    dbConn.CreateTable<AlergenosModel>();
                }
                if (GetInfo("PredefinidosModel") != 6)
                {
                    if (GetInfo("PredefinidosModel") != 0)
                        dbConn.DropTable<PredefinidosModel>();

                    dbConn.CreateTable<PredefinidosModel>();
                }
                if (GetInfo("CuponesModel") != 27)
                {
                    if (GetInfo("CuponesModel") != 0)
                        dbConn.DropTable<CuponesModel>();

                    dbConn.CreateTable<CuponesModel>();
                }
                if (GetInfo("OfertasModel") != 27)
                {
                    if (GetInfo("OfertasModel") != 0)
                        dbConn.DropTable<OfertasModel>();

                    dbConn.CreateTable<OfertasModel>();
                }
                if (GetInfo("CuponesUsuarioModel") != 7)
                {
                    if (GetInfo("CuponesUsuarioModel") != 0)
                        dbConn.DropTable<CuponesUsuarioModel>();

                    dbConn.CreateTable<CuponesUsuarioModel>();
                }
                if (GetInfo("PueblosModel") != 20)
                {
                    if (GetInfo("PueblosModel") != 0)
                        dbConn.DropTable<PueblosModel>();

                    dbConn.CreateTable<PueblosModel>();
                }
                if (GetInfo("MensajesModel") != 6)
                {
                    if (GetInfo("MensajesModel") != 0)
                        dbConn.DropTable<MensajesModel>();

                    dbConn.CreateTable<MensajesModel>();
                }
                if (GetInfo("CodeErrors") != 3)
                {
                    if (GetInfo("CodeErrors") != 0)
                        dbConn.DropTable<CodeErrors>();

                    dbConn.CreateTable<CodeErrors>();
                    CargaErroresPayComet();
                }
                if (GetInfo("TarjetaModel") != 9)
                {
                    if (GetInfo("TarjetaModel") != 0)
                        dbConn.DropTable<TarjetaModel>();

                    dbConn.CreateTable<TarjetaModel>();
                }
                if (GetInfo("ZonaModel") != 11)
                {
                    if (GetInfo("ZonaModel") != 0)
                        dbConn.DropTable<ZonaModel>();

                    dbConn.CreateTable<ZonaModel>();
                }
                if (GetInfo("RepartidorModel") != 10)
                {
                    if (GetInfo("RepartidorModel") != 0)
                        dbConn.DropTable<RepartidorModel>();

                    dbConn.CreateTable<RepartidorModel>();
                }
                if (GetInfo("IngredientesModel") != 6)
                {
                    if (GetInfo("IngredientesModel") != 0)
                        dbConn.DropTable<IngredientesModel>();

                    dbConn.CreateTable<IngredientesModel>();
                }
                if (GetInfo("ZonasRepartidorModel") != 3)
                {
                    if (GetInfo("ZonasRepartidorModel") != 0)
                        dbConn.DropTable<ZonasRepartidorModel>();

                    dbConn.CreateTable<ZonasRepartidorModel>();
                }
                if (GetInfo("IngredienteProductoModel") != 9)
                {
                    if (GetInfo("IngredienteProductoModel") != 0)
                        dbConn.DropTable<IngredienteProductoModel>();

                    dbConn.CreateTable<IngredienteProductoModel>();
                }

                if (GetInfo("Pedido") != 54)
                {
                    if (GetInfo("Pedido") != 0)
                        dbConn.DropTable<Pedido>();

                    dbConn.CreateTable<Pedido>();
                }
                if (GetInfo("ConfiguracionModel") != 9)
                {
                    if (GetInfo("ConfiguracionModel") != 0)
                        dbConn.DropTable<ConfiguracionModel>();

                    dbConn.CreateTable<ConfiguracionModel>();
                }
                if (GetInfo("ConfiguracionAdmin") != 79)
                {
                    if (GetInfo("ConfiguracionAdmin") != 0)
                        dbConn.DropTable<ConfiguracionAdmin>();

                    dbConn.CreateTable<ConfiguracionAdmin>();
                }
                if (GetInfo("FavoritosModel") != 7)
                {
                    if (GetInfo("FavoritosModel") != 0)
                        dbConn.DropTable<FavoritosModel>();

                    dbConn.CreateTable<FavoritosModel>();
                }
                if (GetInfo("TokenModel") != 3)
                {
                    if (GetInfo("TokenModel") != 0)
                        dbConn.DropTable<TokenModel>();

                    dbConn.CreateTable<TokenModel>();
                }
                if (GetInfo("NotificacionModel") != 6)
                {
                    if (GetInfo("NotificacionModel") != 0)
                        dbConn.DropTable<NotificacionModel>();

                    dbConn.CreateTable<NotificacionModel>();
                }
                if (GetInfo("Establecimiento") != 104)
                {
                    if (GetInfo("Establecimiento") != 0)
                        dbConn.DropTable<Establecimiento>();

                    dbConn.CreateTable<Establecimiento>();
                }
                if (GetInfo("CarritoModel") != 25)
                {
                    if (GetInfo("CarritoModel") != 0)
                        dbConn.DropTable<CarritoModel>();

                    dbConn.CreateTable<CarritoModel>();
                }
                if (GetInfo("TokenResponse") != 10)
                {
                    if (GetInfo("TokenResponse") != 0)
                        dbConn.DropTable<TokenResponse>();

                    dbConn.CreateTable<TokenResponse>();
                }
                if (GetInfo("ArticuloModel") != 34)
                {
                    if (GetInfo("ArticuloModel") != 0)
                        dbConn.DropTable<ArticuloModel>();

                    dbConn.CreateTable<ArticuloModel>();
                }
                if (GetInfo("PuntosUsuarioModel") != 4)
                {
                    if (GetInfo("PuntosUsuarioModel") != 0)
                        dbConn.DropTable<PuntosUsuarioModel>();

                    dbConn.CreateTable<PuntosUsuarioModel>();
                }
                if (GetInfo("ReservaModel") != 11)
                {
                    if (GetInfo("ReservaModel") != 0)
                        dbConn.DropTable<ReservaModel>();

                    dbConn.CreateTable<ReservaModel>();
                }
                if (GetInfo("PedidoModel") != 18)
                {
                    if (GetInfo("PedidoModel") != 0)
                        dbConn.DropTable<PedidoModel>();

                    dbConn.CreateTable<PedidoModel>();
                }
                if (GetInfo("EstablecimientoFiscalModel") != 10)
                {
                    if (GetInfo("EstablecimientoFiscalModel") != 0)
                        dbConn.DropTable<EstablecimientoFiscalModel>();

                    dbConn.CreateTable<EstablecimientoFiscalModel>();
                }
                App.erroresPaycomet = new List<CodeErrors>(getErroresPaycomet());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;

            }
        }
        #region Global
        internal void GetConfiguracionGeneral()
        {
            try
            {
                List<ConfiguracionGlobalModel> global = dbConn.Query<ConfiguracionGlobalModel>("Select * From [ConfiguracionGlobalModel]");
                if (global.Count > 0)
                {
                    App.global = global[0];
                    ResponseServiceWS.terminalPaycomet = global[0].terminalPaycomet;
                    ResponseServiceWS.apiKeyPaycomet = global[0].apiPaycomet;
                }
                else
                    App.ResponseWS.getConfiguracionGeneral();
            }
            catch (Exception)
            {
                App.ResponseWS.getConfiguracionGeneral();
            }
        }
        internal void SaveConfiguracionGeneral(ConfiguracionGlobalModel conf)
        {
            try
            {
                dbConn.InsertOrReplace(conf);
                dbConn.Commit();
            }
            catch (Exception)
            {
            }
        }
        #endregion
        private void CargaErroresPayComet()
        {
            InsertaErrorCode(0, "Sin error");
            InsertaErrorCode(1, AppResources.SoloError);
            InsertaErrorCode(100, "Tarjeta caducada");
            InsertaErrorCode(101, "Tarjeta en lista negra");
            InsertaErrorCode(102, "Operación no permitida para el tipo de tarjeta");
            InsertaErrorCode(103, "Por favor, contacte con el banco emisor");
            InsertaErrorCode(104, "Error inesperado");
            InsertaErrorCode(105, "Crédito insuficiente para realizar el cargo");
            InsertaErrorCode(106, "Tarjeta no dada de alta o no registrada por el banco emisor");
            InsertaErrorCode(107, "Error de formato en los datos capturados. CodValid");
            InsertaErrorCode(108, "Error en el número de la tarjeta");
            InsertaErrorCode(109, "Error en FechaCaducidad");
            InsertaErrorCode(110, "Error en los datos");
            InsertaErrorCode(111, "Bloque CVC2 incorrecto");
            InsertaErrorCode(112, "Por favor, contacte con el banco emisor");
            InsertaErrorCode(113, "Tarjeta de crédito no válida");
            InsertaErrorCode(114, "La tarjeta tiene restricciones de crédito");
            InsertaErrorCode(115, "El emisor de la tarjeta no pudo identificar al propietario");
            InsertaErrorCode(116, "Pago no permitido en operaciones fuera de línea");
            InsertaErrorCode(118, "Tarjeta caducada. Por favor retenga físicamente la tarjeta");
            InsertaErrorCode(119, "Tarjeta en lista negra. Por favor retenga físicamente la tarjeta");
            InsertaErrorCode(120, "Tarjeta perdida o robada. Por favor retenga físicamente la tarjeta");
            InsertaErrorCode(121, "Error en CVC2. Por favor retenga físicamente la tarjeta");
            InsertaErrorCode(122, "Error en el proceso pre-transacción. Inténtelo más tarde");
            InsertaErrorCode(123, "Operación denegada. Por favor retenga físicamente la tarjeta");
            InsertaErrorCode(124, "Cierre con acuerdo");
            InsertaErrorCode(125, "Cierre sin acuerdo");
            InsertaErrorCode(126, "No es posible cerrar en este momento");
            InsertaErrorCode(127, "Parámetro no válido");
            InsertaErrorCode(128, "Las transacciones no fueron finalizadas");
            InsertaErrorCode(129, "Referencia interna duplicada");
            InsertaErrorCode(130, "Operación anterior no encontrada. No se pudo ejecutar la devolución");
            InsertaErrorCode(131, "Preautorización caducada");
            InsertaErrorCode(132, "Operación no válida con la moneda actual");
            InsertaErrorCode(133, "Error en formato del mensaje");
            InsertaErrorCode(134, "Mensaje no reconocido por el sistema");
            InsertaErrorCode(135, "Bloque CVC2 incorrecto");
            InsertaErrorCode(137, "Tarjeta no válida");
            InsertaErrorCode(138, "Error en mensaje de pasarela");
            InsertaErrorCode(139, "Error en formato de pasarela");
            InsertaErrorCode(140, "Tarjeta inexistente");
            InsertaErrorCode(141, "Cantidad cero o no válida");
            InsertaErrorCode(142, "Operación cancelada");
            InsertaErrorCode(143, "Error de autenticación");
            InsertaErrorCode(144, "Denegado debido al nivel de seguridad");
            InsertaErrorCode(145, "Error en el mensaje PUC. Contacte con PAYCOMET");
            InsertaErrorCode(146, "Error del sistema");
            InsertaErrorCode(147, "Transacción duplicada");
            InsertaErrorCode(148, "Error de MAC");
            InsertaErrorCode(149, "Liquidación rechazada");
            InsertaErrorCode(150, "Fecha/hora del sistema no sincronizada");
            InsertaErrorCode(151, "Fecha de caducidad no válida");
            InsertaErrorCode(152, "No se pudo encontrar la preautorización");
            InsertaErrorCode(153, "No se encontraron los datos solicitados");
            InsertaErrorCode(154, "No se puede realizar la operación con la tarjeta de crédito proporcionada");
            InsertaErrorCode(155, "Este método requiere la activación del protocolo VHASH");
            InsertaErrorCode(195, "Requiere autenticación SCA");
            InsertaErrorCode(500, "Error inesperado");
            InsertaErrorCode(501, "Error inesperado");
            InsertaErrorCode(502, "Error inesperado");
            InsertaErrorCode(504, "Transacción cancelada previamente");
            InsertaErrorCode(505, "Transacción original denegada");
            InsertaErrorCode(506, "Datos de confirmación no válidos");
            InsertaErrorCode(507, "Error inesperado");
            InsertaErrorCode(508, "Transacción aún en proceso");
            InsertaErrorCode(509, "Error inesperado");
            InsertaErrorCode(510, "No es posible la devolución");
            InsertaErrorCode(511, "Error inesperado");
            InsertaErrorCode(512, "No es posible contactar con el banco emisor. Inténtelo más tarde");
            InsertaErrorCode(513, "Error inesperado");
            InsertaErrorCode(514, "Error inesperado");
            InsertaErrorCode(515, "Error inesperado");
            InsertaErrorCode(516, "Error inesperado");
            InsertaErrorCode(517, "Error inesperado");
            InsertaErrorCode(518, "Error inesperado");
            InsertaErrorCode(519, "Error inesperado");
            InsertaErrorCode(520, "Error inesperado");
            InsertaErrorCode(521, "Error inesperado");
            InsertaErrorCode(522, "Error inesperado");
            InsertaErrorCode(523, "Error inesperado");
            InsertaErrorCode(524, "Error inesperado");
            InsertaErrorCode(525, "Error inesperado");
            InsertaErrorCode(526, "Error inesperado");
            InsertaErrorCode(527, "Tipo de transacción desconocido");
            InsertaErrorCode(528, "Error inesperado");
            InsertaErrorCode(529, "Error inesperado");
            InsertaErrorCode(530, "Error inesperado");
            InsertaErrorCode(531, "Error inesperado");
            InsertaErrorCode(532, "Error inesperado");
            InsertaErrorCode(533, "Error inesperado");
            InsertaErrorCode(534, "Error inesperado");
            InsertaErrorCode(535, "Error inesperado");
            InsertaErrorCode(536, "Error inesperado");
            InsertaErrorCode(537, "Error inesperado");
            InsertaErrorCode(538, "Operación no cancelable");
            InsertaErrorCode(539, "Error inesperado");
            InsertaErrorCode(540, "Error inesperado");
            InsertaErrorCode(541, "Error inesperado");
            InsertaErrorCode(542, "Error inesperado");
            InsertaErrorCode(543, "Error inesperado");
            InsertaErrorCode(544, "Error inesperado");
            InsertaErrorCode(545, "Error inesperado");
            InsertaErrorCode(546, "Error inesperado");
            InsertaErrorCode(547, "Error inesperado");
            InsertaErrorCode(548, "Error inesperado");
            InsertaErrorCode(549, "Error inesperado");
            InsertaErrorCode(550, "Error inesperado");
            InsertaErrorCode(551, "Error inesperado");
            InsertaErrorCode(552, "Error inesperado");
            InsertaErrorCode(553, "Error inesperado");
            InsertaErrorCode(554, "Error inesperado");
            InsertaErrorCode(555, "No se pudo encontrar la operación previa");
            InsertaErrorCode(556, "Inconsistencia de datos en la validación de la cancelación");
            InsertaErrorCode(557, "El pago diferido no existe");
            InsertaErrorCode(558, "Error inesperado");
            InsertaErrorCode(559, "Error inesperado");
            InsertaErrorCode(560, "Error inesperado");
            InsertaErrorCode(561, "Error inesperado");
            InsertaErrorCode(562, "La tarjeta no admite preautorizaciones");
            InsertaErrorCode(563, "Inconsistencia de datos en confirmación");
            InsertaErrorCode(564, "Error inesperado");
            InsertaErrorCode(565, "Error inesperado");
            InsertaErrorCode(567, "Operación de devolución no definida correctamente");
            InsertaErrorCode(568, "Comunicación online incorrecta");
            InsertaErrorCode(569, "Operación denegada");
            InsertaErrorCode(1000, "Cuenta no encontrada. Revise su configuración");
            InsertaErrorCode(1001, "Usuario no encontrado. Contacte con PAYCOMET");
            InsertaErrorCode(1002, "Error en respuesta de pasarela. Contacte con PAYCOMET");
            InsertaErrorCode(1003, "Firma no válida. Por favor, revise su configuración");
            InsertaErrorCode(1004, "Acceso no permitido");
            InsertaErrorCode(1005, "Formato de tarjeta de crédito no válido");
            InsertaErrorCode(1006, "Error en el campo Código de Validación");
            InsertaErrorCode(1007, "Error en el campo Fecha de Caducidad");
            InsertaErrorCode(1008, "Referencia de preautorización no encontrada");
            InsertaErrorCode(1009, "Datos de preautorización no encontrados");
            InsertaErrorCode(1010, "No se pudo enviar la devolución. Por favor reinténtelo más tarde");
            InsertaErrorCode(1011, "No se pudo conectar con el host");
            InsertaErrorCode(1012, "No se pudo resolver el proxy");
            InsertaErrorCode(1013, "No se pudo resolver el host");
            InsertaErrorCode(1014, "Inicialización fallida");
            InsertaErrorCode(1015, "No se ha encontrado el recurso HTTP");
            InsertaErrorCode(1016, "El rango de opciones no es válido para la transferencia HTTP");
            InsertaErrorCode(1017, "No se construyó correctamente el POST");
            InsertaErrorCode(1018, "El nombre de usuario no se encuentra bien formateado");
            InsertaErrorCode(1019, "Se agotó el tiempo de espera en la petición");
            InsertaErrorCode(1020, "Sin memoria");
            InsertaErrorCode(1021, "No se pudo conectar al servidor SSL");
            InsertaErrorCode(1022, "Protocolo no soportado");
            InsertaErrorCode(1023, "La URL dada no está bien formateada y no puede usarse");
            InsertaErrorCode(1024, "El usuario en la URL se formateó de manera incorrecta");
            InsertaErrorCode(1025, "No se pudo registrar ningún recurso disponible para completar la operación");
            InsertaErrorCode(1026, "Referencia externa duplicada");
            InsertaErrorCode(1027, "El total de las devoluciones no puede superar la operación original");
            InsertaErrorCode(1028, "La cuenta no se encuentra activa. Contacte con PAYCOMET");
            InsertaErrorCode(1029, "La cuenta no se encuentra certificada. Contacte con PAYCOMET");
            InsertaErrorCode(1030, "El producto está marcado para eliminar y no puede ser utilizado");
            InsertaErrorCode(1031, "Permisos insuficientes");
            InsertaErrorCode(1032, "El producto no puede ser utilizado en el entorno de pruebas");
            InsertaErrorCode(1033, "El producto no puede ser utilizado en el entorno de producción");
            InsertaErrorCode(1034, "No ha sido posible enviar la petición de devolución");
            InsertaErrorCode(1035, "Error en el campo IP de origen de la operación");
            InsertaErrorCode(1036, "Error en formato XML");
            InsertaErrorCode(1037, "El elemento raíz no es correcto");
            InsertaErrorCode(1038, "Campo DS_MERCHANT_AMOUNT incorrecto");
            InsertaErrorCode(1039, "Campo DS_MERCHANT_ORDER incorrecto");
            InsertaErrorCode(1040, "Campo DS_MERCHANT_MERCHANTCODE incorrecto");
            InsertaErrorCode(1041, "Campo DS_MERCHANT_CURRENCY incorrecto");
            InsertaErrorCode(1042, "Campo DS_MERCHANT_PAN incorrecto");
            InsertaErrorCode(1043, "Campo DS_MERCHANT_CVV2 incorrecto");
            InsertaErrorCode(1044, "Campo DS_MERCHANT_TRANSACTIONTYPE incorrecto");
            InsertaErrorCode(1045, "Campo DS_MERCHANT_TERMINAL incorrecto");
            InsertaErrorCode(1046, "Campo DS_MERCHANT_EXPIRYDATE incorrecto");
            InsertaErrorCode(1047, "Campo DS_MERCHANT_MERCHANTSIGNATURE incorrecto");
            InsertaErrorCode(1048, "Campo DS_ORIGINAL_IP incorrecto");
            InsertaErrorCode(1049, "No se encuentra el cliente");
            InsertaErrorCode(1050, "La nueva cantidad a preautorizar no puede superar la cantidad de la preautorización original");
            InsertaErrorCode(1099, "Error inesperado");
            InsertaErrorCode(1100, "Limite diario por tarjeta excedido");
            InsertaErrorCode(1103, "Error en el campo ACCOUNT");
            InsertaErrorCode(1104, "Error en el campo USERCODE");
            InsertaErrorCode(1105, "Error en el campo TERMINAL");
            InsertaErrorCode(1106, "Error en el campo OPERATION");
            InsertaErrorCode(1107, "Error en el campo REFERENCE");
            InsertaErrorCode(1108, "Error en el campo AMOUNT");
            InsertaErrorCode(1109, "Error en el campo CURRENCY");
            InsertaErrorCode(1110, "Error en el campo SIGNATURE");
            InsertaErrorCode(1120, "Operación no disponible");
            InsertaErrorCode(1121, "No se encuentra el cliente");
            InsertaErrorCode(1122, "Usuario no encontrado. Contacte con PAYCOMET");
            InsertaErrorCode(1123, "Firma no válida. Por favor, revise su configuración");
            InsertaErrorCode(1124, "Operación no disponible con el usuario especificado");
            InsertaErrorCode(1125, "Operación no válida con una moneda distinta de la fijada en el producto");
            InsertaErrorCode(1127, "Cantidad cero o no válida");
            InsertaErrorCode(1128, "Conversión de la moneda actual no válida");
            InsertaErrorCode(1129, "Cantidad no válida");
            InsertaErrorCode(1130, "No se encuentra el producto");
            InsertaErrorCode(1131, "Operación no válida con la moneda actual");
            InsertaErrorCode(1132, "Operación no válida con una moneda distinta de la fijada en el producto");
            InsertaErrorCode(1133, "Información del botón corrupta");
            InsertaErrorCode(1134, "La subscripción no puede ser mayor de la fecha de caducidad de la tarjeta");
            InsertaErrorCode(1135, "DS_EXECUTE no puede ser true si DS_SUBSCRIPTION_STARTDATE es diferente de hoy.");
            InsertaErrorCode(1136, "Error en el campo PAYTPV_OPERATIONS_MERCHANTCODE");
            InsertaErrorCode(1137, "PAYTPV_OPERATIONS_TERMINAL debe ser Array");
            InsertaErrorCode(1138, "PAYTPV_OPERATIONS_OPERATIONS debe ser Array");
            InsertaErrorCode(1139, "Error en el campo PAYTPV_OPERATIONS_SIGNATURE");
            InsertaErrorCode(1140, "No se encuentra alguno de los PAYTPV_OPERATIONS_TERMINAL");
            InsertaErrorCode(1141, "Error en el intervalo de fechas solicitado");
            InsertaErrorCode(1142, "La solicitud no puede tener un intervalo mayor a 6 meses");
            InsertaErrorCode(1143, "El estado de la operación es incorrecto");
            InsertaErrorCode(1144, "Error en los importes de la búsqueda");
            InsertaErrorCode(1145, "El tipo de operación solicitado no existe");
            InsertaErrorCode(1146, "Tipo de ordenación no reconocido");
            InsertaErrorCode(1147, "PAYTPV_OPERATIONS_SORTORDER no válido");
            InsertaErrorCode(1148, "Fecha de inicio de suscripción errónea");
            InsertaErrorCode(1149, "Fecha de final de suscripción errónea");
            InsertaErrorCode(1150, "Error en la periodicidad de la suscripción");
            InsertaErrorCode(1151, "Falta el parámetro usuarioXML");
            InsertaErrorCode(1152, "Falta el parámetro codigoCliente");
            InsertaErrorCode(1153, "Falta el parámetro usuarios");
            InsertaErrorCode(1154, "Falta el parámetro firma");
            InsertaErrorCode(1155, "El parámetro usuarios no tiene el formato correcto");
            InsertaErrorCode(1156, "Falta el parámetro type");
            InsertaErrorCode(1157, "Falta el parámetro name");
            InsertaErrorCode(1158, "Falta el parámetro surname");
            InsertaErrorCode(1159, "Falta el parámetro email");
            InsertaErrorCode(1160, "Falta el parámetro password");
            InsertaErrorCode(1161, "Falta el parámetro language");
            InsertaErrorCode(1162, "Falta el parámetro maxamount o su valor no puede ser 0");
            InsertaErrorCode(1163, "Falta el parámetro multicurrency");
            InsertaErrorCode(1165, "El parámetro permissions_specs no tiene el formato correcto");
            InsertaErrorCode(1166, "El parámetro permissions_products no tiene el formato correcto");
            InsertaErrorCode(1167, "El parámetro email no parece una dirección válida");
            InsertaErrorCode(1168, "El parámetro password no tiene la fortaleza suficiente");
            InsertaErrorCode(1169, "El valor del parámetro type no está admitido");
            InsertaErrorCode(1170, "El valor del parámetro language no está admitido");
            InsertaErrorCode(1171, "El formato del parámetro maxamount no está permitido");
            InsertaErrorCode(1172, "El valor del parámetro multicurrency no está admitido");
            InsertaErrorCode(1173, "El valor del parámetro permission_id - permissions_specs no está admitido");
            InsertaErrorCode(1174, "No existe el usuario");
            InsertaErrorCode(1175, "El usuario no tiene permisos para acceder al método altaUsario");
            InsertaErrorCode(1176, "No se encuentra la cuenta de cliente");
            InsertaErrorCode(1177, "No se pudo cargar el usuario de la cuenta");
            InsertaErrorCode(1178, "La firma no es correcta");
            InsertaErrorCode(1179, "No existen productos asociados a la cuenta");
            InsertaErrorCode(1180, "El valor del parámetro product_id - permissions_products no está autorizado");
            InsertaErrorCode(1181, "El valor del parámetro permission_id -permissions_products no está admitido");
            InsertaErrorCode(1185, "Límite mínimo por operación no permitido");
            InsertaErrorCode(1186, "Límite máximo por operación no permitido");
            InsertaErrorCode(1187, "Límite máximo diario no permitido");
            InsertaErrorCode(1188, "Límite máximo mensual no permitido");
            InsertaErrorCode(1189, "Cantidad máxima por tarjeta / 24h. no permitida");
            InsertaErrorCode(1190, "Cantidad máxima por tarjeta / 24h. / misma dirección IP no permitida");
            InsertaErrorCode(1191, "Límite de transacciones por dirección IP /día (diferentes tarjetas) no permitido");
            InsertaErrorCode(1192, "País no admitido (dirección IP del cliente)");
            InsertaErrorCode(1193, "Tipo de tarjeta (crédito / débito) no admitido");
            InsertaErrorCode(1194, "Marca de la tarjeta no admitida");
            InsertaErrorCode(1195, "Categoría de la tarjeta no admitida");
            InsertaErrorCode(1196, "Transacción desde país distinto al emisor de la tarjeta no admitida");
            InsertaErrorCode(1197, "Operación denegada. Filtro país emisor de la tarjeta no admitido");
            InsertaErrorCode(1198, "Superado el límite de scoring");
            InsertaErrorCode(1200, "Operación denegada. Filtro misma tarjeta, distinto país en las últimas 24 horas");
            InsertaErrorCode(1201, "Número de intentos consecutivos erróneos con la misma tarjeta excedidos");
            InsertaErrorCode(1202, "Número de intentos fallidos (últimos 30 minutos) desde la misma dirección ip excedidos");
            InsertaErrorCode(1203, "Las credenciales no son válidas o no están configuradas");
            InsertaErrorCode(1204, "Recibido token incorrecto");
            InsertaErrorCode(1205, "No ha sido posible realizar la operación");
            InsertaErrorCode(1206, "providerID no disponible");
            InsertaErrorCode(1207, "Falta el parámetro operaciones o no tiene el formato correcto");
            InsertaErrorCode(1208, "Falta el parámetro paycometMerchant");
            InsertaErrorCode(1209, "Falta el parámetro merchatID");
            InsertaErrorCode(1210, "Falta el parámetro terminalID");
            InsertaErrorCode(1211, "Falta el parámetro tpvID");
            InsertaErrorCode(1212, "Falta el parámetro operationType");
            InsertaErrorCode(1213, "Falta el parámetro operationResult");
            InsertaErrorCode(1214, "Falta el parámetro operationAmount");
            InsertaErrorCode(1215, "Falta el parámetro operationCurrency");
            InsertaErrorCode(1216, "Falta el parámetro operationDatetime");
            InsertaErrorCode(1217, "Falta el parámetro originalAmount");
            InsertaErrorCode(1218, "Falta el parámetro pan");
            InsertaErrorCode(1219, "Falta el parámetro expiryDate");
            InsertaErrorCode(1220, "Falta el parámetro reference");
            InsertaErrorCode(1221, "Falta el parámetro signature");
            InsertaErrorCode(1222, "Falta el parámetro originalIP o no tiene el formato correcto");
            InsertaErrorCode(1223, "Falta el parámetro authCode o errorCode");
            InsertaErrorCode(1224, "No se encuentra el producto de la operación");
            InsertaErrorCode(1225, "El tipo de la operación no está admitido");
            InsertaErrorCode(1226, "El resultado de la operación no está admitido");
            InsertaErrorCode(1227, "La moneda de la operación no está admitida");
            InsertaErrorCode(1228, "La fecha de la operación no tiene el formato correcto");
            InsertaErrorCode(1229, "La firma no es correcta");
            InsertaErrorCode(1230, "No se encuentra información de la cuenta asociada");
            InsertaErrorCode(1231, "No se encuentra información del producto asociado");
            InsertaErrorCode(1232, "No se encuentra información del usuario asociado");
            InsertaErrorCode(1233, "El producto no está configurado como multimoneda");
            InsertaErrorCode(1234, "La cantidad de la operación no tiene el formato correcto");
            InsertaErrorCode(1235, "La cantidad original de la operación no tiene el formato correcto");
            InsertaErrorCode(1236, "La tarjeta no tiene el formato correcto");
            InsertaErrorCode(1237, "La fecha de caducidad de la tarjeta no tiene el formato correcto");
            InsertaErrorCode(1238, "No puede inicializarse el servicio");
            InsertaErrorCode(1239, "No puede inicializarse el servicio");
            InsertaErrorCode(1240, "Método no implementado");
            InsertaErrorCode(1241, "No puede inicializarse el servicio");
            InsertaErrorCode(1242, "No puede finalizarse el servicio");
            InsertaErrorCode(1243, "Falta el parámetro operationCode");
            InsertaErrorCode(1244, "Falta el parámetro bankName");
            InsertaErrorCode(1245, "Falta el parámetro csb");
            InsertaErrorCode(1246, "Falta el parámetro userReference");
            InsertaErrorCode(1247, "No se encuentra el FUC enviado");
            InsertaErrorCode(1248, "Referencia externa duplicada. Operación en curso.");
            InsertaErrorCode(1249, "No se encuentra el parámetro [DS_]AGENT_FEE");
            InsertaErrorCode(1250, "El parámetro [DS_]AGENT_FEE no tienen el formato correcto");
            InsertaErrorCode(1251, "El parámetro DS_AGENT_FEE no es correcto");
            InsertaErrorCode(1252, "No se encuentra el parámetro CANCEL_URL");
            InsertaErrorCode(1253, "El parámetro CANCEL_URL no es correcto");
            InsertaErrorCode(1254, "Comercio con titular seguro y titular sin clave de compra segura");
            InsertaErrorCode(1255, "Llamada finalizada por el cliente");
            InsertaErrorCode(1256, "Llamada finalizada, intentos incorrectos excedidos");
            InsertaErrorCode(1257, "Llamada finalizada, intentos de operación excedidos");
            InsertaErrorCode(1258, "stationID no disponible");
            InsertaErrorCode(1259, "No ha sido posible establecer la sesión IVR");
            InsertaErrorCode(1260, "Falta el parámetro merchantCode");
            InsertaErrorCode(1261, "El parámetro merchantCode no es correcto");
            InsertaErrorCode(1262, "Falta el parámetro terminalIDDebtor");
            InsertaErrorCode(1263, "Falta el parámetro terminalIDCreditor");
            InsertaErrorCode(1264, "No dispone de permisos para realizar la operación");
            InsertaErrorCode(1265, "La cuenta Iban (terminalIDDebtor) no es válida");
            InsertaErrorCode(1266, "La cuenta Iban (terminalIDCreditor) no es válida");
            InsertaErrorCode(1267, "El BicCode de la cuenta Iban (terminalIDDebtor) no es válido");
            InsertaErrorCode(1268, "El BicCode de la cuenta Iban (terminalIDCreditor) no es válido");
            InsertaErrorCode(1269, "Falta el parámetro operationOrder");
            InsertaErrorCode(1270, "El parámetro operationOrder no tiene el formato correcto");
            InsertaErrorCode(1271, "El parámetro operationAmount no tiene el formato correcto");
            InsertaErrorCode(1272, "El parámetro operationDatetime no tiene el formato correcto");
            InsertaErrorCode(1273, "El parámetro operationConcept contiene caracteres inválidos o excede de 140 caracteres");
            InsertaErrorCode(1274, "No ha sido posible grabar la operación SEPA");
            InsertaErrorCode(1275, "No ha sido posible grabar la operación SEPA");
            InsertaErrorCode(1276, "No ha sido posible generar un token de operación");
            InsertaErrorCode(1277, "Valor de scoring no válido");
            InsertaErrorCode(1278, "El formato del parámetro idioma no es correcto");
            InsertaErrorCode(1279, "El formato del Titular de la tarjeta no es correcto");
            InsertaErrorCode(1280, "El número de tarjeta no es correcto");
            InsertaErrorCode(1281, "El formato del mes no es correcto");
            InsertaErrorCode(1282, "El formato del a√±o no es correcto");
            InsertaErrorCode(1283, "El formato del cvc2 no es correcto");
            InsertaErrorCode(1284, "El formato del parámetro JETID no es correcto");
            InsertaErrorCode(1288, "Parámetro splitId no válido");
            InsertaErrorCode(1289, "Parámetro splitId no autorizado");
            InsertaErrorCode(1290, "Este terminal no permite (split) transfers");
            InsertaErrorCode(1291, "No ha sido posible grabar la operación (split) transfer");
            InsertaErrorCode(1292, "La fecha de la operación original no puede superar 90 días");
            InsertaErrorCode(1293, "(Split) Transfer original no encontrada");
            InsertaErrorCode(1294, "El total de las revocaciones no puede superar el (split) transfer original");
            InsertaErrorCode(1295, "No ha sido posible grabar la operación (split) transfer reversal");
            InsertaErrorCode(1296, "Falta el parámetro uniqueIdCreditor");
            InsertaErrorCode(1297, "La cuenta bancaria no está certificada.");
            InsertaErrorCode(1298, "Falta el parámetro companyNameCreditor.");
            InsertaErrorCode(1299, "El parámetro companyTypeCreditor no es válido.");
            InsertaErrorCode(1300, "El parámetro swiftCodeCreditor no es válido.");
            InsertaErrorCode(1301, "Se ha excedido el número de operaciones por petición.");
            InsertaErrorCode(1302, "Operación denegada. Filtro límite de operaciones por IP últimas 24 horas.");
            InsertaErrorCode(1303, "Operación denegada. Filtro acumulado importe por IP últimas 24 horas.");
            InsertaErrorCode(1304, "La cuenta no está configurada correctamente.");
            InsertaErrorCode(1305, "Falta el parámetro merchantCustomerId.");
            InsertaErrorCode(1306, "El parámetro merchantCustomerIban no es válido.");
            InsertaErrorCode(1307, "Falta el parámetro fileContent.");
            InsertaErrorCode(1308, "Extensión de documento no válida.");
            InsertaErrorCode(1309, "El documento excede el tama√±o máximo.");
            InsertaErrorCode(1310, "El tipo de documento no es válido.");
            InsertaErrorCode(1311, "Límite de operaciones por referencia / diferente IP no permitido");
            InsertaErrorCode(1312, "Operación SEPA Credit Transfer denegada");
            InsertaErrorCode(1313, "No existe payment_info");
            InsertaErrorCode(1314, "El tipo de cuenta bancaria no es IBAN");
            InsertaErrorCode(1315, "No se han encontrado documentos");
            InsertaErrorCode(1316, "Error en la subida de documentos");
            InsertaErrorCode(1317, "Error en la descarga de documentos");
            InsertaErrorCode(1318, "La documentación requerida está incompleta");
            InsertaErrorCode(1319, "No permitido. La moneda no es EUR");
            InsertaErrorCode(1320, "El estado de la factura no es COMPLETE");
            InsertaErrorCode(1321, "La excepción enviada no esta habilitada");
            InsertaErrorCode(1322, "Se requiere challenge para finalizar la operación");
            InsertaErrorCode(1323, "La información obligatoria del MERCHANT_DATA no se ha enviado");
            InsertaErrorCode(1324, "El parámetro DS_USER_INTERACTION no es válido");
            InsertaErrorCode(1325, "Challenge requerido y usuario no presente");
            InsertaErrorCode(1326, "Denegación por controles de seguridad en el procesador");
            InsertaErrorCode(1327, "Datos de operación EMV3DS incorrectos o no indicados");
            InsertaErrorCode(1328, "Error en la recepción de parámetros: debe ser EMAIL o SMS");
            InsertaErrorCode(1329, "Ha fallado el envío del email");
            InsertaErrorCode(1330, "Ha fallado el envío del SMS");
            InsertaErrorCode(1331, "Plantilla no encontrada");
            InsertaErrorCode(1332, "Alcanzado límite de peticiones por minuto");
            InsertaErrorCode(1333, "Móvil no configurado para el envío de SMS en Sandbox");
            InsertaErrorCode(1334, "Email no configurado para el envío de Emails en Sandbox");
            InsertaErrorCode(1335, "No se encuentra el parámetro DS_MERCHANT_IDENTIFIER");
            InsertaErrorCode(1336, "El parámetro DS_MERCHANT_IDENTIFIER no es correcto");
            InsertaErrorCode(1337, "Ruta de notificación no configurada");
            InsertaErrorCode(1338, "Ruta de notificación no responde correctamente");
            InsertaErrorCode(1339, "Configuración de terminales incorrecta");
            InsertaErrorCode(1340, "Método de pago no disponible");
            InsertaErrorCode(1341, "Bizum. Fallo en la autenticación. Bloqueo tras tres intentos.");
            InsertaErrorCode(1342, "Bizum. Operación cancelada. El usuario no desea seguir.");
            InsertaErrorCode(1343, "Bizum. Abono rechazado por beneficiario.");
            InsertaErrorCode(1344, "Bizum. Cargo rechazado por ordenante.");
            InsertaErrorCode(1345, "Bizum. El procesador rechaza la operación.");
            InsertaErrorCode(1346, "Bizum. Saldo disponible insuficiente.");
            InsertaErrorCode(1351, "PSD2: Campo billAddrCountry no incluido con billAddrState suministrado");
            InsertaErrorCode(1352, "PSD2: Campo shipAddrCountry no incluido con shipAddrState suministrado");
            InsertaErrorCode(1353, "PSD2: Campo billAddrCountry debe ser en formato ISO 3166-1 alfa-3");
            InsertaErrorCode(1354, "PSD2: Campo billAddrState debe ser en formato ISO 3166-2");
            InsertaErrorCode(1355, "PSD2: Campo shipAddrCountry debe ser en formato ISO 3166-1 alfa-3");
            InsertaErrorCode(1356, "PSD2: Campo shipAddrState debe ser en formato ISO 3166-2");
            InsertaErrorCode(1360, "PSD2: Campo MERCHANT_SCA_EXCEPTION con valor incorrecto");
            InsertaErrorCode(1361, "PSD2: No se suministra MERCHANT_TRX_TYPE obligatorio con MERCHANT_SCA_EXCEPTION=MIT");
            InsertaErrorCode(1362, "PSD2: Campo MERCHANT_TRX_TYPE con valor incorrecto");
            InsertaErrorCode(1363, "PSD2: Campos purchaseInstalData, recurringExpiry y recurringFrequency obligatorios en operación Instalment (MERCHANT_TRX_TYPE = I)");
            InsertaErrorCode(1364, "PSD2: Campos purchaseInstalData, recurringExpiry y recurringFrequency obligatorios en operación Recurring (MERCHANT_TRX_TYPE = R)");
            InsertaErrorCode(1365, "PSD2: El valor purchaseInstalData debe ser mayor que 1");
            InsertaErrorCode(1366, "PSD2: El valor recurringExpiry debe tener el formato AAAAMMDD");
            InsertaErrorCode(1400, "Bizum no ha podido autenticar el usuario");
        }
        internal List<Categoria> GetCategorias()
        {
            try
            {
                List<Categoria> list = dbConn.Query<Categoria>("Select * From [Categoria] order by orden");
                if (list.Count > 0)
                {
                    return list;
                }

                return new List<Categoria>();

            }
            catch (Exception)
            {
                return new List<Categoria>();
            }
        }
        internal List<ZonasRepartidorModel> GetZonasRepartidor(int idRepartidor)
        {
            try
            {
                List<ZonasRepartidorModel> list = dbConn.Query<ZonasRepartidorModel>("Select * From [ZonasRepartidorModel] Where idRepartidor=" + idRepartidor);
                if (list.Count > 0)
                {
                    return list;
                }

                return new List<ZonasRepartidorModel>();

            }
            catch (Exception)
            {
                return new List<ZonasRepartidorModel>();
            }
        }
        internal List<AlergenosModel> GetAlergenos()
        {
            try
            {
                List<AlergenosModel> list = dbConn.Query<AlergenosModel>("Select * From [AlergenosModel]");
                if (list.Count > 0)
                {
                    return list;
                }
                else
                    return ResponseServiceWS.GetAlergenos();

            }
            catch (Exception)
            {
                return new List<AlergenosModel>();
            }
        }
        internal EstablecimientoFiscalModel GetDatosFiscalesEstablecimiento(int idEstablecimiento)
        {
            try
            {
                List<EstablecimientoFiscalModel> list = dbConn.Query<EstablecimientoFiscalModel>("Select * From [EstablecimientoFiscalModel] Where idEstablecimiento=" + idEstablecimiento);
                if (list.Count > 0)
                {
                    return list.FirstOrDefault();
                }
                else
                    return new EstablecimientoFiscalModel();

            }
            catch (Exception)
            {
                return new EstablecimientoFiscalModel();
            }
        }
        internal string GetCodigo()
        {
            int longitud = App.global.longitudCodigo;
            Guid miGuid = Guid.NewGuid();
            string token = Convert.ToBase64String(miGuid.ToByteArray());
            token = token.Replace("=", "").Replace("+", "").Replace("/", "").Replace("?", "").Replace("&", "").Replace(".", "");

            return token.Substring(0, longitud);
        }

        internal List<TarjetaModel> GetTarjetas(int idUsuario)
        {
            try
            {
                List<TarjetaModel> list = dbConn.Query<TarjetaModel>("Select * From [TarjetaModel] Where idUsuario=" + idUsuario);
                if (list.Count > 0)
                {
                    return list;
                }

                return new List<TarjetaModel>();

            }
            catch (Exception)
            {
                return new List<TarjetaModel>();
            }
        }
        internal ConfiguracionAdmin GetConfiguracionAdminSQLite()
        {
            try
            {
                ConfiguracionAdmin c = dbConn.Query<ConfiguracionAdmin>("Select * From [ConfiguracionAdmin]").FirstOrDefault();
                if (c != null)
                {
                    return c;
                }
                else
                    return App.ResponseWS.getConfiguracionGlobal();

                return new ConfiguracionAdmin();

            }
            catch (Exception)
            {
                return new ConfiguracionAdmin();

            }
        }

        internal List<PedidoModel> getListPedidos()
        {
            try
            {
                List<PedidoModel> listPedido = dbConn.Query<PedidoModel>("Select * From [PedidoModel]");
                if (listPedido.Count > 0)
                {
                    return listPedido;
                }

                return new List<PedidoModel>();

            }
            catch (Exception)
            {
                return new List<PedidoModel>();
            }
        }
        internal List<MensajesModel> getMensajes()
        {
            try
            {
                List<MensajesModel> mensajes = dbConn.Query<MensajesModel>("Select * From [MensajesModel]");
                if (mensajes.Count > 0)
                {
                    foreach (MensajesModel m in mensajes)
                    {
                        if (App.idioma.Equals("EN") || App.idioma.Equals("GB") || App.idioma.Equals("US"))
                            m.valor = m.valor_eng;
                        else if (App.idioma.Equals("FR"))
                            m.valor = m.valor_fr;
                        else if (App.idioma.Equals("DE"))
                            m.valor = m.valor_ger;
                    }
                    return mensajes;
                }
                else
                    return App.ResponseWS.getMensajes();
            }
            catch (Exception)
            {
                return new List<MensajesModel>();
            }
        }
        internal List<PredefinidosModel> getMensajesPredefinidos()
        {
            try
            {
                List<PredefinidosModel> mensajes = dbConn.Query<PredefinidosModel>("Select * From [PredefinidosModel]");
                if (mensajes.Count > 0)
                {
                    return mensajes;
                }
                else
                    return App.ResponseWS.getMensajesPredefinidos();
            }
            catch (Exception)
            {
                return new List<PredefinidosModel>();
            }
        }

        internal void GuardarPedido(List<PedidoModel> pedidosModel)
        {
            try
            {
                dbConn.InsertAll(pedidosModel);
                dbConn.Commit();
            }
            catch (Exception)
            {
            }
        }
        internal List<CodeErrors> getErroresPaycomet()
        {
            try
            {
                List<CodeErrors> listPedido = dbConn.Query<CodeErrors>("Select * From [CodeErrors]");
                if (listPedido.Count > 0)
                {
                    return listPedido;
                }

                return new List<CodeErrors>();

            }
            catch (Exception)
            {
                return new List<CodeErrors>();
            }
        }
        internal void InsertaErrorCode(int codigo, string error)
        {
            try
            {
                CodeErrors c = new CodeErrors();
                c.errorCode = codigo;
                c.textError = error;
                dbConn.Insert(c);
                dbConn.Commit();
            }
            catch (Exception)
            {
            }
        }
        internal void GuardaAlergenos(List<AlergenosModel> pedidosModel)
        {
            try
            {
                foreach (AlergenosModel a in pedidosModel)
                    dbConn.InsertOrReplace(a);
                dbConn.Commit();
            }
            catch (Exception)
            {
            }
        }
        internal bool GuardaDatosFiscalesEstablecimiento(EstablecimientoFiscalModel fiscal)
        {
            try
            {
                dbConn.InsertOrReplace(fiscal);
                dbConn.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private UsuarioModel usuario;
        public UsuarioModel Usuario
        {
            get { return usuario; }
            set { usuario = value; }
        }

        private string installID;

        public string InstallId
        {
            get { return installID; }
            set { installID = value; }
        }

        internal List<CarritoModel> Getcarrito()
        {
            try
            {
                List<CarritoModel> list = dbConn.Query<CarritoModel>("Select * From [CarritoModel]");
                if (list.Count > 0)
                {
                    return list;
                }

                return new List<CarritoModel>();

            }
            catch (Exception)
            {
                return new List<CarritoModel>();
            }
        }
        internal List<ReservaModel> GetReservas()
        {
            try
            {
                List<ReservaModel> list = dbConn.Query<ReservaModel>("Select * From [ReservaModel] order by fecha desc");
                if (list.Count > 0)
                {
                    return list;
                }

                return new List<ReservaModel>();

            }
            catch (Exception)
            {
                return new List<ReservaModel>();
            }
        }

        internal ObservableCollection<CabeceraPedido> traeMisPedidos()
        {
            try
            {
                List<Pedido> listPedido = dbConn.Query<Pedido>("Select * From [Pedido] WHERE idEstadoPedido<>3 and idEstadoPedido<>4");
                if (listPedido.Count > 0)
                {
                    if (listPedido.Count > 0)
                        return convertirToPedidoInterno(listPedido);
                }

                return new ObservableCollection<CabeceraPedido>();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ObservableCollection<CabeceraPedido>();
            }
        }

        internal ObservableCollection<CabeceraPedido> getPedidoPendienteEstadoRecogido()
        {
            try
            {
                List<Pedido> listPedido = dbConn.Query<Pedido>("Select * From [Pedido] WHERE idEstadoPedido = 4 ");
                if (listPedido.Count > 0)
                {
                    if (listPedido.Count > 0)
                        return convertirToPedidoInterno(listPedido);
                }

                return new ObservableCollection<CabeceraPedido>();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ObservableCollection<CabeceraPedido>();
            }
        }


        internal async Task<List<ArticuloModel>> getProductosEstablecimiento(int id)
        {
            try
            {
                List<ArticuloModel> list = dbConn.Query<ArticuloModel>("Select * From [ArticuloModel] WHERE idEstablecimiento=" + id + " and estado=1");
                if (list.Count > 0)
                {
                    if (list.Count > 0)
                        return await convertirArticuloEnProducto(list);
                }

                return new List<ArticuloModel>();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ArticuloModel>();
            }
        }

        internal async Task<List<ArticuloModel>> getProductosEstablecimientoCat(int id)
        {
            try
            {
                List<ArticuloModel> list = dbConn.Query<ArticuloModel>("Select * From [ArticuloModel] WHERE idCategoria=" + id + " and estado=1");
                if (list.Count > 0)
                {
                    if (list.Count > 0)
                        return await convertirArticuloEnProducto(list);
                }

                return new List<ArticuloModel>();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ArticuloModel>();
            }
        }
        internal async Task<List<ArticuloModel>> getProductosTodosEstablecimiento(int id)
        {
            try
            {
                List<ArticuloModel> list = dbConn.Query<ArticuloModel>("Select * From [ArticuloModel] WHERE idEstablecimiento=" + id);
                if (list.Count > 0)
                {
                    if (list.Count > 0)
                        return await convertirArticuloEnProducto(list);
                }

                return new List<ArticuloModel>();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ArticuloModel>();
            }
        }
        internal List<ZonaModel> getZonas()
        {
            try
            {
                List<ZonaModel> list = dbConn.Query<ZonaModel>("Select * From [ZonaModel] order by nombre");
                if (list.Count > 0)
                {
                    return list;
                }
                else
                {
                    return App.ResponseWS.getListadoZonas(Preferences.Get("idPueblo", 0));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ZonaModel>();
            }
        }

        internal async Task<List<IngredienteProductoModel>> GetIngredienteProducto(int idProducto)
        {
            try
            {
                /*List<IngredienteProductoModel> list = dbConn.Query<IngredienteProductoModel>("Select * From [IngredienteProductoModel] WHERE idProducto=" + idProducto + " order by nombre");
                if (list.Count > 0)
                {
                    return list;
                }
                else
                {*/
                List<IngredienteProductoModel> list = await App.ResponseWS.listadoIngredientesProducto(idProducto);
                if (list.Count > 0)
                {
                    return list;
                }
                else
                    return new List<IngredienteProductoModel>();
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<IngredienteProductoModel>();
            }
        }
        internal List<IngredientesModel> GetIngredientes(int idEst)
        {
            try
            {
                List<IngredientesModel> list = dbConn.Query<IngredientesModel>("Select * From [IngredientesModel] WHERE idEstablecimiento=" + idEst + " order by nombre");
                if (list.Count > 0)
                {
                    return list;
                }
                else
                {
                    return App.ResponseWS.listadoIngredientes(idEst);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<IngredientesModel>();
            }
        }
        internal List<RepartidorModel> GetRepartidores()
        {
            try
            {
                List<RepartidorModel> list = dbConn.Query<RepartidorModel>("Select * From [RepartidorModel] order by nombre");
                if (list.Count > 0)
                    return list;
                else
                    return App.ResponseWS.listadoRepartidores();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<RepartidorModel>();
            }
        }
        internal List<RepartidorBindableModel> GetRepartidoresBindables()
        {
            try
            {
                List<RepartidorModel> list = dbConn.Query<RepartidorModel>("Select * From [RepartidorModel] order by nombre");
                if (list.Count > 0)
                {
                    List<RepartidorBindableModel> list2 = new List<RepartidorBindableModel>();
                    foreach (RepartidorModel r in list)
                    {
                        RepartidorBindableModel repartidor = new RepartidorBindableModel();
                        repartidor.activo = r.activo;
                        repartidor.Cantidad = 0;
                        repartidor.eliminado = r.eliminado;
                        repartidor.foto = r.foto;
                        repartidor.id = r.id;
                        repartidor.idPueblo = r.idPueblo;
                        repartidor.idGrupo = r.idGrupo;
                        repartidor.idUsuario = r.idUsuario;
                        repartidor.nombre = r.nombre;
                        repartidor.pin = r.pin;
                        list2.Add(repartidor);
                    }
                    return list2;
                }
                else
                {
                    list = App.ResponseWS.listadoRepartidores();
                    if (list.Count > 0)
                    {
                        List<RepartidorBindableModel> list2 = new List<RepartidorBindableModel>();
                        foreach (RepartidorModel r in list)
                        {
                            RepartidorBindableModel repartidor = new RepartidorBindableModel();
                            repartidor.activo = r.activo;
                            repartidor.Cantidad = 0;
                            repartidor.eliminado = r.eliminado;
                            repartidor.foto = r.foto;
                            repartidor.id = r.id;
                            repartidor.idPueblo = r.idPueblo;
                            repartidor.idGrupo = r.idGrupo;
                            repartidor.idUsuario = r.idUsuario;
                            repartidor.nombre = r.nombre;
                            repartidor.pin = r.pin;
                            list2.Add(repartidor);
                        }
                        return list2;
                    }
                    else
                        return new List<RepartidorBindableModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<RepartidorBindableModel>();
            }
        }

        internal async Task<List<ArticuloModel>> getProductosCategoria(string cat)
        {
            try
            {
                List<ArticuloModel> list = dbConn.Query<ArticuloModel>("Select * From [ArticuloModel] WHERE idEstablecimiento=" + App.EstActual.idEstablecimiento + " AND categoria='" + cat + "'");
                if (list.Count > 0)
                {
                    if (list.Count > 0)
                        return await convertirArticuloEnProducto(list);
                }

                return new List<ArticuloModel>();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ArticuloModel>();
            }
        }
        private async Task<List<ArticuloModel>> convertirArticuloEnProducto(List<ArticuloModel> lista)
        {
            try
            {

                int idArt = 0;
                foreach (ArticuloModel p in lista)
                {
                    if (p.listadoAlergenos == null)
                        p.listadoAlergenos = new ObservableCollection<AlergenosModel>();
                    if (p.listadoOpciones == null)
                        p.listadoOpciones = new ObservableCollection<OpcionesModel>();
                    if (p.listadoIngredientes == null)
                        p.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>();

                    if (idArt != p.idArticulo)
                    {
                        idArt = p.idArticulo;

                        if (!string.IsNullOrWhiteSpace(p.opciones))
                        {
                            String[] auxOpc;
                            auxOpc = p.opciones.Split('|');
                            OpcionesModel op;
                            foreach (String a in auxOpc)
                            {
                                String[] auxOpc2;
                                auxOpc2 = a.Split(';');
                                op = new OpcionesModel();
                                op.id = int.Parse(auxOpc2[0]);
                                op.opcion = auxOpc2[1];
                                op.opcion_eng = auxOpc2[4];
                                op.opcion_ger = auxOpc2[5];
                                op.opcion_fr = auxOpc2[6];
                                op.puntos = int.Parse(auxOpc2[7]);
                                op.tipoIncremento = int.Parse(auxOpc2[2]);
                                NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
                                op.precio = auxOpc2[3].Replace(".", ",");
                                op.seleccionado = false;

                                p.listadoOpciones.Add(op);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(p.alergenos))
                        {
                            String[] auxOpc;
                            auxOpc = p.alergenos.Split('|');
                            AlergenosModel op;
                            foreach (String a in auxOpc)
                            {
                                String[] auxOpc2;
                                auxOpc2 = a.Split(';');
                                op = new AlergenosModel();
                                op.id = int.Parse(auxOpc2[0]);
                                op.nombre = auxOpc2[1];
                                op.imagen = auxOpc2[2];

                                p.listadoAlergenos.Add(op);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return lista;
        }

        internal void ActualizaCarrito(List<CarritoModel> carrito)
        {
            try
            {
                dbConn.Execute("DELETE FROM [CarritoModel] Where idEstablecimiento=" + App.EstActual.idEstablecimiento);
                int i = dbConn.InsertAll(carrito);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        internal void SaveZonasRepartidor(List<ZonasRepartidorModel> zonas, int idRepartidor)
        {
            try
            {
                dbConn.Execute("DELETE FROM [ZonasRepartidorModel] Where idRepartidor=" + idRepartidor);
                int i = dbConn.InsertAll(zonas);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        internal bool SaveConfiguracionAdminSQLite(ConfiguracionAdmin c)
        {
            try
            {
                dbConn.InsertOrReplace(c);
                dbConn.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private ObservableCollection<CabeceraPedido> convertirToPedidoInterno(List<Pedido> listPedido)
        {
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            LineasPedido lineaPedido;
            CabeceraPedido cabeceraPedido = new CabeceraPedido();
            foreach (var item in listPedido)
            {
                //if ((item.tipoProducto == 1 && App.DAUtil.Usuario.rol == 6) || (item.tipoProducto == 0 && App.DAUtil.Usuario.rol == 7) || App.DAUtil.Usuario.rol== (int)RolesEnum.Establecimiento)
                //{
                if (result.Count == 0)
                {
                    cabeceraPedido = getCabecera(item);
                    result.Add(cabeceraPedido);
                }
                else
                {
                    var count = 0;
                    count = result.Where(p => p.codigoPedido.Equals(item.codigoPedido)).Count();
                    if (count == 0)
                    {
                        cabeceraPedido = getCabecera(item);
                        result.Add(cabeceraPedido);
                    }
                }
                //}
            }

            foreach (var item in listPedido)
            {
                //if ((item.tipoProducto == 1 && App.DAUtil.Usuario.rol == 6) || (item.tipoProducto == 0 && App.DAUtil.Usuario.rol == 7) || App.DAUtil.Usuario.rol== (int)RolesEnum.Establecimiento)
                //{
                foreach (var item2 in result)
                {
                    if (item.codigoPedido.Equals(item2.codigoPedido))
                    {
                        lineaPedido = new LineasPedido();
                        lineaPedido = getLineaPedido(item, item2.ColorPedido);
                        if (item2.lineasPedidos == null)
                            item2.lineasPedidos = new ObservableCollection<LineasPedido>();
                        if (item2.lineasPedidosAdd == null)
                            item2.lineasPedidosAdd = new ObservableCollection<LineasPedido>();
                        if (lineaPedido.estadoProducto == 3)
                            item2.lineasPedidosAdd.Add(lineaPedido);
                        else
                            item2.lineasPedidos.Add(lineaPedido);
                    }
                }
                //}
            }

            return new ObservableCollection<CabeceraPedido>(result.OrderBy(p => p.horaPedido).ToList());
        }

        internal void saveZonas(List<ZonaModel> listProducto)
        {
            dbConn.DeleteAll<ZonaModel>();
            foreach (ZonaModel c in listProducto)
            {
                dbConn.Insert(c);
            }
            dbConn.Commit();
        }
        internal void savePueblos(List<PueblosModel> listPueblos)
        {
            dbConn.DeleteAll<PueblosModel>();
            dbConn.InsertAll(listPueblos);
            dbConn.Commit();
        }
        internal void saveTarjetas(List<TarjetaModel> listPueblos)
        {
            foreach (TarjetaModel c in listPueblos)
            {
                dbConn.InsertOrReplace(c);
            }
            dbConn.Commit();
        }
        internal void saveCategorias(List<Categoria> cats)
        {
            dbConn.DeleteAll<Categoria>();
            dbConn.InsertAll(cats);
            dbConn.Commit();
        }

        private static CabeceraPedido getCabecera(Pedido item)
        {

            CabeceraPedido cabeceraPedido = new CabeceraPedido();
            cabeceraPedido.idPedido = item.idPedido;
            cabeceraPedido.codigoPedido = item.codigoPedido;
            cabeceraPedido.idEstablecimiento = item.idEstablecimiento;
            cabeceraPedido.idUsuario = item.idUsuario;
            cabeceraPedido.idDetalle = item.idDetalle;
            cabeceraPedido.horaPedido = item.horaPedido;
            cabeceraPedido.comentario = item.comentario;
            cabeceraPedido.fechaEntrega = item.fechaEntrega;
            cabeceraPedido.estadoDetalle = item.estadoDetalle;
            cabeceraPedido.estadoPedido = item.estadoPedido;
            cabeceraPedido.idEstadoPedido = item.idEstadoPedido;
            cabeceraPedido.ColorPedido = item.colorPedido;
            return cabeceraPedido;
        }
        private static LineasPedido getLineaPedido(Pedido item, string color)
        {
            LineasPedido lineaPedido = new LineasPedido();
            lineaPedido.nombreProducto = item.nombreProducto;
            lineaPedido.cantidad = item.cantidad;
            lineaPedido.precio = item.precio;
            lineaPedido.importe = item.importe;
            lineaPedido.desripcionProducto = item.desripcionProducto;
            lineaPedido.imagenProducto = item.imagenProducto;
            lineaPedido.idProducto = item.idProducto;
            lineaPedido.estadoProducto = item.estadoDetalle;
            lineaPedido.tipoComida = item.tipoProducto;
            lineaPedido.ColorPedido = color;
            return lineaPedido;
        }
        internal void saveArticulosEstablecimiento(List<ArticuloModel> lista)
        {
            if (lista.Count > 0)
                dbConn.Query<ArticuloModel>("DELETE FROM [ArticuloModel] WHERE idEstablecimiento=" + lista[0].idEstablecimiento);
            foreach (ArticuloModel c in lista)
            {
                dbConn.Insert(c);
            }
            dbConn.Commit();
        }
        internal void GuardaCarrito(List<CarritoModel> carrito)
        {
            try
            {
                foreach (CarritoModel c in carrito)
                {
                    if (c.id == 0 || c.id == -1)
                        dbConn.Insert(c);
                    else
                    {
                        dbConn.Update(c);
                    }
                    App.DAUtil.config.idEstablecimiento = c.idEstablecimiento;
                    Preferences.Set("idEvento", c.idEvento);
                }

                dbConn.Commit();

            }
            catch (Exception)
            {
            }
        }

        internal TokenResponse getToken()
        {
            try
            {
                List<TokenResponse> listPersona = dbConn.Query<TokenResponse>("Select * From [TokenResponse]");
                if (listPersona.Count > 0)
                {
                    return listPersona.FirstOrDefault<TokenResponse>();
                }

                return new TokenResponse();

            }
            catch (Exception)
            {
                return new TokenResponse();
            }
        }

        private String error;

        public String Error
        {
            get { return error; }
            set { error = value; }
        }

        private static SQLiteConnection dbConn;

        public INavigationService NavigationService { get; }


        private static INavigationService navigationService;

        public List<string> listadoOpcionesPorRol = new List<string>();

        internal bool DoIHaveInternet()
        {
            //TODO TLL
            //return CrossConnectivity.Current.IsConnected;
            return true;
        }



        public bool SaveNotificacionesSQLite(NotificacionModel notificaciones, int tipo)
        {
            try
            {
                if (tipo == 0)
                    dbConn.Insert(notificaciones);
                else
                    dbConn.Update(notificaciones);
                dbConn.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool nuevaReserva(ReservaModel r)
        {
            try
            {
                dbConn.Insert(r);
                dbConn.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool actualizaReservas(List<ReservaModel> r)
        {
            try
            {
                dbConn.DeleteAll<ReservaModel>();
                dbConn.InsertAll(r);
                dbConn.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool actualizaMensajes(List<MensajesModel> r)
        {
            try
            {
                dbConn.DeleteAll<MensajesModel>();
                dbConn.InsertAll(r);
                dbConn.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool actualizaMensajesPredefinidos(List<PredefinidosModel> r)
        {
            try
            {
                dbConn.DeleteAll<PredefinidosModel>();
                dbConn.InsertAll(r);
                dbConn.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool eliminaPedidos()
        {
            try
            {
                dbConn.DeleteAll<Pedido>();
                dbConn.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool savePedidos(List<Pedido> pedidos)
        {
            try
            {
                foreach (Pedido p in pedidos)
                    dbConn.InsertOrReplace(p);
                dbConn.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static int GetInfo(string nombreTabla)
        {

            var info = dbConn.GetTableInfo(nombreTabla);
            return info.Count;

        }

        internal UsuarioModel GetUsuarioSQLite()
        {
            try
            {
                List<UsuarioModel> listPersona = dbConn.Query<UsuarioModel>("Select * From [UsuarioModel]");
                if (listPersona.Count > 0)
                {
                    return listPersona.FirstOrDefault();
                }

                return null;

            }
            catch (SQLite.SQLiteException ex)
            {

                throw ex.InnerException;
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }

        }
        internal Establecimiento GetEstablecimientoSQL(int id)
        {
            try
            {
                List<Establecimiento> listPersona = dbConn.Query<Establecimiento>("Select * From [Establecimiento] where idEstablecimiento=" + id);
                if (listPersona.Count > 0)
                {
                    return listPersona.FirstOrDefault();
                }
                else
                {
                    List<Establecimiento> lis = ResponseServiceWS.getListadoEstablecimientos();
                    SaveEstablecimientos(lis);
                    return lis.Where(p => p.idEstablecimiento == id).FirstOrDefault();
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }

        }
        internal List<FavoritosModel> getFavoritos()
        {
            try
            {
                List<FavoritosModel> listPersona = dbConn.Query<FavoritosModel>("Select * From [FavoritosModel] LIMIT 3");
                if (listPersona.Count > 0)
                {
                    return listPersona;
                }
                return new List<FavoritosModel>();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<FavoritosModel>();
            }

        }
        internal List<Establecimiento> GetEstablecimientosSQL()
        {
            try
            {
                string sSQL = "";
                //if (miLocacilizacion != null)
                //    sSQL = "Select * From [Establecimiento] WHERE distancia<=" + Preferences.Get("km", 30) + " order by distancia";
                //else
                sSQL = "Select * From [Establecimiento] Where idGrupo=" + Preferences.Get("idGrupo", 1) + " order by orden";

                List<Establecimiento> listaEstablecimientos = dbConn.Query<Establecimiento>(sSQL);

                if (listaEstablecimientos == null)
                    return new List<Establecimiento>();
                else
                    return listaEstablecimientos;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<Establecimiento>();
            }

        }
        internal void ObtienePueblo()
        {
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    if (Preferences.Get("idPueblo", App.DAUtil.Usuario.idPueblo) != App.DAUtil.Usuario.idPueblo)
                    {
                        PueblosModel pueblo = App.DAUtil.GetPueblosSQLite().Where(p => p.id == Preferences.Get("idPueblo", App.DAUtil.Usuario.idPueblo)).FirstOrDefault();
                        if (pueblo == null)
                        {
                            pueblo = App.DAUtil.GetPueblosSQLite().Where(p => p.id == App.DAUtil.Usuario.idPueblo).FirstOrDefault();
                        }
                        Preferences.Set("idPueblo", pueblo.id);
                        Preferences.Set("idGrupo", pueblo.idGrupo);
                    }
                    else
                    {
                        PueblosModel pueblo = App.DAUtil.GetPueblosSQLite().Where(p => p.id == App.DAUtil.Usuario.idPueblo).FirstOrDefault();
                        Preferences.Set("idPueblo", App.DAUtil.Usuario.idPueblo);
                        Preferences.Set("idGrupo", pueblo.idGrupo);
                    }
                }
                else
                {
                    PueblosModel pueblo = App.DAUtil.GetPueblosSQLite().Where(p => p.id == Preferences.Get("idPueblo", 0)).FirstOrDefault();
                    Preferences.Set("idGrupo", pueblo.idGrupo);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Preferences.Set("idPueblo", 0);
                Preferences.Set("idGrupo", 0);
            }
        }
        internal List<PueblosModel> GetPueblosSQLite()
        {
            try
            {
                string sSQL = "";
                //if (miLocacilizacion != null)
                //    sSQL = "Select * From [Establecimiento] WHERE distancia<=" + Preferences.Get("km", 30) + " order by distancia";
                //else
                sSQL = "Select * From [PueblosModel] order by nombre";

                List<PueblosModel> listaPueblos = dbConn.Query<PueblosModel>(sSQL);

                if (listaPueblos.Count == 0)
                    return App.ResponseWS.getPueblos();
                else
                    return listaPueblos;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<PueblosModel>();
            }

        }
        internal List<PueblosModel> GetPueblosSQLite(int idPueblo)
        {
            try
            {
                string sSQL = "";
                //if (miLocacilizacion != null)
                //    sSQL = "Select * From [Establecimiento] WHERE distancia<=" + Preferences.Get("km", 30) + " order by distancia";
                //else
                sSQL = "Select * From [PueblosModel] Where id=" + idPueblo + " order by nombre";

                List<PueblosModel> listaPueblos = dbConn.Query<PueblosModel>(sSQL);

                if (listaPueblos == null)
                    return new List<PueblosModel>();
                else
                    return listaPueblos;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<PueblosModel>();
            }

        }
        internal bool SaveConfiguracionUsuarioSQLite(UsuarioModel persona)
        {
            try
            {
                dbConn.DeleteAll<UsuarioModel>();
                int i = dbConn.Insert(persona);
                dbConn.Commit();
                return true;
            }
            catch (Exception ex)
            {

                string error = ex.Message.ToString();
                if (error.Contains("telefono"))
                {
                    dbConn.DropTable<UsuarioModel>();
                    dbConn.CreateTable<UsuarioModel>();
                    dbConn.InsertOrReplace(persona);
                    dbConn.Commit();
                    return true;
                }

                return false;
            }
        }

        internal bool DeleteUsuarioSQLite()
        {
            try
            {

                dbConn.DeleteAll<UsuarioModel>();
                dbConn.DeleteAll<TokenResponse>();
                dbConn.DeleteAll<EstablecimientoFiscalModel>();
                dbConn.DeleteAll<TarjetaModel>();
                dbConn.DeleteAll<ZonaModel>();
                dbConn.DeleteAll<RepartidorModel>();
                dbConn.DeleteAll<IngredientesModel>();
                dbConn.DeleteAll<ZonasRepartidorModel>();
                dbConn.DeleteAll<IngredienteProductoModel>();
                dbConn.DeleteAll<Pedido>();
                dbConn.DeleteAll<ConfiguracionModel>();
                dbConn.DeleteAll<ConfiguracionAdmin>();
                dbConn.DeleteAll<FavoritosModel>();
                dbConn.DeleteAll<NotificacionModel>();
                dbConn.DeleteAll<CarritoModel>();
                dbConn.DeleteAll<ReservaModel>();
                dbConn.DeleteAll<PedidoModel>();
                dbConn.Commit();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        internal bool VaciaCarrito()
        {
            try
            {
                if (App.EstActual != null)
                {
                    dbConn.Execute("DELETE FROM [CarritoModel] Where idEstablecimiento=" + App.EstActual.idEstablecimiento);
                    dbConn.Commit();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        internal bool VaciaConfig()
        {
            try
            {

                dbConn.DeleteAll<ConfiguracionModel>();
                dbConn.Commit();
                config = new ConfiguracionModel();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal bool SaveConfiguracionSQLite(ConfiguracionModel c)
        {
            try
            {
                dbConn.InsertOrReplace(c);
                dbConn.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal void SaveEstablecimientos(List<Establecimiento> listEstablecimientos)
        {
            try
            {
                foreach (Establecimiento e in listEstablecimientos)
                    dbConn.InsertOrReplace(e);
                dbConn.Commit();
            }
            catch (Exception)
            {

            }
        }
        internal void ActualizaEstablecimiento(Establecimiento e)
        {
            try
            {
                dbConn.Update(e);
                dbConn.Commit();
            }
            catch (Exception)
            {

            }
        }
        internal void DeleteEstablecimientos()
        {
            try
            {
                dbConn.DeleteAll<Establecimiento>();
                dbConn.Commit();
            }
            catch (Exception)
            {

            }
        }
        internal void DeleteEstablecimientoCategoria(int id)
        {
            try
            {
                dbConn.Execute("DELETE FROM Establecimiento WHere idCategoria=" + id);
                dbConn.Commit();
            }
            catch (Exception)
            {

            }
        }
        internal bool EnviaEmail(string email, string asunto, string cuerpo)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage xemail = new MailMessage();
                // START
                xemail.From = new MailAddress("polloandaluzapp@gmail.com", "Asador Morón");
                xemail.To.Add(email);
                xemail.Subject = asunto;
                xemail.Body = cuerpo;
                xemail.IsBodyHtml = true;
                //END
                SmtpServer.Timeout = 5000;
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new NetworkCredential("polloandaluzapp@gmail.com", "rebmuguhpvffmzdx");
                SmtpServer.Send(xemail);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public string TranslateText(string input)
        {
            /*string url = "https://www.linguee.es/espanol-ingles/traduccion/" + input.Replace(" ","+") +".html";
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;
            string result = webClient.DownloadString(url);
            result = result.Substring(result.IndexOf("dictLink featured") + "dictLink featured'>".Length);
            //result = result.Substring(result.IndexOf(">") + 1);
            result = result.Substring(0, result.IndexOf("</a>"));
            return result.Trim();*/
            return "";
        }

        internal void ActualizaIngredientesProducto(List<IngredienteProductoModel> ingredientes)
        {
            try
            {
                dbConn.Execute("DELETE FROM [IngredienteProductoModel] Where idProducto=" + ingredientes[0].idProducto);
                int i = dbConn.InsertAll(ingredientes);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        internal void ActualizaIngredientes(List<IngredientesModel> ingredientes)
        {
            try
            {
                dbConn.Execute("DELETE FROM [IngredientesModel] Where idEstablecimiento=" + ingredientes[0].idEstablecimiento);
                int i = dbConn.InsertAll(ingredientes);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        internal void ActualizaRepartidores(List<RepartidorModel> repartidores)
        {
            try
            {
                foreach (RepartidorModel repartidor in repartidores)
                    dbConn.InsertOrReplace(repartidor);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        internal void ActualizaRepartidor(RepartidorModel repartidor)
        {
            try
            {
                dbConn.InsertOrReplace(repartidor);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        internal void NuevoIngredienteProducto(IngredienteProductoModel ingrediente)
        {
            try
            {
                int i = dbConn.Insert(ingrediente);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        internal void NuevaTarjeta(TarjetaModel tarjeta)
        {
            try
            {
                int i = dbConn.Insert(tarjeta);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        internal void BorraTarjeta(TarjetaModel tarjeta)
        {
            try
            {
                int i = dbConn.Delete(tarjeta);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        internal void NuevoIngrediente(IngredientesModel ingrediente)
        {
            try
            {
                int i = dbConn.Insert(ingrediente);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        internal void ActualizaIngredienteProducto(IngredienteProductoModel ingrediente)
        {
            try
            {
                dbConn.Execute("DELETE FROM [IngredienteProductoModel] Where id=" + ingrediente.id);
                int i = dbConn.Insert(ingrediente);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        internal void ActualizaIngrediente(IngredientesModel ingrediente)
        {
            try
            {
                dbConn.Execute("DELETE FROM [IngredientesModel] Where id=" + ingrediente.id);
                int i = dbConn.Insert(ingrediente);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        internal void EliminaIngredienteProducto(int id)
        {
            try
            {
                dbConn.Execute("DELETE FROM [IngredienteProductoModel] Where id=" + id);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        internal void EliminaFavoritos()
        {
            try
            {
                dbConn.DeleteAll<FavoritosModel>();
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        internal void NuevoFavorito(Establecimiento fav)
        {
            try
            {
                FavoritosModel f = new FavoritosModel()
                {
                    activo = 1,
                    fechaActivacion = DateTime.Now,
                    id = fav.id,
                    idEstablecimiento = fav.idEstablecimiento,
                    nombreEstablecimiento = fav.nombre,
                    idUsuario = App.DAUtil.usuario.idUsuario
                };
                dbConn.Insert(f);
                dbConn.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        internal void EliminaProductos()
        {
            dbConn.DeleteAll<ArticuloModel>();
        }

        private string codificaTexto(string texto)
        {
            texto = texto.Replace('ó', 'o').Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ú', 'u').Replace('ñ', 'n').Replace('Ó', 'O').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ú', 'U').Replace('Ñ', 'N').Replace("€", "E").Replace("º", "").Replace("ª", "");
            return texto;
        }
    }
}
