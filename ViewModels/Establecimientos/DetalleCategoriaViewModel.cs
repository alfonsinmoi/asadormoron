using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Utils;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using AsadorMoron.Recursos;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using AsadorMoron.Services;

namespace AsadorMoron.ViewModels.Establecimientos
{

    public class DetalleCategoriaViewModel : ViewModelBase
    {
        private static bool tienePermisoGaleria = false;
        private static bool tienePermisoCamara = false;
        private string NombreFoto = "";
        private string NombreFotoLogo = "";
        private bool subirFoto = false;
        Guid g;
        string path;

        public DetalleCategoriaViewModel() { App.DAUtil.ConFoto = false; }

        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                if (!App.DAUtil.ConFoto)
                {
                    ListadoTipos = new ObservableCollection<TipoEstablecimientoModel>();
                    TipoEstablecimientoModel tip = new TipoEstablecimientoModel();
                    tip.Id = 0;
                    tip.Nombre = AppResources.Bebidas.ToUpper();
                    listadoTipos.Add(tip);
                    tip = new TipoEstablecimientoModel();
                    tip.Id = 1;
                    tip.Nombre = AppResources.Comidas.ToUpper();
                    listadoTipos.Add(tip);
                    if (navigationData != null)
                    {
                        try
                        {
                            _categoria = (Categoria)navigationData;
                            if (App.DAUtil.Idioma.Equals("ENG"))
                            {
                                Nombre = _categoria.nombre_eng;
                            }
                            else if (App.DAUtil.Idioma.Equals("FR"))
                            {
                                Nombre = _categoria.nombre_fr;
                            }
                            else if (App.DAUtil.Idioma.Equals("GER"))
                            {
                                Nombre = _categoria.nombre_ger;
                            }
                            else
                            {
                                Nombre = _categoria.nombre;
                            }
                            if (_categoria.estado == 1)
                                Estado = AppResources.Activo.ToUpper();
                            else
                                Estado = AppResources.Inactivo.ToUpper();
                            ValorEstado = _categoria.estado;
                            Orden = _categoria.orden;
                            Imagen = _categoria.imagen;
                            TipoSeleccionado = ListadoTipos.FirstOrDefault((obj) => obj.Nombre.ToUpper().Equals(_categoria.tipo.ToUpper()));
                            Tipo = TipoSeleccionado.Nombre;
                            Productos = _categoria.numeroProductos;
                            VisibleNormal = true;
                            VisibleEdicion = false;
                            IconoEditar = true;
                            EnableBtnGuardar = true;
                        }
                        catch (Exception)
                        {
                            IdEstablecimiento = (int)navigationData;
                            VisibleNormal = false;
                            VisibleEdicion = true;
                            IconoEditar = false;
                            EnableBtnGuardar = false;
                        }

                    }
                    else
                    {
                        VisibleNormal = false;
                        VisibleEdicion = true;
                        IconoEditar = false;
                        EnableBtnGuardar = false;
                    }
                }
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Metodos
        private void VerProductosExecute()
        {
            App.DAUtil.NavigationService.NavigateToAsync<ArticulosViewModel>(_categoria);
        }

        private async void Guardar()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                await Task.Delay(200);
                await Task.Run(async () => { await initGuardar(); }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                {

                    App.DAUtil.NavigationService.NavigateBackAsync().ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));

                }));
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void Cancelar()
        {
            App.DAUtil.NavigationService.NavigateBackAsync();
        }

        private void checkEnableBtnGuardar()
        {
            EnableBtnGuardar = !string.IsNullOrEmpty(Nombre);
        }

        private void EditarCategoriaCommandExecute(object parametro)
        {
            try
            {
                if (App.DAUtil.Idioma.Equals("ENG"))
                {
                    Nombre = _categoria.nombre_eng;
                }
                else if (App.DAUtil.Idioma.Equals("FR"))
                {
                    Nombre = _categoria.nombre_fr;
                }
                else if (App.DAUtil.Idioma.Equals("GER"))
                {
                    Nombre = _categoria.nombre_ger;
                }
                else
                {
                    Nombre = _categoria.nombre;
                }
                TipoSeleccionado = ListadoTipos.FirstOrDefault((obj) => obj.Nombre.ToUpper().Equals(_categoria.tipo.ToUpper()));
                if (_categoria.estado == 1)
                    Estado = AppResources.Activo.ToUpper();
                else
                    Estado = AppResources.Inactivo.ToUpper();
                ValorEstado = _categoria.estado;
                VisibleNormal = false;
                VisibleEdicion = true;
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void SacarFoto()
        {
            SacarFotoAsync().ConfigureAwait(false);
        }
        private static async Task CompruebaPermisosGaleria()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Photos>();
                    if (status == PermissionStatus.Granted)
                        tienePermisoGaleria = true;
                }
                else
                    tienePermisoGaleria = true;
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private static async Task CompruebaPermisosCamara()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                    if (status == PermissionStatus.Granted)
                        tienePermisoCamara = true;
                }
                else
                    tienePermisoCamara = true;

            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task SacarFotoAsync()
        {
            try
            {
                await CompruebaPermisosCamara();
                await CompruebaPermisosGaleria();
                if (tienePermisoGaleria && tienePermisoCamara)
                {
                    App.DAUtil.ConFoto = true;
                    string url = Imagen;
                    var result = await App.userdialog.ActionSheetAsync(AppResources.ElegirFoto, AppResources.Cancelar, null, null, AppResources.Galeria, AppResources.Camara);

                    if (result.Equals(AppResources.Camara))
                    {

                        var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            PhotoSize = PhotoSize.MaxWidthHeight,
                            MaxWidthHeight = 300,
                            Directory = "Sample",
                            Name = "foto.jpg"
                        });
                        if (file == null)
                            return;
                        else
                        {
                            try { App.userdialog.ShowLoading(AppResources.Guardando); } catch (Exception) { }


                            var a = file.Path.Split('/');

                            Imagen = "";
                            path = file.Path;
                            NombreFoto = a[a.Length - 1];
                            g = Guid.NewGuid();
                            Imagen = file.Path;
                            subirFoto = true;
                        }
                        App.userdialog.HideLoading();
                    }
                    else if (result.Equals(AppResources.Galeria))
                    {

                        if (!CrossMedia.Current.IsPickPhotoSupported)
                        {
                            return;
                        }
                        var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                        {
                            PhotoSize = PhotoSize.MaxWidthHeight,
                            MaxWidthHeight = 300
                        });


                        if (file == null)
                        {
                            return;
                        }
                        try { App.userdialog.ShowLoading(AppResources.Guardando); } catch (Exception) { }
                        Image image = new Image();
                        image.Source = ImageSource.FromStream(() =>
                        {
                            var stream = file.GetStream();
                            file.Dispose();

                            return stream;
                        });
                        try { App.userdialog.ShowLoading(AppResources.Guardando); } catch (Exception) { }


                        var a = file.Path.Split('/');
                        Imagen = "";
                        path = file.Path;
                        NombreFoto = a[a.Length - 1];
                        g = Guid.NewGuid();
                        Imagen = file.Path;
                        subirFoto = true;
                        App.userdialog.HideLoading();

                    }
                    else
                    {
                        App.DAUtil.ConFoto = false;
                    }
                }
                else if (tienePermisoCamara)
                {
                    App.DAUtil.ConFoto = true;
                    string url = Imagen;
                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 300,
                        Directory = "Sample",
                        Name = "foto.jpg"
                    });
                    if (file == null)
                        return;
                    else
                    {
                        try { App.userdialog.ShowLoading(AppResources.Guardando); } catch (Exception) { }

                        var a = file.Path.Split('/');
                        Imagen = "";
                        path = file.Path;
                        NombreFoto = a[a.Length - 1];
                        g = Guid.NewGuid();
                        Imagen = file.Path;
                        subirFoto = true;

                    }
                    App.userdialog.HideLoading();
                }
                else if (tienePermisoGaleria)
                {
                    App.DAUtil.ConFoto = true;
                    string url = Imagen;
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        return;
                    }
                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 300
                    });


                    if (file == null)
                    {
                        return;
                    }
                    try { App.userdialog.ShowLoading(AppResources.Guardando); } catch (Exception) { }
                    Image image = new Image();
                    image.Source = ImageSource.FromStream(() =>
                    {
                        var stream = file.GetStream();
                        file.Dispose();

                        return stream;
                    });
                    var a = file.Path.Split('/');
                    Imagen = "";
                    path = file.Path;
                    NombreFoto = a[a.Length - 1];
                    g = Guid.NewGuid();
                    Imagen = file.Path;
                    subirFoto = true;
                    App.userdialog.HideLoading();
                }
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }
        }
        private async Task initGuardar()
        {
            try
            {
                if (_categoria == null)
                {
                    _categoria = new Categoria();
                    if (App.DAUtil.Idioma.Equals("ENG"))
                    {
                        _categoria.nombre_eng = Nombre;
                    }
                    else if (App.DAUtil.Idioma.Equals("FR"))
                    {
                        _categoria.nombre_fr = Nombre;
                    }
                    else if (App.DAUtil.Idioma.Equals("GER"))
                    {
                        _categoria.nombre_ger = Nombre;
                    }
                    else
                    {
                        _categoria.nombre = Nombre;
                    }
                    _categoria.numeroImpresora = 1;
                    _categoria.imagen = $"{ResponseServiceWS.urlPro}images/categorias/" + g + ".jpg";
                    _categoria.orden = Orden;
                    _categoria.estado = ValorEstado;
                    _categoria.idTipo = TipoSeleccionado.Id;
                    _categoria.idEstablecimiento = IdEstablecimiento;
                    _categoria.idGrupo = 1;

                    if (App.ResponseWS.nuevaCategoria(_categoria) != -1)
                    {
                        try
                        {
                            if (subirFoto)
                            {
                                ResponseServiceWS.UploadImage(Imagen, g.ToString() + ".jpg", "categorias", "");
                            }
                            if (App.DAUtil.Usuario.establecimientos != null)
                                App.DAUtil.Usuario.establecimientos.Find((obj) => obj.idEstablecimiento == IdEstablecimiento).numeroCategorias += 1;
                        }
                        catch (Exception)
                        {

                        }
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.CategoriaOK, AppResources.App, AppResources.Aceptar);
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                    }
                }
                else
                {
                    if (App.DAUtil.Idioma.Equals("ENG"))
                    {
                        _categoria.nombre_eng = Nombre;
                    }
                    else if (App.DAUtil.Idioma.Equals("FR"))
                    {
                        _categoria.nombre_fr = Nombre;
                    }
                    else if (App.DAUtil.Idioma.Equals("GER"))
                    {
                        _categoria.nombre_ger = Nombre;
                    }
                    else
                    {
                        _categoria.nombre = Nombre;
                    }
                    _categoria.estado = ValorEstado;
                    _categoria.idTipo = TipoSeleccionado.Id;
                    _categoria.numeroImpresora = 1;
                    _categoria.imagen = $"{ResponseServiceWS.urlPro}images/categorias/" + g + ".jpg";
                    _categoria.orden = Orden;
                    PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == App.DAUtil.Usuario.idPueblo);
                    _categoria.idGrupo = pu.idGrupo;
                    if (App.ResponseWS.actualizaCategoria(_categoria))
                    {
                        if (subirFoto)
                        {
                            ResponseServiceWS.UploadImage(Imagen, g.ToString() + ".jpg", "categorias", "");
                        }
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.CategoriaOK, AppResources.App, AppResources.Aceptar);
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                    }
                }

            }
            catch (Exception ex)
            {
                // 
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
            }
        }

        #endregion

        #region Comandos
        public ICommand VerProductos { get { return new Command(VerProductosExecute); } }
        public ICommand CommandGuardar { get { return new Command(Guardar); } }
        public ICommand CommandCancelar { get { return new Command(Cancelar); } }
        public ICommand EditarCategoriaCommand { get { return new Command((parametro) => EditarCategoriaCommandExecute(parametro)); } }
        public ICommand CmdCambiaImagen { get { return new Command(SacarFoto); } }
        #endregion

        #region Propiedades
        private Categoria _categoria;
        private int IdEstablecimiento;
        private string nombre;
        public string Nombre
        {
            get { return nombre; }
            set
            {
                if (nombre != value)
                {
                    nombre = value;
                    OnPropertyChanged(nameof(Nombre));
                    checkEnableBtnGuardar();
                }
            }
        }
        private int numeroProductos = 0;
        public int Productos
        {
            get { return numeroProductos; }
            set
            {
                if (numeroProductos != value)
                {
                    numeroProductos = value;
                    OnPropertyChanged(nameof(Productos));
                }
            }
        }

        private string tipo;
        public string Tipo
        {
            get { return tipo; }
            set
            {
                if (tipo != value)
                {
                    tipo = value;
                    OnPropertyChanged(nameof(Tipo));
                }
            }
        }

        private string estado;
        public string Estado
        {
            get { return estado; }
            set
            {
                if (estado != value)
                {
                    estado = value;
                    OnPropertyChanged(nameof(Estado));
                }
            }
        }

        private int valorEstado = 1;
        public int ValorEstado
        {
            get { return valorEstado; }
            set
            {
                if (valorEstado != value)
                {
                    valorEstado = value;
                    OnPropertyChanged(nameof(ValorEstado));
                }
            }
        }
        private int orden = 1;
        public int Orden
        {
            get { return orden; }
            set
            {
                if (orden != value)
                {
                    orden = value;
                    OnPropertyChanged(nameof(Orden));
                }
            }
        }

        private ObservableCollection<TipoEstablecimientoModel> listadoTipos;
        public ObservableCollection<TipoEstablecimientoModel> ListadoTipos
        {
            get { return listadoTipos; }
            set
            {
                if (listadoTipos != value)
                {
                    listadoTipos = value;
                    OnPropertyChanged(nameof(ListadoTipos));
                }
            }
        }

        private TipoEstablecimientoModel tipoSeleccionado;
        public TipoEstablecimientoModel TipoSeleccionado
        {
            get { return tipoSeleccionado; }
            set
            {
                if (tipoSeleccionado != value)
                {
                    tipoSeleccionado = value;
                    OnPropertyChanged(nameof(TipoSeleccionado));
                }
            }
        }

        private bool visibleNormal;
        public bool VisibleNormal
        {
            get { return visibleNormal; }
            set
            {
                if (visibleNormal != value)
                {
                    visibleNormal = value;
                    OnPropertyChanged(nameof(VisibleNormal));
                }
            }
        }

        private bool visibleEdicion;
        public bool VisibleEdicion
        {
            get { return visibleEdicion; }
            set
            {
                if (visibleEdicion != value)
                {
                    visibleEdicion = value;
                    OnPropertyChanged(nameof(VisibleEdicion));
                }
            }
        }

        private bool enableBtnGuardar;
        public bool EnableBtnGuardar
        {
            get { return enableBtnGuardar; }

            set
            {
                enableBtnGuardar = value;
                OnPropertyChanged(nameof(EnableBtnGuardar));
            }
        }

        private bool iconoEditar;
        public bool IconoEditar
        {
            get { return iconoEditar; }
            set
            {
                iconoEditar = value;
                OnPropertyChanged(nameof(IconoEditar));
            }
        }
        private string imagen;
        public string Imagen
        {
            get
            {
                return imagen;
            }
            set
            {
                if (imagen != value)
                {
                    imagen = value;
                    OnPropertyChanged(nameof(Imagen));
                }
            }
        }
        #endregion
    }
}
