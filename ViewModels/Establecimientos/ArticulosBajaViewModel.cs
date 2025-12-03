using AsadorMoron.Interfaces;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Syncfusion.XlsIO;
// using Plugin.FilePicker; // MAUI uses FilePicker
using System.IO;
// 
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using System.Linq;
using System.Diagnostics;

namespace AsadorMoron.ViewModels.Establecimientos
{
    public class ArticulosBajaViewModel : ViewModelBase
    {
        public ArticulosBajaViewModel()
        {
            if (Comidas != null)
                ActualizaProductos().ConfigureAwait(false);
        }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                if (App.DAUtil.Usuario.rol == (int)RolesEnum.Administrador)
                    ImagenNuevo = "export_excel.png";
                else
                    ImagenNuevo = "nuevo.png";
                App.DAUtil.EnTimer = false;
                Logo = "logocabeceraazul.png";
                if (_establecimiento == null)
                {
                    if (_allGroups != null)
                        _allGroups.Clear();

                    List<ArticuloModel> lista;
                    if (navigationData.GetType() == typeof(Establecimiento))
                    {
                        _establecimiento = (Establecimiento)navigationData;
                        lista = await App.DAUtil.getProductosEstablecimiento(_establecimiento.idEstablecimiento);
                    }
                    else
                    {
                        _establecimiento = App.EstActual;
                        _categoria = (Categoria)navigationData;
                        lista = await App.DAUtil.getProductosCategoria(_categoria.nombre);
                    }
                    if (!string.IsNullOrEmpty(_establecimiento.logo))
                        Logo = _establecimiento.logo;
                    PuedeReservar = (_establecimiento.puedeReservar==1 && App.DAUtil.Usuario != null);

                    Comidas = new List<Comida>();
                    int idArt = 0;
                    foreach (ArticuloModel p in lista)
                    {
                        if (idArt != p.idArticulo && p.estado == 0)
                        {
                            idArt = p.idArticulo;
                            Comida co = new Comida();
                            co.articulo = p;
                            co.cantidad = 0;
                            try
                            {
                                co.idEstablecimiento = _establecimiento.idEstablecimiento;
                            }
                            catch (Exception)
                            {
                                co.idEstablecimiento = 0;
                            }
                            co.botonesVisibles = false;
                            Comidas.Add(co);
                        }
                    }

                    await Filtrar();
                    await ActualizaProductos();
                    // 
                    await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                    {
                        App.userdialog.HideLoading();
                    }));
                }
                else
                {
                    await ActualizaProductos();
                    await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                    {
                        App.userdialog.HideLoading();
                    }));
                }
            }
            catch (Exception ex)
            {
                // 
            }
            finally
            {
                App.userdialog.HideLoading();
            }
        }

        #region Metodos

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
                        _allGroups = new ObservableCollection<CategoriasArticulosGroupModel>();
                        int idCat = 0;
                        int i = 0;
                        CategoriasArticulosGroupModel ev = null;
                        String paraBuscar = "";
                        if (TextoBusqueda != null)
                            paraBuscar = TextoBusqueda;

                        foreach (Comida a in Comidas)
                        {
                            try
                            {
                                if (a.articulo.nombre.ToString().ToUpper().Contains(paraBuscar.ToUpper()) || string.IsNullOrWhiteSpace(paraBuscar)
                                    || a.articulo.descripcion.ToString().ToUpper().Contains(paraBuscar.ToUpper())
                                    || a.articulo.alergenos.ToString().ToUpper().Contains(paraBuscar.ToUpper())
                                    || a.articulo.ingredientes.ToString().ToUpper().Contains(paraBuscar.ToUpper()))
                                {
                                    if (idCat != a.articulo.idCategoria)
                                    {
                                        idCat = a.articulo.idCategoria;
                                        if (ev != null)
                                            _allGroups.Add(ev);
                                        string color = "";
                                        if (a.articulo.idTipoCategoria == 1)
                                            color = "#F8C149";
                                        else
                                            color = "#FFFFFF";
                                        if (i == 0)
                                            ev = new CategoriasArticulosGroupModel(a.articulo.categoria, a.articulo.categoria_eng, a.articulo.categoria_ger, a.articulo.categoria_fr, true, color);
                                        else
                                            ev = new CategoriasArticulosGroupModel(a.articulo.categoria, a.articulo.categoria_eng, a.articulo.categoria_ger, a.articulo.categoria_fr, false, color);
                                        ev.ColorCategoria = color;
                                        i++;
                                    }

                                    a.cantidad = 0;
                                    if (ev != null)
                                        ev.Add(a);
                                }
                            }
                            catch (Exception ex2)
                            {
                                Console.WriteLine(ex2.Message);
                            }
                        }
                        if (Comidas.Count > 0)
                        {
                            if (ev != null)
                            {
                                _allGroups.Add(ev);
                            }
                        }
                        await UpdateListContent();
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
                // 
            }
        }
        private async void Activar()
        {
            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaActivarProductos, AppResources.No, AppResources.Si);
            Activar2(result);
        }
        private void Activar2(bool hacer)
        {
            try
            {
                if (hacer)
                {
                    try { App.userdialog.ShowLoading(AppResources.Grabando); } catch (Exception) { }
                    Task.Run(() =>
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            foreach (Comida c in Comidas)
                            {
                                if (c.articulo.seleccionado)
                                {
                                    c.articulo.estado = 1;
                                    App.ResponseWS.actualizaProducto(c.articulo);

                                }
                            }
                            await ActualizaProductos();
                        });
                    });
                    App.userdialog.HideLoading();
                }
                else
                {
                    App.userdialog.HideLoading();
                }
            }
            catch (Exception ex)
            {
                // 
                App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
            }
        }
        private void articuloSeleccionado(object idArticulo)
        {
            try
            {
                Comida articulo = Comidas.Find((obj) => obj.articulo.idArticulo == (int)idArticulo);
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    App.DAUtil.Idioma = "ES";
                    await App.DAUtil.NavigationService.NavigateToAsync<DetalleComidaViewModel>(articulo.articulo);
                });
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
                    await App.ResponseWS.getListadoProductosEstablecimiento(_establecimiento.idEstablecimiento, false);
                    lista = await App.DAUtil.getProductosTodosEstablecimiento(_establecimiento.idEstablecimiento);
                    Comidas = new List<Comida>();
                    int idArt = 0;
                    foreach (ArticuloModel p in lista.Where(p=>p.estado==0))
                    {
                            if (_categoria != null)
                            {
                                if (_categoria.id == p.idCategoria)
                                {
                                    if (idArt != p.idArticulo)
                                    {
                                        idArt = p.idArticulo;
                                        Comida co = new Comida();
                                        co.articulo = p;
                                        co.cantidad = 0;
                                        try
                                        {
                                            co.idEstablecimiento = _establecimiento.idEstablecimiento;
                                        }
                                        catch (Exception)
                                        {
                                            co.idEstablecimiento = 0;
                                        }
                                        co.botonesVisibles = false;
                                        Comidas.Add(co);
                                    }
                                }
                            }
                            else
                            {
                                if (idArt != p.idArticulo)
                                {
                                    idArt = p.idArticulo;
                                    Comida co = new Comida();
                                    co.articulo = p;
                                    co.cantidad = 0;
                                    try
                                    {
                                        co.idEstablecimiento = _establecimiento.idEstablecimiento;
                                    }
                                    catch (Exception)
                                    {
                                        co.idEstablecimiento = 0;
                                    }
                                    co.botonesVisibles = false;
                                    Comidas.Add(co);
                                }
                            }
                    }
                    await Filtrar();
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void NuevoProductoCommandExecute(object parametro)
        {
            try
            {
                bool esAdmin = App.DAUtil.Usuario.rol == (int)RolesEnum.Administrador;
                if (esAdmin)
                {
                    try
                    {
                        try { App.userdialog.ShowLoading(AppResources.ImportandoProductos); } catch (Exception) { }

                        string[] fileTypes = null;
                        if (DeviceInfo.Platform.ToString() == "iOS")
                        {
                            fileTypes = new string[] { "org.openxmlformats.spreadsheetml.sheet", "com.microsoft.excel.xls" };
                        }
                        if (DeviceInfo.Platform.ToString() == "Android")
                            fileTypes = new string[] { "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };

                        if (DeviceInfo.Platform.ToString() == "WinUI")
                            fileTypes = new string[] { ".xls", ".xlsx" };
                        PickAndShow(fileTypes);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception choosing file: " + ex.ToString());
                    }
                }
                else
                {
                    try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        App.DAUtil.Idioma = "ES";
                        await App.DAUtil.NavigationService.NavigateToAsync<DetalleComidaViewModel>();
                    });
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void PickAndShow(string[] fileTypes)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var result = await FilePicker.Default.PickAsync(new PickOptions
                        {
                            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                            {
                                { DevicePlatform.iOS, new[] { "com.microsoft.excel.xls", "org.openxmlformats.spreadsheetml.sheet" } },
                                { DevicePlatform.Android, new[] { "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                                { DevicePlatform.WinUI, new[] { ".xls", ".xlsx" } },
                                { DevicePlatform.MacCatalyst, new[] { "com.microsoft.excel.xls", "org.openxmlformats.spreadsheetml.sheet" } }
                            })
                        });
                        if (result != null)
                        {
                            string a = result.FileName;
                            string b = result.FullPath;

                            ExcelEngine excelEngine = new ExcelEngine();
                            Syncfusion.XlsIO.IApplication application = excelEngine.Excel;
                            //application.DefaultVersion = ExcelVersion.Excel2013;
                            try
                            {
                                string resourcePath = result.FullPath;
                                Stream fileStream = File.OpenRead(resourcePath);

                                //Opens the workbook 
                                IWorkbook workbook = application.Workbooks.Open(fileStream);

                                //Access first worksheet from the workbook.
                                IWorksheet worksheet = workbook.Worksheets[0];

                                if (await App.ResponseWS.tienePedidos(App.EstActual.idEstablecimiento))
                                {
                                    await App.customDialog.ShowDialogAsync(AppResources.EstablecimientoConPedidos, AppResources.App, AppResources.Cerrar);
                                }
                                else
                                {
                                    await App.ResponseWS.eliminaCategoriasYProductos(App.EstActual.idEstablecimiento);
                                    bool seguir = true;
                                    int i = 2;

                                    List<Categoria> categorias = new List<Categoria>();
                                    do
                                    {
                                        if (string.IsNullOrEmpty(worksheet.Range["B" + i].Text))
                                            seguir = false;
                                        else
                                        {
                                            int idCat = 0;
                                            Categoria cat = categorias.Find(pq => pq.nombre.Equals(worksheet.Range["B" + i].Text.ToUpper()));
                                            if (cat == null)
                                            {
                                                Categoria c = new Categoria();
                                                c.estado = 1;
                                                c.idEstablecimiento = App.EstActual.idEstablecimiento;
                                                c.idTipo = 1;
                                                c.nombre = worksheet.Range["B" + i].Text.ToUpper();
                                                c.nombre_eng = worksheet.Range["B" + i].Text.ToUpper();
                                                c.nombre_fr = worksheet.Range["B" + i].Text.ToUpper();
                                                c.nombre_ger = worksheet.Range["B" + i].Text.ToUpper();
                                                PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p4 => p4.id == App.DAUtil.Usuario.idPueblo);
                                                c.idGrupo = pu.idGrupo;
                                                c.id = App.ResponseWS.nuevaCategoria(c);
                                                idCat = c.id;
                                                categorias.Add(c);

                                            }
                                            else
                                                idCat = cat.id;

                                            ArticuloModel p = new ArticuloModel();
                                            p.alergenos = "";
                                            p.Cantidad = 0;
                                            p.categoria = worksheet.Range["B" + i].Text.ToUpper();
                                            if (string.IsNullOrEmpty(worksheet.Range["D" + i].Text))
                                                p.descripcion = "";
                                            else
                                                p.descripcion = worksheet.Range["D" + i].Text.ToUpper();
                                            p.estado = 1;
                                            p.estadoCategoria = 1;
                                            p.idCategoria = idCat;
                                            p.idEstablecimiento = App.EstActual.idEstablecimiento;
                                            p.idTipoCategoria = 1;
                                            p.imagen = ResponseServiceWS.urlPro + "images/logo_producto.png";
                                            p.ingredientes = "";
                                            p.nombre = worksheet.Range["A" + i].Text.ToUpper();
                                            p.opciones = "";
                                            p.precio = double.Parse(worksheet.Range["C" + i].Value.ToString().Replace(".", ","));
                                            try
                                            {
                                                p.vistaEnvios = int.Parse(worksheet.Range["F" + i].Text);
                                            }
                                            catch (Exception)
                                            {
                                                p.vistaEnvios = 1;
                                            }
                                            try
                                            {
                                                p.vistaLocal = int.Parse(worksheet.Range["E" + i].Text);
                                            }
                                            catch (Exception)
                                            {
                                                p.vistaLocal = 1;
                                            }
                                            try
                                            {
                                                p.precioLocal = double.Parse(worksheet.Range["G" + i].Value.ToString().Replace(".", ","));
                                            }
                                            catch (Exception)
                                            {
                                                p.precioLocal = p.precio;
                                            }
                                            App.ResponseWS.nuevoProducto(p);
                                            i++;
                                        }
                                    } while (seguir);
                                    List<ArticuloModel> lista = await App.DAUtil.getProductosEstablecimiento(_establecimiento.idEstablecimiento);
                                    Comidas = new List<Comida>();
                                    int idArt = 0;
                                    foreach (ArticuloModel p in lista)
                                    {
                                        if (idArt != p.idArticulo)
                                        {
                                            idArt = p.idArticulo;
                                            Comida co = new Comida();
                                            co.articulo = p;
                                            co.cantidad = 0;
                                            try
                                            {
                                                co.idEstablecimiento = _establecimiento.idEstablecimiento;
                                            }
                                            catch (Exception)
                                            {
                                                co.idEstablecimiento = 0;
                                            }
                                            co.botonesVisibles = false;
                                            Comidas.Add(co);
                                        }
                                    }
                                    await Filtrar();
                                    await ActualizaProductos();
                                    await App.customDialog.ShowDialogAsync(AppResources.ImportacionCartaOK.Replace("%1",categorias.Count.ToString()).Replace("%2",(i-2).ToString()), AppResources.App, AppResources.Cerrar);
                                }

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                await App.customDialog.ShowDialogAsync(AppResources.ArchivoIncorrecto, AppResources.App, AppResources.Cerrar);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        App.userdialog.HideLoading();
                    }
                });
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void headerTapped(Object obj)
        {
            try
            {
                int selectedIndex = 0;
                int i = 0;
                foreach (CategoriasArticulosGroupModel c in _expandedGroups)
                {
                    if (c.Categoria.Equals(obj.ToString()))
                        selectedIndex = i;
                    else
                        _allGroups[i].Expanded = false;
                    i += 1;
                }
                _allGroups[selectedIndex].Expanded = !_allGroups[selectedIndex].Expanded;
                UpdateListContent().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task UpdateListContent()
        {
            try
            {
                await Task.Run(() =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _expandedGroups = new List<CategoriasArticulosGroupModel>();

                        foreach (CategoriasArticulosGroupModel group in _allGroups)
                        {
                            //Create new FoodGroups so we do not alter original list
                            CategoriasArticulosGroupModel newGroup = new CategoriasArticulosGroupModel(group.Categoria, group.Categoria_eng, group.Categoria_ger, group.Categoria_fr, group.Expanded, group.ColorCategoria);
                            //Add the count of food items for Lits Header Titles to use
                            newGroup.EventosCount = group.Count;
                            if (group.Expanded)
                            {
                                foreach (Comida food in group)
                                {
                                    string color = "";
                                    if (food.articulo.idTipoCategoria == 1)
                                        color = "#F8C149";
                                    else
                                        color = "#FFFFFF";
                                    newGroup.ColorCategoria = color;
                                    newGroup.Add(food);
                                }
                            }
                            _expandedGroups.Add(newGroup);

                        }
                        ListadoCategorias = new ObservableCollection<CategoriasArticulosGroupModel>(_expandedGroups);
                    });
                });
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async void EliminarProductoCommandExecute(object parametro)
        {
            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaEliminarProductos, AppResources.No, AppResources.Si);
            if (result)
            {
                ArticuloModel r = (ArticuloModel)parametro;
                r.eliminado = 1;
                if (App.ResponseWS.actualizaProducto(r))
                {
                    var listadoCategoriasTemp = ListadoCategorias;
                    foreach (List<Comida> item in listadoCategoriasTemp)
                    {
                        item.RemoveAll(a => a.articulo.idArticulo == r.idArticulo);
                    }
                    ListadoCategorias = null;
                    ListadoCategorias = listadoCategoriasTemp;
                    await App.customDialog.ShowDialogAsync(AppResources.EliminaProductoOK, AppResources.App, AppResources.Cerrar);
                }
                else
                    await App.customDialog.ShowDialogAsync(AppResources.EliminaProductoKO, AppResources.App, AppResources.Cerrar);
            }
        }
        #endregion

        #region Comandos
        public ICommand EliminarProductoCommand { get { return new Command((parametro) => EliminarProductoCommandExecute(parametro)); } }
        public ICommand HeaderTapped { get { return new Command(headerTapped); } }
        public ICommand NuevoProductoCommand { get { return new Command((parametro) => NuevoProductoCommandExecute(parametro)); } }
        public ICommand ArticuloSeleccionado { get { return new Command(articuloSeleccionado); } }
        public ICommand cmdActivar { get { return new Command(Activar); } }
        public ICommand btnFiltrar { get { return new DelegateCommandAsync(Filtrar); } }
        #endregion

        #region Propiedades
        private Establecimiento _establecimiento;
        private Categoria _categoria;

        private ObservableCollection<CategoriasArticulosGroupModel> _allGroups;
        private List<CategoriasArticulosGroupModel> _expandedGroups;

        private ObservableCollection<CategoriasArticulosGroupModel> listadoCategorias;
        public ObservableCollection<CategoriasArticulosGroupModel> ListadoCategorias
        {
            get { return listadoCategorias; }
            set
            {
                listadoCategorias = value;
                OnPropertyChanged(nameof(ListadoCategorias));
            }
        }

        private CategoriasArticulosGroupModel categoriaSeleccionada;
        public CategoriasArticulosGroupModel CategoriaSeleccionada
        {
            get { return categoriaSeleccionada; }
            set
            {
                if (categoriaSeleccionada != value)
                {
                    categoriaSeleccionada = value;
                    OnPropertyChanged(nameof(CategoriaSeleccionada));
                }
            }
        }

        private List<OpcionesModel> listOpcionesModel;
        public List<OpcionesModel> ListOpcionesModel
        {
            get { return listOpcionesModel; }
            set
            {
                if (listOpcionesModel != value)
                {
                    listOpcionesModel = value;
                    OnPropertyChanged(nameof(ListOpcionesModel));
                }
            }
        }

        private List<Comida> comidas;
        public List<Comida> Comidas
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

        private string textoBusqueda;
        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set
            {
                if (textoBusqueda != value)
                {
                    textoBusqueda = value;
                    Filtrar().ConfigureAwait(false);
                    OnPropertyChanged(nameof(TextoBusqueda));
                }
            }
        }

        private bool todoSeleccionado;
        public bool TodoSeleccionado
        {
            get
            {
                return todoSeleccionado;
            }
            set
            {
                if (value != todoSeleccionado)
                {
                    todoSeleccionado = value;
                    OnPropertyChanged(nameof(TodoSeleccionado));
                    foreach (Comida c in Comidas)
                    {
                        c.articulo.seleccionado = TodoSeleccionado;
                    }
                }
            }
        }

        private bool permiteReservar;
        public bool PuedeReservar
        {
            get
            {
                return permiteReservar;
            }
            set
            {
                if (permiteReservar != value)
                {
                    permiteReservar = value;
                    OnPropertyChanged(nameof(PuedeReservar));
                }
            }
        }

        private string imagenNuevo = "nuevo.png";
        public string ImagenNuevo
        {
            get
            {
                return imagenNuevo;
            }
            set
            {
                if (imagenNuevo != value)
                {
                    imagenNuevo = value;
                    OnPropertyChanged(nameof(ImagenNuevo));
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
        #endregion

    }
}
