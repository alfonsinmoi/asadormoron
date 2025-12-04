// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
// 
using AsadorMoron.Services;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using CommunityToolkit.Mvvm.Input;

namespace AsadorMoron.ViewModels.Clientes
{
    public class CartaProductosViewModel : ViewModelBase
    {
        public CartaProductosViewModel()
        {
            App.entradoEnCarta = false;
        }
        List<Comida> originalComidas;
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                Logado = App.DAUtil.Usuario != null;
                App.DAUtil.EnTimer = false;
                carrito = App.DAUtil.Getcarrito();
                Cantidad = "0";
                Idioma = App.idioma;
                int ca = 0;
                foreach (CarritoModel c in carrito)
                {
                    ca += c.cantidad;
                }
                Cantidad = ca.ToString();

                SistemaPuntos = App.EstActual.configuracion.sistemaPuntos;
                if (SistemaPuntos)
                {
                    Puntos = ResponseServiceWS.getPuntosEstablecimiento();
                    foreach (CarritoModel c in carrito.Where(p => p.porPuntos == 1).ToList())
                    {
                        Puntos -= c.puntos;
                    }
                }
                else
                    Puntos = 0;

                if (_categoria == null)
                {
                    _categoria = (Categoria)navigationData;
                    EsPorPuntos = _categoria.esPuntos;
                    App.esPorPuntos = EsPorPuntos;
                    System.Diagnostics.Debug.WriteLine($"[CartaProductos] Cargando productos para categoría: {_categoria.id} - {_categoria.nombre}");
                    List<ArticuloModel> listaServer = await App.ResponseWS.getListadoProductosEstablecimientoCat(_categoria.id, false);
                    System.Diagnostics.Debug.WriteLine($"[CartaProductos] Productos del servidor: {listaServer?.Count ?? 0}");
                    List<ArticuloModel> listaSQLite = await App.DAUtil.getProductosEstablecimientoCat(_categoria.id);
                    System.Diagnostics.Debug.WriteLine($"[CartaProductos] Productos de SQLite: {listaSQLite?.Count ?? 0}");
                    // Usar los datos del servidor si existen, si no los de SQLite
                    List<ArticuloModel> lista = (listaServer != null && listaServer.Count > 0)
                        ? listaServer.FindAll(p => p.estado == 1 && p.estadoCategoria == 1)
                        : listaSQLite.FindAll(p => p.estado == 1 && p.estadoCategoria == 1);
                    System.Diagnostics.Debug.WriteLine($"[CartaProductos] Productos filtrados (estado=1): {lista?.Count ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"[CartaProductos] aceptaEncargos: {App.EstActual.configuracion.aceptaEncargos}");
                    if (App.EstActual.configuracion.aceptaEncargos)
                    {
                        App.listaProductos = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1);
                        lista = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1);
                    }
                    else
                    {
                        App.listaProductos = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1 && p.porEncargo == false);
                        lista = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1 && p.porEncargo == false);
                    }
                    System.Diagnostics.Debug.WriteLine($"[CartaProductos] Productos después filtro vistaEnvios: {lista?.Count ?? 0}");
                    if (App.EstActual.horario.Equals(AppResources.Cerrado))
                        ModoTienda = true;
                    else
                        ModoTienda = !Logado;

                    if (EsPorPuntos)
                        ModoTienda = true;
                    originalComidas = new List<Comida>();
                    int idArt = 0;

                    foreach (ArticuloModel p in lista)
                    {
                        if (idArt != p.idArticulo)
                        {
                            idArt = p.idArticulo;
                            Comida co = new Comida();
                            co.articulo = p;
                            try
                            {
                                if (carrito.Count > 0)
                                {
                                    var item = carrito.Find(p2 => p2.idArticulo == idArt);
                                    if (item != null)
                                    {
                                        co.cantidad = item.cantidad;
                                        p.Cantidad = item.cantidad;
                                    }
                                    else
                                    {
                                        co.cantidad = 0;
                                        p.Cantidad = 0;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                co.cantidad = 0;
                            }
                            try
                            {
                                co.idEstablecimiento = App.EstActual.idEstablecimiento;
                            }
                            catch (Exception)
                            {
                                co.idEstablecimiento = 0;
                            }
                            //co.noTieneOpciones = (co.articulo.listadoOpciones.Count == 0 && co.articulo.listadoIngredientes.Count == 0);
                            co.noTieneOpciones = (string.IsNullOrEmpty(p.opciones) && string.IsNullOrEmpty(p.ingredientes));
                            co.botonesVisibles = false;
                            if (co.articulo.idArticulo == 11806)
                                co.noTieneOpciones = false;

                            if (co.articulo.puntos > 0 && SistemaPuntos && co.articulo.puntos<Puntos)
                                co.articulo.visiblePuntos = true;
                            originalComidas.Add(co);
                        }
                    }
                    await Filtrar();
                    await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                    {
                        App.userdialog.HideLoading();
                    }));
                }
                else
                    await ActualizaProductos();
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Métodos
        private void IrDetallePedido()
        {
            try
            {
                if (carrito.Count > 0)
                {
                    try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        App.DAUtil.Idioma = "ES";
                        if (App.autoPedidoAdmin != null)
                        {
                            carrito[0].direccion = App.autoPedidoAdmin.direccion;
                            carrito[0].idZona = App.autoPedidoAdmin.idZona;
                            carrito[0].poblacion = App.autoPedidoAdmin.poblacion;
                            carrito[0].observaciones = App.autoPedidoAdmin.nombre + Environment.NewLine + App.autoPedidoAdmin.telefono;
                            Preferences.Set("idPueblo", App.autoPedidoAdmin.idPueblo);
                        }
                        await App.DAUtil.NavigationService.NavigateToAsync<DetallePedidoViewModel>(carrito);
                    });
                }
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
                // 
            }
        }
        private void IrDetalleEstablecimiento()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    App.DAUtil.Idioma = "ES";
                    await App.DAUtil.NavigationService.NavigateToAsync<DetalleEstablecimientoParaClienteViewModel>(App.EstActual);
                });
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
                // 
            }
        }
        public async Task Filtrar()
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            List<Comida> c = new List<Comida>();
                            String paraBuscar = "";
                            if (!string.IsNullOrEmpty(TextoBusqueda))
                            {
                                paraBuscar = TextoBusqueda;

                                foreach (Comida a in originalComidas)
                                {
                                    try
                                    {
                                        if (a.articulo != null)
                                        {
                                            if (string.IsNullOrEmpty(a.articulo.alergenos))
                                                a.articulo.alergenos = "";
                                            if (string.IsNullOrEmpty(a.articulo.ingredientes))
                                                a.articulo.ingredientes = "";
                                            if (a.articulo.nombre.ToString().ToUpper().Contains(paraBuscar.ToUpper()) || string.IsNullOrWhiteSpace(paraBuscar)
                                                || a.articulo.descripcion.ToString().ToUpper().Contains(paraBuscar.ToUpper())
                                                || a.articulo.alergenos.ToString().ToUpper().Contains(paraBuscar.ToUpper())
                                                || a.articulo.ingredientes.ToString().ToUpper().Contains(paraBuscar.ToUpper()))
                                            {
                                                c.Add(a);
                                            }
                                        }
                                    }
                                    catch (Exception ex2)
                                    {
                                        Console.WriteLine(ex2.Message);
                                    }
                                }
                                Comidas = new ObservableCollection<Comida>(c);
                            }
                            else
                            {
                                Comidas = new ObservableCollection<Comida>(originalComidas);
                            }
                            
                        });
                    }
                    catch (Exception)
                    {
                        App.userdialog.HideLoading();
                    }
                });
            }
            catch (Exception ex)
            {
                await App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
                // 
            }
        }
        private void articuloSeleccionado(object idArticulo)
        {
            try
            {
                Comida articulo = (Comida)idArticulo;
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    App.DAUtil.Idioma = "ES";
                    await App.DAUtil.NavigationService.NavigateToAsync<DetalleArticuloViewModel>(articulo.articulo);
                    
                });
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
                // 
            }
        }
        private async Task Add(object parametro)
        {
            try
            {
                ArticuloModel articulo = (ArticuloModel)parametro;
                bool continuar = true;
                if (articulo.porEncargo && carrito.Where(p => p.porEncargo == true).ToList().Count == 0)
                    continuar = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaEncargo, AppResources.No, AppResources.Si);
                if (continuar)
                {
                    if (string.IsNullOrEmpty(Cantidad))
                        Cantidad = "1";
                    else
                        Cantidad = (int.Parse(Cantidad) + 1).ToString();

                    CarritoModel c = carrito.Find((obj) => obj.idArticulo == articulo.idArticulo && obj.esMenu == false);
                    if (c != null)
                        c.cantidad = c.cantidad + 1;
                    else
                    {
                        c = new CarritoModel();
                        c.id = articulo.id;
                        c.cantidad += 1;
                        c.porPuntos = 0;
                        c.puntos = 0;
                        c.comida = articulo.nombre;
                        c.comida_eng = articulo.nombre_eng;
                        c.comida_ger = articulo.nombre_ger;
                        c.comida_fr = articulo.nombre_fr;
                        c.idEstablecimiento = articulo.idEstablecimiento;
                        c.idArticulo = articulo.idArticulo;
                        c.imagen = articulo.imagen;
                        c.comentario = articulo.comentario == null ? "" : articulo.comentario;
                        c.observaciones = "";
                        c.porEncargo = articulo.porEncargo;
                        NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
                        c.precio = articulo.precio;
                        c.opcion = 0;
                        carrito.Add(c);
                    }
                    articulo.Cantidad = c.cantidad;
                    App.DAUtil.ActualizaCarrito(carrito);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task AddPorPuntos(object parametro)
        {
            try
            {
                ArticuloModel articulo = (ArticuloModel)parametro;
                if (articulo.puntos < Puntos)
                {
                    bool continuar = true;
                    if (articulo.porEncargo && carrito.Where(p => p.porEncargo == true).ToList().Count == 0)
                        continuar = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaEncargo, AppResources.No, AppResources.Si);
                    if (continuar)
                    {
                        if (string.IsNullOrEmpty(Cantidad))
                            Cantidad = "1";
                        else
                            Cantidad = (int.Parse(Cantidad) + 1).ToString();
                        CarritoModel c = new CarritoModel();
                        c.id = articulo.id;
                        c.cantidad = 1;
                        c.porPuntos = 1;
                        c.puntos = articulo.puntos;
                        c.comida = articulo.nombre;
                        c.comida_eng = articulo.nombre_eng;
                        c.comida_ger = articulo.nombre_ger;
                        c.comida_fr = articulo.nombre_fr;
                        c.idEstablecimiento = articulo.idEstablecimiento;
                        c.idArticulo = articulo.idArticulo;
                        c.imagen = articulo.imagen;
                        c.observaciones = "";
                        c.porEncargo = articulo.porEncargo;
                        NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
                        c.precio = articulo.precio;
                        c.opcion = 0;
                        carrito.Add(c);
                        App.DAUtil.ActualizaCarrito(carrito);
                        Filtrar();
                        Puntos -= articulo.puntos;
                        if (Puntos <= articulo.puntos)
                            articulo.visiblePuntos = false;
                    }
                }
                else
                {
                    await App.customDialog.ShowDialogAsync("No tiene suficientes puntos", AppResources.App, AppResources.Cerrar);
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
                ArticuloModel articulo = (ArticuloModel)parametro;
                CarritoModel c = carrito.Where((obj) => obj.idArticulo == articulo.idArticulo).FirstOrDefault();
                if (c != null)
                {
                    if (!string.IsNullOrEmpty(Cantidad))
                        Cantidad = (int.Parse(Cantidad) - 1).ToString();

                    if (Cantidad.Equals("0"))
                        Cantidad = "";

                    if (c.cantidad > 0)
                    {
                        c.cantidad = c.cantidad - 1;
                        articulo.Cantidad = c.cantidad;
                    }
                    else
                    {
                        c = new CarritoModel();
                        c.id = articulo.id;
                        c.cantidad -= 1;
                        c.comida = articulo.nombre;
                        c.comida_eng = articulo.nombre_eng;
                        c.comida_ger = articulo.nombre_ger;
                        c.comida_fr = articulo.nombre_fr;
                        c.idEstablecimiento = articulo.idEstablecimiento;
                        c.idArticulo = articulo.idArticulo;
                        c.imagen = articulo.imagen;
                        c.observaciones = "";
                        c.porEncargo = articulo.porEncargo;
                        NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
                        c.precio = articulo.precio;
                        c.opcion = 0;
                        carrito.Add(c);
                    }
                    if (c.cantidad == 0)
                        carrito.Remove(c);

                    App.DAUtil.ActualizaCarrito(carrito);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task ActualizaProductos()
        {
            try
            {
                if (!App.entradoEnCarta)
                {
                    List<ArticuloModel> lista;
                    if (App.listaProductos != null && App.listaProductos.Count > 0)
                        lista = App.listaProductos;//await App.ResponseWS.getListadoProductosEstablecimiento(_establecimiento.idEstablecimiento, false);
                    else
                    {
                        lista = await App.ResponseWS.getListadoProductosEstablecimientoCat(_categoria.id, false);
                        lista = (await App.DAUtil.getProductosEstablecimientoCat(_categoria.id)).FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1);
                        if (App.EstActual.configuracion.aceptaEncargos)
                        {
                            App.listaProductos = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1);
                            lista = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1);
                        }
                        else
                        {
                            App.listaProductos = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1 && p.porEncargo == false);
                            lista = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1 && p.porEncargo == false);
                        }
                    }
                    originalComidas = new List<Comida>();
                    int idArt = 0;
                    carrito = App.DAUtil.Getcarrito();
                    Cantidad = "0";
                    int ca = 0;
                    foreach (CarritoModel c in carrito)
                    {
                        ca += c.cantidad;
                    }
                    Cantidad = ca.ToString();

                    foreach (ArticuloModel p in lista)
                    {
                        if (idArt != p.idArticulo)
                        {
                            idArt = p.idArticulo;
                            Comida co = new Comida();

                            co.articulo = p;
                            try
                            {
                                if (carrito.Count > 0)
                                {
                                    var item = carrito.Find(p2 => p2.idArticulo == idArt);
                                    if (item != null)
                                    {
                                        co.cantidad = item.cantidad;
                                        p.Cantidad = item.cantidad;
                                    }
                                    else
                                    {
                                        co.cantidad = 0;
                                        p.Cantidad = 0;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                co.cantidad = 0;
                            }
                            try
                            {
                                co.idEstablecimiento = App.EstActual.idEstablecimiento;
                            }
                            catch (Exception)
                            {
                                co.idEstablecimiento = 0;
                            }
                            //co.noTieneOpciones = (co.articulo.listadoOpciones.Count == 0 && co.articulo.listadoIngredientes.Count == 0);
                            co.noTieneOpciones = (string.IsNullOrEmpty(p.opciones) && string.IsNullOrEmpty(p.ingredientes));
                            co.botonesVisibles = false;

                            originalComidas.Add(co);
                        }
                    }
                    await Filtrar();
                }
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
                // 
            }
        }
        #endregion

        #region Comandos
        public ICommand IrDetallePedidoCommand { get { return new Command(IrDetallePedido); } }
        public ICommand cmdDetalleEstablecimiento { get { return new Command(IrDetalleEstablecimiento); } }
        public ICommand btnFiltrar { get { return new DelegateCommandAsync(Filtrar); } }
        public ICommand ArticuloSeleccionado { get { return new Command(articuloSeleccionado); } }
        public ICommand ClickMasCommand { get { return new Command((parametro) => Add(parametro)); } }
        public ICommand ClickPorPuntosCommand { get { return new Command((parametro) => AddPorPuntos(parametro)); } }
        public ICommand ClickMenosCommand { get { return new Command((parametro) => remove(parametro)); } }
        #endregion

        #region Propiedades
        Categoria _categoria;
        List<CarritoModel> carrito = new List<CarritoModel>();

        private string textoBusqueda;
        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set
            {
                if (textoBusqueda != value)
                {
                    textoBusqueda = value;
                    Filtrar();
                    OnPropertyChanged(nameof(TextoBusqueda));
                }
            }
        }
        private ObservableCollection<Comida> comidas;
        public ObservableCollection<Comida> Comidas
        {
            get { return comidas; }
            set
            {
                if (comidas != value)
                {
                    comidas = value;
                    OnPropertyChanged(nameof(Comidas));
                }
            }
        }
        private bool sistemaPuntos;
        public bool SistemaPuntos
        {
            get { return sistemaPuntos; }
            set
            {
                if (sistemaPuntos != value)
                {
                    sistemaPuntos = value;
                    OnPropertyChanged(nameof(SistemaPuntos));
                }
            }
        }
        private int puntos;
        public int Puntos
        {
            get { return puntos; }
            set
            {
                if (puntos != value)
                {
                    puntos = value;
                    OnPropertyChanged(nameof(Puntos));
                }
            }
        }
        private string cantidad = "";
        public string Cantidad
        {
            get
            {
                return cantidad;
            }
            set
            {
                if (cantidad != value)
                {
                    cantidad = value;
                    OnPropertyChanged(nameof(Cantidad));
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
        private bool modoTienda;
        public bool ModoTienda
        {
            get
            {
                return modoTienda;
            }
            set
            {
                if (modoTienda != value)
                {
                    modoTienda = value;
                    OnPropertyChanged(nameof(ModoTienda));
                }
            }
        }
        private bool logado = true;
        public bool Logado
        {
            get
            {
                return logado;
            }
            set
            {
                if (logado != value)
                {
                    logado = value;
                    OnPropertyChanged(nameof(Logado));
                }
            }
        }
        private bool esPorPuntos = false;
        public bool EsPorPuntos
        {
            get
            {
                return esPorPuntos;
            }
            set
            {
                if (esPorPuntos != value)
                {
                    esPorPuntos = value;
                    OnPropertyChanged(nameof(EsPorPuntos));
                }
            }
        }
        public ICommand loginCommand { get { return new Command(IrLogin); } }
        private void IrLogin()
        {
            try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await App.DAUtil.NavigationService.NavigateToAsyncMenu<LoginViewModel>();
            });
        }
        #endregion

    }
}
