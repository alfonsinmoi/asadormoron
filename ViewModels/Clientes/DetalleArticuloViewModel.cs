using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AsadorMoron.Recursos;
// 
using AsadorMoron.Services;

namespace AsadorMoron.ViewModels.Clientes
{

    public class DetalleArticuloViewModel : ViewModelBase
    {
        public DetalleArticuloViewModel()
        {
            App.entradoEnCarta = true;
            if (App.DAUtil.NotificacionPantalla.Equals(""))
            {
                if (App.userdialog == null)
                {
                    try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
            }
        }
        public bool esInicio = true;
        public async override Task InitializeAsync(object navigationData)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine($"[DA] 0 - INICIO InitializeAsync");

            bool esKiosko = (App.DAUtil.Usuario?.kiosko ?? 0) == 1;
            System.Diagnostics.Debug.WriteLine($"[DA] 1 - esKiosko={esKiosko} ({sw.ElapsedMilliseconds}ms)");

            try
            {
                App.DAUtil.EnTimer = false;
                System.Diagnostics.Debug.WriteLine($"[DA] 2 - EnTimer=false ({sw.ElapsedMilliseconds}ms)");

                Idioma = App.idioma;
                System.Diagnostics.Debug.WriteLine($"[DA] 3 - Idioma={Idioma} ({sw.ElapsedMilliseconds}ms)");

                VisiblePuntos = App.esPorPuntos;
                System.Diagnostics.Debug.WriteLine($"[DA] 4 - VisiblePuntos={VisiblePuntos} ({sw.ElapsedMilliseconds}ms)");

                if (navigationData != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[DA] 5 - navigationData NOT NULL ({sw.ElapsedMilliseconds}ms)");

                    // Asignar al campo directamente primero (sin notificación)
                    var articuloTemp = (ArticuloModel)navigationData;
                    System.Diagnostics.Debug.WriteLine($"[DA] 6a - Articulo cast: id={articuloTemp?.idArticulo} ({sw.ElapsedMilliseconds}ms)");

                    // Inicializar listas vacías ANTES de asignar
                    articuloTemp.listadoOpciones ??= new ObservableCollection<OpcionesModel>();
                    articuloTemp.listadoAlergenos ??= new ObservableCollection<AlergenosModel>();
                    articuloTemp.listadoIngredientes ??= new ObservableCollection<IngredienteProductoModel>();
                    System.Diagnostics.Debug.WriteLine($"[DA] 7 - Listas inicializadas ({sw.ElapsedMilliseconds}ms)");

                    // Ahora asignar (esto disparará OnPropertyChanged)
                    articulo = articuloTemp; // Asignar al campo, no a la propiedad
                    System.Diagnostics.Debug.WriteLine($"[DA] 6b - Articulo asignado al campo ({sw.ElapsedMilliseconds}ms)");
                    System.Diagnostics.Debug.WriteLine($"[DA] 6c - IMAGEN: '{articulo?.imagen}' ({sw.ElapsedMilliseconds}ms)");

                    // Carrito (operación local rápida)
                    carrito = App.DAUtil.Getcarrito();
                    System.Diagnostics.Debug.WriteLine($"[DA] 8 - Carrito obtenido: {carrito?.Count ?? 0} items ({sw.ElapsedMilliseconds}ms)");

                    int totalCantidad = carrito.Count;
                    foreach (CarritoModel c in carrito)
                        totalCantidad += c.cantidad;
                    cantidad = totalCantidad.ToString(); // Asignar a campo directamente
                    System.Diagnostics.Debug.WriteLine($"[DA] 9 - Cantidad={cantidad} ({sw.ElapsedMilliseconds}ms)");

                    // Combo especial - asignar a campo directamente
                    combo = Articulo.idArticulo == 11806;
                    System.Diagnostics.Debug.WriteLine($"[DA] 10 - Combo={combo} ({sw.ElapsedMilliseconds}ms)");

                    if (combo)
                    {
                        tieneOpciones = true;
                        Combo1 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 1));
                        Combo2 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 2));
                        Combo3 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 3));
                        Combo4 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 4));
                        Combo5 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 5));
                        Combo6 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 6));
                        Combo7 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 7));
                        Combo8 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 8));
                        if (Combo1.Count > 0) Combo1.First().Seleccionado = true;
                        if (Combo2.Count > 0) Combo2.First().Seleccionado = true;
                        if (Combo3.Count > 0) Combo3.First().Seleccionado = true;
                        if (Combo4.Count > 0) Combo4.First().Seleccionado = true;
                        if (Combo5.Count > 0) Combo5.First().Seleccionado = true;
                        if (Combo6.Count > 0) Combo6.First().Seleccionado = true;
                        if (Combo7.Count > 0) Combo7.First().Seleccionado = true;
                        if (Combo8.Count > 0) Combo8.First().Seleccionado = true;
                        System.Diagnostics.Debug.WriteLine($"[DA] 10b - Combos cargados ({sw.ElapsedMilliseconds}ms)");
                    }

                    // KIOSKO: Cargar datos sincrónicamente desde SQLite (más rápido)
                    // NO KIOSKO: Cargar del servidor en paralelo
                    if (esKiosko)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DA] 11 - KIOSKO: Cargando datos SQLite ({sw.ElapsedMilliseconds}ms)");

                        // Cargar opciones desde SQLite (instantáneo) - sin notificación
                        // NOTA: En Kiosko siempre intentamos cargar opciones desde SQLite,
                        // sin depender del campo Articulo.opciones que puede venir vacío
                        if (Articulo.listadoOpciones.Count == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"[DA] 12 - Cargando opciones... ({sw.ElapsedMilliseconds}ms)");
                            //var opciones = App.DAUtil.GetOpcionesProductoKiosko(Articulo.idArticulo);
                            var opciones = await App.ResponseWS.getOpcionesProducto(Articulo.idArticulo);
                            System.Diagnostics.Debug.WriteLine($"[DA] 13 - Opciones obtenidas: {opciones?.Count ?? 0} ({sw.ElapsedMilliseconds}ms)");
                            if (opciones?.Count > 0)
                                Articulo.listadoOpciones = new ObservableCollection<OpcionesModel>(opciones);
                            System.Diagnostics.Debug.WriteLine($"[DA] 14 - listadoOpciones asignada ({sw.ElapsedMilliseconds}ms)");
                        }

                        // Cargar ingredientes desde SQLite (síncrono para Kiosko - instantáneo)
                        System.Diagnostics.Debug.WriteLine($"[DA] 15 - Cargando ingredientes... ({sw.ElapsedMilliseconds}ms)");
                        var ingredientes = await App.DAUtil.GetIngredienteProducto(Articulo.idArticulo);
                        System.Diagnostics.Debug.WriteLine($"[DA] 16 - Ingredientes obtenidos: {ingredientes?.Count ?? 0} ({sw.ElapsedMilliseconds}ms)");
                        if (ingredientes?.Count > 0)
                            Articulo.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>(ingredientes);
                        System.Diagnostics.Debug.WriteLine($"[DA] 17 - listadoIngredientes asignado ({sw.ElapsedMilliseconds}ms)");

                        // Cargar alergenos desde SQLite - sin notificación
                        // NOTA: En Kiosko siempre intentamos cargar alérgenos desde SQLite
                        if (Articulo.listadoAlergenos.Count == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"[DA] 18 - Cargando alergenos... ({sw.ElapsedMilliseconds}ms)");
                            var alergenos = App.DAUtil.GetAlergenosProductoKiosko(Articulo.idArticulo);
                            System.Diagnostics.Debug.WriteLine($"[DA] 19 - Alergenos obtenidos: {alergenos?.Count ?? 0} ({sw.ElapsedMilliseconds}ms)");
                            if (alergenos?.Count > 0)
                                Articulo.listadoAlergenos = new ObservableCollection<AlergenosModel>(alergenos);
                            System.Diagnostics.Debug.WriteLine($"[DA] 20 - listadoAlergenos asignada ({sw.ElapsedMilliseconds}ms)");
                        }
                        System.Diagnostics.Debug.WriteLine($"[DA] 21 - KIOSKO datos cargados ({sw.ElapsedMilliseconds}ms)");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[DA] 11 - NO KIOSKO: Cargando del servidor ({sw.ElapsedMilliseconds}ms)");
                        // NO Kiosko: cargar del servidor en paralelo
                        var tareas = new List<Task>();
                        List<OpcionesModel> opcionesCargadas = null;
                        List<IngredienteProductoModel> ingredientesCargados = null;
                        List<AlergenosModel> alergenosCargados = null;

                        if (Articulo.listadoOpciones.Count == 0 && !string.IsNullOrEmpty(Articulo.opciones))
                        {
                            tareas.Add(Task.Run(async () =>
                            {
                                var opciones = await App.ResponseWS.getOpcionesProducto(Articulo.idArticulo);
                                if (opciones?.Count > 0) opcionesCargadas = opciones;
                            }));
                        }

                        tareas.Add(Task.Run(async () =>
                        {
                            var ingredientes = await App.DAUtil.GetIngredienteProducto(Articulo.idArticulo);
                            if (ingredientes?.Count > 0) ingredientesCargados = ingredientes;
                        }));

                        if (Articulo.listadoAlergenos.Count == 0 && !string.IsNullOrEmpty(Articulo.alergenos))
                        {
                            tareas.Add(Task.Run(async () =>
                            {
                                var alergenos = await App.ResponseWS.getAlergenosProducto(Articulo.idArticulo);
                                if (alergenos?.Count > 0) alergenosCargados = alergenos;
                            }));
                        }

                        System.Diagnostics.Debug.WriteLine($"[DA] 12 - Esperando {tareas.Count} tareas... ({sw.ElapsedMilliseconds}ms)");
                        if (tareas.Count > 0) await Task.WhenAll(tareas);
                        System.Diagnostics.Debug.WriteLine($"[DA] 13 - Tareas completadas ({sw.ElapsedMilliseconds}ms)");

                        // Asignar directamente sin OnPropertyChanged (se notifica al final)
                        if (opcionesCargadas?.Count > 0)
                            Articulo.listadoOpciones = new ObservableCollection<OpcionesModel>(opcionesCargadas);
                        if (ingredientesCargados?.Count > 0)
                            Articulo.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>(ingredientesCargados);
                        if (alergenosCargados?.Count > 0)
                            Articulo.listadoAlergenos = new ObservableCollection<AlergenosModel>(alergenosCargados);
                        System.Diagnostics.Debug.WriteLine($"[DA] 14 - Datos asignados ({sw.ElapsedMilliseconds}ms)");
                    }

                    System.Diagnostics.Debug.WriteLine($"[DA] 22 - Datos: opc={Articulo.listadoOpciones?.Count ?? 0}, ing={Articulo.listadoIngredientes?.Count ?? 0}, ale={Articulo.listadoAlergenos?.Count ?? 0} ({sw.ElapsedMilliseconds}ms)");

                    // Configuración del establecimiento (usar cache si existe)
                    cantidadIngredientes = Articulo.numeroIngredientes;
                    cantidadActual = 0;
                    System.Diagnostics.Debug.WriteLine($"[DA] 23 - cantidadIngredientes={cantidadIngredientes} ({sw.ElapsedMilliseconds}ms)");

                    if (App.DAUtil.Usuario != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DA] 24 - Usuario NOT NULL ({sw.ElapsedMilliseconds}ms)");

                        // Usar configuración cacheada si existe
                        var config = App.EstActual?.configuracion;
                        System.Diagnostics.Debug.WriteLine($"[DA] 25 - Config cacheada: {config != null} ({sw.ElapsedMilliseconds}ms)");

                        if (config == null && !esKiosko)
                        {
                            System.Diagnostics.Debug.WriteLine($"[DA] 26 - Cargando config del servidor... ({sw.ElapsedMilliseconds}ms)");
                            config = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual?.idEstablecimiento ?? 0);
                            System.Diagnostics.Debug.WriteLine($"[DA] 27 - Config del servidor obtenida ({sw.ElapsedMilliseconds}ms)");
                            if (App.EstActual != null)
                                App.EstActual.configuracion = config;
                        }

                        if (esKiosko)
                        {
                            System.Diagnostics.Debug.WriteLine($"[DA] 28 - Configurando modo Kiosko ({sw.ElapsedMilliseconds}ms)");
                            // Kiosko: modo tienda siempre activo, sin puntos - asignar a campos
                            modoTienda = true;
                            sistemaPuntos = false;
                            puntos = 0;
                            textoIngredientes = config?.textoIngredientes ?? "Ingredientes";
                            System.Diagnostics.Debug.WriteLine($"[DA] 29 - Kiosko configurado: ModoTienda={modoTienda} ({sw.ElapsedMilliseconds}ms)");
                        }
                        else if (config != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"[DA] 28 - Configurando modo normal ({sw.ElapsedMilliseconds}ms)");
                            modoTienda = !config.modoEscaparate;
                            sistemaPuntos = config.sistemaPuntos;
                            textoIngredientes = config.textoIngredientes;

                            if (sistemaPuntos)
                            {
                                System.Diagnostics.Debug.WriteLine($"[DA] 29 - Cargando puntos... ({sw.ElapsedMilliseconds}ms)");
                                puntos = ResponseServiceWS.getPuntosEstablecimiento();
                                System.Diagnostics.Debug.WriteLine($"[DA] 30 - Puntos obtenidos: {puntos} ({sw.ElapsedMilliseconds}ms)");
                                foreach (CarritoModel c in carrito.Where(p => p.porPuntos == 1))
                                    puntos -= c.puntos;
                            }
                            else
                                puntos = 0;

                            if (App.EstActual?.horario?.Equals(AppResources.Cerrado) == true)
                                modoTienda = false;
                            System.Diagnostics.Debug.WriteLine($"[DA] 31 - Config normal aplicada: ModoTienda={modoTienda} ({sw.ElapsedMilliseconds}ms)");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[DA] 24 - Usuario NULL, modo sin login ({sw.ElapsedMilliseconds}ms)");
                        modoTienda = false;
                        Articulo.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>();
                    }

                    //System.Diagnostics.Debug.WriteLine($"[DA] 32 - Config aplicada: ModoTienda={modoTienda}, esKiosko={esKiosko} ({sw.ElapsedMilliseconds}ms)");

                    // Preparar UI - asignar a campos directamente
                    System.Diagnostics.Debug.WriteLine($"[DA] 33 - Preparando UI... ({sw.ElapsedMilliseconds}ms)");
                    total = Articulo.precio;
                    totalPuntos = Articulo.puntos;
                    cantidadOpcion = 1;
                    textoCaja = AppResources.AñadirCarrito + " (" + total.ToString("N2") + "€)";
                    textoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + totalPuntos + " p.)";
                    System.Diagnostics.Debug.WriteLine($"[DA] 34 - Total={total}, TextoCaja asignado ({sw.ElapsedMilliseconds}ms)");

                    // Seleccionar primera opción - asignar a campo directamente
                    if (Articulo.listadoOpciones.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DA] 35 - Seleccionando primera opción... ({sw.ElapsedMilliseconds}ms)");
                        opcionSeleccionada = Articulo.listadoOpciones[0];
                        Articulo.listadoOpciones[0].seleccionado = true;
                        System.Diagnostics.Debug.WriteLine($"[DA] 36 - Primera opción seleccionada ({sw.ElapsedMilliseconds}ms)");
                    }

                    // Procesar opciones para visibilidad de puntos - usar campos
                    System.Diagnostics.Debug.WriteLine($"[DA] 37 - Procesando {Articulo.listadoOpciones.Count} opciones... ({sw.ElapsedMilliseconds}ms)");
                    foreach (OpcionesModel op in Articulo.listadoOpciones)
                    {
                        try
                        {
                            var carritoItem = carrito.Find(pr => pr.idArticulo == Articulo.idArticulo && pr.opcion == op.id);
                            op.cantidad = carritoItem?.cantidad ?? 0;
                            op.visiblePuntos = totalPuntos + op.puntos <= puntos && sistemaPuntos && totalPuntos > 0;
                        }
                        catch
                        {
                            op.cantidad = 0;
                            op.visiblePuntos = false;
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"[DA] 38 - Opciones procesadas ({sw.ElapsedMilliseconds}ms)");

                    // Procesar ingredientes - crear lista primero, asignar al CAMPO (sin notificación)
                    System.Diagnostics.Debug.WriteLine($"[DA] 39 - Procesando {Articulo.listadoIngredientes.Count} ingredientes... ({sw.ElapsedMilliseconds}ms)");
                    var listaIngredientesTemp = new List<IngredienteProductoBindableModel>(Articulo.listadoIngredientes.Count);
                    foreach (IngredienteProductoModel l in Articulo.listadoIngredientes)
                    {
                        listaIngredientesTemp.Add(new IngredienteProductoBindableModel
                        {
                            Cantidad = 0,
                            id = l.id,
                            idIngrediente = l.idIngrediente,
                            idProducto = l.idProducto,
                            nombre = l.nombre,
                            nombre_eng = l.nombre_eng,
                            nombre_ger = l.nombre_ger,
                            nombre_fr = l.nombre_fr,
                            precio = l.precio,
                            puntos = l.puntos,
                            visiblePuntos = totalPuntos + l.puntos <= puntos && sistemaPuntos && l.puntos > 0
                        });
                    }
                    // Asignar al campo directamente (sin OnPropertyChanged - se notifica al final)
                    listadoIngredientes = new ObservableCollection<IngredienteProductoBindableModel>(listaIngredientesTemp);
                    System.Diagnostics.Debug.WriteLine($"[DA] 40 - Ingredientes procesados ({sw.ElapsedMilliseconds}ms)");

                    // Establecer flags de visibilidad - asignar a campos directamente (sin OnPropertyChanged)
                    tieneAlergenos = Articulo.listadoAlergenos.Count > 0;
                    tieneIngredientees = Articulo.listadoIngredientes.Count > 0;
                    tieneOpciones = Articulo.listadoOpciones.Count > 0 || combo;
                    System.Diagnostics.Debug.WriteLine($"[DA] 41 - Flags: Opc={tieneOpciones}, Ing={tieneIngredientees}, Ale={tieneAlergenos}, Tienda={modoTienda} ({sw.ElapsedMilliseconds}ms)");

                    // Forzar actualización de TODOS los bindings relevantes - una sola vez al final
                    System.Diagnostics.Debug.WriteLine($"[DA] 42 - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    // Notificar todas las propiedades de una vez
                    System.Diagnostics.Debug.WriteLine($"[DA] 42A - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(OpcionesLista));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42B - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(AlergenosLista));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42C - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(ListadoIngredientes));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42D - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(TieneOpciones));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42E - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(TieneIngredientes));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42F - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(TieneAlergenos));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42G - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(ModoTienda));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42H - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(TextoCaja));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42I - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(TextoCajaPuntos));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42J - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(Total));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42K - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(TotalPuntos));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42L - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(CantidadOpcion));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42M - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(Cantidad));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42N - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(Combo));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42O - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(SistemaPuntos));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42P - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(Puntos));
                    System.Diagnostics.Debug.WriteLine($"[DA] 42Q - OnPropertyChanged batch... ({sw.ElapsedMilliseconds}ms)");
                    OnPropertyChanged(nameof(OpcionSeleccionada));
                    OnPropertyChanged(nameof(Articulo)); // Para la imagen
                    System.Diagnostics.Debug.WriteLine($"[DA] 43 - OnPropertyChanged completado ({sw.ElapsedMilliseconds}ms)");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[DA] 5 - navigationData IS NULL! ({sw.ElapsedMilliseconds}ms)");
                }
                esInicio = false;
                System.Diagnostics.Debug.WriteLine($"[DA] 44 - esInicio=false ({sw.ElapsedMilliseconds}ms)");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DA] ERROR: {ex.Message}\n{ex.StackTrace}");
                App.userdialog.HideLoading();
            }

            System.Diagnostics.Debug.WriteLine($"[DA] 45 - FIN TOTAL: {sw.ElapsedMilliseconds}ms");
            System.Diagnostics.Debug.WriteLine($"[DA] 46 - Llamando base.InitializeAsync...");
            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
            System.Diagnostics.Debug.WriteLine($"[DA] 47 - FIN COMPLETO: {sw.ElapsedMilliseconds}ms");
        }

        #region Metodos

        private void ActualizarCaja()
        {
            try
            {
                if (OpcionSeleccionada == null && Articulo.listadoOpciones.Count > 0)
                {
                    App.customDialog.ShowDialogAsync(AppResources.SeleccioneOpcion, AppResources.App, AppResources.Cerrar);
                }
                else if (CantidadOpcion == 0)
                {
                    App.customDialog.ShowDialogAsync(AppResources.IndiqueCantidad, AppResources.App, AppResources.Cerrar);
                }
                else if (cantidadIngredientes != cantidadActual && Articulo.fuerzaIngredientes == 1)
                {
                    App.customDialog.ShowDialogAsync(AppResources.IndiqueNumeroIngredientes, AppResources.App, AppResources.Cerrar);
                }
                else
                {
                    CarritoModel c = new CarritoModel();
                    c.id = -1;
                    c.cantidad = CantidadOpcion;
                    if (!Combo)
                    {
                        if (OpcionSeleccionada != null)
                        {
                            c.comida = Articulo.nombre + " (" + OpcionSeleccionada.opcion + ")";
                            c.comida_eng = Articulo.nombre_eng + " (" + OpcionSeleccionada.opcion_eng + ")";
                            c.comida_ger = Articulo.nombre_ger + " (" + OpcionSeleccionada.opcion_ger + ")";
                            c.comida_fr = Articulo.nombre_eng + " (" + OpcionSeleccionada.opcion_fr + ")";
                        }
                        else
                        {
                            c.comida = Articulo.nombre;
                            c.comida_eng = Articulo.nombre_eng;
                            c.comida_ger = Articulo.nombre_ger;
                            c.comida_fr = Articulo.nombre_fr;
                        }
                    }
                    else
                    {
                        c.comida = Combo1.First(p => p.Seleccionado == true).nombre + ", " + Combo2.First(p => p.Seleccionado == true).nombre + ", " + Combo3.First(p => p.Seleccionado == true).nombre + ", ";
                        c.comida += Combo4.First(p => p.Seleccionado == true).nombre + ", " + Combo5.First(p => p.Seleccionado == true).nombre + ", " + Combo6.First(p => p.Seleccionado == true).nombre + ", ";
                        c.comida += Combo7.First(p => p.Seleccionado == true).nombre + " y " + Combo8.First(p => p.Seleccionado == true).nombre + ", ";
                        c.comida = c.comida.ToUpper();
                        c.comida_eng = c.comida;
                        c.comida_ger = c.comida;
                        c.comida_fr = c.comida;
                    }
                    foreach (IngredienteProductoBindableModel p in ListadoIngredientes)
                    {
                        if (p.Cantidad > 0)
                        {
                            c.comida += Environment.NewLine + p.Cantidad + " x " + p.nombre;
                            c.comida_eng += Environment.NewLine + p.Cantidad + " x " + p.nombre_eng;
                            c.comida_ger += Environment.NewLine + p.Cantidad + " x " + p.nombre_ger;
                            c.comida_fr += Environment.NewLine + p.Cantidad + " x " + p.nombre_fr;
                        }
                    }
                    c.porEncargo = Articulo.porEncargo;
                    c.idEstablecimiento = Articulo.idEstablecimiento;
                    c.idArticulo = Articulo.idArticulo;
                    c.imagen = Articulo.imagen;
                    c.observaciones = "";
                    c.comentario = Comentario;
                    c.precio = Total / c.cantidad;
                    c.puntos = 0;
                    c.porPuntos = 0;
                    if (OpcionSeleccionada != null)
                        c.opcion = opcionSeleccionada.id;
                    else
                        c.opcion = 0;

                    carrito.Add(c);
                    App.DAUtil.ActualizaCarrito(carrito);
                    /*MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.DAUtil.NavigationService.NavigateBackAsync();
                    });*/
                    App.DAUtil.NavigationService.NavigateBackAsync();
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void ActualizarCajaPuntos()
        {
            try
            {
                if (TotalPuntos > Puntos)
                {
                    App.customDialog.ShowDialogAsync("No tiene suficientes puntos", AppResources.App, AppResources.Cerrar);
                }
                else if (OpcionSeleccionada == null && Articulo.listadoOpciones.Count > 0)
                {
                    App.customDialog.ShowDialogAsync(AppResources.SeleccioneOpcion, AppResources.App, AppResources.Cerrar);
                }
                else if (CantidadOpcion == 0)
                {
                    App.customDialog.ShowDialogAsync(AppResources.IndiqueCantidad, AppResources.App, AppResources.Cerrar);
                }
                else if (cantidadIngredientes != cantidadActual && Articulo.fuerzaIngredientes == 1)
                {
                    App.customDialog.ShowDialogAsync(AppResources.IndiqueNumeroIngredientes, AppResources.App, AppResources.Cerrar);
                }
                else if (Articulo.puntos > Puntos)
                    App.customDialog.ShowDialogAsync("No tiene suficientes puntos", AppResources.App, AppResources.Cerrar);
                else
                {
                    CarritoModel c = new CarritoModel();
                    c.id = -1;
                    c.cantidad = CantidadOpcion;
                    if (OpcionSeleccionada != null)
                    {
                        c.comida = Articulo.nombre + " (" + OpcionSeleccionada.opcion + ")";
                        c.comida_eng = Articulo.nombre_eng + " (" + OpcionSeleccionada.opcion_eng + ")";
                        c.comida_ger = Articulo.nombre_ger + " (" + OpcionSeleccionada.opcion_ger + ")";
                        c.comida_fr = Articulo.nombre_eng + " (" + OpcionSeleccionada.opcion_fr + ")";
                    }
                    else
                    {
                        c.comida = Articulo.nombre;
                        c.comida_eng = Articulo.nombre_eng;
                        c.comida_ger = Articulo.nombre_ger;
                        c.comida_fr = Articulo.nombre_fr;
                    }
                    foreach (IngredienteProductoBindableModel p in ListadoIngredientes)
                    {
                        if (p.Cantidad > 0)
                        {
                            c.comida += Environment.NewLine + p.Cantidad + " x " + p.nombre;
                            c.comida_eng += Environment.NewLine + p.Cantidad + " x " + p.nombre_eng;
                            c.comida_ger += Environment.NewLine + p.Cantidad + " x " + p.nombre_ger;
                            c.comida_fr += Environment.NewLine + p.Cantidad + " x " + p.nombre_fr;
                        }
                    }
                    c.porEncargo = Articulo.porEncargo;
                    c.idEstablecimiento = Articulo.idEstablecimiento;
                    c.idArticulo = Articulo.idArticulo;
                    c.imagen = Articulo.imagen;
                    c.observaciones = "";
                    c.comentario = Comentario;
                    c.puntos = TotalPuntos;
                    c.porPuntos = 1;
                    c.precio = Total / c.cantidad;
                    if (OpcionSeleccionada != null)
                        c.opcion = opcionSeleccionada.id;
                    else
                        c.opcion = 0;

                    carrito.Add(c);
                    App.DAUtil.ActualizaCarrito(carrito);
                    App.DAUtil.NavigationService.NavigateBackAsync();
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private bool _navegandoPedido = false;
        private async void IrDetallePedido()
        {
            if (_navegandoPedido) return;
            _navegandoPedido = true;

            try
            {
                if (carrito.Count > 0)
                {
                    // Mostrar loading INMEDIATAMENTE
                    try { App.userdialog?.ShowLoading(AppResources.Cargando); } catch { }

                    // Pequeña pausa para que el loading se renderice
                    await Task.Delay(30);

                    App.DAUtil.Idioma = "ES";
                    await App.DAUtil.NavigationService.NavigateToAsync<DetallePedidoViewModel>(carrito);
                }
            }
            catch (Exception ex)
            {
                App.userdialog?.HideLoading();
                System.Diagnostics.Debug.WriteLine($"[IrDetallePedido] Error: {ex.Message}");
            }
            finally
            {
                _navegandoPedido = false;
            }
        }

        private void add(object parametro)
        {
            try
            {
                CantidadOpcion++;
                if (OpcionSeleccionada != null)
                {
                    Total = App.ParseaPrecio(OpcionSeleccionada.precio) * cantidadOpcion;
                    TotalPuntos = OpcionSeleccionada.puntos * cantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += CantidadOpcion * l.puntos;
                    }
                    TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                else
                {
                    Total = Articulo.precio * cantidadOpcion;
                    TotalPuntos = Articulo.puntos * cantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += CantidadOpcion * l.puntos;
                    }
                    TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                //VisiblePuntos = TotalPuntos <= Puntos && SistemaPuntos && TotalPuntos > 0;
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void addIng(object parametro)
        {
            try
            {
                int idIngrediente = (int)parametro;
                if (cantidadIngredientes == 0 || cantidadIngredientes > cantidadActual)
                {
                    IngredienteProductoBindableModel opt = ListadoIngredientes.Where((obj) => obj.idIngrediente == idIngrediente).FirstOrDefault();
                    opt.Cantidad++;
                    cantidadActual++;
                    if (OpcionSeleccionada != null)
                    {
                        Total = App.ParseaPrecio(OpcionSeleccionada.precio) * CantidadOpcion;
                        TotalPuntos = OpcionSeleccionada.puntos * CantidadOpcion;
                        foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                        {
                            Total += l.precio * l.Cantidad * CantidadOpcion;
                            TotalPuntos += l.puntos * CantidadOpcion;
                        }
                        TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                        TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                    }
                    else
                    {
                        Total = Articulo.precio * cantidadOpcion;
                        TotalPuntos = Articulo.puntos * cantidadOpcion;
                        foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                        {
                            Total += l.precio * l.Cantidad * CantidadOpcion;
                            TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                        }
                        TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                        TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                    }
                    //VisiblePuntos = TotalPuntos <= Puntos && SistemaPuntos && TotalPuntos > 0;
                }
                else if (cantidadIngredientes > 0)
                {
                    App.customDialog.ShowDialogAsync(AppResources.NoPuedeSeleccionarMas + TextoIngredientes, AppResources.App, AppResources.Cerrar);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void removeIng(object parametro)
        {
            try
            {
                int idIngrediente = (int)parametro;
                IngredienteProductoBindableModel opt = ListadoIngredientes.Where((obj) => obj.idIngrediente == idIngrediente).FirstOrDefault();
                if (opt.Cantidad > 0)
                    opt.Cantidad--;
                if (cantidadActual > 0)
                    cantidadActual--;
                if (OpcionSeleccionada != null)
                {
                    Total = App.ParseaPrecio(OpcionSeleccionada.precio) * CantidadOpcion;
                    TotalPuntos = OpcionSeleccionada.puntos * CantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                    }
                    TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                else
                {
                    Total = Articulo.precio * cantidadOpcion;
                    TotalPuntos = Articulo.puntos * cantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                    }
                    TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                //VisiblePuntos = TotalPuntos <= Puntos && SistemaPuntos && TotalPuntos > 0;
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
                if (CantidadOpcion > 0)
                    CantidadOpcion--;

                if (OpcionSeleccionada != null)
                {
                    Total = App.ParseaPrecio(OpcionSeleccionada.precio) * CantidadOpcion;
                    TotalPuntos = OpcionSeleccionada.puntos * CantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                    }
                    TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                else
                {
                    Total = Articulo.precio * cantidadOpcion;
                    TotalPuntos = Articulo.puntos * cantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                    }
                    TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                //VisiblePuntos = TotalPuntos <= Puntos && SistemaPuntos && TotalPuntos > 0;
            }
            catch (Exception ex)
            {
                // 
            }
        }

        #endregion

        #region Comandos

        public ICommand cmdActualizarCaja { get { return new Command(ActualizarCaja); } }
        public ICommand cmdActualizarCajaPuntos { get { return new Command(ActualizarCajaPuntos); } }
        public ICommand IrDetallePedidoCommand { get { return new Command(IrDetallePedido); } }
        public ICommand ClickMasCommand { get { return new Command((parametro) => add(parametro)); } }
        public ICommand ClickMasIngCommand { get { return new Command((parametro) => addIng(parametro)); } }
        public ICommand ClickMenosIngCommand { get { return new Command((parametro) => removeIng(parametro)); } }
        public ICommand ClickMenosCommand { get { return new Command((parametro) => remove(parametro)); } }

        #endregion

        #region Propiedades


        List<CarritoModel> carrito = new List<CarritoModel>();
        private ArticuloModel articulo;
        public ArticuloModel Articulo
        {
            get
            {
                return articulo;
            }
            set
            {
                if (articulo != value)
                {
                    articulo = value;
                    OnPropertyChanged(nameof(Articulo));
                    // Notificar también las propiedades derivadas
                    OnPropertyChanged(nameof(OpcionesLista));
                    OnPropertyChanged(nameof(AlergenosLista));
                }
            }
        }

        // Propiedades directas para listas - binding más rápido que Articulo.listadoOpciones
        public ObservableCollection<OpcionesModel> OpcionesLista
        {
            get => Articulo?.listadoOpciones ?? new ObservableCollection<OpcionesModel>();
            set
            {
                if (Articulo != null)
                {
                    Articulo.listadoOpciones = value;
                    OnPropertyChanged(nameof(OpcionesLista));
                }
            }
        }

        public ObservableCollection<AlergenosModel> AlergenosLista
        {
            get => Articulo?.listadoAlergenos ?? new ObservableCollection<AlergenosModel>();
            set
            {
                if (Articulo != null)
                {
                    Articulo.listadoAlergenos = value;
                    OnPropertyChanged(nameof(AlergenosLista));
                }
            }
        }

        private bool combo;
        public bool Combo
        {
            get
            {
                return combo;
            }
            set
            {
                if (combo != value)
                {
                    combo = value;
                    OnPropertyChanged(nameof(Combo));
                }
            }
        }
        private int cantidadIngredientes = 0;
        private int cantidadActual = 0;
        private bool tieneOpcionSeleccionada;
        public bool TieneOpcionSeleccionada
        {
            get
            {
                return tieneOpcionSeleccionada;
            }
            set
            {
                if (tieneOpcionSeleccionada != value)
                {
                    tieneOpcionSeleccionada = value;
                    OnPropertyChanged(nameof(TieneOpcionSeleccionada));
                }
            }
        }
        private bool modoTienda;
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
        private bool visiblePuntos;
        public bool VisiblePuntos
        {
            get { return visiblePuntos; }
            set
            {
                if (visiblePuntos != value)
                {
                    visiblePuntos = value;
                    OnPropertyChanged(nameof(VisiblePuntos));
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
        private string textoIngredientes;
        public string TextoIngredientes
        {
            get { return textoIngredientes; }
            set
            {
                if (textoIngredientes != value)
                {
                    textoIngredientes = value;
                    OnPropertyChanged(nameof(TextoIngredientes));
                }
            }
        }
        private bool tieneOpciones;
        public bool TieneOpciones
        {
            get { return tieneOpciones; }
            set
            {
                if (tieneOpciones != value)
                {
                    tieneOpciones = value;
                    OnPropertyChanged(nameof(TieneOpciones));
                }
            }
        }
        private bool tieneIngredientees;
        public bool TieneIngredientes
        {
            get
            {
                return tieneIngredientees;
            }
            set
            {
                if (tieneIngredientees != value)
                {
                    tieneIngredientees = value;
                    OnPropertyChanged(nameof(TieneIngredientes));
                }
            }
        }
        private int cantidadOpcion;
        public int CantidadOpcion
        {
            get
            {
                return cantidadOpcion;
            }
            set
            {
                if (cantidadOpcion != value)
                {
                    cantidadOpcion = value;
                    OnPropertyChanged(nameof(CantidadOpcion));
                }
            }
        }

        private OpcionesModel opcionSeleccionada;
        public OpcionesModel OpcionSeleccionada
        {
            get
            {
                return opcionSeleccionada;
            }
            set
            {
                if (opcionSeleccionada != value)
                {
                    opcionSeleccionada = value;
                    OnPropertyChanged(nameof(OpcionSeleccionada));
                    if (CantidadOpcion == 0)
                        CantidadOpcion = 1;
                    try
                    {
                        Total = App.ParseaPrecio(OpcionSeleccionada.precio) * CantidadOpcion;
                        TotalPuntos = OpcionSeleccionada.puntos * CantidadOpcion;
                        foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                        {
                            Total += l.precio * l.Cantidad * CantidadOpcion;
                            TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                        }
                        TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                        TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                        TieneOpcionSeleccionada = true;
                    }
                    catch (Exception ex)
                    {
                        // 
                    }
                }
            }
        }
        private ObservableCollection<IngredienteProductoBindableModel> listadoIngredientes;
        public ObservableCollection<IngredienteProductoBindableModel> ListadoIngredientes
        {
            get
            {
                return listadoIngredientes;
            }
            set
            {
                if (listadoIngredientes != value)
                {
                    listadoIngredientes = value;
                    OnPropertyChanged(nameof(ListadoIngredientes));
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
        private string textoCaja;
        public string TextoCaja
        {
            get
            {
                return textoCaja;
            }
            set
            {
                if (textoCaja != value)
                {
                    textoCaja = value;
                    OnPropertyChanged(nameof(TextoCaja));
                }
            }
        }

        private ObservableCollection<ComboModel> combo1;
        public ObservableCollection<ComboModel> Combo1
        {
            get
            {
                return combo1;
            }
            set
            {
                if (combo1 != value)
                {
                    combo1 = value;
                    OnPropertyChanged(nameof(Combo1));
                }
            }
        }
        private ObservableCollection<ComboModel> combo2;
        public ObservableCollection<ComboModel> Combo2
        {
            get
            {
                return combo2;
            }
            set
            {
                if (combo2 != value)
                {
                    combo2 = value;
                    OnPropertyChanged(nameof(Combo2));
                }
            }
        }
        private ObservableCollection<ComboModel> combo3;
        public ObservableCollection<ComboModel> Combo3
        {
            get
            {
                return combo3;
            }
            set
            {
                if (combo3 != value)
                {
                    combo3 = value;
                    OnPropertyChanged(nameof(Combo3));
                }
            }
        }
        private ObservableCollection<ComboModel> combo4;
        public ObservableCollection<ComboModel> Combo4
        {
            get
            {
                return combo4;
            }
            set
            {
                if (combo4 != value)
                {
                    combo4 = value;
                    OnPropertyChanged(nameof(Combo4));
                }
            }
        }
        private ObservableCollection<ComboModel> combo5;
        public ObservableCollection<ComboModel> Combo5
        {
            get
            {
                return combo5;
            }
            set
            {
                if (combo5 != value)
                {
                    combo5 = value;
                    OnPropertyChanged(nameof(Combo5));
                }
            }
        }
        private ObservableCollection<ComboModel> combo6;
        public ObservableCollection<ComboModel> Combo6
        {
            get
            {
                return combo6;
            }
            set
            {
                if (combo6 != value)
                {
                    combo6 = value;
                    OnPropertyChanged(nameof(Combo6));
                }
            }
        }
        private ObservableCollection<ComboModel> combo7;
        public ObservableCollection<ComboModel> Combo7
        {
            get
            {
                return combo7;
            }
            set
            {
                if (combo7 != value)
                {
                    combo7 = value;
                    OnPropertyChanged(nameof(Combo7));
                }
            }
        }
        private ObservableCollection<ComboModel> combo8;
        public ObservableCollection<ComboModel> Combo8
        {
            get
            {
                return combo8;
            }
            set
            {
                if (combo8 != value)
                {
                    combo8 = value;
                    OnPropertyChanged(nameof(Combo8));
                }
            }
        }
        private string textoCajaPuntos;
        public string TextoCajaPuntos
        {
            get
            {
                return textoCajaPuntos;
            }
            set
            {
                if (textoCajaPuntos != value)
                {
                    textoCajaPuntos = value;
                    OnPropertyChanged(nameof(TextoCajaPuntos));
                }
            }
        }

        private double total;
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
        private int totalPuntos;
        public int TotalPuntos
        {
            get
            {
                return totalPuntos;
            }
            set
            {
                if (totalPuntos != value)
                {
                    totalPuntos = value;
                    OnPropertyChanged(nameof(TotalPuntos));
                }
            }
        }

        private string cantidad;
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
        private bool tieneAlergenos;
        public bool TieneAlergenos
        {
            get { return tieneAlergenos; }
            set
            {
                if (tieneAlergenos != value)
                {
                    tieneAlergenos = value;
                    OnPropertyChanged(nameof(TieneAlergenos));
                }
            }
        }
        private string comentario = "";
        public string Comentario
        {
            get { return comentario; }
            set
            {
                if (comentario != value)
                {
                    comentario = value;
                    OnPropertyChanged(nameof(Comentario));
                }
            }
        }
        #endregion


    }
}