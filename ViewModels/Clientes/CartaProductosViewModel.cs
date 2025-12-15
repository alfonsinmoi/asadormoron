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
using AsadorMoron.Utils;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using CommunityToolkit.Mvvm.Input;

namespace AsadorMoron.ViewModels.Clientes
{
    public class CartaProductosViewModel : ViewModelBase
    {
        private bool _navegando = false;
        private readonly Debouncer _searchDebouncer = new();

        public CartaProductosViewModel()
        {
            App.entradoEnCarta = false;
        }
        List<Comida> originalComidas;
        public override async Task InitializeAsync(object navigationData)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine($"[CP] InitializeAsync INICIO (0ms)");

            try
            {
                // OPTIMIZADO: Inicialización rápida sin bloqueo
                Logado = App.DAUtil.Usuario != null;
                App.DAUtil.EnTimer = false;
                carrito = App.DAUtil.Getcarrito();
                Idioma = App.idioma;
                System.Diagnostics.Debug.WriteLine($"[CP] Config local ({sw.ElapsedMilliseconds}ms)");

                // OPTIMIZADO: Usar LINQ Sum en lugar de foreach
                Cantidad = carrito.Sum(c => c.cantidad).ToString();

                // Verificar si es modo Kiosko
                bool esKiosko = (App.DAUtil.Usuario?.kiosko ?? 0) == 1;

                // KIOSKO: Desactivar sistema de puntos
                if (esKiosko)
                {
                    SistemaPuntos = false;
                    Puntos = 0;
                    System.Diagnostics.Debug.WriteLine($"[CP] KIOSKO - Sistema puntos OFF ({sw.ElapsedMilliseconds}ms)");
                }
                else
                {
                    SistemaPuntos = App.EstActual.configuracion?.sistemaPuntos ?? false;
                    if (SistemaPuntos)
                    {
                        System.Diagnostics.Debug.WriteLine($"[CP] Antes getPuntos ({sw.ElapsedMilliseconds}ms)");
                        Puntos = ResponseServiceWS.getPuntosEstablecimiento() - carrito.Where(p => p.porPuntos == 1).Sum(c => c.puntos);
                        System.Diagnostics.Debug.WriteLine($"[CP] Después getPuntos ({sw.ElapsedMilliseconds}ms)");
                    }
                    else
                        Puntos = 0;
                }

                if (_categoria == null)
                {
                    _categoria = (Categoria)navigationData;
                    EsPorPuntos = esKiosko ? false : _categoria.esPuntos; // Kiosko nunca usa puntos
                    App.esPorPuntos = EsPorPuntos;
                    System.Diagnostics.Debug.WriteLine($"[CP] Categoria: {_categoria?.nombre} ({sw.ElapsedMilliseconds}ms)");

                    List<ArticuloModel> lista = null;

                    // Primero intentar cargar de SQLite
                    System.Diagnostics.Debug.WriteLine($"[CP] Antes SQLite ({sw.ElapsedMilliseconds}ms)");
                    var listaSQLite = await App.DAUtil.getProductosEstablecimientoCat(_categoria.id);
                    System.Diagnostics.Debug.WriteLine($"[CP] SQLite: {listaSQLite?.Count ?? 0} items ({sw.ElapsedMilliseconds}ms)");

                    // Usar SQLite si hay datos
                    if (listaSQLite != null && listaSQLite.Count > 0)
                    {
                        lista = listaSQLite.FindAll(p => p.estado == 1 && p.estadoCategoria == 1);
                        System.Diagnostics.Debug.WriteLine($"[CP] Usando SQLite ({sw.ElapsedMilliseconds}ms)");
                    }
                    else
                    {
                        // Solo si no hay datos en SQLite, llamar al servidor
                        System.Diagnostics.Debug.WriteLine($"[CP] SQLite vacío, llamando servidor ({sw.ElapsedMilliseconds}ms)");
                        var listaServer = await App.ResponseWS.getListadoProductosEstablecimientoCat(_categoria.id, false);
                        lista = listaServer?.FindAll(p => p.estado == 1 && p.estadoCategoria == 1) ?? new List<ArticuloModel>();
                        System.Diagnostics.Debug.WriteLine($"[CP] Servidor: {lista?.Count ?? 0} items ({sw.ElapsedMilliseconds}ms)");
                    }
                    System.Diagnostics.Debug.WriteLine($"[CP] Productos filtrados: {lista?.Count ?? 0} ({sw.ElapsedMilliseconds}ms)");

                    // Asegurar que configuracion existe para Kiosko
                    if (esKiosko && App.EstActual.configuracion == null)
                    {
                        App.EstActual.configuracion = new ConfiguracionEstablecimiento { servicioActivo = true, aceptaEncargos = true };
                    }

                    bool aceptaEncargos = App.EstActual.configuracion?.aceptaEncargos ?? true;
                    System.Diagnostics.Debug.WriteLine($"[CartaProductos] aceptaEncargos: {aceptaEncargos}");

                    if (aceptaEncargos)
                    {
                        App.listaProductos = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1);
                        lista = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1);
                    }
                    else
                    {
                        App.listaProductos = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1 && p.porEncargo == false);
                        lista = lista.FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1 && p.porEncargo == false);
                    }
                    System.Diagnostics.Debug.WriteLine($"[CP] Filtro vistaEnvios: {lista?.Count ?? 0} ({sw.ElapsedMilliseconds}ms)");

                    string horario = App.EstActual?.horario ?? "";
                    if (horario.Equals(AppResources.Cerrado))
                        ModoTienda = true;
                    else
                        ModoTienda = !Logado;

                    if (EsPorPuntos)
                        ModoTienda = true;

                    System.Diagnostics.Debug.WriteLine($"[CP] Antes crear originalComidas ({sw.ElapsedMilliseconds}ms)");

                    // OPTIMIZADO: Agrupar carrito por idArticulo (puede haber duplicados con opciones distintas)
                    var carritoDict = carrito
                        .GroupBy(c => c.idArticulo)
                        .ToDictionary(g => g.Key, g => g.Sum(c => c.cantidad));
                    int idEstablecimiento = App.EstActual?.idEstablecimiento ?? 0;

                    // OPTIMIZADO: Pre-dimensionar lista para evitar redimensionamientos
                    originalComidas = new List<Comida>(lista.Count);
                    int idArt = 0;

                    foreach (ArticuloModel p in lista)
                    {
                        if (idArt != p.idArticulo)
                        {
                            idArt = p.idArticulo;

                            // OPTIMIZADO: Búsqueda O(1) en diccionario
                            int cantidadCarrito = 0;
                            if (carritoDict.TryGetValue(idArt, out int cant))
                            {
                                cantidadCarrito = cant;
                            }
                            p.Cantidad = cantidadCarrito;

                            var co = new Comida
                            {
                                articulo = p,
                                cantidad = cantidadCarrito,
                                idEstablecimiento = idEstablecimiento,
                                noTieneOpciones = string.IsNullOrEmpty(p.opciones) && string.IsNullOrEmpty(p.ingredientes),
                                botonesVisibles = false
                            };

                            // Caso especial
                            if (p.idArticulo == 11806)
                                co.noTieneOpciones = false;

                            if (p.puntos > 0 && SistemaPuntos && p.puntos < Puntos)
                                p.visiblePuntos = true;

                            originalComidas.Add(co);
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"[CP] originalComidas: {originalComidas.Count} ({sw.ElapsedMilliseconds}ms)");

                    System.Diagnostics.Debug.WriteLine($"[CP] Antes Filtrar ({sw.ElapsedMilliseconds}ms)");
                    await Filtrar();
                    System.Diagnostics.Debug.WriteLine($"[CP] Después Filtrar ({sw.ElapsedMilliseconds}ms)");

                    System.Diagnostics.Debug.WriteLine($"[CP] Antes base.InitializeAsync ({sw.ElapsedMilliseconds}ms)");
                    await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                    {
                        App.userdialog.HideLoading();
                        System.Diagnostics.Debug.WriteLine($"[CP] HideLoading ({sw.ElapsedMilliseconds}ms)");
                    }));
                    System.Diagnostics.Debug.WriteLine($"[CP] Después base.InitializeAsync ({sw.ElapsedMilliseconds}ms)");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[CP] ActualizaProductos (recarga) ({sw.ElapsedMilliseconds}ms)");
                    await ActualizaProductos();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CP] ERROR: {ex.Message} ({sw.ElapsedMilliseconds}ms)");
                App.userdialog.HideLoading();
            }

            System.Diagnostics.Debug.WriteLine($"[CP] FIN TOTAL: {sw.ElapsedMilliseconds}ms");

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Métodos
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
        /// <summary>
        /// OPTIMIZADO: Filtrar con debounce para búsqueda rápida
        /// </summary>
        private async Task FiltrarConDebounce()
        {
            await _searchDebouncer.DebounceAsync(async () =>
            {
                await FiltrarInterno();
            }, 200); // 200ms de delay
        }

        public async Task Filtrar()
        {
            await FiltrarInterno();
        }

        private async Task FiltrarInterno()
        {
            try
            {
                if (originalComidas == null) return;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (string.IsNullOrEmpty(TextoBusqueda))
                    {
                        Comidas = new ObservableCollection<Comida>(originalComidas);
                    }
                    else
                    {
                        // OPTIMIZADO: Usar LINQ con StringComparison (más rápido)
                        var paraBuscar = TextoBusqueda;
                        var resultados = originalComidas
                            .Where(a => a.articulo != null && (
                                (a.articulo.nombre?.Contains(paraBuscar, StringComparison.OrdinalIgnoreCase) == true) ||
                                (a.articulo.descripcion?.Contains(paraBuscar, StringComparison.OrdinalIgnoreCase) == true) ||
                                (a.articulo.alergenos?.Contains(paraBuscar, StringComparison.OrdinalIgnoreCase) == true) ||
                                (a.articulo.ingredientes?.Contains(paraBuscar, StringComparison.OrdinalIgnoreCase) == true)
                            ))
                            .ToList();

                        Comidas = new ObservableCollection<Comida>(resultados);
                    }

                    // DEBUG: Verificar estado final de Comidas
                    System.Diagnostics.Debug.WriteLine($"[BOTONES] Comidas asignadas: {Comidas?.Count ?? 0} items, ModoTienda={ModoTienda}");
                    if (Comidas?.Count > 0)
                    {
                        foreach (var c in Comidas.Take(3))
                        {
                            System.Diagnostics.Debug.WriteLine($"[BOTONES] -> '{c.articulo?.nombre}': noTieneOpciones={c.noTieneOpciones}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CartaProductos] Error filtrar: {ex.Message}");
            }
        }
        private async void articuloSeleccionado(object idArticulo)
        {
            if (_navegando) return;
            _navegando = true;

            // Mostrar loading INMEDIATAMENTE
            try { App.userdialog?.ShowLoading(AppResources.Cargando); } catch { }

            try
            {
                // Pequeña pausa para que el loading se renderice
                await Task.Delay(30);

                Comida articulo = (Comida)idArticulo;
                App.DAUtil.Idioma = "ES";
                await App.DAUtil.NavigationService.NavigateToAsync<DetalleArticuloViewModel>(articulo.articulo);
            }
            catch (Exception ex)
            {
                App.userdialog?.HideLoading();
                App.customDialog.ShowDialogAsync(AppResources.ErrorMensaje + ex.Message, AppResources.SoloError, AppResources.Cerrar);
            }
            finally
            {
                _navegando = false;
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
                        lista = App.listaProductos;
                    else
                    {
                        // OPTIMIZADO: Solo cargar de SQLite (ya sincronizado previamente)
                        lista = (await App.DAUtil.getProductosEstablecimientoCat(_categoria.id)).FindAll(p => p.estado == 1 && p.estadoCategoria == 1 && p.vistaEnvios == 1);
                        if (App.EstActual.configuracion?.aceptaEncargos == true)
                        {
                            App.listaProductos = lista;
                        }
                        else
                        {
                            lista = lista.FindAll(p => p.porEncargo == false);
                            App.listaProductos = lista;
                        }
                    }
                    originalComidas = new List<Comida>();
                    int idArt = 0;
                    carrito = App.DAUtil.Getcarrito();
                    // OPTIMIZADO: Usar LINQ Sum en lugar de foreach
                    Cantidad = carrito.Sum(c => c.cantidad).ToString();

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
                    OnPropertyChanged(nameof(TextoBusqueda));
                    // OPTIMIZADO: Usar debounce para no filtrar en cada tecla
                    _ = FiltrarConDebounce();
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
