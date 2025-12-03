using Newtonsoft.Json;
using AsadorMoron.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Devices;
// 
using System.Text;
using AsadorMoron.Utils;
using System.Diagnostics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.Models.PayComet;
using AsadorMoron.Recursos;
using System.Data;

namespace AsadorMoron.Services
{
    public class ResponseServiceWS
    {
#if DEBUG
        //public static string urlPro = "https://qoorder.com/pa_ws_test/";
        public static string urlPro = "https://qoorder.com/pa_ws/";
#else
        public static string urlPro = "https://qoorder.com/pa_ws/";
#endif
        #region Global
        private static readonly string urlPaycomet = "https://rest.paycomet.com/v1/";
        public static int terminalPaycomet = 0;
        public static string apiKeyPaycomet = "";
        public static void UploadImage(string path, string nombre, string carpeta, string antiguo)
        {
            try
            {
                System.Net.WebClient Client = new System.Net.WebClient();
                Client.Headers.Add("Content-Type", "binary/octet-stream");
                byte[] result = Client.UploadFile(ResponseServiceWS.urlPro + "uploadImagen.php?nombre=" + nombre + "&carpeta=" + carpeta + "&antiguo=" + antiguo, "POST", path);
                string s = Encoding.UTF8.GetString(result, 0, result.Length);
            }
            catch (Exception ex)
            {
                // 
            }
        }
        public static void UploadPDF(string path, string nombre, string carpeta, string antiguo)
        {
            try
            {
                System.Net.WebClient Client = new System.Net.WebClient();
                Client.Headers.Add("Content-Type", "application/pdf");
                byte[] result = Client.UploadFile(ResponseServiceWS.urlPro + "uploadImagen.php?nombre=" + nombre + "&carpeta=" + carpeta + "&antiguo=" + antiguo, "POST", path);
                string s = Encoding.UTF8.GetString(result, 0, result.Length);
            }
            catch (Exception ex)
            {
                // 
            }
        }



        public static void UploadPin(string path, string nombre, string archivo, string carpeta, string antiguo)
        {
            try
            {
                System.Net.WebClient Client = new System.Net.WebClient();
                Client.Headers.Add("Content-Type", "binary/octet-stream");
                byte[] result = Client.UploadFile(ResponseServiceWS.urlPro + "uploadPin.php?nombre=" + nombre + "&carpeta=" + carpeta + "&archivo=" + archivo + "&antiguo=" + antiguo, "POST", path);
                string s = Encoding.UTF8.GetString(result, 0, result.Length);
            }
            catch (Exception ex)
            {
                // 
            }
        }

        internal static MenuDiarioModel getMenuDiario(int idEstablecimiento)
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "menuDiario.php/GET?idEstablecimiento=" + idEstablecimiento;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    MenuDiarioModel menu = JsonConvert.DeserializeObject<MenuDiarioModel>(resultJSON);
                    if (menu != null)
                    {
                        menu.configuracion = getConfiguracionMenuDiario(idEstablecimiento);
                        menu.productos = getProductosMenuDiario(menu.id);
                        return menu;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                App.userdialog.HideLoading();
            }
            return new MenuDiarioModel();
        }
        static MenuDiarioConfiguracionModel getConfiguracionMenuDiario(int idEstablecimiento)
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "menuDiario.php/GET?configuracion=" + idEstablecimiento;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    MenuDiarioConfiguracionModel menu = JsonConvert.DeserializeObject<MenuDiarioConfiguracionModel>(resultJSON);
                    if (menu != null)
                    {
                        return menu;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                App.userdialog.HideLoading();
            }
            return new MenuDiarioConfiguracionModel();
        }
        static List<MenuDiarioProductosModel> getProductosMenuDiario(int idMenu)
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "menuDiario.php/GET?productos=" + idMenu;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    List<MenuDiarioProductosModel> menu = JsonConvert.DeserializeObject<List<MenuDiarioProductosModel>>(resultJSON);
                    if (menu != null)
                    {
                        return menu;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                App.userdialog.HideLoading();
            }
            return new List<MenuDiarioProductosModel>();
        }

        internal static List<MenuModel> getMenu(int idRol)
        {
            try
            {
                int vis = 1;
                if (DeviceInfo.Platform.ToString() == "WinUI")
                    vis = 2;
                DatosConexionModel.uri = $"{App.DAUtil.miURL}menu.php/GET?rol={idRol}&vis={vis}";
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    List<MenuModel> cuenta = JsonConvert.DeserializeObject<List<MenuModel>>(resultJSON);
                    if (cuenta != null)
                        return cuenta;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                App.userdialog.HideLoading();
            }
            return new List<MenuModel>();
        }

        private static ObservableCollection<CabeceraPedido> convertirToPedidoInterno(ObservableCollection<Pedido> listPedido)
        {
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            LineasPedido lineaPedido;
            CabeceraPedido cabeceraPedido = new CabeceraPedido();
            foreach (var item in listPedido)
            {
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
            }


            foreach (var item in listPedido)
            {
                foreach (var item2 in result)
                {
                    if (item.codigoPedido.Equals(item2.codigoPedido))
                    {
                        lineaPedido = new LineasPedido();
                        lineaPedido = getLineaPedido(item);
                        if (item2.lineasPedidos == null)
                            item2.lineasPedidos = new ObservableCollection<LineasPedido>();
                        if (item2.lineasPedidosAdd == null)
                            item2.lineasPedidosAdd = new ObservableCollection<LineasPedido>();

                        item2.lineasPedidos.Add(lineaPedido);
                    }
                }
            }

            return new ObservableCollection<CabeceraPedido>(result.OrderBy(p => p.horaPedido).ToList());
        }

        private static CabeceraPedido getCabecera2(Pedido item)
        {

            CabeceraPedido cabeceraPedido = new CabeceraPedido();
            cabeceraPedido.idPedido = item.idPedido;
            cabeceraPedido.valorado = item.valorado;
            cabeceraPedido.tipoPago = item.tipoPago;
            cabeceraPedido.codigoPedido = item.codigoPedido;
            cabeceraPedido.idEstablecimiento = item.idEstablecimiento;
            cabeceraPedido.idUsuario = item.idUsuario;
            cabeceraPedido.idDetalle = item.idDetalle;
            cabeceraPedido.horaPedido = item.horaPedido;
            cabeceraPedido.idZonaEstablecimiento = item.idZonaEstablecimiento;
            cabeceraPedido.zonaEstablecimiento = item.zonaEstablecimiento;
            cabeceraPedido.mesa = item.mesa;
            cabeceraPedido.repartidor = item.repartidor;
            cabeceraPedido.horaEntrega = DateTime.Parse(item.horaEntrega).ToString(@"HH\:mm");
            cabeceraPedido.comentario = item.comentario.Trim();
            cabeceraPedido.fechaEntrega = DateTime.Parse(item.horaEntrega);
            cabeceraPedido.estadoDetalle = item.estadoDetalle;
            cabeceraPedido.zona = item.zona;
            cabeceraPedido.idZona = item.idZona;
            cabeceraPedido.tipoVenta = item.tipoVenta;
            cabeceraPedido.colorZona = item.colorZona;
            cabeceraPedido.estadoPedido = item.estadoPedido;
            cabeceraPedido.idEstadoPedido = item.idEstadoPedido;
            cabeceraPedido.nombreEstablecimiento = item.nombreEstablecimiento;
            cabeceraPedido.nombreUsuario = item.nombreUsuario;
            cabeceraPedido.direccionUsuario = item.direccionUsuario;
            cabeceraPedido.telefonoUsuario = item.telefonoUsuario;
            cabeceraPedido.emailUsuario = item.emailUsuario;
            cabeceraPedido.idRepartidor = item.idRepartidor;
            cabeceraPedido.FotoRepartidor = item.fotoRepartidor;
            try
            {
                cabeceraPedido.precioTotalPedido = item.precioTotalPedido;
            }
            catch (Exception)
            {

            }
            if (!string.IsNullOrEmpty(cabeceraPedido.comentario))
                cabeceraPedido.tieneComentario = 1;
            else
                cabeceraPedido.tieneComentario = 0;

            return cabeceraPedido;
        }



        private static CabeceraPedido getCabecera(Pedido item)
        {

            CabeceraPedido cabeceraPedido = new CabeceraPedido();
            cabeceraPedido.idPedido = item.idPedido;
            cabeceraPedido.valorado = item.valorado;
            cabeceraPedido.poblacion = item.poblacion;
            cabeceraPedido.tipoPago = item.tipoPago;
            cabeceraPedido.codigoPedido = item.codigoPedido;
            cabeceraPedido.idEstablecimiento = item.idEstablecimiento;
            cabeceraPedido.idUsuario = item.idUsuario;
            cabeceraPedido.idDetalle = item.idDetalle;
            cabeceraPedido.horaPedido = item.horaPedido;
            cabeceraPedido.idCuenta = item.idCuenta;
            cabeceraPedido.tipo = item.tipo;
            cabeceraPedido.idZonaEstablecimiento = item.idZonaEstablecimiento;
            cabeceraPedido.zonaEstablecimiento = item.zonaEstablecimiento;
            cabeceraPedido.mesa = item.mesa;
            cabeceraPedido.repartidor = item.repartidor;
            cabeceraPedido.horaEntrega = DateTime.Parse(item.horaEntrega).ToString(@"HH\:mm");
            cabeceraPedido.comentario = item.comentario.Trim();
            cabeceraPedido.fechaEntrega = DateTime.Parse(item.horaEntrega);
            cabeceraPedido.estadoDetalle = item.estadoDetalle;
            cabeceraPedido.zona = item.zona;
            cabeceraPedido.transaccion = item.transaccion;
            cabeceraPedido.pagado = item.pagado;
            cabeceraPedido.tipoVenta = item.tipoVenta;
            cabeceraPedido.idZona = item.idZona;
            cabeceraPedido.colorZona = item.colorZona;
            cabeceraPedido.estadoPedido = item.estadoPedido;
            cabeceraPedido.idEstadoPedido = item.idEstadoPedido;
            cabeceraPedido.nombreEstablecimiento = item.nombreEstablecimiento;
            cabeceraPedido.nombreUsuario = item.nombreUsuario;
            cabeceraPedido.direccionUsuario = item.direccionUsuario;
            cabeceraPedido.telefonoUsuario = item.telefonoUsuario;
            cabeceraPedido.emailUsuario = item.emailUsuario;
            cabeceraPedido.idRepartidor = item.idRepartidor;
            cabeceraPedido.FotoRepartidor = item.fotoRepartidor;
            if (!string.IsNullOrEmpty(cabeceraPedido.comentario))
                cabeceraPedido.tieneComentario = 1;
            else
                cabeceraPedido.tieneComentario = 0;

            return cabeceraPedido;
        }
        private static LineasPedido getLineaPedido(Pedido item)
        {
            LineasPedido lineaPedido = new LineasPedido();
            lineaPedido.nombreProducto = item.nombreProducto;
            lineaPedido.cantidad = item.cantidad;
            lineaPedido.precio = item.precio;
            lineaPedido.importe = item.importe;
            lineaPedido.comentario = item.comentarioProducto;
            lineaPedido.numeroImpresora = item.numeroImpresora;
            lineaPedido.desripcionProducto = item.desripcionProducto;
            lineaPedido.imagenProducto = item.imagenProducto;
            lineaPedido.idProducto = item.idProducto;
            lineaPedido.estadoProducto = item.estadoDetalle;
            lineaPedido.tipoComida = item.tipoProducto;
            lineaPedido.tipoVenta = item.tipoVenta;
            return lineaPedido;
        }
        internal static async Task<bool> sendNotificacionVarios(string cuerpo, string filtro)
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "sendNotificationVarios.php?filtro=" + filtro + "&titulo=" + AppResources.App + "&mensaje=" + cuerpo;
                HttpResponseMessage response = App.Client.GetAsync(DatosConexionModel.uri).Result;
                string resultJSON = await response.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal async Task enviaNotificacion(string titulo, string mensaje, string token)
        {
            //#if DEBUG
            //#else
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "sendNotification.php?id=" + token + "&titulo=" + titulo + "&mensaje=" + mensaje;
                HttpResponseMessage response = App.Client.GetAsync(DatosConexionModel.uri).Result;
                string resultJSON = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //#endif
        }
        #endregion
        #region Descuentos
        internal List<CuponesModel> getListadoCupones()
        {
            List<CuponesModel> listCupones = new List<CuponesModel>();
            try
            {
                PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                string requestUri = App.DAUtil.miURL + "promociones.php/GET?cupones=true&idGrupo=" + pu.idGrupo;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listCupones = JsonConvert.DeserializeObject<List<CuponesModel>>(resultJSON);
                }
                if (listCupones == null)
                    listCupones = new List<CuponesModel>();
                return listCupones;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listCupones;
            }
        }
        internal List<SorteosNumerosModel> getNumerosSorteo()
        {
            List<SorteosNumerosModel> listCupones = new List<SorteosNumerosModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "promociones.php/GET?sorteo=true&idUsuario=" + App.DAUtil.Usuario.idUsuario;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listCupones = JsonConvert.DeserializeObject<List<SorteosNumerosModel>>(resultJSON);
                }
                if (listCupones == null)
                    listCupones = new List<SorteosNumerosModel>();
                return listCupones;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listCupones;
            }
        }
        private static string generaNumerosSorteo()
        {

            try
            {
                string requestUri = App.DAUtil.miURL + "promociones.php/GET?sorteo=true&idUsuario=" + App.DAUtil.Usuario.idUsuario + "&idPueblo=" + App.DAUtil.Usuario.idPueblo;
                HttpResponseMessage response = App.Client.PostAsync(requestUri, null).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    return resultJSON.Trim();
                }
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        internal bool compruebaCodigoAmigo(int idPueblo, string codigo)
        {
            List<UsuarioModel> listCupones = new List<UsuarioModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "promociones.php/GET?compruebaCodigoAmigo=true&idPueblo=" + idPueblo + "&codigo=" + codigo;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listCupones = JsonConvert.DeserializeObject<List<UsuarioModel>>(resultJSON);
                    UsuarioModel u = listCupones.FirstOrDefault();
                    return u.idPueblo == idPueblo;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        internal static PromocionAmigoModel getPromocionAmigo()
        {
            try
            {
                string requestUri = App.DAUtil.miURL + "promociones.php/GET?promocionAmigo=true&idPueblo=" + App.DAUtil.Usuario.idPueblo;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    List<PromocionAmigoModel> listCupones = JsonConvert.DeserializeObject<List<PromocionAmigoModel>>(resultJSON);
                    return listCupones.FirstOrDefault();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        internal static PromocionAmigoModel getPromocionAmigo(int idPueblo)
        {
            try
            {
                string requestUri = App.DAUtil.miURL + "promociones.php/GET?promocionAmigo=true&idPueblo=" + idPueblo;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    List<PromocionAmigoModel> listCupones = JsonConvert.DeserializeObject<List<PromocionAmigoModel>>(resultJSON);
                    return listCupones.FirstOrDefault();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        internal static AmigosModel TieneActivacionPendiente()
        {
            try
            {
                string requestUri = App.DAUtil.miURL + "promociones.php/GET?activacion=true&idPueblo=" + App.DAUtil.Usuario.idPueblo + "&id=" + App.DAUtil.Usuario.idUsuario;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    List<AmigosModel> listCupones = JsonConvert.DeserializeObject<List<AmigosModel>>(resultJSON);
                    return listCupones.FirstOrDefault();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        internal static async Task<CuponesSQLModel> AplicarCuponSQL(string cupon)
        {
            CuponesSQLModel cupons = null;
            try
            {
                string requestUri = App.DAUtil.miURL + "promociones.php/GET?cuponSQL=true&cupon=" + cupon + "&idUsuario=" + App.DAUtil.Usuario.idUsuario + "&idPueblo=" + App.DAUtil.Usuario.idPueblo;
                HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                string resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    cupons = JsonConvert.DeserializeObject<CuponesSQLModel>(resultJSON);
                }
                return cupons;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return cupons;
            }
        }
        internal static async Task<bool> RealizaPromocionAmigo(AmigosModel amigo)
        {
            try
            {
                string requestUri = App.DAUtil.miURL + "promociones.php/GET?activaAmigo=true";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(amigo), Encoding.UTF8, "application/json")).Result;
                string resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        internal List<OfertasModel> getListadoOfertas()
        {
            List<OfertasModel> listCupones = new List<OfertasModel>();
            try
            {
                PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                string requestUri = App.DAUtil.miURL + "promociones.php/GET?ofertas=true&idGrupo=" + pu.idGrupo;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listCupones = JsonConvert.DeserializeObject<List<OfertasModel>>(resultJSON);
                }
                if (listCupones == null)
                    listCupones = new List<OfertasModel>();
                return listCupones;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listCupones;
            }
        }
        internal bool editaCromoInvitaAmigo(PromocionAmigoModel promo)
        {

            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    HttpResponseMessage response;
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "promociones.php?promoAmigos=true";
                    if (promo.id == 0)
                        response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(promo), Encoding.UTF8, "application/json")).Result;
                    else
                        response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(promo), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        promo = JsonConvert.DeserializeObject<PromocionAmigoModel>(resultJSON);
                        App.promocionAmigo = promo;
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool editaCupones(CuponesModel cupon)
        {

            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    HttpResponseMessage response;
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "promociones.php?cupones=true";
                    if (cupon.id == 0)
                        response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(cupon), Encoding.UTF8, "application/json")).Result;
                    else
                        response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(cupon), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        cupon = JsonConvert.DeserializeObject<CuponesModel>(resultJSON);
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool editaOfertas(OfertasModel oferta)
        {

            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    HttpResponseMessage response;
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "promociones.php?oferta=true";
                    if (oferta.id == 0)
                        response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(oferta), Encoding.UTF8, "application/json")).Result;
                    else
                        response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(oferta), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        oferta = JsonConvert.DeserializeObject<OfertasModel>(resultJSON);
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
        #region Publicidad
        internal List<PublicidadModel> getPublicidades()
        {
            List<PublicidadModel> listPubli = new List<PublicidadModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "publicidad.php/GET?idPueblo=" + Preferences.Get("idPueblo", 0);
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listPubli = JsonConvert.DeserializeObject<List<PublicidadModel>>(resultJSON);
                }
                if (listPubli == null)
                    listPubli = new List<PublicidadModel>();
                return listPubli;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listPubli;
            }
        }
        internal PublicidadModel acualizalink(PublicidadModel publi)
        {

            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    HttpResponseMessage response;
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "publicidad.php?links=true";
                    response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(publi), Encoding.UTF8, "application/json")).Result;
                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        return JsonConvert.DeserializeObject<PublicidadModel>(resultJSON); ;
                    }
                    else
                        return publi;
                }
                return publi;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return publi;
            }
        }
        internal PublicidadModel acualizaVisualizacion(PublicidadModel publi)
        {

            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    HttpResponseMessage response;
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "publicidad.php?visualizaciones=true";
                    response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(publi), Encoding.UTF8, "application/json")).Result;
                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        return JsonConvert.DeserializeObject<PublicidadModel>(resultJSON); ;
                    }
                    else
                        return publi;
                }
                return publi;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return publi;
            }
        }
        #endregion
        #region Establecimientos
        internal static List<FacturaModel> getFacturasEstablecimiento(int idEstablecimiento)
        {
            List<FacturaModel> listadoFacturas = new List<FacturaModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?idEstablecimientoFactura=" + idEstablecimiento;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    listadoFacturas = JsonConvert.DeserializeObject<List<FacturaModel>>(resultJSON);

                return listadoFacturas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listadoFacturas;
            }
        }
        internal static List<FacturaAdministradorModel> getFacturasAdministrador(int idPueblo)
        {
            List<FacturaAdministradorModel> listadoFacturas = new List<FacturaAdministradorModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "facturas.php/GET?idPueblo=" + idPueblo;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    listadoFacturas = JsonConvert.DeserializeObject<List<FacturaAdministradorModel>>(resultJSON);

                return listadoFacturas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listadoFacturas;
            }
        }
        internal static List<FacturaEstInformeModel> getFacturas()
        {
            List<FacturaEstInformeModel> listadoFacturas = new List<FacturaEstInformeModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "facturas.php/GET?todo=true";
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    listadoFacturas = JsonConvert.DeserializeObject<List<FacturaEstInformeModel>>(resultJSON);

                return listadoFacturas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listadoFacturas;
            }
        }
        internal static Establecimiento getEstablecimiento(int idEstablecimiento)
        {
            try
            {
                List<Establecimiento> listadoHome = new List<Establecimiento>();
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?id=" + idEstablecimiento;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listadoHome = JsonConvert.DeserializeObject<List<Establecimiento>>(resultJSON);
                    Establecimiento es = listadoHome.FirstOrDefault();
                    if (es.activoHoy)
                    {
                        if (es.activoMan == 1 || es.activoTarde == 1)
                            es.horario = ((TimeSpan)es.inicioHoy).ToString(@"hh\:mm") + " - " + ((TimeSpan)es.finHoy).ToString(@"hh\:mm");
                        else
                            es.horario = AppResources.Cerrado;
                    }
                    else
                        es.horario = AppResources.Cerrado;
                    es.configuracion = getConfiguracionEstablecimiento(es.idEstablecimiento);
                    return es;
                }
                return new Establecimiento();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Establecimiento(); ;
            }
        }
        internal static List<ComboModel> getCombos()
        {
            try
            {
                List<ComboModel> listadoHome = new List<ComboModel>();
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?combos=true";
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listadoHome = JsonConvert.DeserializeObject<List<ComboModel>>(resultJSON);
                }
                return listadoHome;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ComboModel>();
            }
        }
        internal static EstablecimientoFiscalModel getDatosFiscalesEstablecimiento(int idEstablecimiento)
        {
            try
            {
                List<EstablecimientoFiscalModel> listadoHome = new List<EstablecimientoFiscalModel>();
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?idFiscal=" + idEstablecimiento;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listadoHome = JsonConvert.DeserializeObject<List<EstablecimientoFiscalModel>>(resultJSON);
                    return listadoHome.FirstOrDefault();
                }
                return new EstablecimientoFiscalModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new EstablecimientoFiscalModel(); ;
            }
        }
        internal static AdministradorFiscalModel getDatosFiscalesGrupoPueblos(int idGrupo)
        {
            try
            {
                List<AdministradorFiscalModel> listadoHome = new List<AdministradorFiscalModel>();
                string requestUri = App.DAUtil.miURL + "facturas.php/GET?idFiscal=" + idGrupo;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listadoHome = JsonConvert.DeserializeObject<List<AdministradorFiscalModel>>(resultJSON);
                    return listadoHome.FirstOrDefault();
                }
                return new AdministradorFiscalModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new AdministradorFiscalModel(); ;
            }
        }
        public static async Task<List<Establecimiento>> getListadoAdmin()
        {
            List<Establecimiento> listadoHome = new List<Establecimiento>();
            List<HomeModel> listado = new List<HomeModel>();
            try
            {
                int idUsuario = 0;
                if (App.DAUtil.Usuario != null)
                {
                    idUsuario = App.DAUtil.Usuario.idUsuario;
                }
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?homeAdmin2=true&idGrupo=" + Preferences.Get("idGrupo", 1) + "&idPueblo=" + Preferences.Get("idPueblo", 0) + "&idUsuario=" + idUsuario;
                HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                string resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listadoHome = JsonConvert.DeserializeObject<List<Establecimiento>>(resultJSON);
                    listadoHome = CargaHorario(listadoHome);
                }
                App.DAUtil.DeleteEstablecimientos();
                App.DAUtil.SaveEstablecimientos(listadoHome);
                return listadoHome;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listadoHome;
            }
        }
        public static async Task<List<Establecimiento>> getListadoAdmin(int idGrupo, int idPueblo)
        {
            List<Establecimiento> listadoHome = new List<Establecimiento>();
            List<HomeModel> listado = new List<HomeModel>();
            try
            {
                int idUsuario = 0;
                if (App.DAUtil.Usuario != null)
                {
                    idUsuario = App.DAUtil.Usuario.idUsuario;
                }
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?homeAdmin2=true&idGrupo=" + idGrupo + "&idPueblo=" + idPueblo + "&idUsuario=" + idUsuario;
                HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                string resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listadoHome = JsonConvert.DeserializeObject<List<Establecimiento>>(resultJSON);
                    listadoHome = CargaHorario(listadoHome);
                }
                App.DAUtil.DeleteEstablecimientos();
                App.DAUtil.SaveEstablecimientos(listadoHome);
                return listadoHome;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listadoHome;
            }
        }

        private static List<Establecimiento> CargaHorario(List<Establecimiento> listadoHome)
        {
            ConfiguracionAdmin conf = getConfiguracionAdmin();
            bool activoMan = true;
            bool activoTarde = true;
            TimeSpan inicioMan = new TimeSpan();
            TimeSpan finMan = new TimeSpan();
            TimeSpan inicioTarde = new TimeSpan();
            TimeSpan finTarde = new TimeSpan();
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    activoMan = conf.activoLunes;
                    activoTarde = conf.activoLunesTarde;
                    inicioMan = conf.inicioLunes;
                    finMan = conf.finLunes;
                    inicioTarde = conf.inicioLunesTarde;
                    finTarde = conf.finLunesTarde;
                    break;
                case DayOfWeek.Tuesday:
                    activoMan = conf.activoMartes;
                    activoTarde = conf.activoMartesTarde;
                    inicioMan = conf.inicioMartes;
                    finMan = conf.finMartes;
                    inicioTarde = conf.inicioMartesTarde;
                    finTarde = conf.finMartesTarde;
                    break;
                case DayOfWeek.Wednesday:
                    activoMan = conf.activoMiercoles;
                    activoTarde = conf.activoMiercolesTarde;
                    inicioMan = conf.inicioMiercoles;
                    finMan = conf.finMiercoles;
                    inicioTarde = conf.inicioMiercolesTarde;
                    finTarde = conf.finMiercolesTarde;
                    break;
                case DayOfWeek.Thursday:
                    activoMan = conf.activoJueves;
                    activoTarde = conf.activoJuevesTarde;
                    inicioMan = conf.inicioJueves;
                    finMan = conf.finJueves;
                    inicioTarde = conf.inicioJuevesTarde;
                    finTarde = conf.finJuevesTarde;
                    break;
                case DayOfWeek.Friday:
                    activoMan = conf.activoViernes;
                    activoTarde = conf.activoViernesTarde;
                    inicioMan = conf.inicioViernes;
                    finMan = conf.finViernes;
                    inicioTarde = conf.inicioViernesTarde;
                    finTarde = conf.finViernesTarde;
                    break;
                case DayOfWeek.Saturday:
                    activoMan = conf.activoSabado;
                    activoTarde = conf.activoSabadoTarde;
                    inicioMan = conf.inicioSabado;
                    finMan = conf.finSabado;
                    inicioTarde = conf.inicioSabadoTarde;
                    finTarde = conf.finSabadoTarde;
                    break;
                case DayOfWeek.Sunday:
                    activoMan = conf.activoDomingo;
                    activoTarde = conf.activoDomingoTarde;
                    inicioMan = conf.inicioDomingo;
                    finMan = conf.finDomingo;
                    inicioTarde = conf.inicioDomingoTarde;
                    finTarde = conf.finDomingoTarde;
                    break;
            }
            foreach (Establecimiento es in listadoHome)
            {
                if (es.finMan > finMan)
                    es.finMan = finMan;
                if (es.inicioMan < inicioMan)
                    es.inicioMan = inicioMan;
                if (es.finTarde > finTarde)
                    es.finTarde = finTarde;
                if (es.inicioTarde < inicioTarde)
                    es.inicioTarde = inicioTarde;
                if (!activoMan || es.inicioMan > es.finMan)
                    es.activoMan = 0;
                if (!activoTarde || es.inicioTarde > es.finTarde)
                    es.activoTarde = 0;
                if (!conf.servicioActivo)
                    es.servicioActivo = false;

                if ((es.activoMan == 0 && es.activoTarde == 0) || !es.servicioActivo)
                    es.horario = AppResources.Cerrado;
                else if (DateTime.Now.Hour <= 16)
                {
                    if (es.activoMan == 0)
                        es.horario = ((TimeSpan)es.inicioTarde).ToString(@"hh\:mm") + " - " + ((TimeSpan)es.finTarde).ToString(@"hh\:mm");
                    else
                    {
                        if (es.finMan < DateTime.Now.TimeOfDay && es.activoTarde == 1)
                            es.horario = ((TimeSpan)es.inicioTarde).ToString(@"hh\:mm") + " - " + ((TimeSpan)es.finTarde).ToString(@"hh\:mm");
                        else
                            es.horario = ((TimeSpan)es.inicioMan).ToString(@"hh\:mm") + " - " + ((TimeSpan)es.finMan).ToString(@"hh\:mm");
                    }
                }
                else
                {
                    if (es.activoTarde == 0)
                        es.horario = AppResources.Cerrado;
                    else
                        es.horario = ((TimeSpan)es.inicioTarde).ToString(@"hh\:mm") + " - " + ((TimeSpan)es.finTarde).ToString(@"hh\:mm");
                }
            }
            return listadoHome;
        }

        internal List<TokensModel> getTokenEstablecimiento(int id)
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "establecimientos.php/GET?idEstablecimientoToken=" + id;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    return JsonConvert.DeserializeObject<List<TokensModel>>(resultJSON);
                }
                return new List<TokensModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getTokenAdministrador: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return new List<TokensModel>();
            }
        }
        public static List<Establecimiento> getListadoEstablecimientos()
        {
            List<Establecimiento> listEstablecimientos = new List<Establecimiento>();
            try
            {
                PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?idGrupo=" + pu.idGrupo;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listEstablecimientos = JsonConvert.DeserializeObject<List<Establecimiento>>(resultJSON);
                    //listEstablecimientos = CargaHorario(listEstablecimientos);
                }
                return listEstablecimientos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listEstablecimientos;
            }
        }
        public static List<Establecimiento> getListadoEstablecimientosPueblo(int idPueblo)
        {
            List<Establecimiento> listEstablecimientos = new List<Establecimiento>();
            try
            {
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?pueblo=true&idPueblo=" + idPueblo;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listEstablecimientos = JsonConvert.DeserializeObject<List<Establecimiento>>(resultJSON);
                }
                return listEstablecimientos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listEstablecimientos;
            }
        }
        public static List<Establecimiento> getListadoEstablecimientos(int id)
        {
            List<Establecimiento> listEstablecimientos = new List<Establecimiento>();
            try
            {
                PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == id);
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?idGrupo=" + pu.idGrupo;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listEstablecimientos = JsonConvert.DeserializeObject<List<Establecimiento>>(resultJSON);
                }
                return listEstablecimientos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listEstablecimientos;
            }
        }
        public static List<Establecimiento> getListadoTodosEstablecimientos(int idPueblo)
        {
            List<Establecimiento> listEstablecimientos = new List<Establecimiento>();
            try
            {
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?todos=true&idPueblo=" + idPueblo;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listEstablecimientos = JsonConvert.DeserializeObject<List<Establecimiento>>(resultJSON);
                }
                return listEstablecimientos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listEstablecimientos;
            }
        }
        internal double getGastosEnvioOtroPueblo()
        {

            try
            {
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?puebloCliente=" + App.DAUtil.Usuario.idPueblo + "&puebloEstablecimiento=" + App.EstActual.idPueblo;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {

                    return JsonConvert.DeserializeObject<GastosOtroPuebloModel>(resultJSON).gastos;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        internal double getGastosEnvioOtroPueblo(int idPuebloCli, int idPuebloEst)
        {

            try
            {
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?puebloCliente=" + idPuebloCli + "&puebloEstablecimiento=" + idPuebloEst;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {

                    return JsonConvert.DeserializeObject<GastosOtroPuebloModel>(resultJSON).gastos;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        internal int nuevoEstablecimiento(Establecimiento establecimiento, string password)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "establecimientos.php?password=" + Crypto.Encrypt(password).Replace(" ", "").Replace("+", "");
                    HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(establecimiento), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        internal bool actualizaEstablecimiento(Establecimiento establecimiento)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "establecimientos.php?actualizaEstablecimiento=true";
                    HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(establecimiento), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        internal EstablecimientoFiscalModel actualizaDatosFiscalesEstablecimiento(EstablecimientoFiscalModel establecimiento)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "establecimientos.php?fiscal=true";
                    HttpResponseMessage response;
                    if (establecimiento.id > 0)
                    {
                        response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(establecimiento), Encoding.UTF8, "application/json")).Result;
                        var resultJSON = response.Content.ReadAsStringAsync().Result;
                        if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                        {
                            App.DAUtil.GuardaDatosFiscalesEstablecimiento(establecimiento);
                            return establecimiento;
                        }
                    }
                    else
                    {
                        response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(establecimiento), Encoding.UTF8, "application/json")).Result;
                        var resultJSON = response.Content.ReadAsStringAsync().Result;

                        if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                        {
                            EstablecimientoFiscalModel f = JsonConvert.DeserializeObject<EstablecimientoFiscalModel>(resultJSON);
                            App.DAUtil.GuardaDatosFiscalesEstablecimiento(f);
                            return f;
                        }
                    }

                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        internal bool nuevaVisita(int idEstablecimiento)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet() && App.DAUtil.Usuario != null)
                {
                    DatosConexionModel.uri = urlPro;
                    VisitaModel visita = new VisitaModel();
                    visita.idEstablecimiento = idEstablecimiento;
                    visita.idUsuario = App.DAUtil.Usuario.idUsuario;
                    visita.modo = Preferences.Get("modoPedido", "OUT");
                    string requestUri = DatosConexionModel.uri + "establecimientos.php?visitas=true";
                    HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(visita), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool nuevaValoracion(ValoracionModel valoracion)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "establecimientos.php?valoraciones=true";
                    HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(valoracion), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        internal async Task<bool> tienePedidos(int idEstablecimiento)
        {
            string result = string.Empty;
            try
            {
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?idEstablecimientoTienePedido=" + idEstablecimiento;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = await response.Content.ReadAsStringAsync();
                ResultdadoModel r = JsonConvert.DeserializeObject<ResultdadoModel>(resultJSON);
                return r.resultado > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool compruebaFranja(int idEstablecimiento, string desde, string hasta, int total)
        {
            string result = string.Empty;
            try
            {
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?idEstablecimientoFranja=" + idEstablecimiento + "&desde=" + desde + "&hasta=" + hasta;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                ResultdadoModel r = JsonConvert.DeserializeObject<ResultdadoModel>(resultJSON);
                return r.resultado < total;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal List<FranjaHorariaModel> compruebaFranjaOtroPueblo(int idEstablecimiento, int idPuebloDestino, int idPuebloOrigen, int total, TimeSpan inicio, TimeSpan fin)
        {
            string result = string.Empty;
            try
            {
                if (inicio < DateTime.Now.TimeOfDay)
                    inicio = DateTime.Now.TimeOfDay;
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?idEstablecimientoFranjaOtroPueblo=" + idEstablecimiento + "&origen=" + idPuebloOrigen + "&destino=" + idPuebloDestino + "&inicio=" + inicio + "&fin=" + fin;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                List<FranjaHorariaModel> r = JsonConvert.DeserializeObject<List<FranjaHorariaModel>>(resultJSON);
                return r;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<FranjaHorariaModel>();
            }
        }
        internal bool compruebaFranjaReparto(int idEstablecimiento, string desde, string hasta, int total)
        {
            string result = string.Empty;
            try
            {
                string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?idEstablecimientoFranjaEncargo=" + idEstablecimiento + "&desde=" + desde + "&hasta=" + hasta;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                ResultdadoModel r = JsonConvert.DeserializeObject<ResultdadoModel>(resultJSON);
                return r.resultado < total;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal static ObservableCollection<CabeceraPedido> getHistoricoPedidosEstablecimiento(Establecimiento est)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?idEstablecimientoHistorico=" + est.idEstablecimiento + "&idGrupo=" + pu.idGrupo + "&idPueblo=" + pu.id;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                        {
                            result = JsonConvert.DeserializeObject<ObservableCollection<CabeceraPedido>>(resultJSON);
                        }

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        internal static List<PreFacturaModel> getHistoricoPedidosEstablecimiento(Establecimiento est, DateTime desde, DateTime hasta)
        {
            List<PreFacturaModel> result = new List<PreFacturaModel>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "establecimientos.php/GET?idEstablecimientoHistorico2=" + est.idEstablecimiento + "&desde=" + desde.ToString("yyyy-MM-dd") + "&hasta=" + hasta.ToString("yyyy-MM-dd");
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                        {
                            result = JsonConvert.DeserializeObject<List<PreFacturaModel>>(resultJSON);
                        }

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        #endregion
        #region Click
        internal async Task<bool> guardaClick(ClickModel click)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    click.fecha = DateTime.Now;
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "logs.php";
                    HttpResponseMessage response = await App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(click), Encoding.UTF8, "application/json"));

                    var resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        Categoria cat = JsonConvert.DeserializeObject<Categoria>(resultJSON);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
        #region Categorias
        internal async Task<bool> eliminaCategoriasYProductos(int idEstablecimiento)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "categorias.php?idEstablecimiento=" + idEstablecimiento;
                    HttpResponseMessage response = await App.Client.DeleteAsync(requestUri);

                    var resultJSON = await response.Content.ReadAsStringAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return false;
        }
        public static List<Categoria> getListadoCategorias(int idEstablecimiento)
        {
            List<Categoria> listCategorias = new List<Categoria>();
            try
            {
                string requestUri = App.DAUtil.miURL + "categorias.php/GET?idEstablecimiento=" + idEstablecimiento;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listCategorias = JsonConvert.DeserializeObject<List<Categoria>>(resultJSON);
                }
                if (listCategorias == null)
                    listCategorias = new List<Categoria>();
                return listCategorias;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listCategorias;
            }
        }
        internal int nuevaCategoria(Categoria categoria)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "categorias.php";
                    HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(categoria), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        Categoria cat = JsonConvert.DeserializeObject<Categoria>(resultJSON);
                        return cat.id;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }
        internal bool actualizaCategoria(Categoria categoria)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "categorias.php";
                    HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(categoria), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
        #region Administradores
        internal static List<PueblosModel> getPueblosAdministrador()
        {
            List<PueblosModel> listPueblos = new List<PueblosModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "administradores.php/GET?idUsuario=" + App.DAUtil.Usuario.idUsuario;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listPueblos = JsonConvert.DeserializeObject<List<PueblosModel>>(resultJSON);
                    if (listPueblos == null)
                        listPueblos = new List<PueblosModel>();
                }
                return listPueblos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<PueblosModel>();
            }
        }
        internal static List<PueblosModel> getPueblosSuperAdministrador()
        {
            List<PueblosModel> listPueblos = new List<PueblosModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "administradores.php/GET?todos=true";
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listPueblos = JsonConvert.DeserializeObject<List<PueblosModel>>(resultJSON);
                    if (listPueblos == null)
                        listPueblos = new List<PueblosModel>();
                }
                return listPueblos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<PueblosModel>();
            }
        }
        #endregion
        #region Productos
        internal static List<ArticuloModel> getListadoProductosMasVendidos()
        {
            List<ArticuloModel> listProducto = new List<ArticuloModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "productos.php/GET?masvendidos=true&idGrupo=" + Preferences.Get("idGrupo", 1);
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listProducto = JsonConvert.DeserializeObject<List<ArticuloModel>>(resultJSON);
                    if (listProducto == null)
                        listProducto = new List<ArticuloModel>();
                }
                return listProducto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ArticuloModel>();
            }
        }
        internal static List<AlergenosModel> GetAlergenos()
        {
            List<AlergenosModel> listProducto = new List<AlergenosModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "productos.php/GET?alergenos=true";
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listProducto = JsonConvert.DeserializeObject<List<AlergenosModel>>(resultJSON);
                    if (listProducto == null)
                        listProducto = new List<AlergenosModel>();
                    App.DAUtil.GuardaAlergenos(listProducto);
                }
                return listProducto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<AlergenosModel>();
            }
        }
        internal static List<ArticuloModel> getListadoProductosMasVendidosLocal()
        {
            List<ArticuloModel> listProducto = new List<ArticuloModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "productos.php/GET?masvendidoslocal=true&idGrupo=" + Preferences.Get("idGrupo1", 1);
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listProducto = JsonConvert.DeserializeObject<List<ArticuloModel>>(resultJSON);
                    if (listProducto == null)
                        listProducto = new List<ArticuloModel>();
                }
                return listProducto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ArticuloModel>();
            }
        }
        internal static List<ArticuloModel> TraePodructosBaja(string ids)
        {
            List<ArticuloModel> listProducto = new List<ArticuloModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "productos.php/GET?ids=" + ids;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listProducto = JsonConvert.DeserializeObject<List<ArticuloModel>>(resultJSON);
                    if (listProducto == null)
                        listProducto = new List<ArticuloModel>();
                }
                return listProducto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ArticuloModel>();
            }
        }
        internal int getCantidadIngredientes(int idProducto)
        {
            try
            {
                int cantidad = 0;
                string requestUri = App.DAUtil.miURL + "productos.php/GET?cantidadIngredientes=" + idProducto;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    cantidad = JsonConvert.DeserializeObject<int>(resultJSON);

                }
                return cantidad;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        internal async Task<List<ArticuloModel>> getListadoProductosEstablecimientoCat(int id, bool todo)
        {
            List<ArticuloModel> listProducto = new List<ArticuloModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "productos.php/GET?idEstablecimientoProductoCat=" + id.ToString();
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listProducto = JsonConvert.DeserializeObject<List<ArticuloModel>>(resultJSON);
                    if (listProducto != null)
                        App.DAUtil.saveArticulosEstablecimiento(listProducto);
                    else
                    {
                        App.DAUtil.EliminaProductos();
                        listProducto = new List<ArticuloModel>();
                    }
                }
                else
                {
                    App.DAUtil.EliminaProductos();
                    listProducto = new List<ArticuloModel>();
                }
                foreach (ArticuloModel p in listProducto)
                {
                    p.listadoOpciones = new ObservableCollection<OpcionesModel>();
                    p.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>();
                    p.listadoAlergenos = new ObservableCollection<AlergenosModel>();
                }
                return listProducto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listProducto;
            }
        }
        internal async Task<List<ArticuloModel>> getListadoProductosEstablecimiento()
        {
            List<ArticuloModel> listProducto = new List<ArticuloModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "productos.php/GET?all=1";
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listProducto = JsonConvert.DeserializeObject<List<ArticuloModel>>(resultJSON);
                    if (listProducto != null)
                        App.DAUtil.saveArticulosEstablecimiento(listProducto);
                    else
                    {
                        App.DAUtil.EliminaProductos();
                        listProducto = new List<ArticuloModel>();
                    }
                }
                else
                {
                    App.DAUtil.EliminaProductos();
                    listProducto = new List<ArticuloModel>();
                }
                foreach (ArticuloModel p in listProducto)
                {
                    p.listadoOpciones = new ObservableCollection<OpcionesModel>();
                    p.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>();
                    p.listadoAlergenos = new ObservableCollection<AlergenosModel>();
                }
                return listProducto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listProducto;
            }
        }
        internal async Task<List<ArticuloModel>> getListadoProductosEstablecimiento(int id, bool todo)
        {
            List<ArticuloModel> listProducto = new List<ArticuloModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "productos.php/GET?idEstablecimientoProducto=" + id.ToString();
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listProducto = JsonConvert.DeserializeObject<List<ArticuloModel>>(resultJSON);
                    if (listProducto != null)
                        App.DAUtil.saveArticulosEstablecimiento(listProducto);
                    else
                    {
                        App.DAUtil.EliminaProductos();
                        listProducto = new List<ArticuloModel>();
                    }
                }
                else
                {
                    App.DAUtil.EliminaProductos();
                    listProducto = new List<ArticuloModel>();
                }
                foreach (ArticuloModel p in listProducto)
                {
                    p.listadoOpciones = new ObservableCollection<OpcionesModel>();
                    p.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>();
                    p.listadoAlergenos = new ObservableCollection<AlergenosModel>();
                }
                return listProducto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listProducto;
            }
        }
        internal static List<ArticuloModel> getListadoProductosCategoria(int id)
        {
            List<ArticuloModel> listProducto = new List<ArticuloModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "productos.php/GET?idCategoriaProducto=" + id.ToString();
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listProducto = JsonConvert.DeserializeObject<List<ArticuloModel>>(resultJSON);
                }
                foreach (ArticuloModel p in listProducto)
                {
                    p.listadoOpciones = new ObservableCollection<OpcionesModel>();
                    p.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>();
                    p.listadoAlergenos = new ObservableCollection<AlergenosModel>();
                }
                return listProducto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listProducto;
            }
        }
        public async Task<List<IngredienteProductoModel>> listadoIngredientesProducto(int idProducto)
        {
            List<IngredienteProductoModel> listCategorias = new List<IngredienteProductoModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "productos.php/GET?idIngredientesProducto=" + idProducto;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listCategorias = JsonConvert.DeserializeObject<List<IngredienteProductoModel>>(resultJSON);
                    if (listCategorias != null)
                    {
                        if (listCategorias.Count > 0)
                        {
                            App.DAUtil.ActualizaIngredientesProducto(listCategorias);
                        }
                    }
                    else
                        listCategorias = new List<IngredienteProductoModel>();
                }
                return listCategorias;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listCategorias;
            }
        }
        public List<IngredientesModel> listadoIngredientes(int idEstablecimiento)
        {
            List<IngredientesModel> listCategorias = new List<IngredientesModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "productos.php/GET?idEstablecimientoIngredientes=" + idEstablecimiento;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listCategorias = JsonConvert.DeserializeObject<List<IngredientesModel>>(resultJSON);
                    if (listCategorias != null)
                    {
                        if (listCategorias.Count > 0)
                        {
                            App.DAUtil.ActualizaIngredientes(listCategorias);
                        }
                    }
                    else
                        listCategorias = new List<IngredientesModel>();
                }
                else
                    listCategorias = new List<IngredientesModel>();
                return listCategorias;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listCategorias;
            }
        }
        internal bool nuevoProducto(ArticuloModel comida)
        {
            try
            {
                if (comida.imagen.EndsWith("/"))
                    comida.imagen = $"{urlPro}images/logo_producto.png";
                if (comida.descripcion == null)
                    comida.descripcion = "";
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "productos.php";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(comida), Encoding.UTF8, "application/json")).Result;

                var resultJSON = response.Content.ReadAsStringAsync().Result;
                ArticuloModel respuesta = JsonConvert.DeserializeObject<ArticuloModel>(resultJSON);
                try
                {
                    comida.idArticulo = respuesta.id;
                    if (comida.listadoAlergenos != null)
                    {
                        foreach (AlergenosModel a in comida.listadoAlergenos)
                        {
                            if (a != null)
                            {
                                if (!insertaAlergenos(a, comida.idArticulo))
                                    return false;
                            }
                        }
                    }
                    if (comida.listadoOpciones != null)
                    {
                        foreach (OpcionesModel a in comida.listadoOpciones)
                        {
                            if (a != null)
                            {
                                if (!insertaOpciones(a, comida.idArticulo))
                                    return false;
                            }
                        }
                    }
                    if (comida.listadoIngredientes != null)
                    {
                        foreach (IngredienteProductoModel a in comida.listadoIngredientes)
                        {
                            if (a != null)
                            {
                                a.idProducto = comida.idArticulo;
                                nuevoIngredienteProducto(a);
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool nuevoProductoMenu(MenuDiarioProductosModel comida)
        {
            try
            {
                if (comida.imagen.EndsWith("/"))
                    comida.imagen = $"{urlPro}images/logo_producto.png";
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "menuDiario.php?productos=true";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(comida), Encoding.UTF8, "application/json")).Result;

                var resultJSON = response.Content.ReadAsStringAsync().Result;
                MenuDiarioProductosModel respuesta = JsonConvert.DeserializeObject<MenuDiarioProductosModel>(resultJSON);
                try
                {
                    comida.id = respuesta.id;

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal int nuevoIngredienteProducto(IngredienteProductoModel ingrediente)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "productos.php?ingredienteProducto=true";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(ingrediente), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                IngredienteProductoModel respuesta = JsonConvert.DeserializeObject<IngredienteProductoModel>(resultJSON);
                App.DAUtil.NuevoIngredienteProducto(respuesta);
                return respuesta.idIngrediente;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        internal bool nuevoIngrediente(IngredientesModel ingrediente)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "productos.php?ingrediente=true";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(ingrediente), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                IngredientesModel respuesta = JsonConvert.DeserializeObject<IngredientesModel>(resultJSON);
                App.DAUtil.NuevoIngrediente(respuesta);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool insertaAlergenos(AlergenosModel alergeno, int idProducto)
        {
            try
            {
                AlergenoProductoModel p = new AlergenoProductoModel();
                p.idAlergeno = alergeno.id;
                p.idProducto = idProducto;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "productos.php?alergenos=true";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                AlergenoProductoModel respuesta = JsonConvert.DeserializeObject<AlergenoProductoModel>(resultJSON);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool insertaOpciones(OpcionesModel opcion, int idProducto)
        {
            try
            {
                OpcionProductoModel p = new OpcionProductoModel();
                p.opcion = opcion.opcion;
                p.valorIncremento = double.Parse(opcion.precio.ToString().Replace(".", ","));
                p.idProducto = idProducto;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "productos.php?opcion=true";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                OpcionesModel respuesta = JsonConvert.DeserializeObject<OpcionesModel>(resultJSON);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool actualizaProducto(ArticuloModel comida)
        {
            try
            {
                if (comida.imagen.EndsWith("/"))
                    comida.imagen = $"{urlPro}images/logo_producto.png";

                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "productos.php";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(comida), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (resultJSON.Trim().Equals(""))
                {
                    eliminaDatosProducto(comida.idArticulo);
                    foreach (AlergenosModel a in comida.listadoAlergenos)
                    {
                        if (a != null)
                        {
                            if (!insertaAlergenos(a, comida.idArticulo))
                                return false;
                        }

                    }
                    NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
                    foreach (OpcionesModel a in comida.listadoOpciones)
                    {
                        if (a != null)
                        {
                            if (!string.IsNullOrEmpty(a.opcion) && !a.precio.Equals("0") && !a.precio.Equals(""))
                            {
                                if (!insertaOpciones(a, comida.idArticulo))
                                    return false;
                            }
                        }
                    }
                    foreach (IngredienteProductoModel a in comida.listadoIngredientes)
                    {
                        if (a != null)
                        {
                            nuevoIngredienteProducto(a);
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool actualizaProductoMenu(MenuDiarioProductosModel comida)
        {
            try
            {
                if (comida.imagen.EndsWith("/"))
                    comida.imagen = $"{urlPro}images/logo_producto.png";

                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "menuDiario.php?productos=true";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(comida), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (resultJSON.Trim().Equals(""))
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool actualizaMenuDiario(MenuDiarioModel comida)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "menuDiario.php";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(comida), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (resultJSON.Trim().Equals(""))
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool actualizaMenuDiarioConfiguracion(MenuDiarioConfiguracionModel comida)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "menuDiario.php?configuracion=true";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(comida), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (resultJSON.Trim().Equals(""))
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal MenuDiarioModel nuevoMenuDiario(MenuDiarioModel comida)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "menuDiario.php";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(comida), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                MenuDiarioModel menu = JsonConvert.DeserializeObject<MenuDiarioModel>(resultJSON); ;
                menu.configuracion = getConfiguracionMenuDiario(menu.idEstablecimiento);
                return menu;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new MenuDiarioModel();
            }
        }
        internal bool actualizaIngredienteProducto(IngredienteProductoModel ingrediente)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "productos.php?ingredienteProducto=true";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(ingrediente), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (resultJSON.Trim().Equals(""))
                {
                    App.DAUtil.ActualizaIngredienteProducto(ingrediente);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool actualizaIngrediente(IngredientesModel ingrediente)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "productos.php?ingrediente=true";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(ingrediente), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (resultJSON.Trim().Equals(""))
                {
                    App.DAUtil.ActualizaIngrediente(ingrediente);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool eliminaDatosProducto(int idProducto)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "productos.php?idProducto=" + idProducto;
                HttpResponseMessage response = App.Client.DeleteAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (resultJSON.Trim().Equals(""))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
        #region Usuarios
        internal static bool LoginSocial(AuthNetworkData socialLoginData)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = App.DAUtil.miURL + $"usuarios.php/GET?email={socialLoginData.Email}&id={socialLoginData.Id}&social=true";
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        UsuarioModel respuesta;
                        if (!resultJSON.ToLower().Equals("false"))
                        {
                            try
                            {
                                respuesta = JsonConvert.DeserializeObject<UsuarioModel>(resultJSON);
                            }
                            catch (Exception)
                            {
                                respuesta = new UsuarioModel();
                            }
                            if (!string.IsNullOrEmpty(respuesta.nombre))
                            {
                                respuesta.password = "";
                                respuesta.idSocial = socialLoginData.Id;
                                respuesta.nombreCompleto = $"{respuesta.nombre} {respuesta.apellidos}";
                                if (App.DAUtil.SaveConfiguracionUsuarioSQLite(respuesta))
                                {

                                    App.DAUtil.Usuario = respuesta;
                                    App.DAUtil.Usuario.tarjetas = getTarjetas(App.DAUtil.Usuario.idUsuario);
                                    if (App.DAUtil.Usuario.rol == (int)RolesEnum.Establecimiento || App.DAUtil.Usuario.rol == (int)RolesEnum.Administrador)
                                    {
                                        App.DAUtil.Usuario.establecimientos = getListadoEstablecimientos(App.DAUtil.Usuario.idPueblo);
                                        App.EstActual = getEstablecimiento(App.DAUtil.Usuario.establecimientos[0].idEstablecimiento);
                                        App.MiEst = App.EstActual;
                                    }
                                    else if (App.DAUtil.Usuario.rol == (int)RolesEnum.Repartidor)
                                    {
                                        App.DAUtil.Usuario.Repartidor = GetRepartidorByIdUsuario(respuesta.idUsuario);
                                    }

                                    Preferences.Set("idGrupo", 1);
                                    Preferences.Set("idPueblo", 1);
                                    /*List<NotificacionModel> poblaciones = listaNotificaciones();
                                    SaveNotificacionesSQLite(poblaciones);*/
                                    return true;
                                }
                                return false;
                            }
                            else
                            {
                                App.DAUtil.Usuario = new UsuarioModel();
                                App.DAUtil.Usuario.apellidos = socialLoginData.Apellidos;
                                App.DAUtil.Usuario.nombre = socialLoginData.Nombre;
                                App.DAUtil.Usuario.nombreCompleto = socialLoginData.Name;
                                App.DAUtil.Usuario.email = socialLoginData.Email;
                                App.DAUtil.Usuario.foto = socialLoginData.Picture.Replace("&", "___");
                                App.DAUtil.Usuario.idSocial = socialLoginData.Id;
                                App.DAUtil.Usuario.codPostal = "";
                                App.DAUtil.Usuario.demo = 0;
                                App.DAUtil.Usuario.direccion = "";
                                App.DAUtil.Usuario.dni = "";
                                App.DAUtil.Usuario.estado = 1;
                                App.DAUtil.Usuario.fechaAlta = DateTime.Now;
                                App.DAUtil.Usuario.fechaNacimiento = DateTime.Now;
                                App.DAUtil.Usuario.idPueblo = 0;
                                App.DAUtil.Usuario.idUsuario = 0;
                                App.DAUtil.Usuario.idZona = 0;
                                App.DAUtil.Usuario.password = "";
                                App.DAUtil.Usuario.pin = "";
                                App.DAUtil.Usuario.platform = DeviceInfo.Platform.ToString().ToLower();
                                App.DAUtil.Usuario.poblacion = "";
                                App.DAUtil.Usuario.provincia = "";
                                App.DAUtil.Usuario.rol = 1;
                                App.DAUtil.Usuario.telefono = "";
                                App.DAUtil.Usuario.token = App.DAUtil.InstallId;
                                App.DAUtil.Usuario.username = socialLoginData.Email;
                                if (DeviceInfo.Platform.ToString() == "iOS")
                                    App.DAUtil.Usuario.version = App.DAUtil.versioniOS;
                                else if (DeviceInfo.Platform.ToString() == "Android")
                                    App.DAUtil.Usuario.version = App.DAUtil.versionAndroid;
                                registroUsuario(App.DAUtil.Usuario);
                                App.DAUtil.SaveConfiguracionUsuarioSQLite(App.DAUtil.Usuario);
                                return true;
                            }
                        }
                        else
                        {
                            App.DAUtil.Usuario = new UsuarioModel();
                            App.DAUtil.Usuario.apellidos = socialLoginData.Apellidos;
                            App.DAUtil.Usuario.nombre = socialLoginData.Nombre;
                            App.DAUtil.Usuario.nombreCompleto = socialLoginData.Name;
                            App.DAUtil.Usuario.email = socialLoginData.Email;
                            App.DAUtil.Usuario.foto = "logo_producto.png";
                            App.DAUtil.Usuario.idSocial = socialLoginData.Id;
                            App.DAUtil.Usuario.codPostal = "";
                            App.DAUtil.Usuario.demo = 0;
                            App.DAUtil.Usuario.direccion = "";
                            App.DAUtil.Usuario.dni = "";
                            App.DAUtil.Usuario.estado = 1;
                            App.DAUtil.Usuario.fechaAlta = DateTime.Now;
                            App.DAUtil.Usuario.fechaNacimiento = DateTime.Now;
                            App.DAUtil.Usuario.idPueblo = 0;
                            App.DAUtil.Usuario.idUsuario = 0;
                            App.DAUtil.Usuario.idZona = 0;
                            App.DAUtil.Usuario.password = "";
                            App.DAUtil.Usuario.pin = "";
                            App.DAUtil.Usuario.platform = DeviceInfo.Platform.ToString().ToLower();
                            App.DAUtil.Usuario.poblacion = "";
                            App.DAUtil.Usuario.provincia = "";
                            App.DAUtil.Usuario.rol = 1;
                            App.DAUtil.Usuario.telefono = "";
                            App.DAUtil.Usuario.token = App.DAUtil.InstallId;
                            App.DAUtil.Usuario.username = socialLoginData.Email;
                            if (DeviceInfo.Platform.ToString() == "iOS")
                                App.DAUtil.Usuario.version = App.DAUtil.versioniOS;
                            else if (DeviceInfo.Platform.ToString() == "Android")
                                App.DAUtil.Usuario.version = App.DAUtil.versionAndroid;
                            App.DAUtil.Usuario.idUsuario = int.Parse(registroUsuario(App.DAUtil.Usuario));
                            App.DAUtil.SaveConfiguracionUsuarioSQLite(App.DAUtil.Usuario);
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        internal static void EncryptaContraseñas()
        {
            DatosConexionModel.uri = urlPro;
            string requestUri = DatosConexionModel.uri + $"usuarios.php";
            HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
            string resultJSON = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
            {
                List<UsuarioModel> respuesta = JsonConvert.DeserializeObject<List<UsuarioModel>>(resultJSON);
                foreach (UsuarioModel u in respuesta.Where(p => p.foto.Contains("tl_rest")))
                {
                    CambioPassEncrypt(u.email, u.password);
                }
            }
        }
        internal static bool Login(string user, string pass)
        {

            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + $"usuarios.php/GET?email={user}&password={Crypto.Encrypt(pass).Replace(" ", "").Replace("+", "")}";
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        UsuarioModel respuesta = JsonConvert.DeserializeObject<UsuarioModel>(resultJSON);

                        if (!string.IsNullOrEmpty(respuesta.nombre))
                        {
                            if (string.IsNullOrEmpty(respuesta.password))
                            {
                                App.DAUtil.Usuario = respuesta;
                                return false;
                            }
                            else
                            {
                                respuesta.password = pass;
                                respuesta.nombreCompleto = $"{respuesta.nombre} {respuesta.apellidos}";
                                if (App.DAUtil.SaveConfiguracionUsuarioSQLite(respuesta))
                                {

                                    App.DAUtil.Usuario = respuesta;
                                    App.DAUtil.Usuario.tarjetas = getTarjetas(App.DAUtil.Usuario.idUsuario);
                                    //App.DAUtil.Usuario.tarjetas = new List<TarjetaModel>();
                                    if (App.DAUtil.Usuario.rol == (int)RolesEnum.Establecimiento || App.DAUtil.Usuario.rol == (int)RolesEnum.Administrador)
                                    {
                                        App.DAUtil.Usuario.establecimientos = getListadoEstablecimientos(App.DAUtil.Usuario.idPueblo);
                                        App.EstActual = getEstablecimiento(App.DAUtil.Usuario.establecimientos[0].idEstablecimiento);
                                        App.MiEst = App.EstActual;
                                    }
                                    else if (App.DAUtil.Usuario.rol == (int)RolesEnum.Repartidor)
                                    {
                                        App.DAUtil.Usuario.Repartidor = GetRepartidorByIdUsuario(respuesta.idUsuario);
                                    }
                                    else if (App.DAUtil.Usuario.rol == (int)RolesEnum.Cliente)
                                        GuardaOnline(1);

                                    Preferences.Set("idGrupo", 1);
                                    Preferences.Set("idPueblo", 1);


                                    return true;
                                }
                            }
                            return false;
                        }
                    }
                    return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Login: " + ex.Message);
                return false;
            }
        }
        internal static bool usuarioVerificado(string user)
        {

            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + $"usuarios.php/GET?usuarioVerificado={user}";
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        ResultdadoModel respuesta = JsonConvert.DeserializeObject<ResultdadoModel>(resultJSON);

                        return respuesta.resultado == 1;
                    }
                    return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Login: " + ex.Message);
                return false;
            }
        }

        public bool RegistraTokenFCM(UsuarioModel usuario)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = urlPro + "usuarios.php";
                    HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json")).Result;
                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        public static bool HaValoradoLaApp()
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "valoraciones.php/GET?idUsuarioApp=" + App.DAUtil.Usuario.idUsuario;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    ValoracionModel val = JsonConvert.DeserializeObject<ValoracionModel>(resultJSON);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                App.userdialog.HideLoading();
                return false;
            }
        }
        public static List<Establecimiento> CargaEstablecimientosUsuario()
        {
            List<Establecimiento> listEstablecimientos = new List<Establecimiento>();
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?idUsuarioEstablecimiento=" + App.DAUtil.Usuario.idUsuario;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listEstablecimientos = JsonConvert.DeserializeObject<List<Establecimiento>>(resultJSON);
                }
                return listEstablecimientos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                App.userdialog.HideLoading();
                return new List<Establecimiento>();
            }
        }
        public static ZonaModel GetZonaByIdUsuario()
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?idUsuarioZona=" + App.DAUtil.Usuario.idUsuario;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    return JsonConvert.DeserializeObject<ZonaModel>(resultJSON);
                }
                return new ZonaModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                App.userdialog.HideLoading();
                return new ZonaModel();
            }
        }
        public static Establecimiento CargaEstablecimientosUsuario(int id)
        {
            List<Establecimiento> listEstablecimientos = new List<Establecimiento>();
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?idUsuarioEstablecimiento=" + id;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listEstablecimientos = JsonConvert.DeserializeObject<List<Establecimiento>>(resultJSON);
                }
                return listEstablecimientos.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                App.userdialog.HideLoading();
                return null;
            }
        }
        internal static async Task<ObservableCollection<CabeceraPedido>> getPedidoPendienteEstadoRecogidoSync(int idUsuario)
        {
            ObservableCollection<Pedido> listPedido;
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?idUsuarioPedidoPendiente=" + idUsuario;
                    string requestUri = DatosConexionModel.uri;

                    HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                    string resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        listPedido = JsonConvert.DeserializeObject<ObservableCollection<Pedido>>(resultJSON);
                        if (listPedido != null)
                        {
                            if (listPedido.Count > 0)
                                result = convertirToPedidoInterno(listPedido);
                        }

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // 
                return result;
            }
        }
        internal List<UsuarioModel> GetListadoUsuariosConPin()
        {
            List<UsuarioModel> listCategorias = new List<UsuarioModel>();
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?conPin=true";
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listCategorias = JsonConvert.DeserializeObject<List<UsuarioModel>>(resultJSON);
                }
                return listCategorias;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listCategorias;
            }
        }
        internal async Task<List<UsuarioModel>> GetAdministradores()
        {
            List<UsuarioModel> listCategorias = new List<UsuarioModel>();
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?administradores=true";
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listCategorias = JsonConvert.DeserializeObject<List<UsuarioModel>>(resultJSON);
                }
                return listCategorias;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listCategorias;
            }
        }
        internal List<UsuarioModel> GetUsuariosFiltro(string filtro)
        {
            List<UsuarioModel> listCategorias = new List<UsuarioModel>();
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?filtro=" + filtro;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listCategorias = JsonConvert.DeserializeObject<List<UsuarioModel>>(resultJSON);
                }
                return listCategorias;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listCategorias;
            }
        }
        internal static int contadorUsuarios()
        {
            string result = string.Empty;
            try
            {
                PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                string requestUri = App.DAUtil.miURL + "usuarios.php/GET?contador=true&idGrupo=" + pu.idGrupo;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                ResultdadoModel r = JsonConvert.DeserializeObject<ResultdadoModel>(resultJSON);
                return r.resultado;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error contadorUsuarios: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        internal List<TokensModel> getTokenMultiAdministrador(int idPueblo)
        {
            try
            {
                //TODO: Gestionar administradores
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?tokenMultiAdmin=" + idPueblo;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    return JsonConvert.DeserializeObject<List<TokensModel>>(resultJSON);
                }
                return new List<TokensModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getTokenAdministrador: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return new List<TokensModel>();
            }
        }
        internal List<TokensModel> getTokenRepartidores(int idEst)
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?tokenRepartidoresEst=" + idEst;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    return JsonConvert.DeserializeObject<List<TokensModel>>(resultJSON);
                }
                return new List<TokensModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getTokenRepartidores: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return new List<TokensModel>();
            }
        }
        internal List<TokensModel> getTokenRepartidores()
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?tokenRepartidores=true&idGrupo=" + Preferences.Get("idGrupo", 0);
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    return JsonConvert.DeserializeObject<List<TokensModel>>(resultJSON);
                }
                return new List<TokensModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getTokenRepartidores: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return new List<TokensModel>();
            }
        }
        internal string getTokenUsuario(int idUsuario)
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?idUsuarioToken=" + idUsuario;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<TokensModel>(resultJSON).token.Trim();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getTokenUsuario: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        internal static UsuarioModel getUsuario(int idUsuario)
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?idUsuario=" + idUsuario;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    UsuarioModel respuesta = JsonConvert.DeserializeObject<UsuarioModel>(resultJSON);

                    if (!string.IsNullOrEmpty(respuesta.nombre))
                    {
                        return respuesta;
                    }

                }
                return new UsuarioModel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getUsuario: " + ex.ToString());
                string error = ex.Message.ToString();
                return new UsuarioModel();
            }
        }
        internal static async Task<UsuarioModel> TraeUsuario(string email, int idPueblo = 0)
        {
            try
            {
                UsuarioModel usu = new UsuarioModel(); ;
                if (App.DAUtil.Usuario != null)
                {
                    //TODO: Mirar al hacer un pedido desde otro pueblo

                    PueblosModel pu = new PueblosModel();
                    if (idPueblo == 0)
                        pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    else
                        pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == idPueblo);
                    DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?emailUsuario=" + email + "&idGrupo=" + pu.idGrupo;
                    string requestUri = DatosConexionModel.uri;

                    HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                    string resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        usu = JsonConvert.DeserializeObject<UsuarioModel>(resultJSON);


                    }
                }
                return usu;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error TraeUsuario: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return new UsuarioModel();
            }
        }
        internal static string registroUsuario(UsuarioModel usu)
        {
            try
            {
                if (usu.idSocial == null)
                    usu.idSocial = "";
                DatosConexionModel.uri = App.DAUtil.miURL + "registroUsuario.php?social=" + Preferences.Get("RedSocial", "") + "&idSocial=" + usu.idSocial + "&idZona=" + usu.idZona + "&pin=" + usu.pin + "&nombre=" + usu.nombre + "&apellidos=" + usu.apellidos + "&dni=" + usu.dni + "&codPostal=" + usu.codPostal + "&poblacion=" + usu.poblacion + "&provincia=" + usu.provincia + "&direccion=" + usu.direccion + "&fechaNacimiento=" + usu.fechaNacimiento.ToString("yyyy-MM-dd") + " & telefono=" + usu.telefono + "&email=" + usu.email + "&password=" + Crypto.Encrypt(usu.password).Replace("+", "").Replace(" ", "") + "&username=" + usu.username + "&foto=" + usu.foto + "&idPueblo=" + usu.idPueblo + "&version=" + usu.version + "&codigo=" + usu.codigo;
                string requestUri = DatosConexionModel.uri;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                /*string x = resultJSON.Trim();
                int value;
                if (int.TryParse(x, out value))
                    App.DAUtil.Usuario.idUsuario = value;*/

                return resultJSON.Trim();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error registroUsuario: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return "ERROR";
            }
        }
        internal static bool insertaValoracion(ValoracionModel val)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "valoraciones.php";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(val), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error insertaValoracion: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal static bool guardaPromoAmigo(GuardaPromoAmigo val, int idPue)
        {
            try
            {
                val.idPueblo = idPue;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "promociones.php?amigos=true";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(val), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error insertaValoracion: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal void BorraToken(string email)
        {
            string result = string.Empty;
            try
            {
                UsuarioModel usu = new UsuarioModel();
                usu.email = email;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "usuarios.php?borrarToken=true";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(usu), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error BorraToken: " + ex.ToString());
                Console.WriteLine(ex.Message);
            }
        }
        internal static async Task<bool> actualizaUsuario(int iD, bool estado, int id1, string password, int id2, int? idRepartidor)
        {
            int est = 1;
            if (!estado)
                est = 0;
            try
            {
                UsuarioModel usu = new UsuarioModel();
                usu.idUsuario = iD;
                usu.password = password;
                usu.estado = est;
                usu.rol = id1;
                if (idRepartidor == null)
                    idRepartidor = 0;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "usuarios.php?idUsuarioRol=" + id2 + "&idRepartidorRol=" + idRepartidor;
                HttpResponseMessage response = await App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(usu), Encoding.UTF8, "application/json"));

                string resultJSON = await response.Content.ReadAsStringAsync();
                return resultJSON.Trim().Equals("");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error actualizaUsuario: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal string actualizaPIN(string nuevoPIN, string email)
        {
            string result = string.Empty;
            try
            {
                UsuarioModel usu = new UsuarioModel();
                usu.email = email;
                usu.pin = nuevoPIN;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "usuarios.php?actualizaPin=true";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(usu), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                return resultJSON.Trim();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error actualizaPIN: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return "ERROR";
            }
        }
        internal string CambioPass(string email, string pass, string pIN)
        {
            string result = string.Empty;
            try
            {
                DatosConexionModel.uri = urlPro;
                UsuarioModel usu = new UsuarioModel();
                usu.email = email;
                usu.pin = pIN;
                usu.password = Crypto.Encrypt(pass).Replace(" ", "").Replace("+", "");
                string requestUri = DatosConexionModel.uri + "usuarios.php?cambiaPass=true";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(usu), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                return resultJSON.Trim();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error CambioPass: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return "ERROR";
            }
        }
        internal static string CambioPassEncrypt(string email, string pass)
        {
            string result = string.Empty;
            try
            {
                DatosConexionModel.uri = urlPro;
                UsuarioModel usu = new UsuarioModel();
                usu.email = email;
                usu.password = Crypto.Encrypt(pass).Replace(" ", "").Replace("+", "");
                string requestUri = DatosConexionModel.uri + "usuarios.php?cambiaPassEncrypt=true";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(usu), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
                return resultJSON.Trim();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error CambioPassEncrypt: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return "ERROR";
            }
        }
        internal async Task<string> actualizaUsuario(UsuarioModel usu)
        {
            string result = string.Empty;
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "usuarios.php";
                HttpResponseMessage response = await App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(usu), Encoding.UTF8, "application/json"));

                string resultJSON = await response.Content.ReadAsStringAsync();
                return resultJSON.Trim();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error actualizaUsuario: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return "ERROR";
            }
        }
        internal async Task<bool> verificaPIN(string email, string pin)
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "usuarios.php/GET?verificaPin=true&email=" + email + "&pin=" + pin;
                string requestUri = DatosConexionModel.uri;

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = await response.Content.ReadAsStringAsync();
                ResultdadoModel r = JsonConvert.DeserializeObject<ResultdadoModel>(resultJSON);
                return r.resultado >= 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error verificaPIN: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal void verificado(string email)
        {
            try
            {
                UsuarioModel usu = new UsuarioModel();
                usu.email = email;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "usuarios.php?verificado=true";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(usu), Encoding.UTF8, "application/json")).Result;

                string resultJSON = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error verificado: " + ex.ToString());
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
        #region Puntos
        internal static int getPuntosEstablecimiento()
        {
            string result = string.Empty;
            try
            {
                string requestUri = $"{App.DAUtil.miURL}puntos.php/GET?puntosEstablecimiento=true&idUsuario={App.DAUtil.Usuario.idUsuario}&idEstablecimiento={App.EstActual.idEstablecimiento}";

                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                ResultdadoModel r = JsonConvert.DeserializeObject<ResultdadoModel>(resultJSON);
                return r.resultado;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error contadorUsuarios: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        #endregion
        #region Repartidores

        internal List<RepartidorModel> ListadoRepartidoresMultiAdmin(string ids)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = DatosConexionModel.uri + "repartidores.php/GET?multiAdmin=true&idPueblos=" + ids;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<RepartidorModel> repartidores = new List<RepartidorModel>();
                        repartidores = JsonConvert.DeserializeObject<List<RepartidorModel>>(resultJSON);
                        if (repartidores != null)
                        {
                            App.DAUtil.ActualizaRepartidores(repartidores.OrderBy(p => p.nombre).ToList());
                            return repartidores.OrderBy(p => p.nombre).ToList();
                        }
                    }
                }
                return new List<RepartidorModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error listadoRepartidores: " + ex.ToString());
                Debug.WriteLine("Error Login: " + ex.ToString());
                return new List<RepartidorModel>();
            }
        }

        internal List<RepartidorModel> listadoRepartidores()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = DatosConexionModel.uri + "repartidores.php/GET?idGrupo=" + pu.idGrupo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<RepartidorModel> repartidores = new List<RepartidorModel>();
                        repartidores = JsonConvert.DeserializeObject<List<RepartidorModel>>(resultJSON);
                        if (repartidores != null)
                        {
                            App.DAUtil.ActualizaRepartidores(repartidores.OrderBy(p => p.nombre).ToList());
                            return repartidores.OrderBy(p => p.nombre).ToList();
                        }
                    }
                }
                return new List<RepartidorModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error listadoRepartidores: " + ex.ToString());
                Debug.WriteLine("Error Login: " + ex.ToString());
                return new List<RepartidorModel>();
            }
        }
        internal List<RepartidorModel> listadoRepartidores(int id)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == id);
                    string requestUri = DatosConexionModel.uri + "repartidores.php/GET?idGrupo=" + pu.idGrupo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<RepartidorModel> repartidores = new List<RepartidorModel>();
                        repartidores = JsonConvert.DeserializeObject<List<RepartidorModel>>(resultJSON);
                        if (repartidores != null)
                        {
                            App.DAUtil.ActualizaRepartidores(repartidores.OrderBy(p => p.nombre).ToList());
                            return repartidores.OrderBy(p => p.nombre).ToList();
                        }
                    }
                }
                return new List<RepartidorModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error listadoRepartidores: " + ex.ToString());
                Debug.WriteLine("Error Login: " + ex.ToString());
                return new List<RepartidorModel>();
            }
        }
        internal async Task listadoRepartidoresAsync()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = DatosConexionModel.uri + "repartidores.php/GET?idGrupo=" + pu.idGrupo;
                    HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                    string resultJSON = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<RepartidorModel> repartidores = new List<RepartidorModel>();
                        repartidores = JsonConvert.DeserializeObject<List<RepartidorModel>>(resultJSON);
                        if (repartidores != null)
                            App.DAUtil.ActualizaRepartidores(repartidores.OrderBy(p => p.nombre).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error listadoRepartidores: " + ex.ToString());
                Debug.WriteLine("Error Login: " + ex.ToString());
            }
        }
        public static RepartidorModel GetRepartidorByIdUsuario(int idUsuario)
        {

            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "repartidores.php/GET?idUsuario=" + idUsuario;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        RepartidorModel respuesta = JsonConvert.DeserializeObject<RepartidorModel>(resultJSON);

                        if (!string.IsNullOrEmpty(respuesta.nombre))
                        {
                            return respuesta;
                        }
                    }
                }
                return new RepartidorModel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error GetRepartidorByIdUsuario: " + ex.ToString());
                return new RepartidorModel();
            }
        }
        internal static List<EstablecimientosRepartidorModel> getEstablecimientosRepartidor(int id)
        {
            List<EstablecimientosRepartidorModel> listCategorias = new List<EstablecimientosRepartidorModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "repartidores.php/GET?idEstablecimientoRepartidor=" + id;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listCategorias = JsonConvert.DeserializeObject<List<EstablecimientosRepartidorModel>>(resultJSON);
                }
                if (listCategorias != null)
                    return listCategorias;
                else
                    return new List<EstablecimientosRepartidorModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getEstablecimientosRepartidor: " + ex.ToString());
                Console.WriteLine(ex.Message);
                return listCategorias;
            }
        }
        internal static void enviaPosicion(double longitude, double latitude)
        {
            //TODO REVISAR
            try
            {
                string requestUri = urlPro + "repartidores.php?latitud=" + latitude.ToString().Replace(",", ".") + "&longitud=" + longitude.ToString().Replace(",", ".") + "&idRepartidor=" + App.DAUtil.Usuario.Repartidor.id;
                HttpResponseMessage response = App.Client.PostAsync(requestUri, null).Result;
                var resultJSON = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        internal string getTokenRepartidor(int idRepartidor)
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "repartidores.php/GET?idTokenRepartidor=" + idRepartidor;
                string requestUri = DatosConexionModel.uri;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<TokensModel>(resultJSON).token.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        internal int nuevoRepartidor(RepartidorModel repartidor, string establecimientos)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "repartidores.php?establecimientos=" + establecimientos;
                    HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(repartidor), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }
        internal int nuevoGasto(GastoModel gasto)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "repartidores.php?gasto=true";
                    HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(gasto), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (!resultJSON.ToLower().Equals("false"))
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }
        internal bool actualizaRepartidor(RepartidorModel repartidor, string establecimientos)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "repartidores.php?establecimientos=" + establecimientos;
                    HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(repartidor), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        public static RepartidorModel CargaRepartidorUsuario(int id)
        {
            List<RepartidorModel> listEstablecimientos = new List<RepartidorModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "repartidores.php/GET?idRepartidorUsuario=" + id;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listEstablecimientos = JsonConvert.DeserializeObject<List<RepartidorModel>>(resultJSON);
                }
                RepartidorModel r = listEstablecimientos.FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new RepartidorModel();
            }
        }
        public static async Task<ObservableCollection<CabeceraPedido>> getListadoPedidosByIdRepartidor(int idRepartidor)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "repartidores.php/GET?idPedidosRepartidor2=" + idRepartidor + "&idGrupo=" + pu.idGrupo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<CabeceraPedido>>(resultJSON);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        public static ObservableCollection<MensajesRepartidorModel> TraeMensajesRepartidor(int idRepartidor)
        {
            ObservableCollection<MensajesRepartidorModel> result = new ObservableCollection<MensajesRepartidorModel>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    int id = App.DAUtil.Usuario.rol == 3 ? App.DAUtil.Usuario.idUsuario : App.EstActual.idEstablecimiento;
                    int admin = App.DAUtil.Usuario.rol == 3 ? 1 : 0;
                    string requestUri = App.DAUtil.miURL + "repartidores.php/GET?idRepartidorMensajes=" + idRepartidor + "&idSender=" + id + "&admin=" + admin;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<MensajesRepartidorModel>>(resultJSON);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        public static ObservableCollection<MensajesRepartidorModel> TraeMensajeRepartidor()
        {
            ObservableCollection<MensajesRepartidorModel> result = new ObservableCollection<MensajesRepartidorModel>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "repartidores.php/GET?idRepartidorMensajesNoLeidos=" + App.DAUtil.Usuario.Repartidor.id;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<MensajesRepartidorModel>>(resultJSON);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        public async Task<bool> EnviarMensajeRepartidor(string mensaje, int idRepartidor)
        {
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    MensajesRepartidorModel men = new MensajesRepartidorModel();
                    men.mensaje = mensaje;
                    men.idRepartidor = idRepartidor;
                    men.idSender = App.DAUtil.Usuario.idUsuario;
                    if (App.DAUtil.Usuario.establecimientos == null)
                    {
                        men.sender = App.DAUtil.Usuario.nombre;
                        men.admin = true;
                    }
                    else
                    {
                        men.admin = false;
                        men.sender = App.EstActual.nombre;
                        men.idSender = App.EstActual.idEstablecimiento;
                    }
                    string requestUri = App.DAUtil.miURL + "repartidores.php/GET?mensaje=true";
                    HttpResponseMessage response = await App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(men), Encoding.UTF8, "application/json"));

                    string resultJSON = await response.Content.ReadAsStringAsync();
                    if (resultJSON.Equals(""))
                    {
                        string token = getTokenRepartidor(idRepartidor);
                        await App.ResponseWS.enviaNotificacion("Nuevo Mensaje", mensaje, token);
                        return true;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool> ContestaMensaje(MensajesRepartidorModel mensaje)
        {
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "repartidores.php/GET?mensaje=true";
                    HttpResponseMessage response = await App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(mensaje), Encoding.UTF8, "application/json"));

                    string resultJSON = await response.Content.ReadAsStringAsync();
                    if (resultJSON.Equals(""))
                    {
                        string contestacion = mensaje.ok == true ? "SI" : "NO";
                        if (mensaje.admin)
                        {
                            List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(App.DAUtil.Usuario.idPueblo);
                            foreach (TokensModel to in tokens)
                                App.ResponseWS.enviaNotificacion("Contestación", App.DAUtil.Usuario.Repartidor.nombre + " ha contestado " + contestacion, to.token);
                        }
                        else
                        {
                            List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(mensaje.idSender);
                            foreach (TokensModel to in tokens3)
                                App.ResponseWS.enviaNotificacion("Contestación", App.DAUtil.Usuario.Repartidor.nombre + " ha contestado " + contestacion, to.token);
                        }
                        return true;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
        #region Zonas
        internal void getZonas()
        {
            try
            {
                if (App.DAUtil?.Usuario != null && App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = App.DAUtil.miURL + "zonas.php/GET?idPueblo=" + App.DAUtil.Usuario.idPueblo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<ZonaModel> zonas = new List<ZonaModel>();
                        zonas = JsonConvert.DeserializeObject<List<ZonaModel>>(resultJSON);
                        if (zonas != null)
                            App.DAUtil.saveZonas(zonas);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getZonas: " + ex.ToString());
            }
        }
        internal async Task getZonasAsync()
        {
            try
            {
                if (App.DAUtil?.Usuario != null && App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = App.DAUtil.miURL + "zonas.php/GET?idPueblo=" + App.DAUtil.Usuario.idPueblo;
                    HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                    string resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<ZonaModel> zonas = new List<ZonaModel>();
                        zonas = JsonConvert.DeserializeObject<List<ZonaModel>>(resultJSON);
                        if (zonas != null)
                            App.DAUtil.saveZonas(zonas);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getZonas: " + ex.ToString());
            }
        }
        internal List<ZonaModel> getZonas(int idPueblo)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = App.DAUtil.miURL + "zonas.php/GET?idPueblo=" + idPueblo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<ZonaModel> zonas = new List<ZonaModel>();
                        zonas = JsonConvert.DeserializeObject<List<ZonaModel>>(resultJSON);
                        if (zonas != null)
                        {
                            App.DAUtil.saveZonas(zonas);
                            return zonas;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return new List<ZonaModel>();
        }
        internal List<ZonaModel> getListadoZonas(int idPueblo)
        {
            List<ZonaModel> listZonas = new List<ZonaModel>();
            try
            {
                string requestUri = App.DAUtil.miURL + "zonas.php/GET?idPueblo=" + idPueblo;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listZonas = JsonConvert.DeserializeObject<List<ZonaModel>>(resultJSON);
                }
                if (listZonas == null)
                    listZonas = new List<ZonaModel>();
                else
                    App.DAUtil.saveZonas(listZonas);
                return listZonas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listZonas;
            }
        }
        internal ZonaModel getZona(int id)
        {
            try
            {
                if (App.DAUtil?.Usuario != null && App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = App.DAUtil.miURL + "zonas.php/GET?idZona=" + id;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<ZonaModel> zonas = new List<ZonaModel>();
                        zonas = JsonConvert.DeserializeObject<List<ZonaModel>>(resultJSON);
                        return zonas.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getZonas: " + ex.ToString());
            }
            return new ZonaModel();
        }
        internal bool actualizaZona(ZonaModel zona)
        {
            try
            {
                if (zona.direccionEnvio == null)
                    zona.direccionEnvio = "";
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "zonas.php";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(zona), Encoding.UTF8, "application/json")).Result;

                var resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    zona = JsonConvert.DeserializeObject<ZonaModel>(resultJSON);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool nuevaZona(ZonaModel zona)
        {

            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    if (zona.direccionEnvio == null)
                        zona.direccionEnvio = "";
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "zonas.php";
                    HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(zona), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        zona = JsonConvert.DeserializeObject<ZonaModel>(resultJSON);
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
        #region Pueblos
        internal List<PueblosModel> getPueblos()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = App.DAUtil.miURL + "pueblos.php/GET";
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<PueblosModel> pueblos = new List<PueblosModel>();
                        pueblos = JsonConvert.DeserializeObject<List<PueblosModel>>(resultJSON);
                        if (pueblos != null)
                        {
                            App.DAUtil.savePueblos(pueblos);
                            return pueblos;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getPueblos: " + ex.ToString());
            }
            return new List<PueblosModel>();
        }
        internal List<GruposPueblosModel> getListadoGruposPueblos()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = App.DAUtil.miURL + "pueblos.php?grupos=true";
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<GruposPueblosModel> pueblos = new List<GruposPueblosModel>();
                        pueblos = JsonConvert.DeserializeObject<List<GruposPueblosModel>>(resultJSON);
                        if (pueblos != null)
                        {
                            return pueblos;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getPueblos: " + ex.ToString());
            }
            return new List<GruposPueblosModel>();
        }
        internal bool actualizaPueblo(PueblosModel pueblo)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "pueblos.php";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(pueblo), Encoding.UTF8, "application/json")).Result;

                var resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    pueblo = JsonConvert.DeserializeObject<PueblosModel>(resultJSON);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool nuevoPueblo(PueblosModel pueblo)
        {

            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "pueblos.php";
                    HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(pueblo), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        pueblo = JsonConvert.DeserializeObject<PueblosModel>(resultJSON);
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
        #region Mensajes
        public List<MensajesModel> getMensajes()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = App.DAUtil.miURL + "mensajes.php/GET";
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<MensajesModel> mensajes = new List<MensajesModel>();
                        mensajes = JsonConvert.DeserializeObject<List<MensajesModel>>(resultJSON);
                        if (mensajes != null)
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
                            App.DAUtil.actualizaMensajes(mensajes);
                            return mensajes;
                        }
                    }
                }
                return new List<MensajesModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getMensajes: " + ex.ToString());
                return new List<MensajesModel>();
            }
        }
        public async Task<List<MensajesModel>> getMensajesAsync()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = App.DAUtil.miURL + "mensajes.php/GET";
                    HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                    string resultJSON = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<MensajesModel> mensajes = new List<MensajesModel>();
                        mensajes = JsonConvert.DeserializeObject<List<MensajesModel>>(resultJSON);
                        App.MensajesGlobal = mensajes;
                        if (mensajes != null)
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
                            App.DAUtil.actualizaMensajes(mensajes);
                            return mensajes;
                        }
                    }
                }
                return new List<MensajesModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getMensajes: " + ex.ToString());
                return new List<MensajesModel>();
            }
        }
        public List<PredefinidosModel> getMensajesPredefinidos()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = App.DAUtil.miURL + "repartidores.php/GET?predefinidos=true";
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<PredefinidosModel> mensajes = new List<PredefinidosModel>();
                        mensajes = JsonConvert.DeserializeObject<List<PredefinidosModel>>(resultJSON);

                        if (mensajes != null)
                        {
                            return mensajes;
                        }
                    }
                }
                return new List<PredefinidosModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getMensajes: " + ex.ToString());
                return new List<PredefinidosModel>();
            }
        }
        public async Task getMensajesPredefinidosAsync()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = App.DAUtil.miURL + "repartidores.php/GET?predefinidos=true";
                    HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                    string resultJSON = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<PredefinidosModel> mensajes = new List<PredefinidosModel>();
                        mensajes = JsonConvert.DeserializeObject<List<PredefinidosModel>>(resultJSON);

                        if (mensajes != null)
                        {
                            App.MensajesPredefinidos = mensajes;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getMensajes: " + ex.ToString());
            }
        }
        #endregion
        #region Configuracion
        internal ConfiguracionAdmin getConfiguracionGlobal()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = "";
                    if (App.DAUtil.Usuario != null)
                    {
                        PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                        requestUri = App.DAUtil.miURL + "configuracion.php/GET?idGrupo=" + pu.idGrupo;
                    }
                    else
                        requestUri = App.DAUtil.miURL + "configuracion.php/GET?idGrupo=1";
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        ConfiguracionAdmin configuracionAdmin = JsonConvert.DeserializeObject<ConfiguracionAdmin>(resultJSON);

                        if (configuracionAdmin != null)
                        {
                            Preferences.Set("ServicioActivo", configuracionAdmin.servicioActivo);
                            Preferences.Set("VersionMinimaAndroid", configuracionAdmin.versionMinimaAndroid);
                            Preferences.Set("VersionMinimaiOS", configuracionAdmin.versionMinimaIOS);

                            if (!configuracionAdmin.servicioActivo)
                            {
                                Preferences.Set("activoHoy", false);
                                Preferences.Set("horario", AppResources.Cerrado);
                            }
                            else
                            {
                                if (DateTime.Now.Hour < 17)
                                {
                                    if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
                                    {
                                        if (configuracionAdmin.activoViernes && configuracionAdmin.inicioViernes != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioViernes.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finViernes.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }

                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                                    {
                                        if (configuracionAdmin.activoLunes && configuracionAdmin.inicioLunes != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioLunes.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finLunes.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                                    {
                                        if (configuracionAdmin.activoSabado && configuracionAdmin.inicioSabado != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioSabado.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finSabado.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        if (configuracionAdmin.activoDomingo && configuracionAdmin.inicioDomingo != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioDomingo.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finDomingo.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday)
                                    {
                                        if (configuracionAdmin.activoJueves && configuracionAdmin.inicioJueves != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioJueves.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finJueves.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                                    {
                                        if (configuracionAdmin.activoMartes && configuracionAdmin.inicioMartes != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioMartes.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finMartes.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday)
                                    {
                                        if (configuracionAdmin.activoMiercoles && configuracionAdmin.inicioMiercoles != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioMiercoles.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finMiercoles.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                }
                                else
                                {
                                    if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
                                    {
                                        if (configuracionAdmin.activoViernesTarde && configuracionAdmin.inicioViernesTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioViernesTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finViernesTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }

                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                                    {
                                        if (configuracionAdmin.activoLunesTarde && configuracionAdmin.inicioLunesTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioLunesTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finLunesTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                                    {
                                        if (configuracionAdmin.activoSabadoTarde && configuracionAdmin.inicioSabadoTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioSabadoTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finSabadoTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        if (configuracionAdmin.activoDomingoTarde && configuracionAdmin.inicioDomingoTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioDomingoTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finDomingoTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday)
                                    {
                                        if (configuracionAdmin.activoJuevesTarde && configuracionAdmin.inicioJuevesTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioJuevesTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finJuevesTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                                    {
                                        if (configuracionAdmin.activoMartesTarde && configuracionAdmin.inicioMartesTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioMartesTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finMartesTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday)
                                    {
                                        if (configuracionAdmin.activoMiercolesTarde && configuracionAdmin.inicioMiercolesTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioMiercolesTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finMiercolesTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                }
                            }
                            return configuracionAdmin;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getConfiguracionGlobal: " + ex.ToString());
            }
            return new ConfiguracionAdmin();
        }
        internal async Task getConfiguracionGlobalAsync()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = "";
                    if (App.DAUtil.Usuario != null)
                    {
                        PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                        requestUri = App.DAUtil.miURL + "configuracion.php/GET?idGrupo=" + pu.idGrupo;
                    }
                    else
                        requestUri = App.DAUtil.miURL + "configuracion.php/GET?idGrupo=1";
                    HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                    string resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        ConfiguracionAdmin configuracionAdmin = JsonConvert.DeserializeObject<ConfiguracionAdmin>(resultJSON);

                        if (configuracionAdmin != null)
                        {
                            Preferences.Set("ServicioActivo", configuracionAdmin.servicioActivo);
                            Preferences.Set("VersionMinimaAndroid", configuracionAdmin.versionMinimaAndroid);
                            Preferences.Set("VersionMinimaiOS", configuracionAdmin.versionMinimaIOS);

                            if (!configuracionAdmin.servicioActivo)
                            {
                                Preferences.Set("activoHoy", false);
                                Preferences.Set("horario", AppResources.Cerrado);
                            }
                            else
                            {
                                if (DateTime.Now.Hour < 17)
                                {
                                    if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
                                    {
                                        if (configuracionAdmin.activoViernes && configuracionAdmin.inicioViernes != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioViernes.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finViernes.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }

                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                                    {
                                        if (configuracionAdmin.activoLunes && configuracionAdmin.inicioLunes != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioLunes.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finLunes.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                                    {
                                        if (configuracionAdmin.activoSabado && configuracionAdmin.inicioSabado != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioSabado.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finSabado.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        if (configuracionAdmin.activoDomingo && configuracionAdmin.inicioDomingo != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioDomingo.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finDomingo.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday)
                                    {
                                        if (configuracionAdmin.activoJueves && configuracionAdmin.inicioJueves != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioJueves.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finJueves.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                                    {
                                        if (configuracionAdmin.activoMartes && configuracionAdmin.inicioMartes != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioMartes.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finMartes.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday)
                                    {
                                        if (configuracionAdmin.activoMiercoles && configuracionAdmin.inicioMiercoles != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioMiercoles.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finMiercoles.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                }
                                else
                                {
                                    if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
                                    {
                                        if (configuracionAdmin.activoViernesTarde && configuracionAdmin.inicioViernesTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioViernesTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finViernesTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }

                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                                    {
                                        if (configuracionAdmin.activoLunesTarde && configuracionAdmin.inicioLunesTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioLunesTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finLunesTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                                    {
                                        if (configuracionAdmin.activoSabadoTarde && configuracionAdmin.inicioSabadoTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioSabadoTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finSabadoTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        if (configuracionAdmin.activoDomingoTarde && configuracionAdmin.inicioDomingoTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioDomingoTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finDomingoTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday)
                                    {
                                        if (configuracionAdmin.activoJuevesTarde && configuracionAdmin.inicioJuevesTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioJuevesTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finJuevesTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                                    {
                                        if (configuracionAdmin.activoMartesTarde && configuracionAdmin.inicioMartesTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioMartesTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finMartesTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                    else if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday)
                                    {
                                        if (configuracionAdmin.activoMiercolesTarde && configuracionAdmin.inicioMiercolesTarde != null)
                                        {
                                            Preferences.Set("activoHoy", true);
                                            Preferences.Set("inicioHoy", configuracionAdmin.inicioMiercolesTarde.ToString(@"hh\:mm"));
                                            Preferences.Set("finHoy", configuracionAdmin.finMiercolesTarde.ToString(@"hh\:mm"));
                                        }
                                        else
                                        {
                                            Preferences.Set("activoHoy", false);
                                            Preferences.Set("horario", AppResources.Cerrado);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getConfiguracionGlobal: " + ex.ToString());
            }
        }
        internal bool saveConfiguracionGeneral()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "configuracion.php?general=true";
                    HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(App.global), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        internal void getConfiguracionGeneral()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = App.DAUtil.miURL + "configuracion.php/GET?general=true";
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        ConfiguracionGlobalModel global = JsonConvert.DeserializeObject<ConfiguracionGlobalModel>(resultJSON);
                        App.DAUtil.SaveConfiguracionGeneral(global);
                        App.global = global;
                        terminalPaycomet = global.terminalPaycomet;
                        apiKeyPaycomet = global.apiPaycomet;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getConfiguracionGeneral: " + ex.ToString());
            }
        }
        internal async Task getConfiguracionGeneralASync()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    string requestUri = App.DAUtil.miURL + "configuracion.php/GET?general=true";
                    HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                    string resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        ConfiguracionGlobalModel global = JsonConvert.DeserializeObject<ConfiguracionGlobalModel>(resultJSON);
                        App.DAUtil.SaveConfiguracionGeneral(global);
                        App.global = global;
                        terminalPaycomet = global.terminalPaycomet;
                        apiKeyPaycomet = global.apiPaycomet;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getConfiguracionGeneral: " + ex.ToString());
            }
        }
        internal static ConfiguracionAdmin getConfiguracionAdmin()
        {
            try
            {
                string requestUri = App.DAUtil.miURL + "configuracion.php/GET?idGrupo=1";
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    ConfiguracionAdmin configuracionAdmin = JsonConvert.DeserializeObject<ConfiguracionAdmin>(resultJSON);

                    if (configuracionAdmin != null)
                        Preferences.Set("ServicioActivo", configuracionAdmin.servicioActivo);
                    if (!configuracionAdmin.servicioActivo)
                    {
                        Preferences.Set("activoHoy", false);
                        Preferences.Set("horario", AppResources.Cerrado);
                    }
                    else
                    {
                        if (DateTime.Now.Hour < 17)
                        {
                            if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
                            {
                                if (configuracionAdmin.activoViernes && configuracionAdmin.inicioViernes != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioViernes.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finViernes.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }

                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                            {
                                if (configuracionAdmin.activoLunes && configuracionAdmin.inicioLunes != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioLunes.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finLunes.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                            {
                                if (configuracionAdmin.activoSabado && configuracionAdmin.inicioSabado != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioSabado.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finSabado.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                            {
                                if (configuracionAdmin.activoDomingo && configuracionAdmin.inicioDomingo != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioDomingo.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finDomingo.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday)
                            {
                                if (configuracionAdmin.activoJueves && configuracionAdmin.inicioJueves != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioJueves.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finJueves.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                            {
                                if (configuracionAdmin.activoMartes && configuracionAdmin.inicioMartes != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioMartes.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finMartes.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday)
                            {
                                if (configuracionAdmin.activoMiercoles && configuracionAdmin.inicioMiercoles != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioMiercoles.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finMiercoles.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                        }
                        else
                        {
                            if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
                            {
                                if (configuracionAdmin.activoViernesTarde && configuracionAdmin.inicioViernesTarde != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioViernesTarde.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finViernesTarde.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }

                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                            {
                                if (configuracionAdmin.activoLunesTarde && configuracionAdmin.inicioLunesTarde != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioLunesTarde.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finLunesTarde.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                            {
                                if (configuracionAdmin.activoSabadoTarde && configuracionAdmin.inicioSabadoTarde != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioSabadoTarde.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finSabadoTarde.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                            {
                                if (configuracionAdmin.activoDomingoTarde && configuracionAdmin.inicioDomingoTarde != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioDomingoTarde.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finDomingoTarde.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday)
                            {
                                if (configuracionAdmin.activoJuevesTarde && configuracionAdmin.inicioJuevesTarde != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioJuevesTarde.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finJuevesTarde.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                            {
                                if (configuracionAdmin.activoMartesTarde && configuracionAdmin.inicioMartesTarde != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioMartesTarde.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finMartesTarde.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                            else if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday)
                            {
                                if (configuracionAdmin.activoMiercolesTarde && configuracionAdmin.inicioMiercolesTarde != null)
                                {
                                    Preferences.Set("activoHoy", true);
                                    Preferences.Set("inicioHoy", configuracionAdmin.inicioMiercolesTarde.ToString(@"hh\:mm"));
                                    Preferences.Set("finHoy", configuracionAdmin.finMiercolesTarde.ToString(@"hh\:mm"));
                                }
                                else
                                {
                                    Preferences.Set("activoHoy", false);
                                    Preferences.Set("horario", AppResources.Cerrado);
                                }
                            }
                        }
                    }
                    return configuracionAdmin;
                    //App.DAUtil.actualizaReservas(configuracionEstablecimiento);
                }
                return new ConfiguracionAdmin();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ConfiguracionAdmin();
            }
        }
        internal static ConfiguracionAdmin getConfiguracionAdmin(int idPueblo)
        {
            try
            {
                string requestUri = App.DAUtil.miURL + "configuracion.php/GET?idPueblo=" + idPueblo;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    ConfiguracionAdmin configuracionAdmin = JsonConvert.DeserializeObject<ConfiguracionAdmin>(resultJSON);

                    return configuracionAdmin;
                    //App.DAUtil.actualizaReservas(configuracionEstablecimiento);
                }
                return new ConfiguracionAdmin();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ConfiguracionAdmin();
            }
        }
        internal static ConfiguracionEstablecimiento getConfiguracionEstablecimiento(int id)
        {
            ConfiguracionEstablecimiento configuracionEstablecimiento = new ConfiguracionEstablecimiento();
            try
            {
                string requestUri = App.DAUtil.miURL + "configuracion.php/GET?idEstablecimiento=" + id;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    configuracionEstablecimiento = JsonConvert.DeserializeObject<ConfiguracionEstablecimiento>(resultJSON);
                }
                return configuracionEstablecimiento;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return configuracionEstablecimiento;
            }
        }
        internal bool updateConfiguracionEstablecimiento(ConfiguracionEstablecimiento configuracionEstablecimiento)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "configuracion.php?idEstablecimiento=" + configuracionEstablecimiento.idEstablecimiento;
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(configuracionEstablecimiento), Encoding.UTF8, "application/json")).Result;

                var resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal bool updateConfiguracionAdmin(ConfiguracionAdmin configuracionAdmin)
        {
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "configuracion.php";
                HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(configuracionAdmin), Encoding.UTF8, "application/json")).Result;

                var resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
        #region Pedidos
        internal async Task<bool> eliminaLineaPedido(int idPedido, int idProducto)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "pedidos.php?idPedido=" + idPedido + "&idProducto=" + idProducto;
                    HttpResponseMessage response = await App.Client.DeleteAsync(requestUri);

                    var resultJSON = await response.Content.ReadAsStringAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return false;
        }
        internal bool ValoraPedido(ValoracionPedido v)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "pedidos.php?valoraPedido=true";
                    HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(v), Encoding.UTF8, "application/json")).Result;

                    var resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        public static async Task<ObservableCollection<CabeceraPedido>> getPedidoPendiente()
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = "";
                    if (App.DAUtil.Usuario.establecimientos.Count == 1)
                        requestUri = App.DAUtil.miURL + "pedidos.php/GET?idEstablecimiento=" + App.DAUtil.Usuario.establecimientos[0].idEstablecimiento;
                    else
                    {
                        string es = "";
                        foreach (Establecimiento e2 in App.DAUtil.Usuario.establecimientos)
                        {
                            if (!es.Equals(""))
                                es += ",";

                            es += e2.idEstablecimiento;
                        }
                        requestUri = App.DAUtil.miURL + "pedidos.php/GET?idEstablecimientoMulti=" + es;
                    }

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<CabeceraPedido> cab = JsonConvert.DeserializeObject<List<CabeceraPedido>>(resultJSON);

                        result = new ObservableCollection<CabeceraPedido>(cab);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // 
                return result;
            }
        }
        internal static async Task<ObservableCollection<CabeceraPedido>> getPedidoPendienteEstadoRecogido(int idUsuario)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?idUsuario=" + idUsuario;

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        listPedido = JsonConvert.DeserializeObject<ObservableCollection<Pedido>>(resultJSON);
                        if (listPedido != null)
                        {
                            if (listPedido.Count > 0)
                                result = convertirToPedidoInterno(listPedido);
                        }

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // 
                return result;
            }
        }
        public static async Task<ObservableCollection<CabeceraPedido>> getPedidoPendienteAdmin()
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?idGrupo=" + pu.idGrupo + "&idPueblo=" + pu.id;


                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = await response?.Content?.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<CabeceraPedido>>(resultJSON);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }

        public static async Task<ObservableCollection<CabeceraPedido>> getPedidoPendienteMultiAdmin(string ids)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?multiAdmin=true&idPueblos=" + ids;

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = await response?.Content?.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<CabeceraPedido>>(resultJSON);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }

        public static async Task<ObservableCollection<CabeceraPedido>> getPedidoPendienteSuperAdmin(string ids)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?superAdmin=" + ids;

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = await response?.Content?.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<CabeceraPedido>>(resultJSON);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        internal static CabeceraPedido TraePedidoPorCodigo(string idPedido)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?codigoPedido=" + idPedido;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        listPedido = JsonConvert.DeserializeObject<ObservableCollection<Pedido>>(resultJSON);
                        if (listPedido != null)
                        {
                            if (listPedido.Count > 0)
                                result = convertirToPedidoInterno(listPedido);
                        }

                    }
                }
                return result[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new CabeceraPedido();
            }
        }
        public static ObservableCollection<CabeceraPedido> getPedidoUsuariosByIdPedido(int idPedido)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?idPedido=" + idPedido;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        listPedido = JsonConvert.DeserializeObject<ObservableCollection<Pedido>>(resultJSON);
                        if (listPedido != null)
                        {
                            if (listPedido.Count > 0)
                                result = convertirToPedidoInterno(listPedido);
                        }

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        internal static ObservableCollection<CabeceraPedido> getHistoricoPedidosAdmin(int numero)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?historicoAdmin=true&idGrupo=" + pu.idGrupo + "&idPueblo=" + pu.id + "&numero=" + numero;

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<CabeceraPedido>>(resultJSON);

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        internal static ObservableCollection<CabeceraPedido> getHistoricoPedidosMultiAdmin(int numero, string ids)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?historicoMultiAdmin=true&ids=67&numero=" + numero;

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<CabeceraPedido>>(resultJSON);

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        internal static ObservableCollection<CabeceraPedido> getHistoricoPedidosAnuladosMultiAdmin(int numero, string ids)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?historicoAnuladosMultiAdmin=true&ids=67&numero=" + numero;

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<CabeceraPedido>>(resultJSON);

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        internal static List<GastoModel> getHistoricoGastosMultiAdmin(int numero, string ids)
        {
            List<GastoModel> result = new List<GastoModel>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?historicoGastosAdmin=true&ids=67&numero=" + numero;

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<List<GastoModel>>(resultJSON);

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        internal static ObservableCollection<CabeceraPedido> getHistoricoPedidosAdmin(int numero, int id)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == id);
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?historicoAdmin=true&idGrupo=" + pu.idGrupo + "&idPueblo=" + pu.id + "&numero=" + numero;

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<CabeceraPedido>>(resultJSON);

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        internal static ObservableCollection<CabeceraPedido> getHistoricoPedidosAdminPuntos()
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?historicoAdminPuntos=true&idGrupo=" + pu.idGrupo + "&idPueblo=" + pu.id;

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<CabeceraPedido>>(resultJSON);

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        internal static ObservableCollection<CabeceraPedido> getHistoricoPedidosMultiAdminPuntos(string ids)
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?historicoMultiAdminPuntos=true&ids=" + ids;

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        result = JsonConvert.DeserializeObject<ObservableCollection<CabeceraPedido>>(resultJSON);

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        public static ObservableCollection<CabeceraPedido> getPedidoUsuarios()
        {
            ObservableCollection<Pedido> listPedido = new ObservableCollection<Pedido>();
            ObservableCollection<CabeceraPedido> result = new ObservableCollection<CabeceraPedido>();
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    string requestUri = App.DAUtil.miURL + "pedidos.php/GET?idUsuarioHistorico=" + App.DAUtil.Usuario.idUsuario;

                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        listPedido = JsonConvert.DeserializeObject<ObservableCollection<Pedido>>(resultJSON);
                        if (listPedido != null)
                        {
                            if (listPedido.Count > 0)
                                result = convertirToPedidoInterno(listPedido);
                        }

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
        internal async Task<bool> cambiaEstadoPedido(int idPedido, int estado)
        {
            string result = string.Empty;
            try
            {

                Pedido p = new Pedido();
                p.id = idPedido;
                p.idEstadoPedido = estado;
                p.idUsuario = App.DAUtil.Usuario.idUsuario;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "pedidos.php?cambiaEstadoNuevo=true";
                HttpResponseMessage response = await App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json"));

                var resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    result = JsonConvert.DeserializeObject<string>(resultJSON);
                    return result.Trim().Equals("1");
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal async Task<string> cambiaTipoVenta(int idPedido, string tipo)
        {
            string result = string.Empty;
            try
            {

                Pedido p = new Pedido();
                p.id = idPedido;
                p.tipoVenta = tipo;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "pedidos.php?cambiaTipoVenta=true";
                HttpResponseMessage response = await App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json"));

                var resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    result = JsonConvert.DeserializeObject<string>(resultJSON);
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "ERROR";
            }
        }
        internal static async Task<string> PedidoNoValorado(int idPedido)
        {
            string result = string.Empty;
            try
            {

                Pedido p = new Pedido();
                p.id = idPedido;
                p.valorado = 2;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "pedidos.php?noValorado=true";
                HttpResponseMessage response = await App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json"));

                var resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    result = JsonConvert.DeserializeObject<string>(resultJSON);
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "ERROR";
            }
        }
        internal async Task<string> cambiaEstadoPedido(string idPedido, int estado)
        {
            string result = string.Empty;
            try
            {

                Pedido p = new Pedido();
                p.codigoPedido = idPedido;
                p.idEstadoPedido = estado;
                p.idUsuario = App.DAUtil.Usuario.idUsuario;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "pedidos.php?cambiaEstado2=true";
                HttpResponseMessage response = await App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json"));

                var resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    result = JsonConvert.DeserializeObject<string>(resultJSON);
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "ERROR";
            }
        }
        internal async Task<string> cambiaEstadoMensaje(string idPedido)
        {
            string result = string.Empty;
            try
            {

                Pedido p = new Pedido();
                p.codigoPedido = idPedido;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "pedidos.php?cambiaEstadoMensaje=idPedido";
                HttpResponseMessage response = await App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json"));

                var resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    result = JsonConvert.DeserializeObject<string>(resultJSON);
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "ERROR";
            }
        }
        internal async Task<bool> pedidoConRepartidor(string idPedido, int idRepartidor)
        {
            try
            {
                Pedido p = new Pedido();
                p.codigoPedido = idPedido;
                p.id = idRepartidor;
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "pedidos.php?pedidoRepartidor=true";
                HttpResponseMessage response = await App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json"));

                var resultJSON = await response.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }
        internal static async Task<bool> eliminaPedido(int idPedido)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "pedidos.php?idPedidoCompleto=" + idPedido;
                    HttpResponseMessage response = await App.Client.DeleteAsync(requestUri);

                    var resultJSON = await response.Content.ReadAsStringAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return false;
        }
        internal static int NuevoPedido(string mesa, int idZonaMesa, string zonaMesa, string tipoVenta, int id, int establecimiento, string codigo, int idZona, string direccion, string observaciones, string horaEntrega, List<LineasPedido> lineas, string idTransaccion, int tipo, string tipoPago, int puntos)
        {
            int idPedido = -1;
            try
            {

                CabeceraPedido p = new CabeceraPedido();
                p.idUsuario = id;
                p.idEstablecimiento = establecimiento;
                p.horaEntrega = horaEntrega;
                p.codigoPedido = codigo;
                p.idZona = idZona;
                p.direccionUsuario = direccion;
                p.tipo = tipo;
                p.idZonaEstablecimiento = idZonaMesa;
                p.mesa = mesa;
                p.zonaEstablecimiento = zonaMesa;
                p.tipoVenta = tipoVenta;
                p.comentario = observaciones;
                p.transaccion = idTransaccion;
                p.tipoPago = tipoPago;
                double total = 0;
                if (App.DAUtil.Usuario.rol == 1)
                    p.nombreUsuario = App.DAUtil.Usuario.nombre + " " + App.DAUtil.Usuario.apellidos.Substring(0, 1) + ".";
                else
                    p.nombreUsuario = App.DAUtil.Usuario.nombre + " (C)";
                if (string.IsNullOrEmpty(idTransaccion))
                {
                    p.pagado = 0;
                }
                else
                    p.pagado = 1;
                foreach (LineasPedido ll in lineas)
                {
                    ll.tipoVenta = tipoVenta;
                    if (ll.comentario == null)
                        ll.comentario = "";
                    if (ll.pagadoConPuntos == 0 && ll.tipoComida == 0)
                        total += ll.precio * ll.cantidad;
                }

                p.lineasPedidos = new ObservableCollection<LineasPedido>(lineas);




                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "pedidos.php";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json")).Result;

                var resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    App.Descuento = 0;
                    CabeceraPedido p2 = JsonConvert.DeserializeObject<CabeceraPedido>(resultJSON);
                    idPedido = p2.idPedido;
                    p.idPedido = idPedido;
                    if (puntos > 0)
                        QuitaPuntos(puntos);
                    if (App.EstActual.configuracion.sistemaPuntos)
                        AñadirPuntos(total);
                    /* ResponseServiceWS.imprimirTicket(idPedido.ToString());
                     int idTicket = ResponseServiceWS.traeIdTicket(idPedido.ToString());
                     if (idTicket == 0)
                         ResponseServiceWS.nuevoTicketTerminal(idPedido.ToString());
                     else
                         ResponseServiceWS.actualizaTicketTerminal(idPedido.ToString(), idTicket);*/

                    if ((App.DAUtil.Usuario.idPueblo == 7 || App.DAUtil.Usuario.idPueblo == 8) && App.DAUtil.Usuario.rol == 1 && p.lineasPedidos.Sum(p20 => p20.cantidad * p20.precio) > 10)
                    {
                        if ((App.DAUtil.Usuario.idPueblo == 7 && DateTime.Now >= new DateTime(2022, 04, 25, 21, 0, 0)) || (App.DAUtil.Usuario.idPueblo == 8 && DateTime.Now >= new DateTime(2022, 04, 21, 21, 0, 0)))
                            Preferences.Set("numerosSorteo", generaNumerosSorteo());
                    }

                }
                return idPedido;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return idPedido;
            }
        }



        private static void QuitaPuntos(int puntos)
        {
            DatosConexionModel.uri = urlPro;
            string requestUri = DatosConexionModel.uri + "puntos.php";
            PuntosUsuarioModel p = new PuntosUsuarioModel();
            p.puntos = puntos;
            p.idUsuario = App.DAUtil.Usuario.idUsuario;
            p.idEstablecimiento = App.EstActual.idEstablecimiento;
            HttpResponseMessage response = App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json")).Result;

            var resultJSON = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
            {

            }
        }
        private static void AñadirPuntos(double total)
        {
            DatosConexionModel.uri = urlPro;
            string requestUri = DatosConexionModel.uri + "puntos.php";
            PuntosUsuarioModel p = new PuntosUsuarioModel();
            if (App.EstActual.configuracion.puntosPorPedido > 0)
                p.puntos = App.EstActual.configuracion.puntosPorPedido;
            else if (App.EstActual.configuracion.puntosPorEuro > 0)
                p.puntos = App.EstActual.configuracion.puntosPorEuro * ((int)total);

            p.idUsuario = App.DAUtil.Usuario.idUsuario;
            p.idEstablecimiento = App.EstActual.idEstablecimiento;
            HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json")).Result;

            var resultJSON = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
            {
            }
        }
        internal static int NuevoPedido(int puntos)
        {
            int idPedido = -1;
            try
            {
                double total = 0;
                foreach (LineasPedido ll in App.pedidoEnCurso.lineasPedidos)
                {
                    if (ll.comentario == null)
                        ll.comentario = "";
                    if (ll.pagadoConPuntos == 0 && ll.tipoComida == 0)
                        total += ll.precio * ll.cantidad;
                }
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "pedidos.php";
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(App.pedidoEnCurso), Encoding.UTF8, "application/json")).Result;

                var resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    CabeceraPedido p2 = JsonConvert.DeserializeObject<CabeceraPedido>(resultJSON);
                    idPedido = p2.idPedido;

                    if (puntos > 0)
                        QuitaPuntos(puntos);
                    if (App.EstActual.configuracion.sistemaPuntos)
                        AñadirPuntos(total);
                }
                return idPedido;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return idPedido;
            }
        }
        internal static int NuevoAutoPedido(AutoPedidoModel auto, int estado)
        {
            int idPedido = -1;
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "pedidos.php?autoPedido2=true&estado=" + estado;
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(auto), Encoding.UTF8, "application/json")).Result;

                var resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    idPedido = int.Parse(resultJSON.Trim());
                }
                return idPedido;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return idPedido;
            }
        }
        internal static int NuevoAutoPedidoRep(AutoPedidoModel auto)
        {
            int idPedido = -1;
            try
            {
                DatosConexionModel.uri = urlPro;
                string requestUri = DatosConexionModel.uri + "pedidos.php?autoPedido3=true&idRepartidor=" + App.DAUtil.Usuario.Repartidor.id;
                HttpResponseMessage response = App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(auto), Encoding.UTF8, "application/json")).Result;

                var resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    idPedido = int.Parse(resultJSON.Trim());
                }
                return idPedido;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return idPedido;
            }
        }
        #endregion
        #region Tarjetas
        internal static List<TarjetaModel> getTarjetas(int idUsuario)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = App.DAUtil.miURL + "tarjetas.php/GET?idUsuario=" + idUsuario;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<TarjetaModel> pueblos = new List<TarjetaModel>();
                        pueblos = JsonConvert.DeserializeObject<List<TarjetaModel>>(resultJSON);

                        if (pueblos != null)
                            App.DAUtil.saveTarjetas(pueblos);
                        return pueblos;
                    }
                }
            }
            catch (Exception)
            {
            }
            return new List<TarjetaModel>();
        }
        internal async Task<bool> nuevaTarjeta(TarjetaModel pueblo)
        {

            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    //pueblo.numero = Crypto.Encrypt(pueblo.numero).Replace(" ", "").Replace("+", "");
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "tarjetas.php";
                    HttpResponseMessage response = await App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(pueblo), Encoding.UTF8, "application/json"));

                    var resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        pueblo = JsonConvert.DeserializeObject<TarjetaModel>(resultJSON);
                        //pueblo.numero = Crypto.Decrypt(pueblo.numero);
                        App.DAUtil.NuevaTarjeta(pueblo);
                        App.DAUtil.Usuario.tarjetas.Add(pueblo);
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        internal async Task<TarjetaModel> infoTarjetaPaycomet(int idUser, string tokenUser)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    InfoCardModel tarjeta = new InfoCardModel();
                    tarjeta.idUser = idUser;
                    tarjeta.terminal = terminalPaycomet;
                    tarjeta.tokenUser = tokenUser;

                    DatosConexionModel.uri = urlPaycomet;
                    string requestUri = DatosConexionModel.uri + "cards/info";

                    HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
                    client.DefaultRequestHeaders.Add("PAYCOMET-API-TOKEN", apiKeyPaycomet);
                    HttpResponseMessage response = new HttpResponseMessage();

                    response = await client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(tarjeta), Encoding.UTF8, "application/json"));


                    string resultJSON = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        ResponseInfoCardModel respuesta = JsonConvert.DeserializeObject<ResponseInfoCardModel>(resultJSON);
                        TarjetaModel t = new TarjetaModel();
                        t.cardBrand = respuesta.cardBrand;
                        t.cardType = respuesta.cardType;
                        t.errorCode = respuesta.errorCode;
                        t.expiryDate = respuesta.expiryDate;
                        t.idUser = idUser;
                        t.idUsuario = App.DAUtil.Usuario.idUsuario;
                        t.pan = respuesta.pan;
                        t.tokenUser = tokenUser;
                        return t;

                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        internal async Task<OperationInfoModel> infoTransactionPaycomet(string order)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    PurchaseModel tarjeta = new PurchaseModel();
                    tarjeta.payment = new Payment();
                    tarjeta.payment.terminal = terminalPaycomet;

                    DatosConexionModel.uri = urlPaycomet;
                    string requestUri = DatosConexionModel.uri + "payments/" + order + "/info";

                    HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
                    client.DefaultRequestHeaders.Add("PAYCOMET-API-TOKEN", apiKeyPaycomet);
                    HttpResponseMessage response = new HttpResponseMessage();

                    response = await client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(tarjeta), Encoding.UTF8, "application/json"));


                    string resultJSON = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        OperationInfoModel respuesta = JsonConvert.DeserializeObject<OperationInfoModel>(resultJSON);
                        return respuesta;

                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        internal async Task<bool> refundPaycomet(string order, int idUser, string tokenUser, string amount, string autnCode)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    //Required: amount,authCode,currency,originalIp,terminal
                    PurchaseModel tarjeta = new PurchaseModel();
                    tarjeta.payment = new Payment();
                    tarjeta.payment.terminal = terminalPaycomet;
                    tarjeta.payment.amount = amount;
                    tarjeta.payment.authCode = autnCode;
                    tarjeta.payment.currency = "EUR";
                    tarjeta.payment.originalIp = "127.0.0.1";

                    DatosConexionModel.uri = urlPaycomet;
                    string requestUri = DatosConexionModel.uri + "payments/" + order + "/refund";

                    HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
                    client.DefaultRequestHeaders.Add("PAYCOMET-API-TOKEN", apiKeyPaycomet);
                    HttpResponseMessage response = new HttpResponseMessage();

                    response = await client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(tarjeta), Encoding.UTF8, "application/json"));


                    string resultJSON = await response.Content.ReadAsStringAsync();
                    return true;
                    /*if (response.IsSuccessStatusCode)
                    {
                        OperationInfoModel respuesta = JsonConvert.DeserializeObject<OperationInfoModel>(resultJSON);
                        return respuesta;

                    }
                    else
                    {
                        return null;
                    }*/
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        internal async Task<bool> eliminaTarjetaPayComet(int idUser, string tokenUser)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    InfoCardModel tarjeta = new InfoCardModel();
                    tarjeta.idUser = idUser;
                    tarjeta.terminal = terminalPaycomet;
                    tarjeta.tokenUser = tokenUser;

                    DatosConexionModel.uri = urlPaycomet;
                    string requestUri = DatosConexionModel.uri + "cards/delete";

                    HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
                    client.DefaultRequestHeaders.Add("PAYCOMET-API-TOKEN", apiKeyPaycomet);
                    HttpResponseMessage response = new HttpResponseMessage();

                    response = await client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(tarjeta), Encoding.UTF8, "application/json"));


                    string resultJSON = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        internal async Task<string> realizaPagoTarjeta(int idUser, string tokenUser, string amount, string order)
        {
            try
            {
                //return "http://url-ok";
                if (App.DAUtil.DoIHaveInternet())
                {
                    PurchaseModel compra = new PurchaseModel();
                    compra.payment = new Payment();
                    compra.payment.amount = amount;
                    compra.payment.currency = "EUR";
                    compra.payment.idUser = idUser;
                    compra.payment.methodId = 1;
                    compra.payment.order = order;
                    compra.payment.originalIp = "127.0.0.1";
                    compra.payment.secure = 1;
                    compra.payment.terminal = terminalPaycomet;
                    compra.payment.tokenUser = tokenUser;

                    DatosConexionModel.uri = urlPaycomet;
                    string requestUri = DatosConexionModel.uri + "payments";

                    HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
                    client.DefaultRequestHeaders.Add("PAYCOMET-API-TOKEN", apiKeyPaycomet);
                    HttpResponseMessage response = new HttpResponseMessage();

                    response = await client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(compra), Encoding.UTF8, "application/json"));


                    string resultJSON = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        ResponsePurchaseModel respuesta = JsonConvert.DeserializeObject<ResponsePurchaseModel>(resultJSON);
                        if (respuesta.errorCode == 0)
                        {
                            if (respuesta.challengeUrl != null)
                                return respuesta.challengeUrl;
                            else
                                return "";
                        }
                        else
                            return App.erroresPaycomet.Where(p => p.errorCode == respuesta.errorCode).FirstOrDefault().textError;
                    }
                    else
                    {
                        return AppResources.HacerPagoKO;
                    }
                }
                return AppResources.HacerPagoKO;
            }
            catch (Exception ex)
            {
                return AppResources.HacerPagoKO + ": " + ex.Message;
            }
        }
        internal async Task<string> realizaPagoBizum(int idUser, string tokenUser, string amount, string order)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    PurchaseModel compra = new PurchaseModel();
                    compra.payment = new Payment();
                    compra.payment.amount = amount;
                    compra.payment.currency = "EUR";
                    compra.payment.idUser = idUser;
                    compra.payment.methodId = 11;
                    compra.payment.order = order;
                    compra.payment.originalIp = "127.0.0.1";
                    compra.payment.secure = 1;
                    compra.payment.terminal = terminalPaycomet;
                    compra.payment.tokenUser = tokenUser;

                    DatosConexionModel.uri = urlPaycomet;
                    string requestUri = DatosConexionModel.uri + "payments";

                    HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
                    client.DefaultRequestHeaders.Add("PAYCOMET-API-TOKEN", apiKeyPaycomet);
                    HttpResponseMessage response = new HttpResponseMessage();

                    response = await client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(compra), Encoding.UTF8, "application/json"));


                    string resultJSON = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        ResponsePurchaseModel respuesta = JsonConvert.DeserializeObject<ResponsePurchaseModel>(resultJSON);
                        if (respuesta.errorCode == 0)
                            return respuesta.challengeUrl;
                        else
                            return App.erroresPaycomet.Where(p => p.errorCode == respuesta.errorCode).FirstOrDefault().textError;
                    }
                    else
                    {
                        return AppResources.HacerPagoKO;
                    }
                }
                return AppResources.HacerPagoKO;
            }
            catch (Exception ex)
            {
                return AppResources.HacerPagoKO + ": " + ex.Message;
            }
        }
        internal async Task<bool> eliminaTarjeta(int idTarjeta)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "tarjetas.php?id=" + idTarjeta;
                    HttpResponseMessage response = await App.Client.DeleteAsync(requestUri);

                    var resultJSON = await response.Content.ReadAsStringAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return false;
        }
        #endregion
        #region Informes
        internal static List<CuentasEstablecimientoModel> getCuentasEstablecimiento(DateTime desde, DateTime hasta, int idEstablecimiento)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = App.DAUtil.miURL + "informes.php/GET?cuentasEstablecimiento=true&desde=" + desde.ToString("yyyy-MM-dd") + "&hasta=" + hasta.ToString("yyyy-MM-dd") + "&idEstablecimiento=" + idEstablecimiento;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<CuentasEstablecimientoModel> tickets = new List<CuentasEstablecimientoModel>();
                        tickets = JsonConvert.DeserializeObject<List<CuentasEstablecimientoModel>>(resultJSON);
                        if (tickets != null)
                            return tickets;
                    }
                }
            }
            catch (Exception)
            {
            }
            return new List<CuentasEstablecimientoModel>();
        }
        internal static List<CuentasAdministradorModel> getCuentasAdministrador(DateTime desde, DateTime hasta)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = App.DAUtil.miURL + "facturas.php/GET?cuentasAdministradores=true&desde=" + desde.ToString("yyyy-MM-dd") + "&hasta=" + hasta.ToString("yyyy-MM-dd");
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<CuentasAdministradorModel> tickets = new List<CuentasAdministradorModel>();
                        tickets = JsonConvert.DeserializeObject<List<CuentasAdministradorModel>>(resultJSON);
                        if (tickets != null)
                            return tickets;
                    }
                }
            }
            catch (Exception)
            {
            }
            return new List<CuentasAdministradorModel>();
        }
        internal static List<TicketMedioModel> getInformeTicketMedio(DateTime desde, DateTime hasta)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "informes.php/GET?ticketMedio=true&desde=" + desde.ToString("yyyy-MM-dd") + " 00:00:00 &hasta=" + hasta.ToString("yyyy-MM-dd") + " 23:59:59&idGrupo=" + pu.idGrupo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<TicketMedioModel> tickets = new List<TicketMedioModel>();
                        tickets = JsonConvert.DeserializeObject<List<TicketMedioModel>>(resultJSON);
                        if (tickets != null)
                            return tickets;
                    }
                }
            }
            catch (Exception)
            {
            }
            return new List<TicketMedioModel>();
        }
        internal static List<BeneficiosModel> getInformeBeneficiosDiario()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "informes.php/GET?beneficioDiario=true&idGrupo=" + pu.idGrupo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<BeneficiosModel> beneficios = new List<BeneficiosModel>();
                        beneficios = JsonConvert.DeserializeObject<List<BeneficiosModel>>(resultJSON);
                        if (beneficios != null)
                            return beneficios;
                    }
                }
            }
            catch (Exception)
            {
            }
            return new List<BeneficiosModel>();
        }
        internal static List<BeneficiosModel> getInformeBeneficiosMensual()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "informes.php/GET?beneficioMensual=true&idGrupo=" + pu.idGrupo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<BeneficiosModel> beneficios = new List<BeneficiosModel>();
                        beneficios = JsonConvert.DeserializeObject<List<BeneficiosModel>>(resultJSON);
                        if (beneficios != null)
                            return beneficios;
                    }
                }
            }
            catch (Exception)
            {
            }
            return new List<BeneficiosModel>();
        }
        internal static List<BeneficiosModel> getInformeBeneficiosSemanal()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "informes.php/GET?beneficioSemanal=true&idGrupo=" + pu.idGrupo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<BeneficiosModel> beneficios = new List<BeneficiosModel>();
                        beneficios = JsonConvert.DeserializeObject<List<BeneficiosModel>>(resultJSON);
                        if (beneficios != null)
                            return beneficios;
                    }
                }
            }
            catch (Exception)
            {
            }
            return new List<BeneficiosModel>();
        }
        internal static List<MejoresClientesModel> getMejoresClientes(int idEstablecimiento, int idZona)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "informes.php/GET?mejoresClientes=true&idZona=" + idZona + "&idEstablecimiento=" + idEstablecimiento + "&idGrupo=" + pu.idGrupo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<MejoresClientesModel> clientes = new List<MejoresClientesModel>();
                        clientes = JsonConvert.DeserializeObject<List<MejoresClientesModel>>(resultJSON);
                        if (clientes != null)
                            return clientes;
                    }
                }
            }
            catch (Exception)
            {
            }
            return new List<MejoresClientesModel>();
        }
        internal static List<MasVendidosModel> getProductosMasVendidos(int idEstablecimiento, int idZona)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "informes.php/GET?masVendidos=true&idZona=" + idZona + "&idEstablecimiento=" + idEstablecimiento + "&idGrupo=" + pu.idGrupo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<MasVendidosModel> productos = new List<MasVendidosModel>();
                        productos = JsonConvert.DeserializeObject<List<MasVendidosModel>>(resultJSON);
                        if (productos != null)
                            return productos;
                    }
                }
            }
            catch (Exception)
            {
            }
            return new List<MasVendidosModel>();
        }
        internal static List<UsuarioModel> getClientesInactivos()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    string requestUri = App.DAUtil.miURL + "informes.php/GET?clientesInactivos=true&idGrupo=" + pu.idGrupo;
                    HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                    string resultJSON = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        List<UsuarioModel> clientes = new List<UsuarioModel>();
                        clientes = JsonConvert.DeserializeObject<List<UsuarioModel>>(resultJSON);
                        if (clientes != null)
                            return clientes;
                    }
                }
            }
            catch (Exception)
            {
            }
            return new List<UsuarioModel>();
        }
        #endregion
        #region Facturacion
        internal async Task<int> nuevaFacturaEstablecimiento(FacturaModel factura)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "facturas.php";
                    HttpResponseMessage response = await App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(factura), Encoding.UTF8, "application/json"));

                    var resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }
        internal async Task<int> NuevaFacturaAdministrador(FacturaAdministradorModel factura)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "facturas.php?administrador=true";
                    HttpResponseMessage response = await App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(factura), Encoding.UTF8, "application/json"));

                    var resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }
        #endregion
        #region Online
        public static async Task<bool> GuardaOnline(int tipo)
        {
            if (App.online == null)
                App.online = new OnlineModel();

            switch (tipo)
            {
                case 1:
                    App.online.id = 0;
                    App.online.horaInicio = DateTime.Now;
                    App.online.idPueblo = App.DAUtil.Usuario.idPueblo;
                    App.online.idUsuario = App.DAUtil.Usuario.idUsuario;
                    App.online.tokenUsuario = App.DAUtil.Usuario.token;
                    break;
                case 2:
                    App.online.horaBackground = DateTime.Now;
                    break;
                case 3:
                    App.online.horaResume = DateTime.Now;
                    break;
                case 4:
                    App.online.horaCierre = DateTime.Now;
                    break;
            }
            if (App.online.id == 0)
            {
                try
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "online.php";
                    HttpResponseMessage response = await App.Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(App.online), Encoding.UTF8, "application/json"));

                    var resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        App.online = JsonConvert.DeserializeObject<OnlineModel>(resultJSON);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
            else
            {
                try
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "online.php";
                    HttpResponseMessage response = await App.Client.PutAsync(requestUri, new StringContent(JsonConvert.SerializeObject(App.online), Encoding.UTF8, "application/json"));

                    var resultJSON = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        public async Task<int> getUsuariosOnline()
        {
            List<OnlineModel> listEventos = new List<OnlineModel>();
            try
            {
                PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                DatosConexionModel.uri = App.DAUtil.miURL + "online.php?idGrupo=" + pu.idGrupo;
                string requestUri = DatosConexionModel.uri;
                HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                string resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listEventos = JsonConvert.DeserializeObject<List<OnlineModel>>(resultJSON);
                    return listEventos.Count;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public async Task<int> getTodosUsuariosOnline()
        {
            List<OnlineModel> listEventos = new List<OnlineModel>();
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "online.php?idGrupo=0";
                string requestUri = DatosConexionModel.uri;
                HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                string resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listEventos = JsonConvert.DeserializeObject<List<OnlineModel>>(resultJSON);
                    return listEventos.Count;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        #endregion
        #region No Utilizados
        public List<ReservaModel> getReservaUsuario(int idUSuario)
        {
            List<ReservaModel> listEventos = new List<ReservaModel>();
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "listadoReservas.php?idUsuario=" + idUSuario;
                string requestUri = DatosConexionModel.uri;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listEventos = JsonConvert.DeserializeObject<List<ReservaModel>>(resultJSON);
                    App.DAUtil.actualizaReservas(listEventos);
                }
                return listEventos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listEventos;
            }
        }
        internal bool respondeReserva(int idReserva, int estado, string respuesta)
        {
            try
            {

                DatosConexionModel.uri = App.DAUtil.miURL + "contestaReserva.php";
                string requestUri = $"{DatosConexionModel.uri}?idReserva={idReserva}&estado={estado}&respuesta={respuesta}";
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    if (resultJSON.Trim().Equals("OK"))
                        return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        internal static void nuevoTicketTerminal(string codigo)
        {
            try
            {
                CabeceraPedido cabecera = TraePedidoPorCodigo(codigo);
                ObservableCollection<LineasPedido> lineas = cabecera.lineasPedidos;
                double total = 0;
                string productos = "";
                foreach (LineasPedido linea in lineas)
                {
                    if (!productos.Equals(""))
                        productos += ";";
                    productos += linea.idProducto + "--" + linea.nombreProducto + "--" + linea.cantidad + "--" + linea.precio.ToString().Replace(",", ".");
                    total += linea.importe;
                }
                DatosConexionModel.uri = $"http://{App.DAUtil.Usuario.establecimientos[0].ipImpresora}:9090/terminal/nuevoTicketTerminal.php?total={total.ToString().Replace(",", ".")}&productos={productos}";
                string requestUri = DatosConexionModel.uri;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                actualizaCuentaTerminal(codigo, resultJSON.Trim());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void actualizaCuentaTerminal(string codigo, string idTicket)
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "actualizaCuentaTerminal.php?codigo=" + codigo + "&idTicket=" + idTicket;
                string requestUri = DatosConexionModel.uri;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        internal static void imprimirCuenta(string codigo)
        {
            try
            {
                DatosConexionModel.uri = $"http://{App.DAUtil.Usuario.establecimientos[0].ipImpresora}:9090/ticket/cuenta.php?nombreImpresoraBarra={App.DAUtil.Usuario.establecimientos[0].nombreImpresoraBarra}&ticket={codigo}";
                string requestUri = DatosConexionModel.uri;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        internal static void imprimirTicket(string codigo)
        {
            try
            {
                DatosConexionModel.uri = $"http://{App.DAUtil.Usuario.establecimientos[0].ipImpresora}:9090/ticket/index.php?nombreImpresoraBarra={App.DAUtil.Usuario.establecimientos[0].nombreImpresoraBarra}&nombreImpresoraCocina={App.DAUtil.Usuario.establecimientos[0].nombreImpresoraCocina}&ticket={codigo}";
                string requestUri = DatosConexionModel.uri;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal static bool cerrarCaja()
        {
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "cierreCaja.php?idEstablecimiento=" + App.DAUtil.Usuario.establecimientos[0].idEstablecimiento;
                string requestUri = DatosConexionModel.uri;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (resultJSON.Trim().Equals(""))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public List<ReservaModel> getReservasEstablecimiento(int idEstablecimiento)
        {
            List<ReservaModel> listEventos = new List<ReservaModel>();
            try
            {
                DatosConexionModel.uri = App.DAUtil.miURL + "listadoReservasRestaurante.php?idEstablecimiento=" + idEstablecimiento;
                string requestUri = DatosConexionModel.uri;
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    listEventos = JsonConvert.DeserializeObject<List<ReservaModel>>(resultJSON);
                    App.DAUtil.actualizaReservas(listEventos);
                }
                return listEventos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return listEventos;
            }
        }
        internal bool solicitaReserva(int idEstablecimiento, int idUsuario, DateTime fecha, int comensales, string observaciones)
        {
            try
            {

                DatosConexionModel.uri = App.DAUtil.miURL + "solicitaReserva.php";
                string requestUri = $"{DatosConexionModel.uri}?idEstablecimiento={idEstablecimiento}&idUsuario={idUsuario}&fecha={fecha.ToString("yyyy-MM-dd HH:mm")}&comensales={comensales}&observaciones={observaciones}";
                HttpResponseMessage response = App.Client.GetAsync(requestUri).Result;
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                {
                    if (resultJSON.Trim().Equals("OK"))
                        return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region Gestión de Clientes

        /// <summary>
        /// Obtiene la lista de todos los clientes (rol=1)
        /// </summary>
        public static async Task<List<UsuarioModel>> GetClientesAsync()
        {
            List<UsuarioModel> clientes = new List<UsuarioModel>();
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "usuarios.php/GET?clientes=true";
                    HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                    string resultJSON = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(resultJSON) && !resultJSON.ToLower().Equals("false"))
                    {
                        clientes = JsonConvert.DeserializeObject<List<UsuarioModel>>(resultJSON);
                        // Eliminar duplicados por ID
                        clientes = clientes.GroupBy(c => c.idUsuario).Select(g => g.First()).ToList();
                        Debug.WriteLine($"GetClientesAsync - Clientes recibidos: {clientes.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error GetClientesAsync: " + ex.Message);
            }
            return clientes;
        }

        /// <summary>
        /// Obtiene el histórico de pedidos de un cliente
        /// </summary>
        public static async Task<List<CabeceraPedido>> GetHistoricoPedidosClienteAsync(int idUsuario)
        {
            List<CabeceraPedido> pedidos = new List<CabeceraPedido>();
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + $"usuarios.php/GET?historicoPedidosCliente=true&idCliente={idUsuario}";
                    Debug.WriteLine($"GetHistoricoPedidosClienteAsync - URL: {requestUri}");
                    HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                    string resultJSON = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"GetHistoricoPedidosClienteAsync - Status: {response.StatusCode}, Response: {resultJSON?.Substring(0, Math.Min(200, resultJSON?.Length ?? 0))}");

                    if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(resultJSON) && !resultJSON.ToLower().Equals("false"))
                    {
                        pedidos = JsonConvert.DeserializeObject<List<CabeceraPedido>>(resultJSON);
                        Debug.WriteLine($"GetHistoricoPedidosClienteAsync - Pedidos deserializados: {pedidos?.Count ?? 0}");
                    }
                }
                else
                {
                    Debug.WriteLine("GetHistoricoPedidosClienteAsync - Sin conexión a internet");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error GetHistoricoPedidosClienteAsync: " + ex.Message);
            }
            return pedidos;
        }

        /// <summary>
        /// Actualiza los datos de un cliente
        /// </summary>
        public static bool ActualizarCliente(UsuarioModel cliente)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "usuarios.php?actualizarCliente=true";

                    var datos = new
                    {
                        idUsuario = cliente.idUsuario,
                        nombre = cliente.nombre ?? "",
                        apellidos = cliente.apellidos ?? "",
                        email = cliente.email ?? "",
                        telefono = cliente.telefono ?? "",
                        direccion = cliente.direccion ?? "",
                        poblacion = cliente.poblacion ?? "",
                        provincia = cliente.provincia ?? "",
                        cod_postal = cliente.codPostal ?? "",
                        estado = cliente.estado,
                        saldo = cliente.saldo,
                        kiosko = cliente.kiosko
                    };

                    HttpResponseMessage response = App.Client.PutAsync(requestUri,
                        new StringContent(JsonConvert.SerializeObject(datos), Encoding.UTF8, "application/json")).Result;

                    string resultJSON = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error ActualizarCliente: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Obtiene los puntos de un cliente por establecimiento
        /// </summary>
        public static async Task<List<PuntosUsuarioModel>> GetPuntosClienteAsync(int idUsuario)
        {
            List<PuntosUsuarioModel> puntos = new List<PuntosUsuarioModel>();
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + $"usuarios.php/GET?puntosCliente=true&idCliente={idUsuario}";
                    Debug.WriteLine($"GetPuntosClienteAsync - URL: {requestUri}");
                    HttpResponseMessage response = await App.Client.GetAsync(requestUri);
                    string resultJSON = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"GetPuntosClienteAsync - Status: {response.StatusCode}, Response: {resultJSON?.Substring(0, Math.Min(200, resultJSON?.Length ?? 0))}");

                    if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(resultJSON) && !resultJSON.ToLower().Equals("false"))
                    {
                        puntos = JsonConvert.DeserializeObject<List<PuntosUsuarioModel>>(resultJSON);
                        Debug.WriteLine($"GetPuntosClienteAsync - Puntos deserializados: {puntos?.Count ?? 0}");
                    }
                }
                else
                {
                    Debug.WriteLine("GetPuntosClienteAsync - Sin conexión a internet");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error GetPuntosClienteAsync: " + ex.Message);
            }
            return puntos;
        }

        /// <summary>
        /// Actualiza los puntos de un cliente en un establecimiento
        /// </summary>
        public static bool ActualizarPuntosCliente(int idUsuario, int idEstablecimiento, int puntos)
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    DatosConexionModel.uri = urlPro;
                    string requestUri = DatosConexionModel.uri + "usuarios.php?actualizarPuntos=true";

                    var datos = new
                    {
                        idUsuario = idUsuario,
                        idEstablecimiento = idEstablecimiento,
                        puntos = puntos
                    };

                    Debug.WriteLine($"ActualizarPuntosCliente - URL: {requestUri}");
                    Debug.WriteLine($"ActualizarPuntosCliente - Datos: idUsuario={idUsuario}, idEstablecimiento={idEstablecimiento}, puntos={puntos}");

                    HttpResponseMessage response = App.Client.PutAsync(requestUri,
                        new StringContent(JsonConvert.SerializeObject(datos), Encoding.UTF8, "application/json")).Result;

                    string resultJSON = response.Content.ReadAsStringAsync().Result;

                    Debug.WriteLine($"ActualizarPuntosCliente - Status: {response.StatusCode}, Response: {resultJSON}");

                    if (response.IsSuccessStatusCode && !resultJSON.ToLower().Equals("false"))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error ActualizarPuntosCliente: " + ex.Message);
                return false;
            }
        }

        #endregion
    }
}
