using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.Utils;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Views.Clientes;
using AsadorMoron.Views.Establecimientos;
using Mopups.Services;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Clientes
{
    public class HistoricoPedidosViewModelClient : ViewModelBase
    {
        public HistoricoPedidosViewModelClient()
        {

        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;

                initTimer();
                cargarEstablecimientos();
            }
            catch (Exception ex)
            {
                // 
            }
            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Comandos
        public ICommand InfoUsuarioCommand { get { return new Command(InfoUsuario); } }
        public ICommand InfoPedidoCommand { get { return new Command(InfoPedido); } }
        public ICommand RepetirPedidoCommand { get { return new Command(RepetirPedido); } }
        #endregion

        #region Metodos

        private void InfoUsuario(object codigo)
        {
            try
            {
                if (MopupService.Instance.PopupStack.Count() == 0)
                {
                    string cod = (string)codigo;
                    CabeceraPedido c2 = Listado.Where(p => p.codigoPedido == cod).FirstOrDefault<CabeceraPedido>();
                    ZonaModel z = App.DAUtil.getZonas().Find(p => p.idZona == c2.idZona);
                    string nombrezona = string.Empty;
                    if (z != null)
                        nombrezona = z.nombre;
                    MopupService.Instance.PushAsync(new PopupPageInfoUsuario(c2.nombreUsuario, c2.emailUsuario, c2.telefonoUsuario, c2.direccionUsuario, nombrezona), true);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void InfoPedido(object codigo)
        {
            try
            {
                if (MopupService.Instance.PopupStack.Count() == 0)
                {
                    string cod = (string)codigo;
                    CabeceraPedido c2 = Listado.Where(p => p.codigoPedido == cod).FirstOrDefault<CabeceraPedido>();
                    MopupService.Instance.PushAsync(new PopupPageInfoPedidoCliente(c2), true);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void RepetirPedido(object obj)
        {
            try
            {
                bool continuar = true;
                int idPedido = (int)obj;
                CabeceraPedido cabeceraPedido = ListPedidosTemp.Where(p => p.idPedido == idPedido).FirstOrDefault();
                ObservableCollection<LineasPedido> lineasPedidos = cabeceraPedido.lineasPedidos;
                List<CarritoModel> carritoModels = new List<CarritoModel>();
                foreach (var item in lineasPedidos)
                {
                    int cantidadIngredientes = Regex.Matches(item.nombreProducto, " x ").Count;
                    int cantidadActual = App.ResponseWS.getCantidadIngredientes(item.idProducto);
                    if (cantidadIngredientes != cantidadActual && cantidadActual>0)
                    {
                        continuar = false;
                        App.customDialog.ShowDialogAsync("No se puede repetir este pedido, por que no cumple con el número de ingredientes", AppResources.App, AppResources.Cerrar);
                    }else if (!item.nombreProducto.Contains("GASTOS") && !item.nombreProducto.Contains("Bolsa") && !item.nombreProducto.ToUpper().Contains("DESCUENTO"))
                    {
                        CarritoModel carritoModel = new CarritoModel();
                        carritoModel.cantidad = item.cantidad;
                        carritoModel.comida = item.nombreProducto;
                        carritoModel.idArticulo = item.idProducto;
                        carritoModel.idEstablecimiento = cabeceraPedido.idEstablecimiento;
                        carritoModel.imagen = item.imagenProducto;
                        carritoModel.observaciones = cabeceraPedido.comentario;
                        carritoModel.precio = item.precio;
                        carritoModels.Add(carritoModel);
                    }
                }
                if (continuar)
                {
                    List<ArticuloModel> articulos;
                    string ids = "";
                    foreach (var item in lineasPedidos)
                    {
                        if (!item.nombreProducto.Contains("GASTOS") && !item.nombreProducto.Contains("Bolsa") && !item.nombreProducto.ToUpper().Contains("DESCUENTO"))
                        {
                            if (!ids.Equals(""))
                                ids += ",";
                            ids += item.idProducto;
                        }
                    }
                    articulos = ResponseServiceWS.TraePodructosBaja(ids);
                    foreach (ArticuloModel a in articulos)
                    {
                        carritoModels.Remove(carritoModels.Find(p => p.idArticulo == a.idArticulo));
                    }
                    if (carritoModels.Count > 0)
                    {
                        try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            App.DAUtil.GuardaCarrito(carritoModels);
                                await App.DAUtil.NavigationService.NavigateToAsync<DetallePedidoViewModel>(carritoModels);
                           
                        });
                    }
                }
                else
                    carritoModels.Clear();
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void Filtro()
        {
            try
            {
                if ((string.IsNullOrEmpty(EstablecimientoSeleccionado) || EstablecimientoSeleccionado.Equals(AppResources.Todos)))
                {
                    if (ListPedidosTemp != null)
                    {
                        List<CabeceraPedido> l = ListPedidosTemp.Where(p => ((DateTime)p.horaPedido).Date >= (Desde.Date) && ((DateTime)p.horaPedido).Date <= (Hasta.Date)).ToList();
                        if (l != null)
                        {
                            Listado = new ObservableCollection<CabeceraPedido>(l);
                        }
                        TotalPedidos = Listado.Count;
                    }
                }
                else
                {
                    if (ListPedidosTemp != null)
                    {
                        List<CabeceraPedido> l = new List<CabeceraPedido>();
                        if (EstablecimientoSeleccionado.Equals(AppResources.Todos))
                        {
                            l = ListPedidosTemp.Where(p => ((DateTime)p.horaPedido).Date >= (Desde.Date) && ((DateTime)p.horaPedido).Date <= (Hasta.Date)).ToList();
                        }
                        else
                        {
                            l = ListPedidosTemp.Where(p => p.nombreEstablecimiento.Equals(EstablecimientoSeleccionado) && ((DateTime)p.horaPedido).Date >= (Desde.Date) && ((DateTime)p.horaPedido).Date <= (Hasta.Date)).ToList();
                        }

                        if (l != null)
                        {
                            Listado = new ObservableCollection<CabeceraPedido>(l);
                        }
                        TotalPedidos = Listado.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                // 
            }

        }
        private void initTimer()
        {
            try
            {
                Desde = DateTime.Now.AddDays(-7);
                Hasta = DateTime.Now;
                App.DAUtil.pedidoNuevo = false;
                Listado = new ObservableCollection<CabeceraPedido>();
                Listado2 = new List<CabeceraPedido>();
                ListPedidosTemp = new ObservableCollection<CabeceraPedido>(ResponseServiceWS.getPedidoUsuarios().Where(p=>!p.tipoVenta.Equals("Local")).OrderByDescending(p => p.horaPedido).ToList());
                TotalPedidos = 0;

                string ids = "";
                if (ListPedidosTemp != null && ListPedidosTemp.Count > 0)
                {
                    foreach (CabeceraPedido item in ListPedidosTemp)
                    {
                        int cab = 0;
                        if (!ids.Equals(""))
                            ids += ",";
                        ids += item.idPedido.ToString();
                        cab = Listado2.Where(obj => obj.idPedido == item.idPedido).Count();
                        if (cab == 0)
                        {
                            item.ColorPedido = "#f90314";
                            Listado2.Add(item);
                        }
                    }
                    foreach (CabeceraPedido item in Listado2)
                    {
                        item.lineasPedidos[item.lineasPedidos.Count - 1].IsLatest = true;

                        switch (item.idEstadoPedido)
                        {
                            case (int)EstadoPedido.Nuevo:
                                item.ColorPedido = "#000000";
                                foreach (LineasPedido l in item.lineasPedidos)
                                    l.ColorPedido = "#000000";
                                break;
                            case (int)EstadoPedido.EnProceso:
                                item.ColorPedido = "#4fa9d2";
                                foreach (LineasPedido l in item.lineasPedidos)
                                    l.ColorPedido = "#4fa9d2";
                                break;
                            case (int)EstadoPedido.PorRecoger:
                                item.ColorPedido = "#ff0506";
                                foreach (LineasPedido l in item.lineasPedidos)
                                    l.ColorPedido = "#ff0506";
                                break;
                            case (int)EstadoPedido.Recogido:
                                item.ColorPedido = "#0fa046";
                                foreach (LineasPedido l in item.lineasPedidos)
                                    l.ColorPedido = "#0fa046";
                                break;
                            case (int)EstadoPedido.Entregado:
                                item.ColorPedido = "#4d4fa0";
                                foreach (LineasPedido l in item.lineasPedidos)
                                    l.ColorPedido = "#4d4fa0";
                                break;
                        }
                    }
                    TotalPedidos = Listado2.Count();

                    foreach (var items in Listado2)
                    {
                        double result = 0;
                        foreach (var item in items.lineasPedidos)
                        {
                            result += item.cantidad * item.precio;
                        }
                        items.precioTotalPedido = result;
                    }

                    Listado = new ObservableCollection<CabeceraPedido>(Listado2);
                    //ResponseServiceWS.SetPedidoCero(ids);
                    OnPropertyChanged(nameof(Listado));

                }
            }
            catch (Exception ex)
            {
                // 
            }



        }
        private void cargarEstablecimientos()
        {
            try
            {
                List<Establecimiento> l = ResponseServiceWS.getListadoEstablecimientos();
                List<string> ests = new List<string>();
                ests.Add(AppResources.Todos);
                foreach (Establecimiento e in l)
                {
                    ests.Add(e.nombre);
                }
                Establecimientos = new ObservableCollection<string>(ests);
                EstablecimientoSeleccionado = Establecimientos[0];
            }
            catch (Exception ex)
            {
                // 
            }
        }

        #endregion

        #region Propiedades

        private ObservableCollection<CabeceraPedido> listPedido;

        public ObservableCollection<CabeceraPedido> ListPedido
        {
            get { return listPedido; }
            set
            {
                listPedido = value;
                OnPropertyChanged(nameof(ListPedido));
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

        private List<CabeceraPedido> listado2;
        public List<CabeceraPedido> Listado2
        {
            get { return listado2; }
            set
            {
                if (listado2 != value)
                {
                    listado2 = value;
                    OnPropertyChanged(nameof(Listado2));
                }
            }
        }

        private ObservableCollection<CabeceraPedido> listPedidoTemp;

        public ObservableCollection<CabeceraPedido> ListPedidosTemp
        {
            get { return listPedidoTemp; }
            set
            {
                listPedidoTemp = value;
                OnPropertyChanged(nameof(ListPedidosTemp));
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
        private string establecimientoSeleccionado;
        public string EstablecimientoSeleccionado
        {
            get
            {
                return establecimientoSeleccionado;
            }
            set
            {
                if (establecimientoSeleccionado != value)
                {
                    establecimientoSeleccionado = value;
                    OnPropertyChanged(nameof(EstablecimientoSeleccionado));
                    Filtro();
                }
            }
        }
        private ObservableCollection<string> establecimientos;

        public ObservableCollection<string> Establecimientos
        {
            get { return establecimientos; }
            set
            {
                establecimientos = value;
                OnPropertyChanged(nameof(Establecimientos));
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

        #endregion
    }
}
