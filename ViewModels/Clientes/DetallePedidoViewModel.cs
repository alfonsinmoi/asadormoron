using AsadorMoron.Interfaces;
// 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.Utils;
using Mopups.Services;
using AsadorMoron.Views.Clientes;
using System.Diagnostics;
using AsadorMoron.Models.PayComet;

namespace AsadorMoron.ViewModels.Clientes
{
    public class DetallePedidoViewModel : ViewModelBase
    {
        public DetallePedidoViewModel()
        {
            //NavigateToAddCreditCardPageCommand = new Command(() => ExecuteNavigateToAddCreditCardPageCommand());
        }
        AmigosModel amigos = null;
        bool EsRepartoPropio = false;
        ConfiguracionAdmin cadmin;
        PueblosModel pu = new PueblosModel();
        public async override Task InitializeAsync(object navigationData)
        {

            try
            {
                bool continuar = true;
                Idioma = App.idioma;

                if (continuar)
                {
                    App.saldoGastado = 0;
                    App.amigos = null;
                    App.PendientePromocion = false;
                    Preferences.Set("textoPromocion", "");
                    if (App.listaProductos == null && Carrito2 != null)
                        App.listaProductos = await App.ResponseWS.getListadoProductosEstablecimiento(Carrito2[0].idEstablecimiento, false);
                    if (!cargado)
                    {
                        if (App.DAUtil.Usuario.rol==1)
                            TieneCodigoAmigo = App.promocionAmigo != null;

                        if (TieneCodigoAmigo)
                        {
                            amigos = ResponseServiceWS.TieneActivacionPendiente();
                            App.amigos = amigos;
                            PendientePromocion = amigos != null;
                            Saldo = App.DAUtil.Usuario.saldo;
                            VisibleSaldo = Saldo > 0;
                            if (PendientePromocion)
                            {
                                TextoPromocion = "Realiza un pedido de, al menos, " + App.promocionAmigo.pedidoMinimo.ToString("C2") + " para ganar tus " + amigos.saldoAmigo.ToString("C2") + " de saldo";
                                if (Saldo > 0)
                                    TextoPromocion += Environment.NewLine + "Tiene un saldo acumulado de " + Saldo.ToString("C2");
                            }
                                    
                        }
                        cadmin = ResponseServiceWS.getConfiguracionAdmin();
                        cargado = true;
                        App.DAUtil.EnTimer = false;
                        est = App.EstActual;
                        EsComercio = est.esComercio;
                        Logo = "logocabeceraazul.png";
                        Carrito2 = navigationData as List<CarritoModel>;
                        TieneEncargo = Carrito2.Where(p => p.porEncargo == true).ToList().Count >= 1;
                        if (App.listaProductos == null)
                            App.listaProductos = await App.ResponseWS.getListadoProductosEstablecimiento(Carrito2[0].idEstablecimiento, false);

                        Carrito = new ObservableCollection<CarritoBindable>();
                        if (Carrito2 == null)
                            Carrito2 = new List<CarritoModel>();
                        if (Carrito2.Count > 0)
                            Observaciones = Carrito2[0].observaciones;

                        TieneTarjeta = cadmin.tarjeta;
                        TieneEfectivo = cadmin.efectivo;
                        TieneBizum = cadmin.bizum;
                        TieneDatafono = cadmin.datafono;
                            
                        TieneRecogida = est.recogida == 1;
                        TieneEnvio = est.envio == 1;

                        if (TieneRecogida || TieneEnvio)
                        {
                            Efectivo = Preferences.Get("Pago", "Efectivo").Equals("Efectivo") && TieneEfectivo;
                            Tarjeta = Preferences.Get("Pago", "Efectivo").Equals("Tarjeta") && TieneTarjeta;
                            Bizum = Preferences.Get("Pago", "Efectivo").Equals("Bizum") && TieneBizum;
                            Datafono = Preferences.Get("Pago", "Efectivo").Equals("Datafono") && TieneDatafono;
                            if (!Efectivo && !Tarjeta && !Bizum && !Datafono)
                            {
                                if (TieneTarjeta)
                                    Tarjeta = true;
                                else if (TieneDatafono)
                                    Datafono = true;
                                else if (TieneEfectivo)
                                    Efectivo = true;
                                else if (TieneBizum)
                                    Bizum = true;
                                else
                                {
                                    await App.customDialog.ShowDialogAsync("Esta población no dispone de método de pago configurado" + Environment.NewLine + "Disculpe las molestias", AppResources.App, AppResources.Cerrar);
                                    await App.DAUtil.NavigationService.NavigateBackAsync();
                                }
                            }
                            if (TieneRecogida && TieneEnvio)
                            {
                                Recogida = Preferences.Get("TipoPedido", "Recogida").Equals("Recogida");
                                Envio = Preferences.Get("TipoPedido", "Envio").Equals("Envio");
                                if (Recogida)
                                {
                                    TieneDatafono = false;
                                    if (Datafono)
                                        Efectivo = true;
                                }
                            }
                            else if (TieneRecogida)
                                Recogida = true;
                            else if (TieneEnvio)
                                Envio = true;

                            if (App.DAUtil.Usuario != null)
                            {
                                if (App.DAUtil.Usuario.tarjetas == null)
                                    App.DAUtil.Usuario.tarjetas = new List<TarjetaModel>();
                                List<TarjetaModel> tarjetas = new List<TarjetaModel>(App.DAUtil.Usuario.tarjetas);
                                List<TarjetaBindableModel> tarjetasBindables = new List<TarjetaBindableModel>();
                                if (tarjetas != null && tarjetas.Count > 0)
                                {
                                    foreach (TarjetaModel t in tarjetas)
                                    {
                                        TarjetaBindableModel tar = new TarjetaBindableModel();
                                        tar.tarjeta = t;
                                        if (Preferences.Get("TarjetaSeleccionada", "").Equals(t.pan))
                                        {
                                            tar.fondo = "#000000";
                                            tar.Seleccionada = true;
                                            TarjetaSeleccionada = tar;
                                            App.TarjetaSeleccionada = tar.tarjeta;
                                            Preferences.Set("TarjetaSeleccionada", App.TarjetaSeleccionada.pan);
                                        }
                                        else
                                        {
                                            tar.fondo = "#FFFFFF";
                                            tar.Seleccionada = false;
                                        }

                                        tarjetasBindables.Add(tar);
                                    }
                                    Cards = new ObservableCollection<TarjetaBindableModel>(tarjetasBindables);
                                    if (TarjetaSeleccionada == null && Cards.Count > 0)
                                    {
                                        Cards[0].fondo = "#000000";
                                        Cards[0].Seleccionada = true;
                                        TarjetaSeleccionada = Cards[0];
                                        App.TarjetaSeleccionada = Cards[0].tarjeta;
                                        Preferences.Set("TarjetaSeleccionada", Cards[0].tarjeta.pan);
                                    }
                                }
                            }
                            //TODO: Multipueblo
                            Zonas = new List<ZonaModel>(App.ResponseWS.getListadoZonas(1).Where(p => p.activo == 1));
                            Usuario = App.DAUtil.GetUsuarioSQLite();
                            if (Usuario != null)
                            {
                                if (Zonas.Count == 0 || Zonas == null)
                                {
                                    await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("no_zona")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", Environment.NewLine), AppResources.App, AppResources.Cerrar);

                                    Gastos = 0;
                                    Direccion = "";
                                    CambiaDireccion = true;
                                }
                                else
                                {
                                    ZonaModel z = Zonas.Where(p => p.idZona == Usuario.idZona).FirstOrDefault();
                                    if (z == null)
                                    {
                                        ZonaSeleccionda = ResponseServiceWS.GetZonaByIdUsuario();
                                        if (ZonaSeleccionda == null)
                                        {
                                            await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("no_zona")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", Environment.NewLine), AppResources.App, AppResources.Cerrar);
                                            await App.DAUtil.NavigationService.NavigateToAsyncMenu<PerfilViewModel>();
                                        }
                                        if (Zonas != null && Zonas.Count > 0)
                                            ZonaSeleccionda = Zonas[0];
                                        if (ZonaSeleccionda != null)
                                        {
                                            Modificable = ZonaSeleccionda.modificable == 1;
                                            if (ZonaSeleccionda.cambiaDireccion == 1)
                                            {
                                                CambiaDireccion = true;
                                                if (string.IsNullOrEmpty(direccionCambiada))
                                                    Direccion = Usuario.direccion;
                                                else
                                                    Direccion = direccionCambiada;
                                            }
                                            else
                                            {
                                                CambiaDireccion = false;
                                                direccionCambiada = Direccion;
                                                Direccion = ZonaSeleccionda.direccionEnvio;

                                            }
                                        }
                                    }
                                    else
                                    {
                                        ZonaSeleccionda = z;
                                        if (ZonaSeleccionda != null)
                                        {
                                            Modificable = ZonaSeleccionda.modificable == 1;
                                            if (ZonaSeleccionda.cambiaDireccion == 1)
                                            {
                                                CambiaDireccion = true;
                                                if (string.IsNullOrEmpty(direccionCambiada))
                                                    Direccion = Usuario.direccion;
                                                else
                                                    Direccion = direccionCambiada;
                                            }
                                            else
                                            {
                                                CambiaDireccion = false;
                                                direccionCambiada = Direccion;
                                                Direccion = ZonaSeleccionda.direccionEnvio;

                                            }
                                        }
                                    }
                                }
                            }
                            else
                                Gastos = 0;
                            PrecioTotalPedido = 0;
                            /*if (Carrito2[0].tipo != 1)
                                Gastos = 0;*/
                            foreach (var item in Carrito2)
                            {
                                item.nombreCantidad = string.Format("{0} x{1}", item.comida, item.cantidad);
                                item.precioTotal = item.cantidad * item.precio;
                                precioPedido += item.precioTotal;
                                if (item.porPuntos == 0)
                                    PrecioTotalPedido += item.precioTotal;
                            }
                            Bolsa = (((int)(PrecioTotalPedido / App.EstActual.configuracion.rangoBolsas))+1) * App.EstActual.configuracion.precioBolsa;
                            if (Bolsa == 0)
                                Bolsa = App.EstActual.configuracion.precioBolsa;
                            PrecioTotalPedidoGastos = PrecioTotalPedido + Gastos+Bolsa;
                            if (pu != null)
                                Poblacion = pu.nombre;
                            else
                                Poblacion = Usuario.poblacion;
                            foreach (CarritoModel ca in Carrito2)
                            {
                                CarritoBindable c = new CarritoBindable();
                                c.cantidad = ca.cantidad;
                                c.comida = ca.comida;
                                c.puntos = ca.puntos;
                                c.porPuntos = ca.porPuntos;
                                c.comida_eng = ca.comida_eng;
                                c.comida_fr = ca.comida_fr;
                                c.comentario = ca.comentario;
                                c.comida_ger = ca.comida_ger;
                                c.idArticulo = ca.idArticulo;
                                c.porEncargo = ca.porEncargo;
                                c.idEstablecimiento = ca.idEstablecimiento;
                                c.idEvento = ca.idEvento;
                                c.imagen = ca.imagen;
                                c.nombreCantidad = ca.nombreCantidad;
                                c.observaciones = ca.observaciones;
                                c.opcion = ca.opcion;
                                c.precio = ca.precio;
                                c.precioTotal = ca.precioTotal;
                                Carrito.Add(c);
                            }
                                    
                        }
                        else
                        {
                            await App.customDialog.ShowDialogAsync(AppResources.EstablecimientoSinEnvio, AppResources.SoloError, AppResources.Cerrar);
                            await App.DAUtil.NavigationService.NavigateBackAsync();
                        }
                    }

                    if (App.EstActual.configuracion == null)
                        App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);

                    Gastos = Envio ? ZonaSeleccionda.gastos : 0;

                    Bolsa = (((int)(PrecioTotalPedido / App.EstActual.configuracion.rangoBolsas))+1) * App.EstActual.configuracion.precioBolsa;
                    if (Bolsa == 0)
                        Bolsa = App.EstActual.configuracion.precioBolsa;
                    PrecioTotalPedidoGastos = PrecioTotalPedido + Gastos + Bolsa;
                }
                else
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.NoPuedePedirDesdeSuDireccion, AppResources.App, AppResources.Cerrar).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.DAUtil.NavigationService.NavigateBackAsync(); }));
                }
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }
            finally
            {
                App.userdialog.HideLoading();
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Métodos
        private void SoloTarjeta(bool hacer)
        {
            if (hacer)
            {
                TieneEfectivo = false;
                TieneBizum = false;
                TieneDatafono = false;
                Tarjeta = true;
            }
            else
                App.DAUtil.NavigationService.NavigateBackAsync();
        }
        private async void AplicarCupon()
        {
            if (!string.IsNullOrEmpty(Cupon) && App.DAUtil.Usuario != null && !CuponAplicado)
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                await Task.Delay(200);
                try
                {
                    cup = await ResponseServiceWS.AplicarCuponSQL(Cupon);
                    if (cup != null)
                    {
                        CuponAplicado = true;
                        
                            if (cup.gastosEnvio)
                            {
                                if (Gastos > 0)
                                {
                                    if (cup.tipoDescuento == 0)
                                        Descuento = Gastos * (cup.descuento / 100);
                                    else if (cup.tipoDescuento == 1)
                                        Descuento = cup.descuento;
                                }
                                else
                                {
                                    App.userdialog.HideLoading();
                                    await App.customDialog.ShowDialogAsync("Este cupón no es aplicable", "PolloAndaluz", "Cerrar");
                                }
                            }
                            else
                            {
                                if (cup.tipoDescuento == 0)
                                    Descuento = PrecioTotalPedido * (cup.descuento / 100);
                                else if (cup.tipoDescuento == 1)
                                    Descuento = cup.descuento;

                            }
                            PrecioTotalPedidoGastos -= Descuento;
                        
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync("Este cupón no es aplicable", "PolloAndaluz", "Cerrar");
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    App.userdialog.HideLoading();
                }
            }
        }
        private async void RestarSaldo()
        {
            if (PrecioTotalPedido > Descuento)
            {
                if (Saldo > PrecioTotalPedido - Descuento)
                {
                    Descuento = PrecioTotalPedido;
                    App.saldoGastado = Descuento;
                    PrecioTotalPedidoGastos -= Descuento;
                    Saldo -= Descuento;
                }
                else
                {
                    Descuento = Saldo;
                    App.saldoGastado = Descuento;
                    PrecioTotalPedido = PrecioTotalPedido - Saldo;
                    PrecioTotalPedidoGastos -= Descuento;
                    Saldo = 0;
                }
                if (PendientePromocion)
                {
                    TextoPromocion = "Realiza un pedido de, al menos, " + App.promocionAmigo.pedidoMinimo.ToString("C2") + " para ganar tus " + amigos.saldoAmigo.ToString("C2") + " de saldo";
                    if (Saldo > 0)
                        TextoPromocion += Environment.NewLine + "Tiene un saldo acumulado de " + Saldo.ToString("C2");
                }
                VisibleSaldo = Saldo > 0;
            }
        }
        private async void EnviarPedido(object obj)
        {
            try
            {
                double pedidoMinimo = est.pedidoMinimo;
                if (ZonaSeleccionda != null)
                {
                    if (ZonaSeleccionda.pedidoMinimo > pedidoMinimo)
                        pedidoMinimo = ZonaSeleccionda.pedidoMinimo;
                }
                bool continuar = true;
                string cerrado = "";
                List<Establecimiento> idEstablecimientos = new List<Establecimiento>();
                cadmin = ResponseServiceWS.getConfiguracionAdmin();

                
                if (idEstablecimientos.Count >= 1)
                {
                    if (string.IsNullOrEmpty(Direccion) && Envio)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(AppResources.DireccionVacia, AppResources.Informacion, AppResources.Cerrar);
                    }
                    else if (Carrito2.Count == 0)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(AppResources.CarritoVacio, AppResources.Informacion, AppResources.Cerrar);
                    }
                    else if (ZonaSeleccionda == null && Envio)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("registrado")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                    }
                    else if (!cadmin.servicioActivo || !continuar)
                    {
                        continuar = false;
                        if (cadmin.servicioActivo)
                            await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("no_disponible_est")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", est.nombre).Replace("{1}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                        else
                            await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("no_disponible")).FirstOrDefault<MensajesModel>().valor.Replace("{1}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                    }
                    else if ((precioPedido < pedidoMinimo) && Envio)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(String.Format(App.MensajesGlobal.Where(p => p.clave.Equals("pedido_minimo")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (pedidoMinimo + Gastos).ToString()).Replace("{1}", Environment.NewLine)), AppResources.App, AppResources.Cerrar);
                    }
                    else if (App.DAUtil.Usuario == null)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("registrado")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                    }
                    else if (Tarjeta && App.TarjetaSeleccionada==null)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("sin_tarjeta")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                    }
                    foreach (CarritoModel c in Carrito2)
                    {
                        if (continuar)
                        {
                            if (idEstablecimientos.Find(p => p.idEstablecimiento == c.idEstablecimiento) == null)
                            {
                                Establecimiento est = App.DAUtil.GetEstablecimientoSQL(c.idEstablecimiento);
                                idEstablecimientos.Add(est);
                                if (est.configuracion == null)
                                    est.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(c.idEstablecimiento);
                                if (est.configuracion.pedidoMinimo > pedidoMinimo)
                                    pedidoMinimo = est.configuracion.pedidoMinimo;
                                if (!est.configuracion.servicioActivo)
                                {
                                    continuar = false;
                                    cerrado = est.nombre;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Direccion) && Envio)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(AppResources.DireccionVacia, AppResources.Informacion, AppResources.Cerrar);
                    }
                    else if (Carrito2.Count == 0)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(AppResources.CarritoVacio, AppResources.Informacion, AppResources.Cerrar);
                    }
                    else if (ZonaSeleccionda == null && Envio)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("registrado")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                    }
                    else if (!cadmin.servicioActivo || !continuar)
                    {
                        continuar = false;
                        if (cadmin.servicioActivo)
                            await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("no_disponible_est")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", est.nombre).Replace("{1}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                        else
                            await App.customDialog.ShowDialogAsync(cerrado + AppResources.EstablecimientoCerrado + Environment.NewLine + AppResources.Perdon, AppResources.Informacion, AppResources.Cerrar);
                    }
                    else if ((precioPedido < pedidoMinimo) && Envio)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(String.Format(App.MensajesGlobal.Where(p => p.clave.Equals("pedido_minimo")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (pedidoMinimo + Gastos).ToString()).Replace("{1}", Environment.NewLine)), AppResources.App, AppResources.Cerrar);
                    }
                    else if (App.DAUtil.Usuario == null)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("registrado")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                    }
                    else if (Tarjeta && App.TarjetaSeleccionada == null)
                    {
                        continuar = false;
                        await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("sin_tarjeta")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                    }
                }
                if (continuar)
                {
                    if (Envio)
                    {
                        Carrito2[0].direccion = Direccion;
                        Carrito2[0].poblacion = Poblacion;
                        Carrito2[0].idZona = ZonaSeleccionda.idZona;
                        Carrito2[0].observaciones = Observaciones;
                        Carrito2[0].tipo = 1;
                    }
                    else
                    {
                        Carrito2[0].direccion = "";
                        Carrito2[0].poblacion = "";
                        Carrito2[0].idZona = 0;
                        Carrito2[0].observaciones = Observaciones;
                        Carrito2[0].tipo = 2;
                    }
                    Carrito2[0].gastos = Gastos;

                    try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (!TieneEncargo)
                        {
                            App.Descuento = Descuento;
                            await App.DAUtil.NavigationService.NavigateToAsync<FranjasHorariasViewModel>(Carrito2).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                            {
                                App.userdialog.HideLoading();
                            }));
                        }
                        else
                        {
                            await App.DAUtil.NavigationService.NavigateToAsync<FranjasHorariasEncargoViewModel>(Carrito2).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                            {
                                App.userdialog.HideLoading();
                            }));
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                // 
                App.userdialog.HideLoading();
                await App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje +ex.Message + Environment.NewLine + AppResources.Perdon,AppResources.SoloError, AppResources.Cerrar);
            }
        }
        private async Task HacerPedidoTarjera(bool hacer)
        {
            try
            {
                if (hacer)
                {
                    App.userdialog.ShowLoading(AppResources.Cargando);
                    await Task.Delay(200);
                    double total = 0;
                    if (string.IsNullOrEmpty(Carrito2[0].observaciones))
                        Carrito2[0].observaciones = "";
                    codigoPedido = App.DAUtil.GetCodigo();
                    List<LineasPedido> l = new List<LineasPedido>();
                    int punt = 0;

                    double Bolsa = ((int)(PrecioTotalPedido / App.EstActual.configuracion.rangoBolsas)) * App.EstActual.configuracion.precioBolsa;
                    if (Bolsa == 0)
                        Bolsa = App.EstActual.configuracion.precioBolsa;
                    LineasPedido l2 = new LineasPedido();
                    l2.cantidad = ((int)(PrecioTotalPedido / App.EstActual.configuracion.rangoBolsas));
                    l2.idProducto = 0;
                    l2.precio = Bolsa / l2.cantidad;
                    l2.nombreProducto = "Bolsa";
                    l2.tipoComida = 4;
                    l2.pagadoConPuntos = 0;
                    l.Add(l2);
                    total += Bolsa;
                    foreach (var item in Carrito2)
                    {
                        if (item.cantidad > 0)
                        {
                            LineasPedido l20 = new LineasPedido();
                            l20.cantidad = item.cantidad;
                            l20.idProducto = item.idArticulo;
                            l20.precio = item.precio;
                            l20.tipoComida = 0;
                            l20.nombreProducto = item.comida;
                            l20.pagadoConPuntos = item.porPuntos;
                            if (item.porPuntos == 1)
                                punt += item.puntos;
                            l.Add(l20);
                            total += l20.cantidad * l20.precio;
                        }
                    }
                    l2 = new LineasPedido();
                    l2.cantidad = 1;
                    l2.idProducto = 0;
                    l2.precio = Gastos;
                    l2.nombreProducto = "";
                    l2.tipoComida = 1;
                    l.Add(l2);
                    total += l2.cantidad * l2.precio;
                    if (Descuento > 0)
                    {
                        l2 = new LineasPedido();
                        l2.cantidad = 1;
                        l2.idProducto = 0;
                        l2.precio = Descuento * -1;
                        l2.nombreProducto = "";
                        l2.tipoComida = 3;
                        l2.pagadoConPuntos = 0;
                        l.Add(l2);
                        total -= Descuento;
                    }

                    string tipoVenta = "Envío";
                    if (Recogida)
                        tipoVenta = "Recogida";

                    CabeceraPedido p = new CabeceraPedido();
                    p.idUsuario = App.DAUtil.Usuario.idUsuario;
                    p.idEstablecimiento = Carrito2[0].idEstablecimiento;
                    p.horaEntrega = fecha.ToString("yyyy-MM-dd HH:mm:ss");
                    p.codigoPedido = codigoPedido;
                    p.idZona = Carrito2[0].idZona;
                    p.direccionUsuario = Carrito2[0].direccion;
                    p.tipo = 1;
                    p.idZonaEstablecimiento = 0;
                    p.mesa = "";
                    p.zonaEstablecimiento = "";
                    p.tipoVenta = tipoVenta;
                    p.transaccion = codigoPedido;
                    p.comentario = Carrito2[0].observaciones;
                    p.tipoPago = "Tarjeta";
                    p.nombreUsuario = App.DAUtil.Usuario.nombre + " " + App.DAUtil.Usuario.apellidos.Substring(0, 1) + ".";
                    p.pagado = 1;
                    foreach (LineasPedido ll in l)
                        ll.tipoVenta = tipoVenta;

                    p.lineasPedidos = new ObservableCollection<LineasPedido>(l);
                    p.idCuenta = 0;
                    App.pedidoEnCurso = p;
                    App.carritoEnCurso = Carrito2;
                    await PagoConTarjeta(total,punt);
                }
            }
            catch (Exception ex)
            {
                // 
                App.userdialog.HideLoading();
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
            }
            finally
            {
                App.userdialog.HideLoading();
            }
        }

        private async Task PagoConTarjeta(double total, int punt)
        {
            Preferences.Set("EsPedidoLocal", false);
            Preferences.Set("EsPedidoComercio", true);
            Preferences.Set("dia", dia);
            Preferences.Set("totalPedido", ((int)(total * 100)).ToString());
            App.urlChallengue = await App.ResponseWS.realizaPagoTarjeta(App.TarjetaSeleccionada.idUser, App.TarjetaSeleccionada.tokenUser, ((int)(total * 100)).ToString(), App.pedidoEnCurso.codigoPedido);
            if (App.urlChallengue.StartsWith("http"))
                await App.DAUtil.NavigationService.NavigateToAsyncWithoutMenu<WebViewModel>();
            else if (App.urlChallengue.Equals(""))
            {
                string tipoVenta = "Envío";
                if (Recogida)
                    tipoVenta = "Recogida";

                if (tipoVenta.Equals("Envío"))
                {
                    if (App.EstActual.configuracion == null)
                        App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);
                    
                }
                int idCodigoPedido = ResponseServiceWS.NuevoPedido(punt);
                if (idCodigoPedido > 0)
                {
                    if (!App.pedidoEnCurso.tipoVenta.StartsWith("Local"))
                    {
                        List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(1);
                        foreach (TokensModel to in tokens)
                            App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + App.pedidoEnCurso.codigoPedido, to.token);
                    }
                    if (App.pedidoEnCurso.tipoVenta.StartsWith("Envío"))
                    {
                        List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(App.EstActual.idEstablecimiento);
                        foreach (TokensModel to in tokens2)
                            App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + App.pedidoEnCurso.codigoPedido, to.token);
                    }

                    List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(App.EstActual.idEstablecimiento);
                    foreach (TokensModel to in tokens3)
                        App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido: " + App.pedidoEnCurso.codigoPedido, to.token);

                    List<PedidoModel> pedidosModel = new List<PedidoModel>();
                    DateTime fechaPedido = DateTime.Now;
                    foreach (var item in App.carritoEnCurso)
                    {
                        PedidoModel p = new PedidoModel();
                        p.idArticulo = item.idArticulo;
                        p.idPedido = App.pedidoEnCurso.idPedido;
                        p.idEstablecimiento = item.idEstablecimiento;
                        p.imagen = item.imagen;
                        p.nombreCantidad = item.nombreCantidad;
                        p.observaciones = item.observaciones.Trim();
                        p.precio = item.precio;
                        p.cantidad = item.cantidad;
                        p.comida = item.comida;
                        p.precioTotal = item.precioTotal;
                        p.fechaPedido = fechaPedido;
                        p.nombreEstablecimiento = App.EstActual.nombre;
                        p.horaEntrega = App.pedidoEnCurso.horaEntrega;
                        pedidosModel.Add(p);
                    }
                    App.DAUtil.GuardarPedido(pedidosModel);
                    App.DAUtil.VaciaCarrito();
                    if (App.promocionAmigo != null) {
                        if (PendientePromocion && App.promocionAmigo.pedidoMinimo <= PrecioTotalPedidoGastos) {
                            if (await ResponseServiceWS.RealizaPromocionAmigo(amigos)){
                                Preferences.Set("promocionAplicada", true);
                                Preferences.Set("textoPromocion", $"Enhorabuena!!!. Ha desbloqueado su código amigo, se le han sumado {amigos.saldoAmigo.ToString("C2")} a su saldo.");
                                App.ResponseWS.enviaNotificacion("PolloAndaluz", $"Enhorabuena!!!. Su amigo {App.DAUtil.Usuario.nombre} {App.DAUtil.Usuario.apellidos}, ha realizado su pedido, se le han sumado {amigos.saldoCliente.ToString("C2")} a su saldo." , App.ResponseWS.getTokenUsuario(amigos.idCliente));
                            }
                        }
                    }

                    await App.DAUtil.NavigationService.NavigateToAsyncWithoutMenu<PedidoConfirmadoComercioViewModel>(App.pedidoEnCurso.codigoPedido + ";" + Preferences.Get("dia", ""));
                }
                else
                {
                    if (await App.ResponseWS.cambiaEstadoPedido(App.pedidoEnCurso.idPedido, 99))
                    {
                        OperationInfoModel info = await App.ResponseWS.infoTransactionPaycomet(App.pedidoEnCurso.codigoPedido);
                        await App.ResponseWS.refundPaycomet(App.pedidoEnCurso.codigoPedido, App.TarjetaSeleccionada.idUser, App.TarjetaSeleccionada.tokenUser, ((int)(total * 100)).ToString(), info.payment.authCode);
                        App.userdialog.HideLoading();
                    }
                    await App.DAUtil.NavigationService.NavigateToAsyncWithoutMenu<PagoErroneoViewModel>(App.pedidoEnCurso.codigoPedido + ";" + Preferences.Get("dia", ""));
                }
            }
            else
            {
                //await ResponseServiceWS.eliminaPedido(App.pedidoEnCurso.idPedido);
                App.userdialog.HideLoading();
                await App.customDialog.ShowDialogAsync(App.urlChallengue, "PolloAndaluz", AppResources.OK);
            }
        }

        private async Task HacerPedido(bool hacer)
        {
            try
            {
                if (hacer)
                {
                    App.userdialog.ShowLoading(AppResources.Cargando);
                    await Task.Delay(200);
                    ConfiguracionAdmin conf = ResponseServiceWS.getConfiguracionAdmin();
                    if (string.IsNullOrEmpty(Carrito2[0].observaciones))
                        Carrito2[0].observaciones = "";
                    codigoPedido = App.DAUtil.GetCodigo();
                    List<LineasPedido> l = new List<LineasPedido>();
                    int punt = 0;
                    foreach (var item in Carrito2)
                    {
                        if (item.cantidad > 0)
                        {
                            LineasPedido l20 = new LineasPedido();
                            l20.cantidad = item.cantidad;
                            l20.idProducto = item.idArticulo;
                            l20.precio = item.precio;
                            l20.tipoComida = 0;
                            l20.pagadoConPuntos = item.porPuntos;
                            if (item.porPuntos == 1)
                                punt += item.puntos;
                            l20.nombreProducto = item.comida;
                            l.Add(l20);
                        }
                    }
                    LineasPedido l2 = new LineasPedido();
                    l2.cantidad = 1;
                    l2.idProducto = 0;
                    l2.precio = Gastos;
                    l2.nombreProducto = "";
                    l2.tipoComida = 1;
                    l2.pagadoConPuntos = 0;
                    l.Add(l2);
                    if (Descuento > 0)
                    {
                        l2 = new LineasPedido();
                        l2.cantidad = 1;
                        l2.idProducto = 0;
                        l2.precio = Descuento * -1;
                        l2.nombreProducto = "";
                        l2.tipoComida = 3;
                        l2.pagadoConPuntos = 0;
                        l.Add(l2);
                    }
                    string tipoVenta = "Envío";
                    if (Recogida)
                        tipoVenta = "Recogida";
                    string tipoPago = "Efectivo";
                    if (Bizum)
                        tipoPago = "Bizum";
                    else if (Datafono)
                        tipoPago = "Datafono";

                    if (tipoVenta.Equals("Envío"))
                    {
                        if (App.EstActual.configuracion == null)
                            App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);
                        
                    }

                    int idCodigoPedido = ResponseServiceWS.NuevoPedido("0", 0, "", tipoVenta, App.DAUtil.Usuario.idUsuario, Carrito2[0].idEstablecimiento, codigoPedido, Carrito2[0].idZona, Carrito2[0].direccion, Carrito2[0].observaciones, fecha.ToString("yyyy-MM-dd HH:mm:ss"), l, "", 1, tipoPago,punt);

                    if (idCodigoPedido > 0)
                    {
                        string pedido = string.Empty;
                        if (tipoVenta.StartsWith("Local"))
                        {
                            List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(1);
                            foreach (TokensModel to in tokens)
                                App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + codigoPedido, to.token);
                        }
                        if (tipoVenta.StartsWith("Envío"))
                        {
                            List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(App.EstActual.idEstablecimiento);
                            foreach (TokensModel to in tokens2)
                                App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + codigoPedido, to.token);
                        }

                        List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(App.EstActual.idEstablecimiento);
                        foreach (TokensModel to in tokens3)
                            await App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido: " + codigoPedido, to.token);


                        List<PedidoModel> pedidosModel = new List<PedidoModel>();
                        DateTime fechaPedido = DateTime.Now;
                        foreach (var item in Carrito2)
                        {
                            PedidoModel p = new PedidoModel();
                            p.idArticulo = item.idArticulo;
                            p.idPedido = idCodigoPedido;
                            p.idEstablecimiento = item.idEstablecimiento;
                            p.imagen = item.imagen;
                            p.nombreCantidad = item.nombreCantidad;
                            p.observaciones = item.observaciones;
                            p.precio = item.precio;
                            p.cantidad = item.cantidad;
                            p.comida = item.comida;
                            p.precioTotal = item.precioTotal;
                            p.fechaPedido = fechaPedido;
                            p.nombreEstablecimiento = App.EstActual.nombre;
                            p.horaEntrega = conf.inicioComercio.ToString();
                            pedidosModel.Add(p);
                        }
                        App.DAUtil.GuardarPedido(pedidosModel);
                        App.DAUtil.VaciaCarrito();
                        if (App.promocionAmigo != null)
                        {
                            if (PendientePromocion && App.promocionAmigo.pedidoMinimo <= PrecioTotalPedidoGastos)
                            {
                                if (await ResponseServiceWS.RealizaPromocionAmigo(amigos))
                                {
                                    Preferences.Set("promocionAplicada", true);
                                    Preferences.Set("textoPromocion", $"Enhorabuena!!!. Ha desbloqueado su código amigo, se le han sumado {amigos.saldoAmigo.ToString("C2")} a su saldo.");
                                    App.ResponseWS.enviaNotificacion("PolloAndaluz", $"Enhorabuena!!!. Su amigo {App.DAUtil.Usuario.nombre} {App.DAUtil.Usuario.apellidos}, ha realizado su pedido, se le han sumado {amigos.saldoCliente.ToString("C2")} a su saldo.", App.ResponseWS.getTokenUsuario(amigos.idCliente));
                                }
                            }
                        }
                        await App.DAUtil.NavigationService.NavigateToAsyncMenu<PedidoConfirmadoComercioViewModel>(codigoPedido + ";" + dia);
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
                    }
                }
            }
            catch (Exception ex)
            {
                // 
                App.userdialog.HideLoading();
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
            }
            finally
            {
                App.userdialog.HideLoading();
            }
        }

        private void Add(object parametro)
        {
            try
            {
                int articulo = (int)parametro;

                CarritoBindable c = Carrito.Where((obj) => obj.idArticulo == articulo).First();
                CarritoModel c2 = Carrito2.Where((obj) => obj.idArticulo == articulo).First();
                if (c != null)
                {
                    if (!c2.esMenu)
                    {
                        c.cantidad = c.cantidad + 1;
                        c2.cantidad = c2.cantidad + 1;
                        PrecioTotalPedido = 0;
                        precioPedido = 0;
                        foreach (var item in Carrito)
                        {
                            item.nombreCantidad = string.Format("{0} x{1}", item.comida, item.cantidad);
                            item.precioTotal = item.cantidad * item.precio;
                            precioPedido += item.precioTotal;
                            PrecioTotalPedido += item.precioTotal;
                        }
                        foreach (var item in Carrito2)
                        {
                            item.nombreCantidad = string.Format("{0} x{1}", item.comida, item.cantidad);
                            item.precioTotal = item.cantidad * item.precio;
                            precioPedido += item.precioTotal;
                        }

                        Bolsa = (((int)(PrecioTotalPedido / App.EstActual.configuracion.rangoBolsas))+1) * App.EstActual.configuracion.precioBolsa;
                        if (Bolsa == 0)
                            Bolsa = App.EstActual.configuracion.precioBolsa;
                        PrecioTotalPedidoGastos = PrecioTotalPedido + Gastos + Bolsa;
                        App.DAUtil.ActualizaCarrito(Carrito2);
                    }
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void remove(object parametro)
        {
            try
            {
                int articulo = (int)parametro;

                CarritoBindable c = Carrito.Where((obj) => obj.idArticulo == articulo).First();
                CarritoModel c2 = Carrito2.Where((obj) => obj.idArticulo == articulo).First();
                if (c != null)
                {
                    if (!c2.esMenu)
                    {
                        if (c.cantidad > 1)
                        {
                            c.cantidad = c.cantidad - 1;
                            c2.cantidad = c2.cantidad - 1;
                            PrecioTotalPedido = 0;
                            precioPedido = 0;
                            foreach (var item in Carrito)
                            {
                                item.nombreCantidad = string.Format("{0} x{1}", item.comida, item.cantidad);
                                item.precioTotal = item.cantidad * item.precio;
                                PrecioTotalPedido += item.precioTotal;
                                precioPedido += item.precioTotal;
                            }
                            foreach (var item in Carrito2)
                            {
                                item.nombreCantidad = string.Format("{0} x{1}", item.comida, item.cantidad);
                                item.precioTotal = item.cantidad * item.precio;
                            }

                            Bolsa = (((int)(PrecioTotalPedido / App.EstActual.configuracion.rangoBolsas))+1) * App.EstActual.configuracion.precioBolsa;
                            if (Bolsa == 0)
                                Bolsa = App.EstActual.configuracion.precioBolsa;
                            PrecioTotalPedidoGastos = PrecioTotalPedido + Gastos + Bolsa;
                            App.DAUtil.ActualizaCarrito(Carrito2);
                        }
                        else if (c.cantidad == 1)
                        {
                            Carrito.Remove(c);
                            Carrito2.Remove(c2);
                            PrecioTotalPedido = 0;
                            precioPedido = 0;
                            foreach (var item in Carrito)
                            {
                                item.nombreCantidad = string.Format("{0} x{1}", item.comida, item.cantidad);
                                item.precioTotal = item.cantidad * item.precio;
                                PrecioTotalPedido += item.precioTotal;
                                precioPedido += item.precioTotal;
                            }
                            foreach (var item in Carrito2)
                            {
                                item.nombreCantidad = string.Format("{0} x{1}", item.comida, item.cantidad);
                                item.precioTotal = item.cantidad * item.precio;
                            }

                            Bolsa = (((int)(PrecioTotalPedido / App.EstActual.configuracion.rangoBolsas))+1) * App.EstActual.configuracion.precioBolsa;
                            if (Bolsa == 0)
                                Bolsa = App.EstActual.configuracion.precioBolsa;
                            PrecioTotalPedidoGastos = PrecioTotalPedido + Gastos + Bolsa;
                            App.DAUtil.ActualizaCarrito(Carrito2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private async void EliminarTarjeta(object parametro) 
        {
            try
            {
                bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaEliminarTarjeta, AppResources.No, AppResources.Si);
                if (result)
                {
                    var tar = (TarjetaBindableModel)parametro;
                    await App.ResponseWS.eliminaTarjeta(tar.tarjeta.id);
                    await App.ResponseWS.eliminaTarjetaPayComet(tar.tarjeta.idUser, tar.tarjeta.tokenUser);
                    App.DAUtil.BorraTarjeta(tar.tarjeta);
                    if (tar.Seleccionada)
                    {
                        App.TarjetaSeleccionada = null;
                        Preferences.Set("TarjetaSeleccionada", "");
                        TarjetaSeleccionada = null;
                    }
                    App.DAUtil.Usuario.tarjetas.Remove(tar.tarjeta);
                    Cards.Remove(tar);
                    if (Cards.Count > 0)
                    {
                        TarjetaSeleccionada = Cards[0];
                        App.TarjetaSeleccionada = Cards[0].tarjeta;
                        Preferences.Set("TarjetaSeleccionada", Cards[0].tarjeta.pan);
                    }
                    else
                    {
                        App.TarjetaSeleccionada = null;
                        TarjetaSeleccionada = null;
                        Preferences.Set("TarjetaSeleccionada", "");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
        }
        private async void ExecuteNavigateToAddCreditCardPageCommand()
        {
            if (Cards == null)
                Cards = new ObservableCollection<TarjetaBindableModel>();
            // AddCreditCardPage is now a ContentPage, use regular navigation
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddCreditCardPage(ref cards));
        }

        public void SubscribeAddCard()
        {
            MessagingCenter.Subscribe<AddCreditCardPageViewModel, TarjetaBindableModel>(this, "addCard", (s, param) =>
            {
                Cards.Add(param);
                TarjetaSeleccionada = param;
                App.TarjetaSeleccionada = param.tarjeta;
                Preferences.Set("TarjetaSeleccionada", App.TarjetaSeleccionada.pan);
            });
        }

        public void UnsubscribedAddCard()
        {
            MessagingCenter.Unsubscribe<AddCreditCardPageViewModel>(this, "addCard");
        }
        private void SeleccionaTarjetaExe(object tarjeta)
        {
            TarjetaSeleccionada = (TarjetaBindableModel)tarjeta;
        }
        #endregion

        #region Comandos
        public ICommand NavigateToAddCreditCardPageCommand { get { return new Command(ExecuteNavigateToAddCreditCardPageCommand); } }
        public ICommand EnviarPedidoCommand { get { return new Command(EnviarPedido); } }
        public ICommand AplicarCuponCommand { get { return new Command(AplicarCupon); } }
        public ICommand RestarSaldoCommand { get { return new Command(RestarSaldo); } }
        public ICommand SeleccionaTarjeta { get { return new Command(SeleccionaTarjetaExe); } }
        public ICommand ClickMasCommand { get { return new Command((parametro) => Add(parametro)); } }
        public ICommand ClickMenosCommand { get { return new Command((parametro) => remove(parametro)); } }
        public ICommand EliminarTarjetaCommand { get { return new Command((parametro) => EliminarTarjeta(parametro)); } }
        #endregion

        #region Propiedades
        private string direccionCambiada = "";
        private string dia = "";
        DateTime fecha = DateTime.Today;
        private bool cargado = false;
        static string codigoPedido;
        private Establecimiento est;
        private List<CarritoModel> Carrito2;
        private string direccion;
        private bool CuponAplicado = false;
        private CuponesSQLModel cup = null;
        public string Direccion
        {
            get { return direccion; }
            set
            {
                direccion = value;
                OnPropertyChanged(nameof(Direccion));
            }
        }
        private bool tieneEfectivo;
        public bool TieneEfectivo
        {
            get
            {
                return tieneEfectivo;
            }
            set
            {
                if (tieneEfectivo != value)
                {
                    tieneEfectivo = value;
                    OnPropertyChanged(nameof(TieneEfectivo));
                }
            }
        }
        private bool tieneBizum;
        public bool TieneBizum
        {
            get
            {
                return tieneBizum;
            }
            set
            {
                if (tieneBizum != value)
                {
                    tieneBizum = value;
                    OnPropertyChanged(nameof(TieneBizum));
                }
            }
        }
        private bool tieneDatafono;
        public bool TieneDatafono
        {
            get
            {
                return tieneDatafono;
            }
            set
            {
                if (tieneDatafono != value)
                {
                    tieneDatafono = value;
                    OnPropertyChanged(nameof(TieneDatafono));
                }
            }
        }
        private bool tieneRecogida;
        public bool TieneRecogida
        {
            get
            {
                return tieneRecogida;
            }
            set
            {
                if (tieneRecogida != value)
                {
                    tieneRecogida = value;
                    OnPropertyChanged(nameof(TieneRecogida));
                }
            }
        }
        private bool tieneEnvio;
        public bool TieneEnvio
        {
            get
            {
                return tieneEnvio;
            }
            set
            {
                if (tieneEnvio != value)
                {
                    tieneEnvio = value;
                    OnPropertyChanged(nameof(TieneEnvio));
                }
            }
        }
        private bool puebloCambiado;
        public bool PuebloCambiado
        {
            get
            {
                return puebloCambiado;
            }
            set
            {
                if (puebloCambiado != value)
                {
                    puebloCambiado = value;
                    OnPropertyChanged(nameof(PuebloCambiado));
                }
            }
        }
        private bool tieneEncargo;
        public bool TieneEncargo
        {
            get
            {
                return tieneEncargo;
            }
            set
            {
                if (tieneEncargo != value)
                {
                    tieneEncargo = value;
                    OnPropertyChanged(nameof(TieneEncargo));
                }
            }
        }
        private string poblacion;
        public string Poblacion
        {
            get
            {
                return poblacion;
            }
            set
            {
                if (poblacion != value)
                {
                    poblacion = value;
                    OnPropertyChanged(nameof(Poblacion));
                }
            }
        }
        private string idioma;
        public string Idioma
        {
            get
            {
                return idioma;
            }
            set
            {
                if (idioma != value)
                {
                    idioma = value;
                    OnPropertyChanged(nameof(Idioma));
                }
            }
        }
        private string cupon;
        public string Cupon
        {
            get
            {
                return cupon;
            }
            set
            {
                if (cupon != value)
                {
                    cupon = value;
                    OnPropertyChanged(nameof(Cupon));
                }
            }
        }
        private bool recogida;
        public bool Recogida
        {
            get
            {
                return recogida;
            }
            set
            {
                if (recogida != value)
                {
                    recogida = value;
                    OnPropertyChanged(nameof(Recogida));
                    if (Recogida)
                    {
                        Preferences.Set("TipoPedido", "Recogida");
                        Envio = false;
                        Gastos = 0;
                        PrecioTotalPedido = 0;
                        foreach (var item in Carrito2)
                        {
                            item.nombreCantidad = string.Format("{0} x{1}", item.comida, item.cantidad);
                            item.precioTotal = item.cantidad * item.precio;
                            if (item.porPuntos==0)
                                PrecioTotalPedido += item.precioTotal;
                        }
                        Bolsa = (((int)(PrecioTotalPedido / App.EstActual.configuracion.rangoBolsas))+1) * App.EstActual.configuracion.precioBolsa;
                        if (Bolsa == 0)
                            Bolsa = App.EstActual.configuracion.precioBolsa;
                        PrecioTotalPedidoGastos = PrecioTotalPedido + Gastos + Bolsa;
                    }
                    else
                    {
                        if (!Envio)
                            Recogida = true;
                    }
                }
            }
        }
        private bool envio;
        public bool Envio
        {
            get
            {
                return envio;
            }
            set
            {
                if (envio != value)
                {
                    envio = value;
                    OnPropertyChanged(nameof(Envio));
                    if (Envio)
                    {
                        /*if (App.EstActual.configuracion.activaSoloTarjeta && DateTime.Now.TimeOfDay >= App.EstActual.configuracion.horaSoloTarjetaDesde && DateTime.Now.TimeOfDay <= App.EstActual.configuracion.horaSoloTarjetaHasta)
                        {
                            TieneDatafono = false;
                        }
                        else
                        {
                            TieneDatafono = cadmin.datafono;
                        }*/
                        if (PuebloCambiado || (App.EstActual.configuracion.activaSoloTarjeta && DateTime.Now.TimeOfDay >= App.EstActual.configuracion.horaSoloTarjetaDesde && DateTime.Now.TimeOfDay <= App.EstActual.configuracion.horaSoloTarjetaHasta))
                        {
                            TieneEfectivo = false;
                            TieneBizum = false; ;
                            if (pu.especial)
                                TieneDatafono = cadmin.datafono;
                            else
                                TieneDatafono = false;
                        }
                        else
                        {
                            TieneEfectivo = cadmin.efectivo;
                            TieneBizum = cadmin.bizum;
                            TieneDatafono = cadmin.datafono;
                        }
                        Preferences.Set("TipoPedido", "Envio");
                        Recogida = false;
                        if (ZonaSeleccionda != null)
                        {
                            if (Envio && EsRepartoPropio)
                                Gastos = App.EstActual.configuracion.gastosEnvioPropio;
                            else
                                Gastos = Envio ? ZonaSeleccionda.gastos : 0;
                            PrecioTotalPedido = 0;
                            foreach (var item in Carrito2)
                            {
                                item.nombreCantidad = string.Format("{0} x{1}", item.comida, item.cantidad);
                                item.precioTotal = item.cantidad * item.precio;
                                if (item.porPuntos == 0)
                                    PrecioTotalPedido += item.precioTotal;
                            }
                            Bolsa = (((int)(PrecioTotalPedido / App.EstActual.configuracion.rangoBolsas))+1) * App.EstActual.configuracion.precioBolsa;
                            if (Bolsa == 0)
                                Bolsa = App.EstActual.configuracion.precioBolsa;
                            PrecioTotalPedidoGastos = PrecioTotalPedido + Gastos + Bolsa;
                        }
                    }
                    else
                    {
                        if (!Recogida)
                            Envio = true;
                        else
                        {
                            TieneDatafono = false;
                            if (Datafono)
                                Efectivo = true;
                        }
                    }
                }
            }
        }
        private bool tieneTarjeta;
        public bool TieneTarjeta
        {
            get
            {
                return tieneTarjeta;
            }
            set
            {
                if (tieneTarjeta != value)
                {
                    tieneTarjeta = value;
                    OnPropertyChanged(nameof(TieneTarjeta));
                }
            }
        }
        private bool efectivo;
        public bool Efectivo
        {
            get
            {
                return efectivo;
            }
            set
            {
                if (efectivo != value)
                {
                    efectivo = value;
                    OnPropertyChanged(nameof(Efectivo));
                    if (Efectivo)
                    {
                        Preferences.Set("Pago", "Efectivo");
                        Tarjeta = false;
                        Bizum = false;
                        Datafono = false;
                    }
                    if (!Bizum && !Efectivo && !Tarjeta && !Datafono)
                        Efectivo = true;
                }
            }
        }
        private bool bizum;
        public bool Bizum
        {
            get
            {
                return bizum;
            }
            set
            {
                if (bizum != value)
                {
                    bizum = value;
                    OnPropertyChanged(nameof(Bizum));
                    if (Bizum)
                    {
                        Preferences.Set("Pago", "Bizum");
                        Tarjeta = false;
                        Efectivo = false;
                        Datafono = false;
                    }
                    if (!Bizum && !Efectivo && !Tarjeta && !Datafono)
                        Bizum = true;
                }
            }
        }
        private bool datafono;
        public bool Datafono
        {
            get
            {
                return datafono;
            }
            set
            {
                if (datafono != value)
                {
                    datafono = value;
                    OnPropertyChanged(nameof(Datafono));
                    if (Datafono)
                    {
                        Preferences.Set("Pago", "Datafono");
                        Tarjeta = false;
                        Efectivo = false;
                        Bizum = false;
                    }
                    if (!Bizum && !Efectivo && !Tarjeta && !Datafono)
                        Datafono = true;
                }
            }
        }
        private bool tarjeta;
        public bool Tarjeta
        {
            get
            {
                return tarjeta;
            }
            set
            {
                if (tarjeta != value)
                {
                    tarjeta = value;
                    OnPropertyChanged(nameof(Tarjeta));
                    if (Tarjeta)
                    {
                        Preferences.Set("Pago", "Tarjeta");
                        Efectivo = false;
                        Bizum = false;
                        Datafono = false;
                    }
                    if (!Bizum && !Efectivo && !Tarjeta && !Datafono)
                        Tarjeta = true;
                }
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

        private ObservableCollection<CarritoBindable> carrito;

        public ObservableCollection<CarritoBindable> Carrito
        {
            get { return carrito; }
            set
            {
                carrito = value;
                OnPropertyChanged(nameof(Carrito));
            }
        }
        private bool tieneCodigoAmigo = false;
        public bool TieneCodigoAmigo
        {
            get { return tieneCodigoAmigo; }
            set
            {
                if (tieneCodigoAmigo != value)
                {
                    tieneCodigoAmigo = value;
                    OnPropertyChanged(nameof(TieneCodigoAmigo));
                }
            }
        }
        private double saldo = 0;
        public double Saldo
        {
            get { return saldo; }
            set
            {
                if (saldo != value)
                {
                    saldo = value;
                    OnPropertyChanged(nameof(Saldo));
                }
            }
        }
        private bool visibleSaldo = false;
        public bool VisibleSaldo
        {
            get { return visibleSaldo; }
            set
            {
                if (visibleSaldo != value)
                {
                    visibleSaldo = value;
                    OnPropertyChanged(nameof(VisibleSaldo));
                }
            }
        }
        private bool pendientePromocion = false;
        public bool PendientePromocion
        {
            get { return pendientePromocion; }
            set
            {
                if (pendientePromocion != value)
                {
                    pendientePromocion = value;
                    OnPropertyChanged(nameof(PendientePromocion));
                    App.PendientePromocion = PendientePromocion;
                }
            }
        }
        private string textoPromocion="";
        public string TextoPromocion
        {
            get { return textoPromocion; }
            set
            {
                if (textoPromocion != value)
                {
                    textoPromocion = value;
                    OnPropertyChanged(nameof(TextoPromocion));
                }
            }
        }
        private double precioPedido = 0;
        private double precioTotalPedido;

        public double PrecioTotalPedido
        {
            get { return precioTotalPedido; }
            set
            {
                precioTotalPedido = value;
                OnPropertyChanged(nameof(PrecioTotalPedido));
            }
        }

        private double precioTotalPedidoGastos;
        public double PrecioTotalPedidoGastos
        {
            get { return precioTotalPedidoGastos; }
            set
            {
                precioTotalPedidoGastos = value;
                OnPropertyChanged(nameof(PrecioTotalPedidoGastos));
            }
        }
        private double descuento;
        public double Descuento
        {
            get { return descuento; }
            set
            {
                descuento = value;
                OnPropertyChanged(nameof(Descuento));
            }
        }
        private double bolsa;
        public double Bolsa
        {
            get { return bolsa; }
            set
            {
                bolsa = value;
                OnPropertyChanged(nameof(Bolsa));
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
        private UsuarioModel usuario;

        public UsuarioModel Usuario
        {
            get { return usuario; }
            set
            {
                usuario = value;
                OnPropertyChanged(nameof(Usuario));
            }
        }

        private List<ZonaModel> zonas;
        public List<ZonaModel> Zonas
        {
            get { return zonas; }
            set
            {
                if (zonas != value)
                {
                    zonas = value;
                    OnPropertyChanged(nameof(Zonas));
                }
            }
        }

        private string observaciones;

        public string Observaciones
        {
            get { return observaciones; }
            set
            {
                observaciones = value;
                OnPropertyChanged(nameof(Observaciones));
            }
        }
        private bool cambiaDireccion;
        public bool CambiaDireccion
        {
            get
            {
                return cambiaDireccion;
            }
            set
            {
                if (cambiaDireccion != value)
                {
                    cambiaDireccion = value;
                    OnPropertyChanged(nameof(CambiaDireccion));
                }
            }
        }
        private bool modificable;
        public bool Modificable
        {
            get
            {
                return modificable;
            }
            set
            {
                if (modificable != value)
                {
                    modificable = value;
                    OnPropertyChanged(nameof(Modificable));
                }
            }
        }
        private ZonaModel zonaSeleccionda;

        public ZonaModel ZonaSeleccionda
        {
            get { return zonaSeleccionda; }
            set
            {
                try
                {
                    if (value != null)
                    {
                        zonaSeleccionda = value;
                        OnPropertyChanged(nameof(ZonaSeleccionda));
                        if (Usuario != null)
                        {
                            if (ZonaSeleccionda.cambiaDireccion == 1)
                            {
                                CambiaDireccion = true;
                                if (!string.IsNullOrEmpty(Direccion))
                                {
                                    if (Direccion.Equals(ZonaSeleccionda.direccionEnvio))
                                        Direccion = Usuario.direccion;
                                }
                            }
                            else
                            {
                                CambiaDireccion = false;
                                direccionCambiada = Direccion;
                                Direccion = ZonaSeleccionda.direccionEnvio;

                            }
                            Gastos = zonaSeleccionda.gastos;
                            PrecioTotalPedidoGastos = PrecioTotalPedido + Gastos;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 
                }
            }
        }
        private double gastos;
        public double Gastos
        {
            get
            {
                return gastos;
            }
            set
            {
                if (gastos != value)
                {
                    gastos = value;
                    OnPropertyChanged(nameof(Gastos));
                }
            }
        }
        private ObservableCollection<TarjetaBindableModel> cards;
        public ObservableCollection<TarjetaBindableModel> Cards
        {
            get
            {
                return cards;
            }
            set
            {
                if (cards != value)
                {
                    cards = value;
                    OnPropertyChanged(nameof(Cards));
                }
            }
        }
        private TarjetaBindableModel tarjetaSeleccionada;
        public TarjetaBindableModel TarjetaSeleccionada
        {
            get
            {
                return tarjetaSeleccionada;
            }
            set
            {
                if (tarjetaSeleccionada != value)
                {
                    tarjetaSeleccionada = value;
                    OnPropertyChanged(nameof(TarjetaSeleccionada));
                    if (tarjetaSeleccionada != null)
                    {
                        App.TarjetaSeleccionada = TarjetaSeleccionada.tarjeta;
                        if (Cards != null)
                        {
                            foreach (TarjetaBindableModel t in Cards)
                            {
                                if (t.tarjeta.pan.Equals(App.TarjetaSeleccionada.pan))
                                {
                                    t.fondo = "#000000";
                                    t.Seleccionada = true;
                                }
                                else
                                {
                                    t.fondo = "#FFFFFF";
                                    t.Seleccionada = false;
                                }
                            }
                            if (TarjetaSeleccionada != null)
                            {
                                Preferences.Set("TarjetaSeleccionada", TarjetaSeleccionada.tarjeta.pan);
                                App.TarjetaSeleccionada = TarjetaSeleccionada.tarjeta;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
