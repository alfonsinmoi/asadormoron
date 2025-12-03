using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Interfaces;
// 
using AsadorMoron;
using AsadorMoron.Models;
using AsadorMoron.Models.PayComet;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Clientes
{
    public class FranjasHorariasViewModel : ViewModelBase
    {
        #region propiedades

        private int tipo = 0;
        private bool activoHoy;
        private double Gastos;
        private double Bolsas=0;
        static string codigoPedido;
        private List<CarritoModel> Carrito2;
        private string tipoVenta = "Envío";
        private List<FranjaHorariaModel> listado;
        public List<FranjaHorariaModel> Listado
        {
            get
            {
                return listado;
            }
            set
            {
                if (listado != value)
                {
                    listado = value;
                    OnPropertyChanged(nameof(Listado));
                }
            }
        }
        private string texto;
        public string Texto
        {
            get
            {
                return texto;
            }
            set
            {
                if (texto != value)
                {
                    texto = value;
                    OnPropertyChanged(nameof(Texto));
                }
            }
        }
        private FranjaHorariaModel franjaSeleccionada;
        public FranjaHorariaModel FranjaSeleccionada
        {
            get
            {
                return franjaSeleccionada;
            }
            set
            {
                if (franjaSeleccionada != value)
                {
                    franjaSeleccionada = value;
                    franjaSeleccionada.Color = "#f0dd49";
                    if (Listado != null)
                    {
                        foreach (FranjaHorariaModel f in (Listado))
                        {
                            if (f.horaInicio != franjaSeleccionada.horaInicio)
                                f.Color = "#000000";
                        }
                    }
                    OnPropertyChanged(nameof(FranjaSeleccionada));
                }
            }
        }

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

        #endregion

        public FranjasHorariasViewModel() { }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                Carrito2 = navigationData as List<CarritoModel>;
                if (Carrito2[0].tipo == 1)
                    Texto = AppResources.SeleccionHoraRecibir;
                else if (Carrito2[0].tipo == 2)
                {
                    Texto = AppResources.SeleccionHoraRecoger;
                    tipoVenta = "Recogida";
                }
                Listado = new List<FranjaHorariaModel>(CargaFranjas());
                PrecioTotalPedido = 0;
                if (Listado.Count > 0)
                {
                    foreach (var item in Carrito2)
                    {
                        item.nombreCantidad = string.Format("{0} x{1}", item.comida, item.cantidad);
                        item.precioTotal = item.cantidad * item.precio;
                        PrecioTotalPedido += item.precioTotal;
                    }
                    try
                    {
                        tipo = Carrito2[0].tipo;
                        if (Carrito2[0].tipo == 1)
                        {
                            ZonaModel z = App.ResponseWS.getListadoZonas(Preferences.Get("idPueblo", 1)).Where(p => p.idZona == Carrito2[0].idZona).FirstOrDefault();
                            Gastos = z.gastos;
                        }
                        else
                            Gastos = 0;
                    }
                    catch (Exception)
                    {
                        Gastos = 1.90;
                    }
                    PrecioTotalPedidoGastos = PrecioTotalPedido + Gastos;
                }
                else
                {
                    await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("sin_horas")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                    await App.DAUtil.NavigationService.NavigateBackAsync();
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
        public ICommand EnviarPedidoCommand { get { return new Command(EnviarPedido); } }
        #endregion

        #region Métodos

        private void calculaFechas(TimeSpan gIni, TimeSpan gFin, TimeSpan eIni, TimeSpan eFin, bool man)
        {
            try
            {
                if (gIni <= eIni && gFin >= eFin)
                {
                    if (man)
                    {
                        inicioMan = eIni;
                        finMan = eFin;
                    }
                    else
                    {
                        inicioTarde = eIni;
                        finTarde = eFin;
                    }
                }
                else if (gIni >= eIni && gFin >= eFin)
                {
                    if (man)
                    {
                        inicioMan = gIni;
                        finMan = eFin;
                    }
                    else
                    {
                        inicioTarde = gIni;
                        finTarde = eFin;
                    }
                }
                else if (gIni <= eIni && gFin <= eFin)
                {
                    if (man)
                    {
                        inicioMan = eIni;
                        finMan = gFin;
                    }
                    else
                    {
                        inicioTarde = eIni;
                        finTarde = gFin;
                    }
                }
                else if (gIni >= eIni && gFin <= eFin)
                {
                    if (man)
                    {
                        inicioMan = gIni;
                        finMan = gFin;
                    }
                    else
                    {
                        inicioTarde = gIni;
                        finTarde = gFin;
                    }
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        TimeSpan inicioMan;
        TimeSpan finMan;
        TimeSpan inicioTarde;
        TimeSpan finTarde;

        private List<FranjaHorariaModel> CargaFranjas()
        {
            List<FranjaHorariaModel> resultado = new List<FranjaHorariaModel>();
            try
            {
                activoHoy = false;

                if (App.EstActual.servicioActivo)
                {
                    inicioMan = (TimeSpan)App.EstActual.inicioMan;
                    finMan = (TimeSpan)App.EstActual.finMan;
                    inicioTarde = (TimeSpan)App.EstActual.inicioTarde;
                    finTarde = ((TimeSpan)App.EstActual.finTarde);
                    if (inicioMan < DateTime.Now.TimeOfDay)
                        inicioMan = DateTime.Now.AddMinutes(1).TimeOfDay;
                    int ultimoMinuto = int.Parse(inicioMan.Minutes.ToString().Substring(inicioMan.Minutes.ToString().Length - 1));
                    if (ultimoMinuto >= 1 && ultimoMinuto <= 4)
                        inicioMan=inicioMan.Add(new TimeSpan(0, 5 - ultimoMinuto, 0));
                    else if (ultimoMinuto >= 6 && ultimoMinuto <= 9)
                        inicioMan=inicioMan.Add(new TimeSpan(0, 10 - ultimoMinuto, 0));

                    if (inicioTarde < DateTime.Now.TimeOfDay)
                        inicioTarde = DateTime.Now.AddMinutes(1).TimeOfDay;
                    ultimoMinuto = int.Parse(inicioTarde.Minutes.ToString().Substring(inicioTarde.Minutes.ToString().Length - 1));
                    if (ultimoMinuto >= 1 && ultimoMinuto <= 4)
                        inicioTarde=inicioTarde.Add(new TimeSpan(0, 5 - ultimoMinuto, 0));
                    else if (ultimoMinuto >= 6 && ultimoMinuto <= 9)
                        inicioTarde=inicioTarde.Add(new TimeSpan(0, 10 - ultimoMinuto, 0));
                    bool seguir = true;
                    do
                    {

                        if (inicioMan <= finMan && App.EstActual.activoMan == 1)
                        {
                            TimeSpan ts = inicioMan.Add(new TimeSpan(0, 14, 59));
                            DateTime d = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, inicioMan.Hours, inicioMan.Minutes, 0);
                            DateTime d2 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, ts.Hours, ts.Minutes, 0);
                            if (d >= DateTime.Now)
                            {
                                    FranjaHorariaModel f = new FranjaHorariaModel();
                                    f.horaInicioReal = inicioMan;
                                    f.horaInicio = inicioMan.Add(TimeSpan.FromMinutes(App.EstActual.tiempoEntrega)).ToString(@"hh\:mm");
                                    f.horaFin = inicioMan.Add(TimeSpan.FromMinutes(App.EstActual.tiempoEntrega)).Add(new TimeSpan(0, 15, 0)).ToString(@"hh\:mm");
                                    f.Color = "#000000";
                                    resultado.Add(f);
                                    activoHoy = true;
                            }
                            inicioMan = inicioMan.Add(new TimeSpan(0, 15, 0));
                        }
                        else
                            seguir = false;
                    } while (seguir);

                    seguir = true;
                    do
                    {
                        int extra = 0;
                        if ((inicioTarde.Subtract(TimeSpan.FromMinutes(extra)) <= finTarde && App.EstActual.activoTarde == 1))
                        {
                            TimeSpan ts = inicioTarde.Add(new TimeSpan(0, 14, 59)); ;
                            DateTime d = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, inicioTarde.Hours, inicioTarde.Minutes, 0);
                            DateTime d2 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, ts.Hours, ts.Minutes, 0);
                            if (d >= DateTime.Now)
                            {
                                if (resultado.Where(p => p.horaInicio.Equals(inicioTarde.Add(TimeSpan.FromMinutes(App.EstActual.tiempoEntrega)).ToString(@"hh\:mm"))).Count() == 0)
                                {
                                    FranjaHorariaModel f = new FranjaHorariaModel();
                                    f.horaInicioReal = inicioTarde;
                                    f.horaInicio = inicioTarde.Add(TimeSpan.FromMinutes(App.EstActual.tiempoEntrega)).ToString(@"hh\:mm");
                                    f.horaFin = inicioTarde.Add(TimeSpan.FromMinutes(App.EstActual.tiempoEntrega)).Add(new TimeSpan(0, 15, 0)).ToString(@"hh\:mm");
                                    f.Color = "#000000";
                                    resultado.Add(f);
                                    activoHoy = true;
                                }

                            }
                            inicioTarde = inicioTarde.Add(new TimeSpan(0, 15, 0));

                        }
                        else
                            seguir = false;
                    } while (seguir);
                }
            }
            catch (Exception ex)
            {
                // 
            }
            return resultado;
        }

        private async void EnviarPedido(object obj)
        {
            try
            {
                if (FranjaSeleccionada == null)
                {
                    if (tipo == 1)
                        await App.customDialog.ShowDialogAsync(AppResources.SellecioneHoraEntrega, AppResources.App, AppResources.Cerrar);
                    else
                        await App.customDialog.ShowDialogAsync(AppResources.SeleccionHoraRecoger, AppResources.App, AppResources.Cerrar);
                }
                else if (!activoHoy)
                {
                    await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("no_disponible_est")).FirstOrDefault<MensajesModel>().valor.Replace("{1}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                }
                else if (Listado.Count == 0)
                {
                    await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("sin_horas")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                    await App.DAUtil.NavigationService.InitializeAsync();
                }
                else
                {
                    if (Preferences.Get("Pago", "Efectivo").Equals("Efectivo") || Preferences.Get("Pago", "Efectivo").Equals("Bizum"))
                    {
                        if (tipo == 1)
                        {
                            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, App.MensajesGlobal.Where(p => p.clave.Equals("realizar_pedido")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (PrecioTotalPedido + Bolsas + Gastos - App.Descuento).ToString("#0.00")).Replace("{1}", App.EstActual.nombre).Replace("{2}", Environment.NewLine), AppResources.No, AppResources.Si);
                            await HacerPedido(result);
                        }
                        else
                        {
                            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, App.MensajesGlobal.Where(p => p.clave.Equals("realizar_pedido_recogida")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (PrecioTotalPedido + Bolsas + Gastos - App.Descuento).ToString("#0.00")).Replace("{1}", App.EstActual.nombre).Replace("{2}", Environment.NewLine), AppResources.No, AppResources.Si);
                            await HacerPedido(result);
                        }
                    }
                    else if (Preferences.Get("Pago", "Efectivo").Equals("Tarjeta"))
                    {
                        bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, App.MensajesGlobal.Where(p => p.clave.Equals("realizar_pedido_tarjeta")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (PrecioTotalPedido + Bolsas + Gastos - App.Descuento).ToString("#0.00")).Replace("{1}", App.EstActual.nombre).Replace("{2}", Environment.NewLine), AppResources.No, AppResources.Si);
                        await HacerPedidoTarjeta(result);
                    }
                    else if (Preferences.Get("Pago", "Efectivo").Equals("Datafono"))
                    {
                        if (tipo == 1)
                        {
                            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, App.MensajesGlobal.Where(p => p.clave.Equals("realizar_pedido_datafono")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (PrecioTotalPedido + Bolsas + Gastos - App.Descuento).ToString("#0.00")).Replace("{1}", App.EstActual.nombre).Replace("{2}", Environment.NewLine), AppResources.No, AppResources.Si);
                            await HacerPedido(result);
                        }
                        else
                        {
                            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, App.MensajesGlobal.Where(p => p.clave.Equals("realizar_pedido_recogida")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (PrecioTotalPedido + Bolsas + Gastos - App.Descuento).ToString("#0.00")).Replace("{1}", App.EstActual.nombre).Replace("{2}", Environment.NewLine), AppResources.No, AppResources.Si);
                            await HacerPedido(result);
                        }
                    }
                    else
                        await App.customDialog.ShowDialogAsync(AppResources.SeleccioneMetodoPago, AppResources.App, AppResources.Cerrar);
                }
            }
            catch (Exception ex)
            {
                // 
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
                    double total = 0;
                    if (string.IsNullOrEmpty(Carrito2[0].observaciones))
                        Carrito2[0].observaciones = "";
                    codigoPedido = App.DAUtil.GetCodigo();
                    List<LineasPedido> l = new List<LineasPedido>();
                    int punt = 0;
                    int puntosAQuitar = 0;
                    double Bolsa = ((int)(PrecioTotalPedido / App.EstActual.configuracion.rangoBolsas)) * App.EstActual.configuracion.precioBolsa;
                    if (Bolsa == 0)
                        Bolsa = App.EstActual.configuracion.precioBolsa;
                    LineasPedido l2 = new LineasPedido();
                    l2.cantidad = ((int)(PrecioTotalPedido / App.EstActual.configuracion.rangoBolsas));
                    if (l2.cantidad == 0)
                        l2.cantidad = 1;
                    l2.idProducto = 0;
                    l2.precio = Bolsa / l2.cantidad;
                    l2.nombreProducto = "Bolsa";
                    l2.tipoComida = 4;
                    l2.pagadoConPuntos = 0;
                    l.Add(l2);
                    total += Bolsa;
                    Bolsas = Bolsa;
                    foreach (var item in Carrito2)
                    {
                        if (item.cantidad > 0)
                        {
                            LineasPedido l20 = new LineasPedido();
                            l20.cantidad = item.cantidad;
                            l20.idProducto = item.idArticulo;
                            l20.precio = item.precio;
                            l20.tipoComida = 0;
                            l20.comentario = item.comentario;
                            l20.pagadoConPuntos = item.porPuntos;
                            if (item.porPuntos == 0)
                                punt += item.puntos;
                            else if (item.porPuntos == 1)
                                puntosAQuitar += item.puntos;
                            l20.nombreProducto = item.comida;
                            l.Add(l20);
                            total += l20.cantidad * l20.precio;
                        }
                    }
                    l2 = new LineasPedido();
                    l2.cantidad = 1;
                    l2.pagadoConPuntos = 0;
                    l2.idProducto = 0;
                    l2.precio = Gastos;
                    l2.nombreProducto = "";
                    l2.tipoComida = 1;
                    l.Add(l2);
                    total += l2.cantidad * l2.precio;

                    if (App.Descuento > 0)
                    {
                        l2 = new LineasPedido();
                        l2.cantidad = 1;
                        l2.idProducto = 0;
                        l2.precio = App.Descuento * -1;
                        l2.nombreProducto = "";
                        l2.tipoComida = 3;
                        l2.pagadoConPuntos = 0;
                        l.Add(l2);
                        total -= App.Descuento;
                    }
                    if (tipoVenta.Equals("Envío"))
                    {
                        if (App.EstActual.configuracion == null)
                            App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);
                    }
                    int idCodigoPedido = ResponseServiceWS.NuevoPedido("0", 0, "", tipoVenta, App.DAUtil.Usuario.idUsuario, Carrito2[0].idEstablecimiento, codigoPedido, Carrito2[0].idZona, Carrito2[0].direccion, Carrito2[0].observaciones.Trim(), DateTime.Today.ToString("yyyy-MM-dd") + " " + FranjaSeleccionada.horaInicioReal.ToString(@"hh\:mm"), l, "", tipo, Preferences.Get("Pago", "Efectivo"), puntosAQuitar);

                    if (idCodigoPedido > 0 || DeviceInfo.Platform.ToString() == "WinUI")
                    {
                        string pedido = string.Empty;
                        if (tipoVenta.Equals("Envío"))
                        {
                            List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(1);
                            foreach (TokensModel to in tokens)
                                await App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + codigoPedido, to.token);

                            List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(App.EstActual.idEstablecimiento);
                            foreach (TokensModel to in tokens2)
                                await App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + codigoPedido, to.token);
                        }

                        List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(App.EstActual.idEstablecimiento);
                        foreach (TokensModel to in tokens3)
                            await App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido: " + codigoPedido, to.token);

                        if (App.EstActual.configuracion.puntosPorPedido > 0)
                            punt = App.EstActual.configuracion.puntosPorPedido;
                        else if (App.EstActual.configuracion.puntosPorEuro > 0)
                            punt = App.EstActual.configuracion.puntosPorEuro * ((int)total);
                        await EnviarEmail(punt.ToString());

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
                            p.observaciones = item.observaciones.Trim();
                            p.precio = item.precio;
                            p.cantidad = item.cantidad;
                            p.comida = item.comida;
                            p.precioTotal = item.precioTotal;
                            p.fechaPedido = fechaPedido;
                            p.nombreEstablecimiento = App.EstActual.nombre;
                            p.horaEntrega = FranjaSeleccionada.horaInicio;
                            pedidosModel.Add(p);
                        }
                        App.DAUtil.GuardarPedido(pedidosModel);
                        App.DAUtil.VaciaCarrito();
                        if (DeviceInfo.Platform.ToString() != "WinUI")
                            IrAPedidoConfirmadoViewModel();
                        else
                            await App.DAUtil.NavigationService.InitializeAsync();
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
        private async Task EnviarEmail(string puntos)
        {
                try
                {
                string body = "Hola " + App.DAUtil.Usuario.nombre + Environment.NewLine + "Gracias por tu pedido. Acabas de ganar " + puntos + " puntos que se sumar a tu cuenta";
                List<string> lista = new List<string>();
                    App.DAUtil.EnviaEmail(App.DAUtil.Usuario.email, "Confirmación de tu pedido en ASADOR MORÓN", body);
            }
                catch (Exception)
                {
                    await App.customDialog.ShowDialogAsync(AppResources.ErrorEnvioEmail, AppResources.App, AppResources.Cerrar);
                }
        }
        private async Task HacerPedidoTarjeta(bool hacer)
        {
            try
            {
                if (hacer)
                {
                    App.userdialog.ShowLoading(AppResources.Cargando);
                    await Task.Delay(200);
                    if (tipoVenta.Equals("Envío"))
                    {
                        if (App.EstActual.configuracion == null)
                            App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);
                        
                    }

                    double total = 0;
                    if (string.IsNullOrEmpty(Carrito2[0].observaciones))
                        Carrito2[0].observaciones = "";
                    codigoPedido = App.DAUtil.GetCodigo();
                    List<LineasPedido> l = new List<LineasPedido>();
                    int punt = 0;
                    int puntosAQuitar = 0;
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
                    Bolsas = Bolsa;
                    foreach (var item in Carrito2)
                    {
                        if (item.cantidad > 0)
                        {
                            LineasPedido l20 = new LineasPedido();
                            l20.cantidad = item.cantidad;
                            l20.idProducto = item.idArticulo;
                            l20.precio = item.precio;
                            l20.tipoComida = 0;
                            l20.comentario = item.comentario;
                            l20.pagadoConPuntos = item.porPuntos;
                            if (item.porPuntos == 0)
                                punt += item.puntos;
                            else if (item.porPuntos == 1)
                                puntosAQuitar += item.puntos;
                            l20.nombreProducto = item.comida;
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
                    if (App.Descuento > 0)
                    {
                        l2 = new LineasPedido();
                        l2.cantidad = 1;
                        l2.idProducto = 0;
                        l2.precio = App.Descuento * -1;
                        l2.nombreProducto = "";
                        l2.tipoComida = 3;
                        l2.pagadoConPuntos = 0;
                        l.Add(l2);
                        total -= App.Descuento;
                    }
                    CabeceraPedido p = new CabeceraPedido();
                    p.idUsuario = App.DAUtil.Usuario.idUsuario;
                    p.idEstablecimiento = Carrito2[0].idEstablecimiento;
                    p.horaEntrega = DateTime.Today.ToString("yyyy-MM-dd") + " " + FranjaSeleccionada.horaInicioReal.ToString(@"hh\:mm");
                    p.horaPedido = DateTime.Now;
                    p.horaEntrega2 = DateTime.Today.AddSeconds(TimeSpan.Parse(FranjaSeleccionada.horaInicio).TotalSeconds);
                    p.codigoPedido = codigoPedido;
                    p.idZona = Carrito2[0].idZona;
                    p.direccionUsuario = Carrito2[0].direccion;
                    p.tipo = tipo;
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
                    PagoConTarjeta(total,punt,puntosAQuitar);
                    /*int idCodigoPedido = ResponseServiceWS.NuevoPedido();
                    if (idCodigoPedido > 0)
                    {
                        App.pedidoEnCurso.idPedido = idCodigoPedido;
                        PagoConTarjeta(total);
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
                    }*/
                }
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
            }
            finally
            {
                App.userdialog.HideLoading();
            }
        }

        private void IrAPedidoConfirmadoViewModel()
        {
            App.DAUtil.Idioma = "ES";
            App.DAUtil.NavigationService.NavigateToAsyncMenu<PedidoConfirmadoViewModel>(codigoPedido + ";" + FranjaSeleccionada.horaInicio + ";" + tipo + ";" + App.EstActual.nombre);

        }

        private async void PagoConTarjeta(double total,int punt,int puntosAQuitar)
        {
            Preferences.Set("EsPedidoLocal", false);
            Preferences.Set("EsPedidoComercio", false);
            Preferences.Set("totalPedido", ((int)(total * 100)).ToString());
            App.urlChallengue = await App.ResponseWS.realizaPagoTarjeta(App.TarjetaSeleccionada.idUser, App.TarjetaSeleccionada.tokenUser, ((int)(total * 100)).ToString(), App.pedidoEnCurso.codigoPedido);
            if (App.urlChallengue.StartsWith("http"))
                await App.DAUtil.NavigationService.NavigateToAsyncWithoutMenu<WebViewModel>();
            else if (App.urlChallengue.Equals(""))
            {
                int idCodigoPedido = ResponseServiceWS.NuevoPedido(puntosAQuitar);
                if (idCodigoPedido > 0)
                {
                    App.pedidoEnCurso.idPedido = idCodigoPedido;
                    if (App.pedidoEnCurso.tipoVenta.StartsWith("Envío"))
                    {
                        List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(1);
                        foreach (TokensModel to in tokens)
                            App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + App.pedidoEnCurso.codigoPedido, to.token);

                        List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(App.EstActual.idEstablecimiento);
                        foreach (TokensModel to in tokens2)
                            App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + App.pedidoEnCurso.codigoPedido, to.token);
                    }

                    List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(App.EstActual.idEstablecimiento);
                    foreach (TokensModel to in tokens3)
                        App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido: " + App.pedidoEnCurso.codigoPedido, to.token);

                    if (App.EstActual.configuracion.puntosPorPedido > 0)
                        punt = App.EstActual.configuracion.puntosPorPedido;
                    else if (App.EstActual.configuracion.puntosPorEuro > 0)
                        punt = App.EstActual.configuracion.puntosPorEuro * ((int)total);
                    await EnviarEmail(punt.ToString());

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
                    await App.DAUtil.NavigationService.NavigateToAsyncWithoutMenu<PedidoConfirmadoViewModel>(App.pedidoEnCurso.codigoPedido + ";" + App.pedidoEnCurso.horaEntrega2.ToString("HH:mm") + ";" + App.pedidoEnCurso.tipo + ";" + App.EstActual.nombre);

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
                await App.customDialog.ShowDialogAsync(App.urlChallengue, "Asador Morón", AppResources.OK);
            }
        }


        #endregion
    }
}
