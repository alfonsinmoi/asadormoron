using AsadorMoron.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AsadorMoron.Services
{
    /// <summary>
    /// Servicio para precargar todos los datos necesarios en modo Kiosko.
    /// Carga productos, opciones, ingredientes, alergenos y categorias al inicio
    /// y los guarda en SQLite para uso offline durante todo el flujo.
    /// </summary>
    public class KioskoPreloadService
    {
        private static KioskoPreloadService _instance;
        public static KioskoPreloadService Instance => _instance ??= new KioskoPreloadService();

        /// <summary>
        /// Indica si los datos de Kiosko ya han sido precargados
        /// </summary>
        public bool DatosPrecargados { get; private set; } = false;

        /// <summary>
        /// Lista de productos precargados con todas sus opciones, ingredientes y alergenos
        /// </summary>
        public List<ArticuloModel> ProductosPrecargados { get; private set; }

        /// <summary>
        /// Lista de categorias precargadas
        /// </summary>
        public List<Categoria> CategoriasPrecargadas { get; private set; }

        /// <summary>
        /// Verifica si el usuario actual es Kiosko
        /// </summary>
        public static bool EsKiosko => (App.DAUtil?.Usuario?.kiosko ?? 0) == 1;

        /// <summary>
        /// Precarga todos los datos necesarios para el modo Kiosko.
        /// Debe llamarse al inicio de la app despues del login.
        /// </summary>
        public async Task<bool> PrecargarDatosKioskoAsync(int idEstablecimiento)
        {
            if (!EsKiosko)
            {
                Debug.WriteLine("[Kiosko] El usuario no es Kiosko, saltando precarga");
                return false;
            }

            var sw = Stopwatch.StartNew();
            Debug.WriteLine($"[Kiosko] Iniciando precarga de datos para establecimiento {idEstablecimiento}...");

            try
            {
                // Cargar categorias y productos en paralelo
                var categoriasTask = CargarCategoriasAsync(idEstablecimiento);
                var productosTask = CargarProductosCompletosAsync(idEstablecimiento);

                await Task.WhenAll(categoriasTask, productosTask);

                CategoriasPrecargadas = await categoriasTask;
                ProductosPrecargados = await productosTask;

                // Guardar todo en SQLite
                await GuardarEnSQLiteAsync();

                DatosPrecargados = true;
                Debug.WriteLine($"[Kiosko] Precarga completada en {sw.ElapsedMilliseconds}ms");
                Debug.WriteLine($"[Kiosko] Categorias: {CategoriasPrecargadas?.Count ?? 0}, Productos: {ProductosPrecargados?.Count ?? 0}");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Kiosko] Error en precarga: {ex.Message}");
                DatosPrecargados = false;
                return false;
            }
        }

        /// <summary>
        /// Carga las categorias del establecimiento
        /// </summary>
        private async Task<List<Categoria>> CargarCategoriasAsync(int idEstablecimiento)
        {
            try
            {
                string url = $"{App.DAUtil.miURL}categorias.php/GET?idEstablecimiento={idEstablecimiento}";
                var response = await App.Client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && !json.ToLower().Equals("false"))
                {
                    var categorias = JsonConvert.DeserializeObject<List<Categoria>>(json);
                    Debug.WriteLine($"[Kiosko] Categorias cargadas: {categorias?.Count ?? 0}");
                    return categorias ?? new List<Categoria>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Kiosko] Error cargando categorias: {ex.Message}");
            }
            return new List<Categoria>();
        }

        /// <summary>
        /// Carga TODOS los productos con sus opciones, ingredientes y alergenos
        /// </summary>
        private async Task<List<ArticuloModel>> CargarProductosCompletosAsync(int idEstablecimiento)
        {
            var productos = new List<ArticuloModel>();

            try
            {
                // Cargar productos basicos
                string url = $"{App.DAUtil.miURL}productos.php/GET?idEstablecimientoProducto={idEstablecimiento}";
                var response = await App.Client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && !json.ToLower().Equals("false"))
                {
                    productos = JsonConvert.DeserializeObject<List<ArticuloModel>>(json) ?? new List<ArticuloModel>();
                    Debug.WriteLine($"[Kiosko] Productos basicos cargados: {productos.Count}");
                }

                // Para cada producto, cargar opciones, ingredientes y alergenos en paralelo
                var tareas = new List<Task>();
                foreach (var producto in productos)
                {
                    tareas.Add(CargarDetallesProductoAsync(producto));
                }

                await Task.WhenAll(tareas);
                Debug.WriteLine($"[Kiosko] Detalles de productos cargados");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Kiosko] Error cargando productos: {ex.Message}");
            }

            return productos;
        }

        /// <summary>
        /// Carga opciones, ingredientes y alergenos para un producto
        /// </summary>
        private async Task CargarDetallesProductoAsync(ArticuloModel producto)
        {
            try
            {
                // Inicializar listas
                producto.listadoOpciones ??= new ObservableCollection<OpcionesModel>();
                producto.listadoIngredientes ??= new ObservableCollection<IngredienteProductoModel>();
                producto.listadoAlergenos ??= new ObservableCollection<AlergenosModel>();

                var tareas = new List<Task>();

                // Cargar opciones - siempre intentar cargar del servidor
                // (el campo producto.opciones puede venir vacío pero aún así tener opciones)
                if (!string.IsNullOrEmpty(producto.ingredientes))
                {
                tareas.Add(Task.Run(async () =>
                {
                    var opciones = await CargarOpcionesAsync(producto.idArticulo);
                    Debug.WriteLine($"[Kiosko] Producto {producto.idArticulo} ({producto.nombre}): {opciones?.Count ?? 0} opciones cargadas del servidor");
                    if (opciones != null && opciones.Count > 0)
                    {
                        producto.listadoOpciones = new ObservableCollection<OpcionesModel>(opciones);
                    }
                }));
                }
                // Cargar ingredientes si tiene
                if (!string.IsNullOrEmpty(producto.opciones))
                {
                    tareas.Add(Task.Run(async () =>
                    {
                        var ingredientes = await CargarIngredientesAsync(producto.idArticulo);
                        if (ingredientes != null && ingredientes.Count > 0)
                        {
                            producto.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>(ingredientes);
                        }
                    }));
                }

                // Cargar alergenos - siempre intentar cargar del servidor
                tareas.Add(Task.Run(async () =>
                {
                    var alergenos = await CargarAlergenosAsync(producto.idArticulo);
                    if (alergenos != null && alergenos.Count > 0)
                    {
                        producto.listadoAlergenos = new ObservableCollection<AlergenosModel>(alergenos);
                    }
                }));

                await Task.WhenAll(tareas);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Kiosko] Error cargando detalles del producto {producto.idArticulo}: {ex.Message}");
            }
        }

        /// <summary>
        /// Carga las opciones de un producto
        /// </summary>
        private async Task<List<OpcionesModel>> CargarOpcionesAsync(int idProducto)
        {
            try
            {
                string url = $"{App.DAUtil.miURL}productos.php/GET?idOpcionesProducto={idProducto}";
                var response = await App.Client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && !json.ToLower().Equals("false"))
                {
                    return JsonConvert.DeserializeObject<List<OpcionesModel>>(json);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Kiosko] Error cargando opciones producto {idProducto}: {ex.Message}");
            }
            return new List<OpcionesModel>();
        }

        /// <summary>
        /// Carga los ingredientes de un producto
        /// </summary>
        private async Task<List<IngredienteProductoModel>> CargarIngredientesAsync(int idProducto)
        {
            try
            {
                string url = $"{App.DAUtil.miURL}productos.php/GET?idIngredientesProducto={idProducto}";
                var response = await App.Client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && !json.ToLower().Equals("false"))
                {
                    return JsonConvert.DeserializeObject<List<IngredienteProductoModel>>(json);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Kiosko] Error cargando ingredientes producto {idProducto}: {ex.Message}");
            }
            return new List<IngredienteProductoModel>();
        }

        /// <summary>
        /// Carga los alergenos de un producto
        /// </summary>
        private async Task<List<AlergenosModel>> CargarAlergenosAsync(int idProducto)
        {
            try
            {
                string url = $"{App.DAUtil.miURL}productos.php/GET?idAlergenosProducto={idProducto}";
                var response = await App.Client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && !json.ToLower().Equals("false"))
                {
                    return JsonConvert.DeserializeObject<List<AlergenosModel>>(json);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Kiosko] Error cargando alergenos producto {idProducto}: {ex.Message}");
            }
            return new List<AlergenosModel>();
        }

        /// <summary>
        /// Guarda todos los datos precargados en SQLite
        /// </summary>
        private async Task GuardarEnSQLiteAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    // Guardar categorias
                    if (CategoriasPrecargadas != null && CategoriasPrecargadas.Count > 0)
                    {
                        App.DAUtil.GuardarCategoriasKiosko(CategoriasPrecargadas);
                    }

                    // OPTIMIZADO: Usar método batch para guardar todos los productos en un solo lock
                    // Esto evita contención con otras operaciones SQLite (CartaViewModel, etc)
                    if (ProductosPrecargados != null && ProductosPrecargados.Count > 0)
                    {
                        App.DAUtil.GuardarTodosProductosKioskoBatch(ProductosPrecargados);
                    }

                    Debug.WriteLine("[Kiosko] Datos guardados en SQLite correctamente");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Kiosko] Error guardando en SQLite: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Obtiene los productos desde SQLite (para uso durante el flujo de Kiosko)
        /// </summary>
        public List<ArticuloModel> ObtenerProductosDesdeSQLite(int idCategoria = 0)
        {
            try
            {
                if (idCategoria > 0)
                {
                    return App.DAUtil.GetProductosKioskoByCategoria(idCategoria);
                }
                return App.DAUtil.GetProductosKiosko();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Kiosko] Error obteniendo productos de SQLite: {ex.Message}");
                return new List<ArticuloModel>();
            }
        }

        /// <summary>
        /// Obtiene las categorias desde SQLite
        /// </summary>
        public List<Categoria> ObtenerCategoriasDesdeSQLite()
        {
            try
            {
                return App.DAUtil.GetCategoriasKiosko();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Kiosko] Error obteniendo categorias de SQLite: {ex.Message}");
                return new List<Categoria>();
            }
        }

        /// <summary>
        /// Limpia los datos precargados (al cerrar sesion)
        /// </summary>
        public void LimpiarDatos()
        {
            ProductosPrecargados = null;
            CategoriasPrecargadas = null;
            DatosPrecargados = false;
            Debug.WriteLine("[Kiosko] Datos de precarga limpiados");
        }
    }
}
