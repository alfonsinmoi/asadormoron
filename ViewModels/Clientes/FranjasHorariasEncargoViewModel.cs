using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Interfaces;
// 
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
    public class FranjasHorariasEncargoViewModel : ViewModelBase
    {
        #region propiedades

        private int tipo = 0;
        private bool activoHoy;
        private double Gastos;
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
        private DateTime minDate;
        public DateTime MinDate
        {
            get
            {
                return minDate;
            }
            set
            {
                if (minDate != value)
                {
                    minDate = value;
                    OnPropertyChanged(nameof(MinDate));
                }
            }
        }
        private DateTime maxDate;
        public DateTime MaxDate
        {
            get
            {
                return maxDate;
            }
            set
            {
                if (maxDate != value)
                {
                    maxDate = value;
                    OnPropertyChanged(nameof(MaxDate));
                }
            }
        }
        private ObservableCollection<DateTime> blackDates;
        public ObservableCollection<DateTime> BlackDates
        {
            get
            {
                return blackDates;
            }
            set
            {
                if (blackDates != value)
                {
                    blackDates = value;
                    OnPropertyChanged(nameof(BlackDates));
                }
            }
        }
        private DateTime fechaSeleccionada = DateTime.Now.Date;
        public DateTime FechaSeleccionada
        {
            get
            {
                return fechaSeleccionada;
            }
            set
            {
                if (fechaSeleccionada != value)
                {
                    fechaSeleccionada = value;
                    OnPropertyChanged(nameof(FechaSeleccionada));
                    if (FechaSeleccionada != null)
                    {
                        if (FechaSeleccionada >= MinDate && fechaSeleccionada <= MaxDate)
                        {
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
                                        ZonaModel z = App.ResponseWS.getListadoZonas(Preferences.Get("idPueblo", 0)).Where(p => p.idZona == Carrito2[0].idZona).FirstOrDefault();
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
                        }
                        else
                        {
                            Listado = new List<FranjaHorariaModel>();
                            FranjaSeleccionada = null;
                        }
                    }
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
                    franjaSeleccionada.Color = "#000000";
                    if (Listado != null)
                    {
                        foreach (FranjaHorariaModel f in (Listado))
                        {
                            if (f.horaInicio != franjaSeleccionada.horaInicio)
                                f.Color = "#fef400";
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

        public FranjasHorariasEncargoViewModel() { }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                Carrito2 = navigationData as List<CarritoModel>;
                if (Carrito2[0].tipo == 1)
                    Texto =AppResources.SeleccionFechaRecibir;
                else if (Carrito2[0].tipo == 2)
                {
                    Texto = AppResources.SeleccionFechaRecoger;
                    tipoVenta = "Recogida";
                }
                if (App.EstActual.configuracion == null)
                    App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);
                MinDate = DateTime.Today.AddDays(App.EstActual.configuracion.encargosDiasDesde);
                MaxDate = DateTime.Today.AddDays(App.EstActual.configuracion.encargosDiasHasta);

                bool con = true;
                int i = 0;
                DateTime miFecha = MinDate;
                do
                {
                    if (!CompruebaFecha(miFecha))
                    {
                        BlackDates.Add(miFecha);
                        if (miFecha == minDate)
                            MinDate.AddDays(1);

                        MaxDate.AddDays(1);
                    }
                    miFecha=miFecha.AddDays(1);
                    i++;
                    if (miFecha > MaxDate || i>30)
                        con = false;
                } while (con);

                if (i <= 30)
                {
                    FechaSeleccionada = MinDate;
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

        TimeSpan inicioMan;
        TimeSpan finMan;
        TimeSpan inicioTarde;
        TimeSpan finTarde;

        private bool CompruebaFecha(DateTime miFecha)
        {
            switch (miFecha.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return (App.EstActual.configuracion.activoLunes || App.EstActual.configuracion.activoLunesTarde);
                case DayOfWeek.Tuesday:
                   return (App.EstActual.configuracion.activoMartes || App.EstActual.configuracion.activoMartesTarde);
                case DayOfWeek.Wednesday:
                    return (App.EstActual.configuracion.activoMiercoles || App.EstActual.configuracion.activoMiercolesTarde);
                case DayOfWeek.Thursday:
                    return (App.EstActual.configuracion.activoJueves || App.EstActual.configuracion.activoJuevesTarde);
                case DayOfWeek.Friday:
                    return (App.EstActual.configuracion.activoViernes || App.EstActual.configuracion.activoViernesTarde);
                case DayOfWeek.Saturday:
                    return (App.EstActual.configuracion.activoSabado || App.EstActual.configuracion.activoSabadoTarde);
                case DayOfWeek.Sunday:
                    return (App.EstActual.configuracion.activoDomingo || App.EstActual.configuracion.activoDomingoTarde);
            }
            return false;
        }
        private List<FranjaHorariaModel> CargaFranjas()
        {
            List<FranjaHorariaModel> resultado = new List<FranjaHorariaModel>();
            try
            {
                ConfiguracionAdmin cadmin = ResponseServiceWS.getConfiguracionAdmin();
                activoHoy = false;
                if (cadmin.servicioActivo && App.EstActual.servicioActivo)
                {
                    bool activoMan = false;
                    bool activoTarde = false;
                    switch (FechaSeleccionada.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            activoMan = App.EstActual.configuracion.activoLunes;
                            activoTarde = App.EstActual.configuracion.activoLunesTarde;
                            inicioMan = App.EstActual.configuracion.inicioLunes;
                            finMan = App.EstActual.configuracion.finLunes;
                            inicioTarde = App.EstActual.configuracion.inicioLunesTarde;
                            finTarde = App.EstActual.configuracion.finLunesTarde;
                            break;
                        case DayOfWeek.Tuesday:
                            activoMan = App.EstActual.configuracion.activoMartes;
                            activoTarde = App.EstActual.configuracion.activoMartesTarde;
                            inicioMan = App.EstActual.configuracion.inicioMartes;
                            finMan = App.EstActual.configuracion.finMartes;
                            inicioTarde = App.EstActual.configuracion.inicioMartesTarde;
                            finTarde = App.EstActual.configuracion.finMartesTarde;
                            break;
                        case DayOfWeek.Wednesday:
                            activoMan = App.EstActual.configuracion.activoMiercoles;
                            activoTarde = App.EstActual.configuracion.activoMiercolesTarde;
                            inicioMan = App.EstActual.configuracion.inicioMiercoles;
                            finMan = App.EstActual.configuracion.finMiercoles;
                            inicioTarde = App.EstActual.configuracion.inicioMiercolesTarde;
                            finTarde = App.EstActual.configuracion.finMiercolesTarde;
                            break;
                        case DayOfWeek.Thursday:
                            activoMan = App.EstActual.configuracion.activoJueves;
                            activoTarde = App.EstActual.configuracion.activoJuevesTarde;
                            inicioMan = App.EstActual.configuracion.inicioJueves;
                            finMan = App.EstActual.configuracion.finJueves;
                            inicioTarde = App.EstActual.configuracion.inicioJuevesTarde;
                            finTarde = App.EstActual.configuracion.finJuevesTarde;
                            break;
                        case DayOfWeek.Friday:
                            activoMan = App.EstActual.configuracion.activoViernes;
                            activoTarde = App.EstActual.configuracion.activoViernesTarde;
                            inicioMan = App.EstActual.configuracion.inicioViernes;
                            finMan = App.EstActual.configuracion.finViernes;
                            inicioTarde = App.EstActual.configuracion.inicioViernesTarde;
                            finTarde = App.EstActual.configuracion.finViernesTarde;
                            break;
                        case DayOfWeek.Saturday:
                            activoMan = App.EstActual.configuracion.activoSabado;
                            activoTarde = App.EstActual.configuracion.activoSabadoTarde;
                            inicioMan = App.EstActual.configuracion.inicioSabado;
                            finMan = App.EstActual.configuracion.finSabado;
                            inicioTarde = App.EstActual.configuracion.inicioSabadoTarde;
                            finTarde = App.EstActual.configuracion.finSabadoTarde;
                            break;
                        case DayOfWeek.Sunday:
                            activoMan = App.EstActual.configuracion.activoDomingo;
                            activoTarde = App.EstActual.configuracion.activoDomingoTarde;
                            inicioMan = App.EstActual.configuracion.inicioDomingo;
                            finMan = App.EstActual.configuracion.finDomingo;
                            inicioTarde = App.EstActual.configuracion.inicioDomingoTarde;
                            finTarde = App.EstActual.configuracion.finDomingoTarde;
                            break;
                    }
                    bool seguir = true;
                    do
                    {

                        if ((inicioMan <= finMan && activoMan && Carrito2[0].tipo == 1) || (inicioMan <= finMan && activoMan && Carrito2[0].tipo != 1))
                        {
                            TimeSpan ts = inicioMan.Add(new TimeSpan(0, 59, 59));
                            DateTime d = new DateTime(FechaSeleccionada.Year, FechaSeleccionada.Month, FechaSeleccionada.Day, inicioMan.Hours, inicioMan.Minutes, 0);
                            DateTime d2 = new DateTime(FechaSeleccionada.Year, FechaSeleccionada.Month, FechaSeleccionada.Day, ts.Hours, ts.Minutes, 0);
                            if (d >= DateTime.Now)
                            {
                                if (App.ResponseWS.compruebaFranjaReparto(App.EstActual.idEstablecimiento, d.ToString("yyyy-MM-dd HH:mm:ss"), d2.ToString("yyyy-MM-dd HH:mm:ss"), App.EstActual.configuracion.encargosPorHora))
                                {
                                    FranjaHorariaModel f = new FranjaHorariaModel();
                                    f.horaInicioReal = inicioMan;
                                    f.horaInicio = inicioMan.ToString(@"hh\:mm");
                                    f.horaFin = inicioMan.Add(new TimeSpan(0, 60, 0)).ToString(@"hh\:mm");
                                    f.Color = "#fef400";
                                    resultado.Add(f);
                                    activoHoy = true;
                                }
                            }
                            inicioMan = inicioMan.Add(new TimeSpan(0, 60, 0));
                        }
                        else
                            seguir = false;
                    } while (seguir);

                    seguir = true;
                    do
                    {
                        int extra = 0;
                        switch (FechaSeleccionada.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                extra = cadmin.extraLunes;
                                break;
                            case DayOfWeek.Tuesday:
                                extra = cadmin.extraMartes;
                                break;
                            case DayOfWeek.Wednesday:
                                extra = cadmin.extraMiercoles;
                                break;
                            case DayOfWeek.Thursday:
                                extra = cadmin.extraJueves;
                                break;
                            case DayOfWeek.Friday:
                                extra = cadmin.extraViernes;
                                break;
                            case DayOfWeek.Saturday:
                                extra = cadmin.extraSabado;
                                break;
                            case DayOfWeek.Sunday:
                                extra = cadmin.extraDomingo;
                                break;
                        }
                        if ((inicioTarde.Subtract(TimeSpan.FromMinutes(extra)) <= finTarde && activoTarde && Carrito2[0].tipo == 1) || (inicioTarde.Subtract(TimeSpan.FromMinutes(extra)) <= finTarde && activoTarde && Carrito2[0].tipo != 1))
                        {
                            TimeSpan ts = inicioTarde.Add(new TimeSpan(0, 59, 59)); ;
                            DateTime d = new DateTime(FechaSeleccionada.Year, FechaSeleccionada.Month, FechaSeleccionada.Day, inicioTarde.Hours, inicioTarde.Minutes, 0);
                            DateTime d2 = new DateTime(FechaSeleccionada.Year, FechaSeleccionada.Month, FechaSeleccionada.Day, ts.Hours, ts.Minutes, 0);
                            if (d >= DateTime.Now)
                            {
                                if (resultado.Where(p => p.horaInicio.Equals(inicioTarde.ToString(@"hh\:mm"))).Count() == 0)
                                {
                                    if (App.ResponseWS.compruebaFranjaReparto(App.EstActual.idEstablecimiento, d.ToString("yyyy-MM-dd HH:mm:ss"), d2.ToString("yyyy-MM-dd HH:mm:ss"), App.EstActual.configuracion.encargosPorHora))
                                    {
                                        FranjaHorariaModel f = new FranjaHorariaModel();
                                        f.horaInicioReal = inicioTarde;
                                        f.horaInicio = inicioTarde.ToString(@"hh\:mm");
                                        f.horaFin = inicioTarde.Add(new TimeSpan(0, 60, 0)).ToString(@"hh\:mm");
                                        f.Color = "#fef400";
                                        resultado.Add(f);
                                        activoHoy = true;
                                    }

                                }
                            }
                            inicioTarde = inicioTarde.Add(new TimeSpan(0, 60, 0));

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
                    await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("no_disponible_est")).FirstOrDefault<MensajesModel>().valor.Replace("{1}", Environment.NewLine), AppResources.Informacion,AppResources.Cerrar);
                }
                else if (Listado.Count == 0)
                {
                    await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("sin_horas")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", Environment.NewLine), AppResources.Informacion, AppResources.Cerrar);
                }
                else
                {
                    if (Preferences.Get("Pago", "Efectivo").Equals("Efectivo") || Preferences.Get("Pago", "Efectivo").Equals("Bizum"))
                    {
                        if (tipo == 1)
                        {
                            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, App.MensajesGlobal.Where(p => p.clave.Equals("realizar_pedido")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (PrecioTotalPedido + Gastos).ToString("#0.00")).Replace("{1}", App.EstActual.nombre).Replace("{2}", Environment.NewLine), AppResources.No, AppResources.Si);
                            await HacerPedido(result);
                        }
                        else
                        {
                            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, App.MensajesGlobal.Where(p => p.clave.Equals("realizar_pedido_recogida")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (PrecioTotalPedido + Gastos).ToString("#0.00")).Replace("{1}", App.EstActual.nombre).Replace("{2}", Environment.NewLine), AppResources.No, AppResources.Si);
                            await HacerPedido(result);
                        }
                    }
                    else if (Preferences.Get("Pago", "Efectivo").Equals("Tarjeta"))
                    {
                        bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, App.MensajesGlobal.Where(p => p.clave.Equals("realizar_pedido_tarjeta")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (PrecioTotalPedido + Gastos).ToString("#0.00")).Replace("{1}", App.EstActual.nombre).Replace("{2}", Environment.NewLine), AppResources.No, AppResources.Si);
                        await HacerPedidoTarjeta(result);
                    }
                    else if (Preferences.Get("Pago", "Efectivo").Equals("Datafono"))
                    {
                        if (tipo == 1)
                        {
                            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, App.MensajesGlobal.Where(p => p.clave.Equals("realizar_pedido_datafono")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (PrecioTotalPedido + Gastos).ToString("#0.00")).Replace("{1}", App.EstActual.nombre).Replace("{2}", Environment.NewLine), AppResources.No, AppResources.Si);
                            await HacerPedido(result);
                        }
                        else
                        {
                            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, App.MensajesGlobal.Where(p => p.clave.Equals("realizar_pedido_recogida")).FirstOrDefault<MensajesModel>().valor.Replace("{0}", (PrecioTotalPedido + Gastos).ToString("#0.00")).Replace("{1}", App.EstActual.nombre).Replace("{2}", Environment.NewLine), AppResources.No, AppResources.Si);
                            await HacerPedido(result);
                        }
                    }
                    else
                        await App.customDialog.ShowDialogAsync(AppResources.SeleccioneMetodoPago,AppResources.App, AppResources.Cerrar);
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
                    int puntosParaEmail = 0;
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
                            total += l20.cantidad * l20.precio;
                        }
                    }
                    LineasPedido l2 = new LineasPedido();
                    l2.cantidad = 1;
                    l2.idProducto = 0;
                    l2.pagadoConPuntos = 0;
                    l2.precio = Gastos;
                    l2.nombreProducto = "";
                    l2.tipoComida = 1;
                    l.Add(l2);
                    total += l2.cantidad * l2.precio;
                    if (!tipoVenta.Contains("(Enc.)"))
                        tipoVenta += " (Enc.)";

                    int idCodigoPedido = ResponseServiceWS.NuevoPedido("0", 0, "", tipoVenta, App.DAUtil.Usuario.idUsuario, Carrito2[0].idEstablecimiento, codigoPedido, Carrito2[0].idZona, Carrito2[0].direccion, Carrito2[0].observaciones.Trim(), FechaSeleccionada.ToString("yyyy-MM-dd") + " " + FranjaSeleccionada.horaInicioReal.ToString(@"hh\:mm"), l, "", tipo, Preferences.Get("Pago", "Efectivo"),punt);

                    if (idCodigoPedido > 0)
                    {
                        string pedido = string.Empty;

                        List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(App.EstActual.idPueblo);
                            foreach (TokensModel to in tokens)
                                await App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Encargo para " + App.EstActual.nombre + ": " + codigoPedido, to.token);

                        List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(App.EstActual.idEstablecimiento);
                        foreach (TokensModel to in tokens2)
                            await App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Encargo para " + App.EstActual.nombre + ": " + codigoPedido, to.token);

                        List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(App.EstActual.idEstablecimiento);
                        foreach (TokensModel to in tokens3)
                            await App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Encargo: " + codigoPedido, to.token);



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
                        IrAPedidoConfirmadoViewModel();
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
                        if (App.EstActual.configuracion.repartoPropio && App.EstActual.configuracion.repartoPolloAndaluz)
                        {
                            if (App.EstActual.idPueblo == App.DAUtil.Usuario.idPueblo)
                            {
                                if (App.EstActual.configuracion.preferenciaReparto.Equals("P"))
                                    tipoVenta = "Reparto Propio";
                            }
                        }
                        else if (App.EstActual.configuracion.repartoPropio && !App.EstActual.configuracion.repartoPolloAndaluz)
                            tipoVenta = "Reparto Propio";
                    }

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



                    CabeceraPedido p = new CabeceraPedido();
                    p.idUsuario = App.DAUtil.Usuario.idUsuario;
                    p.idEstablecimiento = Carrito2[0].idEstablecimiento;
                    p.horaEntrega = FechaSeleccionada.ToString("yyyy-MM-dd") + " " + FranjaSeleccionada.horaInicioReal.ToString(@"hh\:mm");
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
                    PagoConTarjeta(total, punt);
                }
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
            }
        }

        private void IrAPedidoConfirmadoViewModel()
        {
            App.DAUtil.Idioma = "ES";
            App.DAUtil.NavigationService.NavigateToAsyncMenu<PedidoConfirmadoViewModel>(codigoPedido + ";" + FranjaSeleccionada.horaInicio + ";" + tipo + ";" + App.EstActual.nombre);
        }

        private async void PagoConTarjeta(double total,int punt)
        {
            Preferences.Set("EsPedidoLocal", false);
            Preferences.Set("EsPedidoComercio", false);
            Preferences.Set("totalPedido", ((int)(total * 100)).ToString());
            App.urlChallengue = await App.ResponseWS.realizaPagoTarjeta(App.TarjetaSeleccionada.idUser, App.TarjetaSeleccionada.tokenUser, ((int)(total * 100)).ToString(), App.pedidoEnCurso.codigoPedido);
            if (App.urlChallengue.StartsWith("http"))
                await App.DAUtil.NavigationService.NavigateToAsyncWithoutMenu<WebViewModel>();
            else if (App.urlChallengue.Equals(""))
            {
                int idCodigoPedido = ResponseServiceWS.NuevoPedido(punt);
                if (idCodigoPedido > 0)
                {
                    App.pedidoEnCurso.idPedido = idCodigoPedido;

                    List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(App.EstActual.idPueblo);
                    foreach (TokensModel to in tokens)
                        App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + App.pedidoEnCurso.codigoPedido, to.token);

                    List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(App.EstActual.idEstablecimiento);
                    foreach (TokensModel to in tokens2)
                        App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + App.pedidoEnCurso.codigoPedido, to.token);

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
                await ResponseServiceWS.eliminaPedido(App.pedidoEnCurso.idPedido);
                App.userdialog.HideLoading();
                await App.customDialog.ShowDialogAsync(App.urlChallengue, "PolloAndaluz", AppResources.OK);
            }
        }


        #endregion
    }
}
