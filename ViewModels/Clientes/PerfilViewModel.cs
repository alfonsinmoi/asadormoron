using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Utils;
using System;
using System.Threading.Tasks;
using AsadorMoron.Recursos;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using System.Linq;
using AsadorMoron.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using AsadorMoron.Messages;

namespace AsadorMoron.ViewModels.Clientes
{

    public class PerfilViewModel : ViewModelBase
    {
        private bool YaEntrado = false;
        string antiguo = "";
        private IUserDialogs dialogHome = App.userdialog;
        private bool subirFoto = false;
        private static bool tienePermisoGaleria = false;
        private static bool tienePermisoCamara = false;
        readonly List<PueblosModel> poblaciones = App.DAUtil.GetPueblosSQLite().FindAll(p => p.cantidad > 0 && p.visibleListado==true);
        Guid g;
        public PerfilViewModel()
        {
            App.DAUtil.ConFoto = false;
            if (App.DAUtil.NotificacionPantalla.Equals(""))
            {
                if (App.userdialog == null)
                {
                    try { dialogHome.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
            }
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
                }
            }
        }
        private string apellidos;
        public string Apellidos
        {
            get { return apellidos; }
            set
            {
                if (apellidos != value)
                {
                    apellidos = value;
                    OnPropertyChanged(nameof(Apellidos));
                }
            }
        }
        private string imagen;
        public string Imagen
        {
            get { return imagen; }
            set
            {
                if (imagen != value)
                {
                    imagen = value;
                    OnPropertyChanged(nameof(Imagen));
                }
            }
        }
        private string poblacion;
        public string Poblacion
        {
            get { return poblacion; }
            set
            {
                if (poblacion != value)
                {
                    poblacion = value;
                    OnPropertyChanged(nameof(Poblacion));
                    if (!string.IsNullOrEmpty(Poblacion))
                        CargaCodPostales();
                }
            }
        }
        private string provincia;
        public string Provincia
        {
            get { return provincia; }
            set
            {
                if (provincia != value)
                {
                    provincia = value;
                    OnPropertyChanged(nameof(Provincia));
                    CargaPueblos();
                }
            }
        }
        private string direccion;
        public string Direccion
        {
            get { return direccion; }
            set
            {
                if (direccion != value)
                {
                    direccion = value;
                    OnPropertyChanged(nameof(Direccion));
                }
            }
        }
        private CodigosPostales codPostal;
        public CodigosPostales CodPostal
        {
            get { return codPostal; }
            set
            {
                if (codPostal != value)
                {
                    codPostal = value;
                    OnPropertyChanged(nameof(CodPostal));
                    if (CodPostal != null)
                    {
                        Preferences.Set("idPueblo", 1);
                        Preferences.Set("idGrupo", 1);
                        Zonas = new ObservableCollection<ZonaModel>(App.ResponseWS.getZonas(CodPostal.idPueblo));
                    }
                }
            }
        }
        private ObservableCollection<string> provincias;
        public ObservableCollection<string> Provincias
        {
            get
            {
                return provincias;
            }
            set
            {
                if (provincias != value)
                {
                    provincias = value;
                    OnPropertyChanged(nameof(Provincias));

                }
            }
        }
        private ObservableCollection<string> pueblos;
        public ObservableCollection<string> Pueblos
        {
            get
            {
                return pueblos;
            }
            set
            {
                if (pueblos != value)
                {
                    pueblos = value;
                    OnPropertyChanged(nameof(Pueblos));
                }
            }
        }
        private ObservableCollection<CodigosPostales> codPostales;
        public ObservableCollection<CodigosPostales> CodPostales
        {
            get
            {
                return codPostales;
            }
            set
            {
                if (codPostales != value)
                {
                    codPostales = value;
                    OnPropertyChanged(nameof(CodPostales));
                }
            }
        }
        private DateTime fechaAlta;
        public DateTime FechaAlta
        {
            get { return fechaAlta; }
            set
            {
                if (fechaAlta != value)
                {
                    fechaAlta = value;
                    OnPropertyChanged(nameof(FechaAlta));
                }
            }
        }

        private string telefono;
        public string Telefono
        {
            get { return telefono; }
            set
            {
                if (telefono != value)
                {
                    telefono = value;
                    OnPropertyChanged(nameof(Telefono));
                }
            }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }
        private bool tieneTel;
        public bool TieneTel
        {
            get { return tieneTel; }
            set
            {
                if (tieneTel != value)
                {
                    tieneTel = value;
                    OnPropertyChanged(nameof(TieneTel));
                }
            }
        }
        private ZonaModel zona;
        public ZonaModel Zona
        {
            get
            {
                return zona;
            }
            set
            {
                if (zona != value)
                {
                    zona = value;
                    OnPropertyChanged(nameof(Zona));
                }
            }
        }
        private ObservableCollection<ZonaModel> zonas;
        public ObservableCollection<ZonaModel> Zonas
        {
            get
            {
                return zonas;
            }
            set
            {
                if (zonas != value)
                {
                    zonas = value;
                    OnPropertyChanged(nameof(Zonas));
                }
            }
        }
        public ICommand CommandGuardar { get { return new Command(Guardar); } }
        public ICommand /*IAsyncRelayCommand*/ CommandEliminar { get { return new AsyncRelayCommand(Eliminar); } }
        public ICommand CommandCancelar { get { return new Command(IrAInicio); } }
        private async Task Eliminar()
        {
            try
            {
                bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, "¿Desea eliminar su cuenta de usuario?, el proceso es irreversible, una vez eliminada, no prodrá recuperar los datos de sus pedidos", AppResources.No, AppResources.Si);
                if (result)
                {
                    try { App.userdialog.ShowLoading("Eliminando Usuario", MaskType.Black); } catch (Exception) { }
                    await Task.Delay(200);
                    if (await InitEliminar())
                    {
                        if (App.DAUtil.DoIHaveInternet())
                            App.ResponseWS.BorraToken(App.DAUtil.Usuario.email);
                        App.DAUtil.DeleteUsuarioSQLite();
                        App.DAUtil.VaciaConfig();
                        App.DAUtil.VaciaCarrito();
                        if (App.DAUtil.Usuario.rol == (int)RolesEnum.Cliente)
                            await ResponseServiceWS.GuardaOnline(2);
                        App.DAUtil.Usuario = null;

                        await App.DAUtil.NavigationService.InitializeAsync();
                    }
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
        private async Task<bool> InitEliminar()
        {
            try
            {
                App.DAUtil.Usuario.estado = 0;
                String error = await App.ResponseWS.actualizaUsuario(App.DAUtil.Usuario);
                if (!error.Contains("ERROR"))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                // 
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                return false;
            }
        }
        private async void Guardar()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Guardando, MaskType.Black); } catch (Exception) { }
                await Task.Delay(200);
                await Task.Run(async () => { await InitGuardar(); }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.userdialog.HideLoading();

                }));
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async void IrAInicio()
        {
            try
            {
                Direccion = App.DAUtil.Usuario.direccion;

                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
              
                    MainThread.BeginInvokeOnMainThread(async() =>
                    {
                        App.DAUtil.Idioma = "ES";
                       await App.DAUtil.NavigationService.InitializeAsync();
                    });

            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task InitGuardar()
        {
            try
            {
                if (string.IsNullOrEmpty(Direccion))
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.DireccionObligatoria, AppResources.App, AppResources.Cerrar);
                }
                else if (Zona == null)
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.ZonaObligatoria, AppResources.App, AppResources.Cerrar);
                }
                else if (string.IsNullOrEmpty(Telefono))
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.TelefonoObligatorio, AppResources.App, AppResources.Cerrar);
                }
                else if (Poblacion == null)
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.PoblacionObligatoria, AppResources.App, AppResources.Cerrar);
                }
                else if (Provincia == null)
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.ProvinciaObligatoria, AppResources.App, AppResources.Cerrar);
                }
                else if (CodPostal == null)
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.CPObligatorio, AppResources.App, AppResources.Cerrar);
                }
                else
                {
                    UsuarioModel usu = App.DAUtil.Usuario;
                    usu.direccion = Direccion;
                    usu.idZona = Zona.idZona;
                    usu.poblacion = Poblacion;
                    usu.provincia = Provincia;
                    usu.telefono = Telefono;
                    usu.idPueblo = poblaciones.Find(p => p.nombre.Equals(Poblacion)).id;
                    usu.codPostal = CodPostal.codPostal;
                    usu.telefono = Telefono;
                    if (subirFoto)
                    {

                        usu.foto = $"{ResponseServiceWS.urlPro}images/clientes/" + g + ".jpg";
                    }
                    else
                        usu.foto = usu.foto;

                    String error = await App.ResponseWS.actualizaUsuario(usu);
                    if (!error.Contains("ERROR"))
                    {
                        App.userdialog.HideLoading();
                        if (subirFoto)
                            ResponseServiceWS.UploadImage(Imagen, g.ToString() + ".jpg", "clientes", antiguo);

                        PueblosModel pu = App.DAUtil.GetPueblosSQLite().Find(p => p.id == usu.idPueblo);
                        Preferences.Set("idGrupo", pu.idGrupo);
                        Preferences.Set("idPueblo", usu.idPueblo);
                        Preferences.Set("puebloInicio", Poblacion);
                        await App.customDialog.ShowDialogAsync(AppResources.DatosModificadosOK, AppResources.App, AppResources.Cerrar);
                        IrAInicio();
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(error, AppResources.App, AppResources.Cerrar);
                    }
                }
            }
            catch (Exception ex)
            {
                // 
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
            }
        }
        public override Task InitializeAsync(object navigationData)
        {
            try
            {
                if (App.userdialog == null)
                {
                    try { dialogHome.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
                try
                {

                    if (!YaEntrado)
                    {
                        YaEntrado = true;
                        Provincias = new ObservableCollection<string>();
                        foreach (PueblosModel p in poblaciones.OrderBy(pr => pr.Provincia))
                        {
                            if (!Provincias.Contains(p.Provincia))
                                Provincias.Add(p.Provincia);
                        }
                        App.DAUtil.EnTimer = false;
                        Nombre = App.DAUtil.Usuario.nombre;
                        Apellidos = App.DAUtil.Usuario.apellidos;
                        Imagen = App.DAUtil.Usuario.foto.Replace("___", "&");

                        if (Imagen.Equals(""))
                            Imagen = "profile.png";
                        else
                            antiguo = App.DAUtil.Usuario.foto.Replace(ResponseServiceWS.urlPro, "");
                        Provincia = App.DAUtil.Usuario.provincia;
                        Poblacion = App.DAUtil.Usuario.poblacion;

                        Direccion = App.DAUtil.Usuario.direccion;
                        if (CodPostales != null)
                        {
                            try
                            {
                                CodPostal = CodPostales.Where(p => p.codPostal.Equals(App.DAUtil.Usuario.codPostal)).FirstOrDefault();
                            }
                            catch (Exception)
                            {

                            }
                        }
                        if (Zonas != null && App.DAUtil.Usuario.idZona > 0)
                        {
                            try
                            {
                                Zona = Zonas.Where(p => p.idZona == App.DAUtil.Usuario.idZona).FirstOrDefault();
                            }
                            catch (Exception)
                            {

                            }
                        }

                        Telefono = App.DAUtil.Usuario.telefono;
                        TieneTel = !string.IsNullOrEmpty(Telefono);

                        Email = App.DAUtil.Usuario.email;
                        FechaAlta = App.DAUtil.Usuario.fechaAlta;
                    }
                    return base.InitializeAsync(navigationData);

                }
                catch (Exception ex)
                {
                    // 
                }
                finally
                {
                    if (App.userdialog != null)
                    {
                        App.userdialog.HideLoading();
                        dialogHome = null;
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        dialogHome = null;
                    }
                }
            }
            catch (Exception ex)
            {
                // 
            }
            return base.InitializeAsync(navigationData);
        }

        public ICommand CmdCambiaImagen { get { return new Command(SacarFoto); } }
        private void SacarFoto()
        {
            SacarFotoAsync().ConfigureAwait(false);
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
                            Imagen = "";

                            var a = file.Path.Split('/');
                            g = Guid.NewGuid();
                            string nFoto = "";
                            nFoto = g + ".jpg";
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
                        Imagen = "";

                        var a = file.Path.Split('/');
                        g = Guid.NewGuid();
                        string nFoto = "";
                        nFoto = g + ".jpg";
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
                        Imagen = "";

                        var a = file.Path.Split('/');
                        g = Guid.NewGuid();
                        string nFoto = "";
                        nFoto = g + ".jpg";
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
                    Imagen = "";

                    var a = file.Path.Split('/');
                    g = Guid.NewGuid();
                    string nFoto = "";
                    nFoto = g + ".jpg";
                    Imagen = file.Path;
                    subirFoto = true;
                    App.userdialog.HideLoading();
                }
                else
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(App.MensajesGlobal.Where(p => p.clave.Equals("no_permiso_fotos")).FirstOrDefault<MensajesModel>().valor, AppResources.App, AppResources.Cerrar);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void CargaCodPostales()
        {
            CodPostales = new ObservableCollection<CodigosPostales>();
            foreach (PueblosModel p in poblaciones.Where(n => n.Provincia.ToUpper().Equals(Provincia.ToUpper()) && n.nombre.ToUpper().Equals(Poblacion.ToUpper())).OrderBy(pr => pr.codPostal))
            {
                foreach (string a in p.codPostal.Split(','))
                {
                    CodigosPostales c = new CodigosPostales();
                    c.codPostal = a;
                    c.idPueblo = p.id;
                    CodPostales.Add(c);
                }
            }
            CodPostal = CodPostales[0];
        }
        private void CargaPueblos()
        {
            try
            {
                Pueblos = new ObservableCollection<string>();
                foreach (PueblosModel p in poblaciones.Where(n => n.Provincia.Equals(Provincia)).OrderBy(pr => pr.nombre))
                {
                    if (!Pueblos.Contains(p.nombre))
                        Pueblos.Add(p.nombre);
                }
                Poblacion = Pueblos[0];
            }
            catch (Exception)
            {
                Poblacion = "";
            }
        }
    }
}
