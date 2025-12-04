//
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
//
using AsadorMoron.Services;
using AsadorMoron.Utils;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using CommunityToolkit.Mvvm.Input;

namespace AsadorMoron.ViewModels.Clientes
{
    public class CartaViewModel : ViewModelBase
    {
        // Servicios optimizados
        private readonly ResponseServiceAsync _serviceAsync = new();
        private readonly Debouncer _searchDebouncer = new();
        private CancellationTokenSource _cts;

        public CartaViewModel()
        {
            App.entradoEnCarta = false;
        }

        List<ArticuloModel> productos;

        public override async Task InitializeAsync(object navigationData)
        {
            // Cancelar operaciones anteriores si existen
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var ct = _cts.Token;

            var benchmarkTotal = PerformanceBenchmark.StartTimer();

            try
            {
                // Inicialización local rápida (no bloquea)
                Logado = App.DAUtil.Usuario != null;
                Kiosko = (App.DAUtil.Usuario?.kiosko ?? 0) == 1;
                App.DAUtil.EnTimer = false;
                Logo = "logomorado.png";
                carrito = App.DAUtil.Getcarrito();
                Idioma = App.idioma;

                // OPTIMIZADO: Usar LINQ Sum en lugar de foreach
                Cantidad = carrito.Sum(c => c.cantidad).ToString();

                _establecimiento = App.EstActual;
                if (!string.IsNullOrEmpty(_establecimiento.logo))
                    Logo = _establecimiento.logo;

                // OPTIMIZADO: Cargar datos en PARALELO con async/await
                // ANTES: 3 llamadas síncronas bloqueantes secuenciales
                // DESPUÉS: 3 llamadas async en paralelo
                var datosTask = _serviceAsync.CargarDatosCartaAsync(_establecimiento.idEstablecimiento, ct);
                var productosTask = productos == null
                    ? App.ResponseWS.getListadoProductosEstablecimiento()
                    : Task.FromResult(productos);

                // Esperar ambas tareas en paralelo
                await Task.WhenAll(datosTask, productosTask);

                var datos = await datosTask;
                productos = (await productosTask)?.FindAll(p => p.estado == 1) ?? new List<ArticuloModel>();

                // Aplicar resultados
                Categorias = new ObservableCollection<Categoria>(datos.Categorias);
                App.EstActual.configuracion = datos.Config;
                SistemaPuntos = datos.Config?.sistemaPuntos ?? false;

                if (SistemaPuntos)
                {
                    // OPTIMIZADO: Usar LINQ Sum en lugar de foreach
                    Puntos = datos.Puntos - carrito.Where(p => p.porPuntos == 1).Sum(c => c.puntos);
                }
                else
                {
                    Puntos = 0;
                }

                // Verificar estado del servicio
                if (App.EstActual.configuracion?.servicioActivo == true)
                {
                    Cerrado = false;
                }
                else
                {
                    TextoCerrado = App.EstActual.configuracion?.textoCerrado ?? "";
                    Cerrado = true;
                }

                // Cargar contador de pollos si es modo kiosko
                if (Kiosko)
                {
                    await CargarContadorPollosAsync();
                }

                await base.InitializeAsync(navigationData);
            }
            catch (OperationCanceledException)
            {
                // Operación cancelada, es esperado
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] Error InitializeAsync: {ex.Message}");
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() => App.userdialog?.HideLoading());
                PerformanceBenchmark.StopAndRecord(benchmarkTotal, "CartaViewModel_InitializeAsync_OPTIMIZADO");
            }
        }

        #region Métodos

        /// <summary>
        /// OPTIMIZADO: Búsqueda con debounce para evitar búsquedas excesivas mientras el usuario escribe
        /// </summary>
        private async Task BuscarProductosConDebounce()
        {
            await _searchDebouncer.DebounceAsync(async () =>
            {
                await BuscarProductosInterno();
            }, 300); // 300ms de delay
        }

        private async Task BuscarProductosInterno()
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

                // OPTIMIZADO: Usar StringComparison para mejor rendimiento
                var busqueda = TextoBusqueda;

                // OPTIMIZADO: Usar LINQ con StringComparison.OrdinalIgnoreCase (más rápido)
                var resultados = productos
                    .Where(p =>
                        (p.nombre?.Contains(busqueda, StringComparison.OrdinalIgnoreCase) == true) ||
                        (p.descripcion?.Contains(busqueda, StringComparison.OrdinalIgnoreCase) == true))
                    .Select(p =>
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

                        return comida;
                    })
                    .ToList();

                // Actualizar en el hilo UI
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ResultadosBusqueda = new ObservableCollection<Comida>(resultados);
                    MostrandoResultados = true;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] Error búsqueda: {ex.Message}");
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

                // OPTIMIZADO: Usar LINQ Sum en lugar de foreach
                Cantidad = carrito.Sum(c => c.cantidad).ToString();

                MostrarPopupKiosko = false;
            }
            catch (Exception ex)
            {
                App.customDialog?.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
            }
        }

        private async Task CargarContadorPollosAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] CargarContadorPollosAsync - Kiosko: {Kiosko}, Establecimiento: {_establecimiento?.idEstablecimiento}");

                if (!Kiosko || _establecimiento == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[CartaViewModel] CargarContadorPollosAsync - Saliendo por validación");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] Llamando GetContadorPollosAsync para establecimiento {_establecimiento.idEstablecimiento}");
                var contador = await ResponseServiceWS.GetContadorPollosAsync(_establecimiento.idEstablecimiento);
                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] Resultado contador: {contador?.cantidad ?? -1}");

                if (contador != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ContadorPollos = contador.cantidad;
                        System.Diagnostics.Debug.WriteLine($"[CartaViewModel] ContadorPollos actualizado a: {ContadorPollos}");
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] Error cargando contador pollos: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] Stack trace: {ex.StackTrace}");
            }
        }

        private async Task SumarPolloAsync()
        {
            try
            {
                if (_establecimiento == null) return;

                var resultado = await ResponseServiceWS.SumarPollosAsync(_establecimiento.idEstablecimiento, 1);
                if (resultado != null)
                {
                    ContadorPollos = resultado.cantidad;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] Error sumando pollo: {ex.Message}");
            }
        }

        private async Task RestarPolloAsync()
        {
            try
            {
                if (_establecimiento == null || ContadorPollos <= 0) return;

                var resultado = await ResponseServiceWS.RestarPollosAsync(_establecimiento.idEstablecimiento, 1);
                if (resultado != null)
                {
                    ContadorPollos = resultado.cantidad;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] Error restando pollo: {ex.Message}");
            }
        }
        #endregion

        #region Comandos
        public ICommand IrDetallePedidoCommand { get { return new Command(IrDetallePedido); } }
        public ICommand loginCommand { get { return new Command(IrLogin); } }
        public ICommand BuscarCommand { get { return new Command(async () => await BuscarProductosConDebounce()); } }
        public ICommand LimpiarBusquedaCommand { get { return new Command(LimpiarBusqueda); } }
        public ICommand ProductoSeleccionadoCommand { get { return new Command(ProductoSeleccionado); } }
        public ICommand ClickMasCommand { get { return new Command((parametro) => Add(parametro)); } }
        public ICommand ClickMenosCommand { get { return new Command((parametro) => Remove(parametro)); } }
        public ICommand AbrirPopupKioskoCommand { get { return new Command(AbrirPopupKiosko); } }
        public ICommand CerrarPopupKioskoCommand { get { return new Command(CerrarPopupKiosko); } }
        public ICommand AnadirProductoKioskoCommand { get { return new Command(AnadirProductoKiosko); } }
        public ICommand SumarPolloCommand { get { return new Command(async () => await SumarPolloAsync()); } }
        public ICommand RestarPolloCommand { get { return new Command(async () => await RestarPolloAsync()); } }
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
                    // OPTIMIZADO: Buscar con debounce para no disparar en cada tecla
                    _ = BuscarProductosConDebounce();
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

        // Contador de pollos asados (solo visible en modo kiosko)
        private int contadorPollos = 0;
        public int ContadorPollos
        {
            get { return contadorPollos; }
            set
            {
                if (contadorPollos != value)
                {
                    contadorPollos = value;
                    OnPropertyChanged(nameof(ContadorPollos));
                }
            }
        }
        #endregion

    }
}
