using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Utils;
using AsadorMoron.Print;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Syncfusion.XlsIO;
using System.IO;
using AsadorMoron.Interfaces;
using Mopups.Services;
using System.Text;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using AsadorMoron.Views.Administrador;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace AsadorMoron.ViewModels.Administrador
{
    public class HistoricoPedidosViewModelAdmin : ViewModelBase
    {
        #region Properties
        private bool todoCargado = false;
        private CancellationTokenSource _filterCts;
        private System.Timers.Timer _debounceTimer;
        public ICommand ToggleDetailCommand { get; private set; }
        int numeroDias = 1;
        public List<CabeceraPedido> ListPedidosTemp;
        private List<GastoModel> ListGastosTemp;
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
        private ObservableCollection<ZonaModel> zonas;
        public ObservableCollection<ZonaModel> Zonas
        {
            get
            {
                return zonas;
            }
            set
            {
                if (zonas != value)
                {
                    zonas = value;
                    OnPropertyChanged(nameof(Zonas));

                }
            }
        }
        private ZonaModel zonaSeleccionada;
        public ZonaModel ZonaSeleccionada
        {
            get
            {
                return zonaSeleccionada;
            }
            set
            {
                if (zonaSeleccionada != value)
                {
                    zonaSeleccionada = value;
                    OnPropertyChanged(nameof(ZonaSeleccionada));
                    Filtro();
                }
            }
        }
        private ObservableCollection<string> tiposPago;
        public ObservableCollection<string> TiposPago
        {
            get
            {
                return tiposPago;
            }
            set
            {
                if (tiposPago != value)
                {
                    tiposPago = value;
                    OnPropertyChanged(nameof(TiposPago));

                }
            }
        }
        private string tipoPago;
        public string TipoPago
        {
            get
            {
                return tipoPago;
            }
            set
            {
                if (tipoPago != value)
                {
                    tipoPago = value;
                    OnPropertyChanged(nameof(TipoPago));
                    Filtro();
                }
            }
        }
        private ObservableCollection<RepartidorModel> repartidores;
        public ObservableCollection<RepartidorModel> Repartidores
        {
            get
            {
                return repartidores;
            }
            set
            {
                if (repartidores != value)
                {
                    repartidores = value;
                    OnPropertyChanged(nameof(Repartidores));
                }
            }
        }
        private ObservableCollection<String> tipos;
        public ObservableCollection<String> Tipos
        {
            get
            {
                return tipos;
            }
            set
            {
                if (tipos != value)
                {
                    tipos = value;
                    OnPropertyChanged(nameof(Tipos));
                }
            }
        }
        private ObservableCollection<String> autos;
        public ObservableCollection<String> Autos
        {
            get
            {
                return autos;
            }
            set
            {
                if (autos != value)
                {
                    autos = value;
                    OnPropertyChanged(nameof(Autos));
                }
            }
        }

        private bool esMultiAdmin;
        public bool EsMultiAdmin
        {
            get
            {
                return esMultiAdmin;
            }
            set
            {
                if (esMultiAdmin != value)
                {
                    esMultiAdmin = value;
                    OnPropertyChanged(nameof(EsMultiAdmin));
                }
            }
        }
        private string tipoSeleccionado;
        public string TipoSeleccionado
        {
            get
            {
                return tipoSeleccionado;
            }
            set
            {
                if (tipoSeleccionado != value)
                {
                    tipoSeleccionado = value;
                    OnPropertyChanged(nameof(TipoSeleccionado));
                    Filtro();
                }
            }
        }
        private string autoSeleccionado;
        public string AutoSeleccionado
        {
            get
            {
                return autoSeleccionado;
            }
            set
            {
                if (autoSeleccionado != value)
                {
                    autoSeleccionado = value;
                    OnPropertyChanged(nameof(AutoSeleccionado));
                    Filtro();
                }
            }
        }
        private RepartidorModel repartidorSeleccionado;
        public RepartidorModel RepartidorSeleccionado
        {
            get
            {
                return repartidorSeleccionado;
            }
            set
            {
                if (repartidorSeleccionado != value)
                {
                    repartidorSeleccionado = value;
                    OnPropertyChanged(nameof(RepartidorSeleccionado));
                    Filtro();
                }
            }
        }
        private List<CabeceraPedido> listado;
        public List<CabeceraPedido> Listado
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
        private ObservableCollection<GastoModel> gastos;
        public ObservableCollection<GastoModel> Gastos
        {
            get { return gastos; }
            set
            {
                if (gastos != value)
                {
                    gastos = value;
                    OnPropertyChanged(nameof(Gastos));
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
        private double totalPedidosE = 0;
        public double TotalPedidosE
        {
            get
            {
                return totalPedidosE;
            }
            set
            {
                if (totalPedidosE != value)
                {
                    totalPedidosE = value;
                    OnPropertyChanged(nameof(TotalPedidosE));
                }
            }
        }
        private double totalGastos = 0;
        public double TotalGastos
        {
            get
            {
                return totalGastos;
            }
            set
            {
                if (totalGastos != value)
                {
                    totalGastos = value;
                    OnPropertyChanged(nameof(TotalGastos));
                }
            }
        }
        private DateTime desde;
        public DateTime Desde
        {
            get
            {
                return desde;
            }
            set
            {
                if (desde != value)
                {

                    desde = value;
                    OnPropertyChanged(nameof(Desde));
                    if (value > hasta)
                    {
                        Hasta = value;
                    }
                    else
                    {
                        Filtro();
                    }

                }
            }
        }
        private DateTime hasta;
        public DateTime Hasta
        {
            get
            {
                return hasta;
            }
            set
            {
                if (hasta != value)
                {
                    hasta = value;
                    OnPropertyChanged(nameof(Hasta));
                    if (value < desde)
                    {
                        Desde = value;
                    }
                    else
                    {
                        Filtro();
                    }

                }
            }
        }
        private double total = 0;
        public double Total
        {
            get
            {
                return total;
            }
            set
            {
                if (total != value)
                {
                    total = value;
                    OnPropertyChanged(nameof(Total));
                }
            }
        }
        #endregion
        private ObservableCollection<string> tiempos;
        public ObservableCollection<string> Tiempos
        {
            get
            {
                return tiempos;
            }
            set
            {
                if (tiempos != value)
                {
                    tiempos = value;
                    OnPropertyChanged(nameof(Tiempos));

                }
            }
        }
        private bool isDetailVisible;

        public bool IsDetailVisible
        {
            get { return isDetailVisible; }
            set
            {
                isDetailVisible = value;
                OnPropertyChanged();
            }
        }
        private string tiempoSeleccionado;
        public string TiempoSeleccionado
        {
            get
            {
                return tiempoSeleccionado;
            }
            set
            {
                if (tiempoSeleccionado != value)
                {
                    tiempoSeleccionado = value;
                    OnPropertyChanged(nameof(TiempoSeleccionado));
                    Filtro();
                }
            }
        }
        private string nombreImpresora;
        private int alturaLinea;
        public HistoricoPedidosViewModelAdmin()
        {
            ToggleDetailCommand = new Command(ToggleDetail);
        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                IsDetailVisible = false;
                App.EstActual = App.MiEst;
                App.DAUtil.EnTimer = false;
                if (App.EstActual != null)
                {
                    if (App.EstActual.configuracion == null)
                        App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);
                    nombreImpresora = App.EstActual.configuracion.nombreImpresora;
                    alturaLinea = App.EstActual.configuracion.alturaLineaImpresora;
                }
                CargarZonas();
                CargarRepartidores();
                CargarTiempos();
                SetTipos();
                InitTimer();
            }
            catch (Exception ex)
            {
                // 
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Command
        public ICommand cmdFilter { get { return new Command(Filtro); } }
        public ICommand cmdMas { get { return new Command(ExeMas); } }
        public ICommand cmdTodo { get { return new Command(ExeTodo); } }
        public ICommand cmdExport => new DelegateCommandAsync(Exportar);
        public ICommand InfoUsuarioPedidoCommand { get { return new Command(InfoUsuarioPedido); } }
        public ICommand PrintCommand { get { return new AsyncRelayCommand<string>(async (parametro) => await Print(parametro)); } }
        #endregion

        #region Methods
        private async void ExeMas()
        {
            try
            {
                App.userdialog?.ShowLoading(AppResources.Cargando);
                numeroDias++;
                await Task.Run(() => CargaPedidos());
            }
            finally
            {
                App.userdialog?.HideLoading();
            }
        }
        private async void ExeTodo()
        {
            try
            {
                App.userdialog?.ShowLoading(AppResources.Cargando);
                numeroDias = 1000;
                await Task.Run(() => CargaPedidos());
            }
            finally
            {
                App.userdialog?.HideLoading();
            }
        }
        private void SetTipos()
        {
            Tipos = new ObservableCollection<string>();
            Tipos.Add("Todos");
            Tipos.Add("Recogida Local");
            Tipos.Add("Envío Domicilio");
            TipoSeleccionado = Tipos[0];

            Autos = new ObservableCollection<string>();
            Autos.Add("Todos");
            Autos.Add("App");
            Autos.Add("Auto Pedido");
            AutoSeleccionado = Autos[0];

            TiposPago = new ObservableCollection<string>();
            TiposPago.Add("Todos");
            TiposPago.Add("Efectivo");
            //TiposPago.Add("Datafono");
            TiposPago.Add("Tarjeta");
            TipoPago = TiposPago[0];
        }
        private void Filtro()
        {
            if (!todoCargado)
                return;

            // Debounce: esperar 150ms antes de ejecutar el filtro
            _debounceTimer?.Stop();
            _debounceTimer?.Dispose();
            _debounceTimer = new System.Timers.Timer(150);
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += async (s, e) =>
            {
                await MainThread.InvokeOnMainThreadAsync(() => EjecutarFiltro());
            };
            _debounceTimer.Start();
        }

        private async void EjecutarFiltro()
        {
            // Cancelar filtro anterior si existe
            _filterCts?.Cancel();
            _filterCts = new CancellationTokenSource();
            var token = _filterCts.Token;

            try
            {
                // Capturar valores ANTES del Task.Run para evitar problemas de threading
                var desdeDate = Desde.Date;
                var hastaDate = Hasta.Date;
                var zona = ZonaSeleccionada;
                var repartidor = RepartidorSeleccionado;
                var tiempo = TiempoSeleccionado;
                var tipo = TipoSeleccionado;
                var auto = AutoSeleccionado;
                var pago = TipoPago;
                var pedidosTemp = ListPedidosTemp;
                var gastosTemp = ListGastosTemp;

                if (pedidosTemp == null)
                    return;

                // Ejecutar el filtrado en un hilo secundario
                var resultado = await Task.Run(() =>
                {
                    if (token.IsCancellationRequested)
                        return (new List<CabeceraPedido>(), new List<GastoModel>(), 0.0, 0.0, 0.0, 0);

                    // Filtrar pedidos - construir query
                    var listaPedidos = pedidosTemp.Where(p =>
                    {
                        if (p.horaPedido == null) return false;
                        var fecha = ((DateTime)p.horaPedido).Date;
                        if (fecha < desdeDate || fecha > hastaDate) return false;

                        if (zona != null && !zona.nombre.Equals("Todas") && !p.idZona.Equals(zona.idZona))
                            return false;

                        if (repartidor != null && !repartidor.nombre.Equals("Todos") && !p.idRepartidor.Equals(repartidor.id))
                            return false;

                        if (!string.IsNullOrEmpty(tiempo) && !tiempo.Equals("Todos"))
                        {
                            var hora = ((DateTime)p.horaPedido).Hour;
                            if (tiempo.Equals("Por la mañana") && hora > 17) return false;
                            if (tiempo.Equals("Por la noche") && hora <= 17) return false;
                        }

                        if (!string.IsNullOrEmpty(tipo) && !tipo.Equals("Todos"))
                        {
                            string tipoVenta = tipo.Equals("Recogida Local") ? "Recogida"
                                             : tipo.Equals("Envío Domicilio") ? "Envío" : "Local";
                            if (p.tipoVenta == null || !p.tipoVenta.Equals(tipoVenta)) return false;
                        }

                        if (!string.IsNullOrEmpty(auto) && !auto.Equals("Todos") && !auto.Equals("Todas"))
                        {
                            if (auto.Equals("Auto Pedido") && (p.nombreUsuario == null || !p.nombreUsuario.Equals("Auto Pedido")))
                                return false;
                            if (!auto.Equals("Auto Pedido") && p.nombreUsuario != null && p.nombreUsuario.Equals("Auto Pedido"))
                                return false;
                        }

                        if (!string.IsNullOrEmpty(pago) && !pago.Equals("Todos"))
                        {
                            if (p.tipoPago == null || !p.tipoPago.Equals(pago)) return false;
                        }

                        return true;
                    }).ToList();

                    if (token.IsCancellationRequested)
                        return (new List<CabeceraPedido>(), new List<GastoModel>(), 0.0, 0.0, 0.0, 0);

                    // Filtrar gastos
                    var listaGastos = gastosTemp != null ? gastosTemp.Where(p =>
                    {
                        if (p.fecha == null) return false;
                        var fecha = ((DateTime)p.fecha).Date;
                        if (fecha < desdeDate || fecha > hastaDate) return false;

                        if (repartidor != null && !repartidor.nombre.Equals("Todos") && !p.idRepartidor.Equals(repartidor.id))
                            return false;

                        if (!string.IsNullOrEmpty(tiempo) && !tiempo.Equals("Todos"))
                        {
                            var hora = ((DateTime)p.fecha).Hour;
                            if (tiempo.Equals("Por la mañana") && hora > 17) return false;
                            if (tiempo.Equals("Por la noche") && hora <= 17) return false;
                        }

                        return true;
                    }).ToList() : new List<GastoModel>();

                    // Calcular totales
                    double totalPedidosE = listaPedidos.Sum(p => p.precioTotalPedido);
                    double totalGastos = listaGastos.Sum(p => p.precio);

                    return (listaPedidos, listaGastos, totalPedidosE + totalGastos, totalPedidosE, totalGastos, listaPedidos.Count);
                }, token);

                if (token.IsCancellationRequested)
                    return;

                // Actualizar UI
                Listado = resultado.Item1;
                Gastos = new ObservableCollection<GastoModel>(resultado.Item2);
                Total = resultado.Item3;
                TotalPedidosE = resultado.Item4;
                TotalGastos = resultado.Item5;
                TotalPedidos = resultado.Item6;
            }
            catch (OperationCanceledException)
            {
                // Ignorar
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en Filtro: {ex.Message}");
            }
        }
        private void CargarTiempos()
        {
            try
            {
                List<string> listTiempos = new List<string>();
                listTiempos.Add("Todos");
                listTiempos.Add("Por la mañana");
                listTiempos.Add("Por la noche");
                Tiempos = new ObservableCollection<string>(listTiempos);
                TiempoSeleccionado = Tiempos[0];
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async void InfoUsuarioPedido(object codigo)
        {
            try
            {
                if (MopupService.Instance.PopupStack.Count() == 0)
                {
                    App.userdialog?.ShowLoading(AppResources.Cargando);
                    string cod = (string)codigo;
                    CabeceraPedido c2 = await Task.Run(() => ResponseServiceWS.TraePedidoPorCodigo(cod));
                    App.userdialog?.HideLoading();
                    await MopupService.Instance.PushAsync(new PopupPageInfoUsuarioPedido(c2), true);
                }
            }
            catch (Exception ex)
            {
                App.userdialog?.HideLoading();
            }
        }
        private void InitTimer()
        {
            try
            {
                Desde = DateTime.Now;
                Hasta = DateTime.Now;
                App.DAUtil.pedidoNuevo = false;
                CargaPedidos();
                CargaGastos();
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void CargaPedidos()
        {
            ListPedidosTemp = new List<CabeceraPedido>(ResponseServiceWS.getHistoricoPedidosMultiAdmin(numeroDias, "1"));
            TotalPedidos = 0;

            if (ListPedidosTemp != null && ListPedidosTemp.Count > 0)
            {
                List<CabeceraPedido> Listado2 = new List<CabeceraPedido>(ListPedidosTemp.Where(p => ((DateTime)p.horaPedido).Date == DateTime.Now.Date));
                TotalPedidos = Listado2.Count();


                Listado = new List<CabeceraPedido>(Listado2);
                if (Listado.Count > 0)
                {
                    Total = Listado.Sum(p => p.precioTotalPedido);
                }
            }
            todoCargado = true;
            Filtro();
        }
        private void CargaGastos()
        {
            ListGastosTemp = new List<GastoModel>(ResponseServiceWS.getHistoricoGastosMultiAdmin(numeroDias, "1"));

            if (ListGastosTemp != null && ListGastosTemp.Count > 0)
            {
                List<GastoModel> Listado2 = new List<GastoModel>(ListGastosTemp.Where(p => ((DateTime)p.fecha).Date == DateTime.Now.Date));

                Gastos = new ObservableCollection<GastoModel>(Listado2);
                if (Listado.Count > 0)
                {
                    Total += Gastos.Sum(p => p.precio);
                }
            }
            todoCargado = true;
            Filtro();
        }
        private void CargarZonas()
        {
            try
            {
                List<ZonaModel> listZonas = App.ResponseWS.getListadoZonas(1);
                List<ZonaModel> zs = new List<ZonaModel>();
                ZonaModel zz = new ZonaModel();
                zz.idZona = 0;
                zz.nombre = "Todas";
                zs.Add(zz);
                foreach (ZonaModel e in listZonas)
                {
                    zs.Add(e);
                }
                Zonas = new ObservableCollection<ZonaModel>(zs);
                ZonaSeleccionada = Zonas[0];
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void CargarRepartidores()
        {
            try
            {
                List<RepartidorModel> listRep = App.ResponseWS.ListadoRepartidoresMultiAdmin("1");
                List<RepartidorModel> zs = new List<RepartidorModel>();
                RepartidorModel zz = new RepartidorModel();
                zz.id = 0;
                zz.nombre = "Todos";
                zs.Add(zz);
                foreach (RepartidorModel e in listRep)
                {
                    zs.Add(e);
                }
                Repartidores = new ObservableCollection<RepartidorModel>(zs);
                RepartidorSeleccionado = Repartidores[0];
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task Print(string codigo)
        {
            try
            {
                if (DeviceInfo.Platform.ToString() == "WinUI")
                    await PrintUWP(codigo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Print:" + ex.Message);
                // 
            }
        }
        private async Task PrintUWP(string codigo)
        {
            try
            {
                if (string.IsNullOrEmpty(nombreImpresora))
                    await App.customDialog.ShowDialogAsync(AppResources.ImpresoraNoConfigurada, AppResources.App, AppResources.Cerrar);
                else
                {
                    AsadorMoron.Print.Printer printer = new AsadorMoron.Print.Printer(nombreImpresora, codigo, "ISO-8859-1");
                    printer.ImprimirTicketPedido(alturaLinea);
                    printer.PrintDocument();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Print UWP:" + ex.Message);
                // 
            }
        }

        async Task Exportar()
        {
            try
            {
                var result = await App.userdialog.ActionSheetAsync(AppResources.ActionInforme, AppResources.Cancelar, null, null, AppResources.PorEstablecimientos, AppResources.PorZonas);
                if (result.Equals(AppResources.PorEstablecimientos))
                    await ExportarEstablecimiento();
                else if (result.Equals(AppResources.PorZonas))
                    await ExportarZona();
            }
            catch (Exception ex)
            {
                // 
            }
        }
        async Task ExportarEstablecimiento()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageWrite>();

                }

                status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageRead>();

                }
            }
            catch (Exception)
            {
                //...
            }
            try
            {
                App.DAUtil.ExportandoExcel = true;
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    Syncfusion.XlsIO.IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Excel2016;

                    //Create a workbook with a worksheet
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];
                    string est = "";
                    int j = 0;
                    int i = 2;
                    string pedido = "";
                    ConfiguracionEstablecimiento conf = new ConfiguracionEstablecimiento();
                    foreach (CabeceraPedido f in Listado.OrderBy(p => p.idEstablecimiento))
                    {

                        if (!est.Equals(f.nombreEstablecimiento))
                        {
                            est = f.nombreEstablecimiento;

                            conf = ResponseServiceWS.getConfiguracionEstablecimiento(f.idEstablecimiento);
                            if (j > 0)
                                workbook.Worksheets.Create();
                            worksheet = workbook.Worksheets[j];
                            worksheet.Name = f.nombreEstablecimiento;
                            //Enter values to the cells from A3 to A5
                            worksheet.Range["A1"].Text = AppResources.Fecha;
                            worksheet.Range["B1"].Text = AppResources.Codigo;
                            worksheet.Range["C1"].Text = AppResources.Pedido;
                            worksheet.Range["D1"].Text = AppResources.Importe;
                            worksheet.Range["E1"].Text = AppResources.Beneficios;

                            //Make the text bold
                            worksheet.Range["A1:E1"].CellStyle.Font.Bold = true;
                            i = 2;
                            worksheet.Range["A" + i].Text = ((DateTime)f.horaPedido).ToString("dd/MM/yyyy HH:mm:ss");
                            worksheet.Range["B" + i].Text = f.codigoPedido;
                            pedido = "";
                            worksheet.Range["C" + i].Text = pedido;
                            worksheet.Range["D" + i].Text = f.precioTotalPedido.ToString();
                            worksheet.Range["E" + i].Text = (f.precioTotalPedido * conf.comision / 100).ToString();
                            worksheet.Range["A2"].AutofitColumns();
                            worksheet.Range["B2"].AutofitColumns();
                            worksheet.Range["C2"].AutofitColumns();
                            worksheet.Range["D2"].AutofitColumns();
                            worksheet.Range["E2"].AutofitColumns();
                            i++;
                            j++;
                        }
                        else
                        {
                            worksheet.Range["A" + i].Text = ((DateTime)f.horaPedido).ToString("dd/MM/yyyy HH:mm:ss");
                            worksheet.Range["B" + i].Text = f.codigoPedido;
                            pedido = "";
                            worksheet.Range["C" + i].Text = pedido;
                            worksheet.Range["D" + i].Text = f.precioTotalPedido.ToString();
                            worksheet.Range["E" + i].Text = (f.precioTotalPedido * conf.comision / 100).ToString();
                            i++;
                        }
                        /*worksheet.Range["C" + i].Text = f.TipoIncidencia.Tipo;
                        worksheet.Range["D" + i].Text = f.FechaPicada.ToString();
                        worksheet.Range["E" + i].Text = f.FechaOriginal.ToString();
                        i += 1;*/
                    }
                    //worksheet.Im(listaFiltrada, 0, 0, false);

                    //Save the workbook to stream in xlsx format. 
                    MemoryStream stream = new MemoryStream();
                    workbook.SaveAs(stream);

                    workbook.Close();

                    //Save the stream as a file in the device and invoke it for viewing
                    Microsoft.Maui.Controls.DependencyService.Get<ISave>().SaveAndView("Pedidos.xlsx", "application/msexcel", stream);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        async Task ExportarZona()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageWrite>();

                }

                status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageRead>();

                }
            }
            catch (Exception)
            {
                //...
            }
            try
            {
                ConfiguracionAdmin cadmin = ResponseServiceWS.getConfiguracionAdmin();
                App.DAUtil.ExportandoExcel = true;
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    Syncfusion.XlsIO.IApplication application = excelEngine.Excel;

                    application.DefaultVersion = ExcelVersion.Excel2016;

                    //Create a workbook with a worksheet
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];
                    int est = 0;
                    int j = 0;
                    int i = 2;
                    string pedido = "";
                    foreach (CabeceraPedido f in Listado.OrderBy(p => p.idZona))
                    {
                        if (!est.Equals(f.idZona))
                        {
                            est = f.idZona;
                            if (j > 0)
                                workbook.Worksheets.Create();
                            worksheet = workbook.Worksheets[j];
                            worksheet.Name = Zonas.Where(p => p.idZona == est).FirstOrDefault<ZonaModel>().nombre;
                            //Enter values to the cells from A3 to A5
                            worksheet.Range["A1"].Text = AppResources.Fecha;
                            worksheet.Range["B1"].Text = AppResources.Codigo;
                            worksheet.Range["C1"].Text = AppResources.Pedido;
                            worksheet.Range["D1"].Text = AppResources.Importe;

                            //Make the text bold
                            worksheet.Range["A1:E1"].CellStyle.Font.Bold = true;
                            i = 2;
                            worksheet.Range["A" + i].Text = ((DateTime)f.horaPedido).ToString("dd/MM/yyyy HH:mm:ss");
                            worksheet.Range["B" + i].Text = f.codigoPedido;
                            pedido = "";
                            worksheet.Range["C" + i].Text = pedido;
                            worksheet.Range["D" + i].Text = f.precioTotalPedido.ToString();
                            worksheet.Range["A2"].AutofitColumns();
                            worksheet.Range["B2"].AutofitColumns();
                            worksheet.Range["C2"].AutofitColumns();
                            worksheet.Range["D2"].AutofitColumns();
                            i++;
                            j++;
                        }
                        else
                        {
                            worksheet.Range["A" + i].Text = ((DateTime)f.horaPedido).ToString("dd/MM/yyyy HH:mm:ss");
                            worksheet.Range["B" + i].Text = f.codigoPedido;
                            pedido = "";
                            worksheet.Range["C" + i].Text = pedido;
                            worksheet.Range["D" + i].Text = f.precioTotalPedido.ToString();
                            i++;
                        }






                        /*worksheet.Range["C" + i].Text = f.TipoIncidencia.Tipo;
                        worksheet.Range["D" + i].Text = f.FechaPicada.ToString();
                        worksheet.Range["E" + i].Text = f.FechaOriginal.ToString();
                        i += 1;*/
                    }
                    //worksheet.Im(listaFiltrada, 0, 0, false);

                    //Save the workbook to stream in xlsx format. 
                    MemoryStream stream = new MemoryStream();
                    workbook.SaveAs(stream);

                    workbook.Close();

                    //Save the stream as a file in the device and invoke it for viewing
                    Microsoft.Maui.Controls.DependencyService.Get<ISave>().SaveAndView("Pedidos.xlsx", "application/msexcel", stream);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        #endregion
        private void ToggleDetail()
        {
            IsDetailVisible = !IsDetailVisible;
        }
    }
}
