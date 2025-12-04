using Newtonsoft.Json;
using AsadorMoron.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using AsadorMoron.Utils;

namespace AsadorMoron.Services
{
    /// <summary>
    /// Versión optimizada y async de ResponseServiceWS.
    /// Reemplaza las llamadas bloqueantes .Result por async/await.
    /// </summary>
    public class ResponseServiceAsync
    {
        private readonly HttpClientService _http = HttpClientService.Instance;
        private static string BaseUrl => ResponseServiceWS.urlPro;

        #region Categorias

        /// <summary>
        /// Obtiene las categorías de un establecimiento de forma async
        /// ANTES: ResponseServiceWS.getListadoCategorias() - BLOQUEANTE
        /// </summary>
        public async Task<List<Categoria>> GetCategoriasAsync(int idEstablecimiento, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var url = $"{App.DAUtil.miURL}categorias.php/GET?idEstablecimiento={idEstablecimiento}";
                var result = await _http.GetAsync<List<Categoria>>(url, ct);
                return result ?? new List<Categoria>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetCategoriasAsync");
            }
        }

        #endregion

        #region Configuracion

        /// <summary>
        /// Obtiene la configuración de un establecimiento de forma async
        /// ANTES: ResponseServiceWS.getConfiguracionEstablecimiento() - BLOQUEANTE
        /// </summary>
        public async Task<ConfiguracionEstablecimiento> GetConfiguracionEstablecimientoAsync(int idEstablecimiento, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var url = $"{App.DAUtil.miURL}configuracion.php/GET?idEstablecimiento={idEstablecimiento}";
                // La API devuelve un objeto único, no una lista
                var result = await _http.GetAsync<ConfiguracionEstablecimiento>(url, ct);
                return result ?? new ConfiguracionEstablecimiento();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetConfiguracionEstablecimientoAsync");
            }
        }

        #endregion

        #region Establecimiento

        /// <summary>
        /// Obtiene un establecimiento por ID de forma async
        /// ANTES: ResponseServiceWS.getEstablecimiento() - BLOQUEANTE
        /// </summary>
        public async Task<Establecimiento> GetEstablecimientoAsync(int idEstablecimiento, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var url = $"{App.DAUtil.miURL}establecimientos.php/GET?id={idEstablecimiento}";
                var list = await _http.GetAsync<List<Establecimiento>>(url, ct);
                var es = list?.FirstOrDefault();

                if (es != null)
                {
                    if (es.activoHoy)
                    {
                        if (es.activoMan == 1 || es.activoTarde == 1)
                            es.horario = ((TimeSpan)es.inicioHoy).ToString(@"hh\:mm") + " - " + ((TimeSpan)es.finHoy).ToString(@"hh\:mm");
                        else
                            es.horario = Recursos.AppResources.Cerrado;
                    }
                    else
                        es.horario = Recursos.AppResources.Cerrado;

                    es.configuracion = await GetConfiguracionEstablecimientoAsync(es.idEstablecimiento, ct);
                    return es;
                }

                return new Establecimiento();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetEstablecimientoAsync");
            }
        }

        #endregion

        #region Puntos

        /// <summary>
        /// Obtiene los puntos del establecimiento de forma async
        /// ANTES: ResponseServiceWS.getPuntosEstablecimiento() - BLOQUEANTE
        /// </summary>
        public async Task<int> GetPuntosEstablecimientoAsync(CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                if (App.DAUtil.Usuario == null) return 0;

                var url = $"{App.DAUtil.miURL}puntos.php/GET?puntosEstablecimiento=true&idUsuario={App.DAUtil.Usuario.idUsuario}&idEstablecimiento={App.EstActual.idEstablecimiento}";
                var result = await _http.GetAsync<ResultdadoModel>(url, ct);
                return result?.resultado ?? 0;
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetPuntosEstablecimientoAsync");
            }
        }

        #endregion

        #region Menu Diario

        /// <summary>
        /// Obtiene el menú diario de forma async
        /// ANTES: ResponseServiceWS.getMenuDiario() - BLOQUEANTE
        /// </summary>
        public async Task<MenuDiarioModel> GetMenuDiarioAsync(int idEstablecimiento, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var url = $"{App.DAUtil.miURL}menuDiario.php/GET?idEstablecimiento={idEstablecimiento}";
                var menu = await _http.GetAsync<MenuDiarioModel>(url, ct);

                if (menu != null && menu.id > 0)
                {
                    // Cargar configuración y productos en paralelo
                    var configTask = GetConfiguracionMenuDiarioAsync(idEstablecimiento, ct);
                    var productosTask = GetProductosMenuDiarioAsync(menu.id, ct);

                    await Task.WhenAll(configTask, productosTask);

                    menu.configuracion = await configTask;
                    menu.productos = await productosTask;
                    return menu;
                }

                return new MenuDiarioModel();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetMenuDiarioAsync");
            }
        }

        private async Task<MenuDiarioConfiguracionModel> GetConfiguracionMenuDiarioAsync(int idEstablecimiento, CancellationToken ct = default)
        {
            var url = $"{App.DAUtil.miURL}menuDiario.php/GET?configuracion={idEstablecimiento}";
            return await _http.GetAsync<MenuDiarioConfiguracionModel>(url, ct);
        }

        private async Task<List<MenuDiarioProductosModel>> GetProductosMenuDiarioAsync(int idMenu, CancellationToken ct = default)
        {
            var url = $"{App.DAUtil.miURL}menuDiario.php/GET?productos={idMenu}";
            return await _http.GetAsync<List<MenuDiarioProductosModel>>(url, ct);
        }

        #endregion

        #region Menu

        /// <summary>
        /// Obtiene el menú por rol de forma async
        /// ANTES: ResponseServiceWS.getMenu() - BLOQUEANTE
        /// </summary>
        public async Task<List<MenuModel>> GetMenuAsync(int idRol, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                int vis = Microsoft.Maui.Devices.DeviceInfo.Platform.ToString() == "WinUI" ? 2 : 1;
                var url = $"{App.DAUtil.miURL}menu.php/GET?rol={idRol}&vis={vis}";
                return await _http.GetAsync<List<MenuModel>>(url, ct) ?? new List<MenuModel>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetMenuAsync");
            }
        }

        #endregion

        #region Combos

        /// <summary>
        /// Obtiene los combos de forma async
        /// ANTES: ResponseServiceWS.getCombos() - BLOQUEANTE
        /// </summary>
        public async Task<List<ComboModel>> GetCombosAsync(CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var url = $"{App.DAUtil.miURL}establecimientos.php/GET?combos=true";
                return await _http.GetAsync<List<ComboModel>>(url, ct) ?? new List<ComboModel>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetCombosAsync");
            }
        }

        #endregion

        #region Cupones y Promociones

        /// <summary>
        /// Obtiene la promoción de amigo de forma async
        /// ANTES: ResponseServiceWS.getPromocionAmigo() - BLOQUEANTE
        /// </summary>
        public async Task<PromocionAmigoModel> GetPromocionAmigoAsync(CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                if (App.DAUtil.Usuario == null) return null;

                var url = $"{App.DAUtil.miURL}promociones.php/GET?promocionAmigo=true&idPueblo={App.DAUtil.Usuario.idPueblo}";
                var list = await _http.GetAsync<List<PromocionAmigoModel>>(url, ct);
                return list?.FirstOrDefault();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetPromocionAmigoAsync");
            }
        }

        /// <summary>
        /// Obtiene los cupones de forma async
        /// ANTES: ResponseServiceWS.getListadoCupones() - BLOQUEANTE
        /// </summary>
        public async Task<List<CuponesModel>> GetCuponesAsync(CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var pueblo = App.DAUtil.GetPueblosSQLite()?.Find(p => p.id == App.DAUtil.Usuario?.idPueblo);
                if (pueblo == null) return new List<CuponesModel>();

                var url = $"{App.DAUtil.miURL}promociones.php/GET?cupones=true&idGrupo={pueblo.idGrupo}";
                return await _http.GetAsync<List<CuponesModel>>(url, ct) ?? new List<CuponesModel>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetCuponesAsync");
            }
        }

        #endregion

        #region Listados Establecimientos

        /// <summary>
        /// Obtiene la lista de establecimientos de forma async
        /// ANTES: ResponseServiceWS.getListadoEstablecimientos() - BLOQUEANTE
        /// </summary>
        public async Task<List<Establecimiento>> GetEstablecimientosAsync(CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var pueblo = App.DAUtil.GetPueblosSQLite()?.Find(p => p.id == App.DAUtil.Usuario?.idPueblo);
                if (pueblo == null) return new List<Establecimiento>();

                var url = $"{App.DAUtil.miURL}establecimientos.php/GET?idGrupo={pueblo.idGrupo}";
                return await _http.GetAsync<List<Establecimiento>>(url, ct) ?? new List<Establecimiento>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetEstablecimientosAsync");
            }
        }

        #endregion

        #region Histórico Pedidos

        /// <summary>
        /// Obtiene el histórico de pedidos de forma async
        /// ANTES: ResponseServiceWS.getHistoricoPedidosEstablecimiento() - BLOQUEANTE
        /// </summary>
        public async Task<ObservableCollection<CabeceraPedido>> GetHistoricoPedidosAsync(Establecimiento est, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                if (App.DAUtil.Usuario == null)
                    return new ObservableCollection<CabeceraPedido>();

                var pueblo = App.DAUtil.GetPueblosSQLite()?.Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                if (pueblo == null)
                    return new ObservableCollection<CabeceraPedido>();

                var url = $"{App.DAUtil.miURL}establecimientos.php/GET?idEstablecimientoHistorico={est.idEstablecimiento}&idGrupo={pueblo.idGrupo}&idPueblo={pueblo.id}";
                return await _http.GetAsync<ObservableCollection<CabeceraPedido>>(url, ct) ?? new ObservableCollection<CabeceraPedido>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetHistoricoPedidosAsync");
            }
        }

        #endregion

        #region Utilidades

        /// <summary>
        /// Carga múltiples datos en paralelo para inicialización rápida
        /// </summary>
        public async Task<(List<Categoria> Categorias, ConfiguracionEstablecimiento Config, int Puntos)>
            CargarDatosCartaAsync(int idEstablecimiento, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var categoriasTask = GetCategoriasAsync(idEstablecimiento, ct);
                var configTask = GetConfiguracionEstablecimientoAsync(idEstablecimiento, ct);
                var puntosTask = GetPuntosEstablecimientoAsync(ct);

                await Task.WhenAll(categoriasTask, configTask, puntosTask);

                return (
                    await categoriasTask,
                    await configTask,
                    await puntosTask
                );
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "CargarDatosCartaAsync_Paralelo");
            }
        }

        #endregion

        #region Inicialización App - Métodos para OnStart

        /// <summary>
        /// Carga datos iniciales de la app en paralelo
        /// Reemplaza las llamadas secuenciales en OnStart()
        /// </summary>
        public async Task<(Establecimiento Establecimiento, List<ComboModel> Combos)>
            CargarDatosInicialesAsync(int idEstablecimiento, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var estTask = GetEstablecimientoAsync(idEstablecimiento, ct);
                var combosTask = GetCombosAsync(ct);

                await Task.WhenAll(estTask, combosTask);

                return (await estTask, await combosTask);
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "CargarDatosInicialesAsync_Paralelo");
            }
        }

        #endregion

        #region Pueblos y Zonas

        /// <summary>
        /// Obtiene los pueblos de forma async
        /// ANTES: ResponseServiceWS.getPueblos() - BLOQUEANTE
        /// </summary>
        public async Task<List<PueblosModel>> GetPueblosAsync(CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                if (!App.DAUtil.DoIHaveInternet())
                    return new List<PueblosModel>();

                var url = $"{App.DAUtil.miURL}pueblos.php/GET";
                var pueblos = await _http.GetAsync<List<PueblosModel>>(url, ct);

                if (pueblos != null && pueblos.Count > 0)
                {
                    App.DAUtil.savePueblos(pueblos);
                }

                return pueblos ?? new List<PueblosModel>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetPueblosAsync");
            }
        }

        /// <summary>
        /// Obtiene las zonas de forma async
        /// ANTES: ResponseServiceWS.getZonas() - BLOQUEANTE
        /// </summary>
        public async Task<List<ZonaModel>> GetZonasAsync(CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                if (!App.DAUtil.DoIHaveInternet())
                    return new List<ZonaModel>();

                var url = $"{App.DAUtil.miURL}zonas.php/GET";
                var zonas = await _http.GetAsync<List<ZonaModel>>(url, ct);

                if (zonas != null && zonas.Count > 0)
                {
                    App.DAUtil.saveZonas(zonas);
                }

                return zonas ?? new List<ZonaModel>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetZonasAsync");
            }
        }

        #endregion

        #region Mensajes

        /// <summary>
        /// Obtiene los mensajes predefinidos de forma async
        /// ANTES: ResponseServiceWS.getMensajesPredefinidos() - BLOQUEANTE
        /// </summary>
        public async Task<List<PredefinidosModel>> GetMensajesPredefinidosAsync(CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                if (!App.DAUtil.DoIHaveInternet())
                    return new List<PredefinidosModel>();

                var url = $"{App.DAUtil.miURL}repartidores.php/GET?predefinidos=true";
                return await _http.GetAsync<List<PredefinidosModel>>(url, ct) ?? new List<PredefinidosModel>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetMensajesPredefinidosAsync");
            }
        }

        #endregion

        #region Tarjetas y Usuarios

        /// <summary>
        /// Obtiene las tarjetas de un usuario de forma async
        /// ANTES: ResponseServiceWS.getTarjetas() - BLOQUEANTE
        /// </summary>
        public async Task<List<TarjetaModel>> GetTarjetasAsync(int idUsuario, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var url = $"{App.DAUtil.miURL}tarjetas.php/GET?idUsuario={idUsuario}";
                return await _http.GetAsync<List<TarjetaModel>>(url, ct) ?? new List<TarjetaModel>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetTarjetasAsync");
            }
        }

        /// <summary>
        /// Obtiene la lista de establecimientos por pueblo de forma async
        /// ANTES: ResponseServiceWS.getListadoEstablecimientos() - BLOQUEANTE
        /// </summary>
        public async Task<List<Establecimiento>> GetListadoEstablecimientosAsync(int idPueblo, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var url = $"{App.DAUtil.miURL}establecimientos.php/GET?idPueblo={idPueblo}";
                return await _http.GetAsync<List<Establecimiento>>(url, ct) ?? new List<Establecimiento>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetListadoEstablecimientosAsync");
            }
        }

        /// <summary>
        /// Obtiene un repartidor por ID de usuario de forma async
        /// ANTES: ResponseServiceWS.GetRepartidorByIdUsuario() - BLOQUEANTE
        /// </summary>
        public async Task<RepartidorModel> GetRepartidorByIdUsuarioAsync(int idUsuario, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                var url = $"{App.DAUtil.miURL}repartidores.php/GET?idUsuario={idUsuario}";
                var list = await _http.GetAsync<List<RepartidorModel>>(url, ct);
                return list?.FirstOrDefault();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetRepartidorByIdUsuarioAsync");
            }
        }

        #endregion

        #region Login Async

        /// <summary>
        /// Login async - NO BLOQUEANTE
        /// ANTES: ResponseServiceWS.Login() - BLOQUEANTE
        /// </summary>
        public async Task<bool> LoginAsync(string user, string pass, CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                if (!App.DAUtil.DoIHaveInternet())
                    return false;

                var encryptedPass = Crypto.Encrypt(pass).Replace(" ", "").Replace("+", "");
                var url = $"{App.DAUtil.miURL}usuarios.php/GET?email={user}&password={encryptedPass}";

                var respuesta = await _http.GetAsync<UsuarioModel>(url, ct);

                if (respuesta == null || string.IsNullOrEmpty(respuesta.nombre))
                    return false;

                if (string.IsNullOrEmpty(respuesta.password))
                {
                    App.DAUtil.Usuario = respuesta;
                    return false;
                }

                respuesta.password = pass;
                respuesta.nombreCompleto = $"{respuesta.nombre} {respuesta.apellidos}";

                if (!App.DAUtil.SaveConfiguracionUsuarioSQLite(respuesta))
                    return false;

                App.DAUtil.Usuario = respuesta;

                // Cargar datos adicionales en paralelo según el rol
                await CargarDatosUsuarioSegunRolAsync(respuesta, ct);

                Microsoft.Maui.Storage.Preferences.Set("idGrupo", 1);
                Microsoft.Maui.Storage.Preferences.Set("idPueblo", 1);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error LoginAsync: {ex.Message}");
                return false;
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "LoginAsync");
            }
        }

        /// <summary>
        /// Carga datos adicionales del usuario según su rol en paralelo
        /// </summary>
        private async Task CargarDatosUsuarioSegunRolAsync(UsuarioModel usuario, CancellationToken ct = default)
        {
            var tarjetasTask = GetTarjetasAsync(usuario.idUsuario, ct);

            if (usuario.rol == (int)RolesEnum.Establecimiento || usuario.rol == (int)RolesEnum.Administrador)
            {
                var estTask = GetListadoEstablecimientosAsync(usuario.idPueblo, ct);
                await Task.WhenAll(tarjetasTask, estTask);

                App.DAUtil.Usuario.tarjetas = await tarjetasTask;
                App.DAUtil.Usuario.establecimientos = await estTask;

                if (App.DAUtil.Usuario.establecimientos?.Count > 0)
                {
                    App.EstActual = await GetEstablecimientoAsync(App.DAUtil.Usuario.establecimientos[0].idEstablecimiento, ct);
                    App.MiEst = App.EstActual;
                }
            }
            else if (usuario.rol == (int)RolesEnum.Repartidor)
            {
                var repartidorTask = GetRepartidorByIdUsuarioAsync(usuario.idUsuario, ct);
                await Task.WhenAll(tarjetasTask, repartidorTask);

                App.DAUtil.Usuario.tarjetas = await tarjetasTask;
                App.DAUtil.Usuario.Repartidor = await repartidorTask;
            }
            else if (usuario.rol == (int)RolesEnum.Cliente)
            {
                App.DAUtil.Usuario.tarjetas = await tarjetasTask;
                ResponseServiceWS.GuardaOnline(1);
            }
            else
            {
                App.DAUtil.Usuario.tarjetas = await tarjetasTask;
            }
        }

        #endregion

        #region Carga Paralela para InitNavigation

        /// <summary>
        /// Carga todos los datos necesarios para InitNavigation en paralelo
        /// </summary>
        public async Task<(List<MensajesModel> Mensajes, List<PredefinidosModel> Predefinidos, List<PueblosModel> Pueblos)>
            CargarDatosNavegacionAsync(CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                // Cargar mensajes, predefinidos, pueblos y zonas en paralelo
                var mensajesTask = GetMensajesAsync(ct);
                var predefinidosTask = GetMensajesPredefinidosAsync(ct);
                var pueblosTask = GetPueblosAsync(ct);
                var zonasTask = GetZonasAsync(ct);

                await Task.WhenAll(mensajesTask, predefinidosTask, pueblosTask, zonasTask);

                return (
                    await mensajesTask,
                    await predefinidosTask,
                    await pueblosTask
                );
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "CargarDatosNavegacionAsync_Paralelo");
            }
        }

        /// <summary>
        /// Obtiene mensajes de forma async (wrapper para consistencia)
        /// </summary>
        private async Task<List<MensajesModel>> GetMensajesAsync(CancellationToken ct = default)
        {
            var sw = PerformanceBenchmark.StartTimer();
            try
            {
                if (!App.DAUtil.DoIHaveInternet())
                    return new List<MensajesModel>();

                var url = $"{App.DAUtil.miURL}mensajes.php/GET";
                var mensajes = await _http.GetAsync<List<MensajesModel>>(url, ct);

                if (mensajes != null)
                {
                    foreach (var m in mensajes)
                    {
                        if (App.idioma.Equals("EN") || App.idioma.Equals("GB") || App.idioma.Equals("US"))
                            m.valor = m.valor_eng;
                        else if (App.idioma.Equals("FR"))
                            m.valor = m.valor_fr;
                        else if (App.idioma.Equals("DE"))
                            m.valor = m.valor_ger;
                    }
                    App.DAUtil.actualizaMensajes(mensajes);
                }

                return mensajes ?? new List<MensajesModel>();
            }
            finally
            {
                PerformanceBenchmark.StopAndRecord(sw, "GetMensajesAsync");
            }
        }

        #endregion
    }
}
