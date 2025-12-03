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
    public class CartaViewModel : ViewModelBase
    {
        public CartaViewModel()
        {
            App.entradoEnCarta = false;
        }
        List<ArticuloModel> productos;
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                if (productos == null)
                    productos = (await App.ResponseWS.getListadoProductosEstablecimiento())
                        .FindAll(p => p.estado == 1);
                Logado = App.DAUtil.Usuario != null;
                Kiosko = (App.DAUtil.Usuario?.kiosko ?? 0) == 1;
                App.DAUtil.EnTimer = false;
                Logo = "logomorado.png";
                carrito = App.DAUtil.Getcarrito();
                Cantidad = "0";
                Idioma = App.idioma;
                int ca = 0;
                foreach (CarritoModel c in carrito)
                {
                    ca += c.cantidad;
                }
                Cantidad = ca.ToString();

                _establecimiento = App.EstActual;
                Categorias = new ObservableCollection<Categoria>(ResponseServiceWS.getListadoCategorias(_establecimiento.idEstablecimiento));
                if (!string.IsNullOrEmpty(_establecimiento.logo))
                    Logo = _establecimiento.logo;

                App.EstActual = _establecimiento;
                App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(_establecimiento.idEstablecimiento);
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
                // 
                await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.userdialog.HideLoading();
                }));


                if (App.EstActual.configuracion.servicioActivo)
                    Cerrado = false;
                else
                {
                    TextoCerrado = App.EstActual.configuracion.textoCerrado;
                    Cerrado = true;
                }

            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Métodos
        private async Task BuscarProductos()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TextoBusqueda))
                {
                    // Si no hay texto de búsqueda, mostrar categorías
                    MostrandoResultados = false;
                    ResultadosBusqueda = new ObservableCollection<Comida>();
                    return;
                }

                try { App.userdialog?.ShowLoading(AppResources.Cargando); } catch (Exception) { }

                // Obtener todos los productos del establecimiento en una sola llamada
                var todosLosProductos = new List<Comida>();
                var busqueda = TextoBusqueda.ToUpper();



                foreach (var p in productos)
                {
                    // Verificar si el producto coincide con la búsqueda
                    var nombre = p.nombre ?? "";
                    var descripcion = p.descripcion ?? "";

                    if (nombre.ToUpper().Contains(busqueda) || descripcion.ToUpper().Contains(busqueda))
                    {
                        var comida = new Comida
                        {
                            articulo = p,
                            idEstablecimiento = _establecimiento.idEstablecimiento,
                            noTieneOpciones = string.IsNullOrEmpty(p.opciones) && string.IsNullOrEmpty(p.ingredientes),
                            cantidad = 0
                        };

                        // Verificar si está en el carrito
                        var itemCarrito = carrito.Find(c => c.idArticulo == p.idArticulo);
                        if (itemCarrito != null)
                        {
                            comida.cantidad = itemCarrito.cantidad;
                            p.Cantidad = itemCarrito.cantidad;
                        }

                        todosLosProductos.Add(comida);
                    }
                }

                ResultadosBusqueda = new ObservableCollection<Comida>(todosLosProductos);
                MostrandoResultados = true;

                App.userdialog?.HideLoading();
            }
            catch (Exception ex)
            {
                App.userdialog?.HideLoading();
                //
            }
        }

        private void LimpiarBusqueda()
        {
            TextoBusqueda = "";
            MostrandoResultados = false;
            ResultadosBusqueda = new ObservableCollection<Comida>();
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

        private void Remove(object parametro)
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

        private void ProductoSeleccionado(object parametro)
        {
            try
            {
                Comida comida = (Comida)parametro;
                try { App.userdialog?.ShowLoading(AppResources.Cargando); } catch (Exception) { }
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    App.DAUtil.Idioma = "ES";
                    await App.DAUtil.NavigationService.NavigateToAsync<DetalleArticuloViewModel>(comida.articulo);
                });
            }
            catch (Exception ex)
            {
                App.customDialog?.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
            }
        }

        private void IrLogin()
        {
            try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await App.DAUtil.NavigationService.NavigateToAsyncMenu<LoginViewModel>();
            });
        }
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
        private void CategoriaSeleccionadaExe(Categoria idCategoria)
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    App.DAUtil.Idioma = "ES";
                    if (idCategoria.navidad)
                        await App.DAUtil.NavigationService.NavigateToAsync<CartaProductosNavidadViewModel>(idCategoria);
                    else
                        await App.DAUtil.NavigationService.NavigateToAsync<CartaProductosViewModel>(idCategoria);
                });
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
                // 
            }
        }
        private void AbrirPopupKiosko()
        {
            NombreProductoKiosko = "";
            PrecioProductoKiosko = "";
            MostrarPopupKiosko = true;
        }

        private void CerrarPopupKiosko()
        {
            MostrarPopupKiosko = false;
        }

        private void AnadirProductoKiosko()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NombreProductoKiosko))
                {
                    App.customDialog?.ShowDialogAsync("Debe indicar un nombre para el producto", AppResources.SoloError, AppResources.Cerrar);
                    return;
                }

                if (string.IsNullOrWhiteSpace(PrecioProductoKiosko) ||
                    !double.TryParse(PrecioProductoKiosko.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double precio))
                {
                    App.customDialog?.ShowDialogAsync("Debe indicar un precio válido", AppResources.SoloError, AppResources.Cerrar);
                    return;
                }

                // Crear el producto personalizado y añadirlo al carrito
                var carritoItem = new CarritoModel
                {
                    id = -1,
                    idArticulo = -(int)(DateTime.Now.Ticks % int.MaxValue), // ID único negativo para productos kiosko
                    cantidad = 1,
                    comida = NombreProductoKiosko,
                    comida_eng = NombreProductoKiosko,
                    comida_ger = NombreProductoKiosko,
                    comida_fr = NombreProductoKiosko,
                    idEstablecimiento = _establecimiento.idEstablecimiento,
                    imagen = "",
                    comentario = "",
                    observaciones = "",
                    porEncargo = false,
                    precio = precio,
                    opcion = 0,
                    porPuntos = 0,
                    puntos = 0
                };

                carrito.Add(carritoItem);
                App.DAUtil.ActualizaCarrito(carrito);

                // Actualizar cantidad en el carrito
                int ca = 0;
                foreach (CarritoModel c in carrito)
                {
                    ca += c.cantidad;
                }
                Cantidad = ca.ToString();

                MostrarPopupKiosko = false;
            }
            catch (Exception ex)
            {
                App.customDialog?.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
            }
        }
        #endregion

        #region Comandos
        public ICommand IrDetallePedidoCommand { get { return new Command(IrDetallePedido); } }
        public ICommand loginCommand { get { return new Command(IrLogin); } }
        public ICommand BuscarCommand { get { return new Command(async () => await BuscarProductos()); } }
        public ICommand LimpiarBusquedaCommand { get { return new Command(LimpiarBusqueda); } }
        public ICommand ProductoSeleccionadoCommand { get { return new Command(ProductoSeleccionado); } }
        public ICommand ClickMasCommand { get { return new Command((parametro) => Add(parametro)); } }
        public ICommand ClickMenosCommand { get { return new Command((parametro) => Remove(parametro)); } }
        public ICommand AbrirPopupKioskoCommand { get { return new Command(AbrirPopupKiosko); } }
        public ICommand CerrarPopupKioskoCommand { get { return new Command(CerrarPopupKiosko); } }
        public ICommand AnadirProductoKioskoCommand { get { return new Command(AnadirProductoKiosko); } }
        #endregion

        #region Propiedades
        Establecimiento _establecimiento;
        List<CarritoModel> carrito = new List<CarritoModel>();
        private ObservableCollection<Categoria> categorias;
        public ObservableCollection<Categoria> Categorias
        {
            get { return categorias; }
            set
            {
                if (categorias != value)
                {
                    categorias = value;
                    OnPropertyChanged(nameof(Categorias));
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
        private bool cerrado;
        public bool Cerrado
        {
            get
            {
                return cerrado;
            }
            set
            {
                if (cerrado != value)
                {
                    cerrado = value;
                    OnPropertyChanged(nameof(Cerrado));
                }
            }
        }
        private string textoCerrado;
        public string TextoCerrado
        {
            get
            {
                return textoCerrado;
            }
            set
            {
                if (textoCerrado != value)
                {
                    textoCerrado = value;
                    OnPropertyChanged(nameof(TextoCerrado));
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
        private Categoria categoriaSeleccionada;
        public Categoria CategoriaSeleccionada
        {
            get
            {
                return categoriaSeleccionada;
            }
            set
            {
                if (categoriaSeleccionada != value)
                {
                    categoriaSeleccionada = value;
                    OnPropertyChanged(nameof(CategoriaSeleccionada));
                    if (CategoriaSeleccionada != null)
                        CategoriaSeleccionadaExe(CategoriaSeleccionada);
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

        // Propiedades para búsqueda
        private string textoBusqueda = "";
        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set
            {
                if (textoBusqueda != value)
                {
                    textoBusqueda = value;
                    OnPropertyChanged(nameof(TextoBusqueda));
                    // Buscar automáticamente al modificar el texto
                    _ = BuscarProductos();
                }
            }
        }

        private bool mostrandoResultados = false;
        public bool MostrandoResultados
        {
            get { return mostrandoResultados; }
            set
            {
                if (mostrandoResultados != value)
                {
                    mostrandoResultados = value;
                    OnPropertyChanged(nameof(MostrandoResultados));
                }
            }
        }

        private ObservableCollection<Comida> resultadosBusqueda = new ObservableCollection<Comida>();
        public ObservableCollection<Comida> ResultadosBusqueda
        {
            get { return resultadosBusqueda; }
            set
            {
                if (resultadosBusqueda != value)
                {
                    resultadosBusqueda = value;
                    OnPropertyChanged(nameof(ResultadosBusqueda));
                }
            }
        }

        private bool modoTienda = false;
        public bool ModoTienda
        {
            get { return modoTienda; }
            set
            {
                if (modoTienda != value)
                {
                    modoTienda = value;
                    OnPropertyChanged(nameof(ModoTienda));
                }
            }
        }

        private bool kiosko = false;
        public bool Kiosko
        {
            get { return kiosko; }
            set
            {
                if (kiosko != value)
                {
                    kiosko = value;
                    OnPropertyChanged(nameof(Kiosko));
                }
            }
        }

        private bool mostrarPopupKiosko = false;
        public bool MostrarPopupKiosko
        {
            get { return mostrarPopupKiosko; }
            set
            {
                if (mostrarPopupKiosko != value)
                {
                    mostrarPopupKiosko = value;
                    OnPropertyChanged(nameof(MostrarPopupKiosko));
                }
            }
        }

        private string nombreProductoKiosko = "";
        public string NombreProductoKiosko
        {
            get { return nombreProductoKiosko; }
            set
            {
                if (nombreProductoKiosko != value)
                {
                    nombreProductoKiosko = value;
                    OnPropertyChanged(nameof(NombreProductoKiosko));
                }
            }
        }

        private string precioProductoKiosko = "";
        public string PrecioProductoKiosko
        {
            get { return precioProductoKiosko; }
            set
            {
                if (precioProductoKiosko != value)
                {
                    precioProductoKiosko = value;
                    OnPropertyChanged(nameof(PrecioProductoKiosko));
                }
            }
        }
        #endregion

    }
}
