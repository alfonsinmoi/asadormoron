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
        private bool _navegando = false;
        private bool cargado = false;
        public CartaViewModel()
        {
            App.entradoEnCarta = false;
        }

        List<ArticuloModel> productos;

        public override async Task InitializeAsync(object navigationData)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine($"[CV] InitializeAsync INICIO (0ms)");

            if (!cargado)
            {
                cargado = true;
                System.Diagnostics.Debug.WriteLine($"[CV] Primera carga ({sw.ElapsedMilliseconds}ms)");
                // Cancelar operaciones anteriores si existen
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                var ct = _cts.Token;

                var benchmarkTotal = PerformanceBenchmark.StartTimer();

                // Mostrar loading al inicio
                try { App.userdialog?.ShowLoading(Recursos.AppResources.Cargando); } catch { }
                System.Diagnostics.Debug.WriteLine($"[CV] ShowLoading ({sw.ElapsedMilliseconds}ms)");

                try
                {
                    // Inicialización local rápida (no bloquea)
                    System.Diagnostics.Debug.WriteLine($"[CV] Iniciando configuración local ({sw.ElapsedMilliseconds}ms)");
                    Logado = App.DAUtil.Usuario != null;
                    Kiosko = (App.DAUtil.Usuario?.kiosko ?? 0) == 1;
                    System.Diagnostics.Debug.WriteLine($"[CV] Logado={Logado}, Kiosko={Kiosko} ({sw.ElapsedMilliseconds}ms)");

                    // Permitir añadir productos para Kiosko, Admin o Establecimiento
                    var rol = App.DAUtil.Usuario?.rol ?? 0;
                    PuedeAnadirProducto = Kiosko ||
                        rol == (int)RolesEnum.Administrador ||
                        rol == (int)RolesEnum.SuperAdmin ||
                        rol == (int)RolesEnum.Establecimiento;

                    App.DAUtil.EnTimer = false;
                    Logo = "logomorado.png";
                    carrito = App.DAUtil.Getcarrito();
                    Idioma = App.idioma;

                    // OPTIMIZADO: Usar LINQ Sum en lugar de foreach
                    Cantidad = carrito.Sum(c => c.cantidad).ToString();

                    _establecimiento = App.EstActual;
                    if (!string.IsNullOrEmpty(_establecimiento.logo))
                        Logo = _establecimiento.logo;

                    // Kiosko: desactivar sistema de puntos
                    if (Kiosko)
                    {
                        System.Diagnostics.Debug.WriteLine($"[CartaViewModel] MODO KIOSKO - Sistema de puntos desactivado");
                    }

                    {
                        // Cargar datos del servidor
                        System.Diagnostics.Debug.WriteLine($"[CV] Cargando datos - idEst: {_establecimiento?.idEstablecimiento} ({sw.ElapsedMilliseconds}ms)");
                        var datosTask = _serviceAsync.CargarDatosCartaAsync(_establecimiento.idEstablecimiento, ct);
                        var productosTask = productos == null
                            ? App.ResponseWS.getListadoProductosEstablecimiento(_establecimiento.idEstablecimiento, true)
                            : Task.FromResult(productos);

                        // Esperar ambas tareas en paralelo
                        System.Diagnostics.Debug.WriteLine($"[CV] Task.WhenAll iniciado ({sw.ElapsedMilliseconds}ms)");
                        await Task.WhenAll(datosTask, productosTask);
                        System.Diagnostics.Debug.WriteLine($"[CV] Task.WhenAll completado ({sw.ElapsedMilliseconds}ms)");

                        var datos = await datosTask;
                        System.Diagnostics.Debug.WriteLine($"[CV] Categorias: {datos.Categorias?.Count ?? 0} ({sw.ElapsedMilliseconds}ms)");
                        productos = (await productosTask)?.FindAll(p => p.estado == 1) ?? new List<ArticuloModel>();
                        System.Diagnostics.Debug.WriteLine($"[CV] Productos: {productos?.Count ?? 0} ({sw.ElapsedMilliseconds}ms)");

                        // Aplicar resultados
                        Categorias = new ObservableCollection<Categoria>(datos.Categorias);
                        App.EstActual.configuracion = datos.Config;

                        // Kiosko NO usa sistema de puntos
                        if (Kiosko)
                        {
                            SistemaPuntos = false;
                            Puntos = 0;
                        }
                        else
                        {
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
                        }
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
                        System.Diagnostics.Debug.WriteLine($"[CV] Cargando contador pollos ({sw.ElapsedMilliseconds}ms)");
                        await CargarContadorPollosAsync();
                        System.Diagnostics.Debug.WriteLine($"[CV] Contador pollos cargado ({sw.ElapsedMilliseconds}ms)");
                    }

                    System.Diagnostics.Debug.WriteLine($"[CV] Antes base.InitializeAsync ({sw.ElapsedMilliseconds}ms)");
                    await base.InitializeAsync(navigationData);
                    System.Diagnostics.Debug.WriteLine($"[CV] Después base.InitializeAsync ({sw.ElapsedMilliseconds}ms)");
                }
                catch (OperationCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine($"[CV] CANCELADO ({sw.ElapsedMilliseconds}ms)");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[CV] ERROR: {ex.Message} ({sw.ElapsedMilliseconds}ms)");
                }
                finally
                {
                    System.Diagnostics.Debug.WriteLine($"[CV] FIN TOTAL: {sw.ElapsedMilliseconds}ms");
                    MainThread.BeginInvokeOnMainThread(() => App.userdialog?.HideLoading());
                    PerformanceBenchmark.StopAndRecord(benchmarkTotal, "CartaViewModel_InitializeAsync_OPTIMIZADO");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[CV] Ya cargado, saltando ({sw.ElapsedMilliseconds}ms)");
            }
        }

        #region Métodos

        /// <summary>
        /// Búsqueda con debounce para evitar búsquedas excesivas mientras el usuario escribe
        /// </summary>
        private async Task BuscarProductosConDebounce()
        {
            await _searchDebouncer.DebounceAsync(() =>
            {
                FiltrarProductos();
                return Task.CompletedTask;
            }, 150); // 150ms - más rápido porque es solo filtrado local
        }

        /// <summary>
        /// Filtra los productos ya cargados en memoria (sin llamadas al servidor)
        /// </summary>
        private void FiltrarProductos()
        {
            try
            {
                var busqueda = TextoBusqueda?.Trim();

                if (string.IsNullOrWhiteSpace(busqueda))
                {
                    MostrandoResultados = false;
                    ResultadosBusqueda = new ObservableCollection<Comida>();
                    return;
                }

                // Si no hay productos cargados, no buscar
                if (productos == null || productos.Count == 0)
                {
                    MostrandoResultados = false;
                    return;
                }

                // Filtrado local - muy rápido
                var resultados = new List<Comida>();
                var idEst = _establecimiento?.idEstablecimiento ?? 0;

                foreach (var p in productos)
                {
                    bool nombreMatch = p.nombre?.IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0;
                    bool descMatch = p.descripcion?.IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (nombreMatch || descMatch)
                    {
                        var itemCarrito = carrito.Find(c => c.idArticulo == p.idArticulo);
                        var cant = itemCarrito?.cantidad ?? 0;
                        p.Cantidad = cant;

                        resultados.Add(new Comida
                        {
                            articulo = p,
                            idEstablecimiento = idEst,
                            noTieneOpciones = string.IsNullOrEmpty(p.opciones) && string.IsNullOrEmpty(p.ingredientes),
                            cantidad = cant
                        });
                    }
                }

                ResultadosBusqueda = new ObservableCollection<Comida>(resultados);
                MostrandoResultados = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] Error búsqueda: {ex.Message}");
            }
        }

        /// <summary>
        /// Ejecuta la búsqueda cuando el usuario pulsa el botón Buscar
        /// </summary>
        private void Buscar()
        {
            FiltrarProductos();
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

        private async void ProductoSeleccionado(object parametro)
        {
            if (_navegando) return;
            _navegando = true;

            try
            {
                Comida comida = (Comida)parametro;
                App.DAUtil.Idioma = "ES";
                await App.DAUtil.NavigationService.NavigateToAsync<DetalleArticuloViewModel>(comida.articulo);
            }
            catch (Exception ex)
            {
                App.customDialog?.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
            }
            finally
            {
                _navegando = false;
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
        private async void IrDetallePedido()
        {
            if (_navegando) return;
            _navegando = true;

            try
            {
                if (carrito.Count > 0)
                {
                    // Mostrar loading INMEDIATAMENTE
                    try { App.userdialog?.ShowLoading(AppResources.Cargando); } catch { }

                    // Pequeña pausa para que el loading se renderice
                    await Task.Delay(30);

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
                }
            }
            catch (Exception ex)
            {
                App.userdialog?.HideLoading();
                _ = App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
            }
            finally
            {
                _navegando = false;
            }
        }
        private async void CategoriaSeleccionadaExe(Categoria idCategoria)
        {
            if (_navegando) return;
            _navegando = true;

            // Mostrar loading INMEDIATAMENTE
            try { App.userdialog?.ShowLoading(AppResources.Cargando); } catch { }

            var sw = System.Diagnostics.Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine($"[PERF] CategoriaSeleccionadaExe INICIO - Categoria: {idCategoria?.nombre}");

            try
            {
                // Pequeña pausa para que el loading se renderice en pantalla
                await Task.Delay(30);

                App.DAUtil.Idioma = "ES";

                System.Diagnostics.Debug.WriteLine($"[PERF] Antes de NavigateToAsync: {sw.ElapsedMilliseconds}ms");
                if (idCategoria.navidad)
                    await App.DAUtil.NavigationService.NavigateToAsync<CartaProductosNavidadViewModel>(idCategoria);
                else
                    await App.DAUtil.NavigationService.NavigateToAsync<CartaProductosViewModel>(idCategoria);
                System.Diagnostics.Debug.WriteLine($"[PERF] Después de NavigateToAsync: {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                App.userdialog?.HideLoading();
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
            }
            finally
            {
                _navegando = false;
                System.Diagnostics.Debug.WriteLine($"[PERF] CategoriaSeleccionadaExe FIN TOTAL: {sw.ElapsedMilliseconds}ms");
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
        public ICommand BuscarCommand { get { return new Command(Buscar); } }
        public ICommand LimpiarBusquedaCommand { get { return new Command(LimpiarBusqueda); } }
        public ICommand ProductoSeleccionadoCommand { get { return new Command(ProductoSeleccionado); } }
        public ICommand ClickMasCommand { get { return new Command((parametro) => Add(parametro)); } }
        public ICommand ClickMenosCommand { get { return new Command((parametro) => Remove(parametro)); } }
        public ICommand AbrirPopupKioskoCommand { get { return new Command(AbrirPopupKiosko); } }
        public ICommand CerrarPopupKioskoCommand { get { return new Command(CerrarPopupKiosko); } }
        public ICommand AnadirProductoKioskoCommand { get { return new Command(AnadirProductoKiosko); } }
        public ICommand SumarPolloCommand { get { return new Command(async () => await SumarPolloAsync()); } }
        public ICommand RestarPolloCommand { get { return new Command(async () => await RestarPolloAsync()); } }
        public ICommand SeleccionarCategoriaCommand { get { return new Command<Categoria>((cat) => CategoriaSeleccionada = cat); } }
        #endregion

        #region Métodos Públicos
        /// <summary>
        /// Refresca el carrito desde la base de datos local.
        /// Llamar desde OnAppearing para actualizar después de volver de DetalleArticulo.
        /// </summary>
        public void RefrescarCarrito()
        {
            try
            {
                carrito = App.DAUtil.Getcarrito();
                Cantidad = carrito.Sum(c => c.cantidad).ToString();

                // Actualizar puntos si aplica
                if (SistemaPuntos && !Kiosko)
                {
                    var puntosUsados = carrito.Where(p => p.porPuntos == 1).Sum(c => c.puntos);
                    // Nota: Puntos base se mantiene, solo restamos los usados en carrito
                }

                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] Carrito refrescado: {carrito.Count} items, cantidad total: {Cantidad}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartaViewModel] Error al refrescar carrito: {ex.Message}");
            }
        }
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
                    // La búsqueda se ejecuta al pulsar el botón Buscar, no automáticamente
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

        private bool puedeAnadirProducto = false;
        public bool PuedeAnadirProducto
        {
            get { return puedeAnadirProducto; }
            set
            {
                if (puedeAnadirProducto != value)
                {
                    puedeAnadirProducto = value;
                    OnPropertyChanged(nameof(PuedeAnadirProducto));
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
