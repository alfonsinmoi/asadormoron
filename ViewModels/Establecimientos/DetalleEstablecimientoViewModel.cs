using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Utils;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using System.Linq;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace AsadorMoron.ViewModels.Establecimientos
{

    public class DetalleEstablecimientoViewModel : ViewModelBase
    {
        public DetalleEstablecimientoViewModel()
        {
            App.DAUtil.ConFoto = false;
        }
        PueblosModel Pueblo;
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                if (!haEntrado)
                {
                    haEntrado = true;
                    App.DAUtil.EnTimer = false;
                    if (!App.DAUtil.ConFoto)
                    {
                        Provincias = new ObservableCollection<string>();
                        try
                        {
                            _establecimiento = App.EstActual;
                            Pueblo = App.DAUtil.GetPueblosSQLite(App.DAUtil.Usuario.idPueblo)[0];
                            poblaciones = new List<PueblosModel>(App.DAUtil.GetPueblosSQLite());
                            Imagen = "logocuadrado.png";
                            Logo = "logocuadrado.png";
                            EsAdmin = App.DAUtil.Usuario.rol == (int)RolesEnum.Administrador;
                            EnableBtnGuardar = false;
                                
                            VisibleFuera = _establecimiento.visibleFuera;
                            Nombre = _establecimiento.nombre;
                            Provincia = _establecimiento.provincia;
                            Direccion = _establecimiento.direccion.ToString();
                            Poblacion = _establecimiento.poblacion;
                                
                            EsAdmin = App.DAUtil.Usuario.rol == (int)RolesEnum.Administrador;
                            try
                            {
                                CodPostal = CodPostales.Where(p => p.codPostal.Equals(_establecimiento.codPostal)).FirstOrDefault();
                            }
                            catch (Exception)
                            { CodPostal = new CodigosPostales(); }
                            Imagen = _establecimiento.imagen;
                            antiguo = _establecimiento.imagen.Replace(ResponseServiceWS.urlPro, "");
                            Orden = _establecimiento.orden;
                            if (string.IsNullOrEmpty(_establecimiento.logo))
                                Logo = "logocuadrado.png";
                            else
                            {
                                antiguoLogo = _establecimiento.logo.Replace(ResponseServiceWS.urlPro, "");
                                Logo = _establecimiento.logo;
                            }
                            Latitud = _establecimiento.latitud.ToString();
                            Longitud = _establecimiento.longitud.ToString();
                            NumeroCategorias = _establecimiento.numeroCategorias;
                            Productos = _establecimiento.numeroProductos;
                            EsComercio = _establecimiento.esComercio;
                            Estado = _establecimiento.estado==1;
                            Envio = _establecimiento.envio==1;
                            Recogida = _establecimiento.recogida==1;
                            Reserva = _establecimiento.puedeReservar==1;
                            RecogeEnBarra = _establecimiento.recogeEnBarra;
                            LlevaAMesa = _establecimiento.llevaAMesa;
                            /*try
                            {
                                ZonaSeleccionada = Zonas.Where(p => p.idZona == _establecimiento.idZona).FirstOrDefault();
                            }
                            catch (Exception)
                            {
                                ZonaSeleccionada = Zonas.First();
                            }*/
                            Telefono = _establecimiento.telefono;
                            Telefono2 = _establecimiento.telefono2;
                            Whatsapp = _establecimiento.whatsapp;
                            Web = _establecimiento.web;
                            EmailContacto = _establecimiento.emailContacto;

                            Email = _establecimiento.email;
                            EnableBtnGuardar = true;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            EnableBtnGuardar = false;
                        }
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

        #region Métodos
        private void VerProductosBajaExecute()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.DAUtil.NavigationService.NavigateToAsync<ArticulosBajaViewModel>(App.EstActual);
                    });
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void Ajustes()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.DAUtil.NavigationService.NavigateToAsync<ConfiguracionEstablecimientoViewModel>(App.EstActual);
                    });
               
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void Horarios()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.DAUtil.NavigationService.NavigateToAsync<HorariosEstablecimientoViewModel>(App.EstActual);
                });

            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void SacarFoto()
        {
            esLogo = false;
            SacarFotoAsync().ConfigureAwait(false);
        }

        private void SacarFotoLogo()
        {
            esLogo = true;
            SacarFotoAsync().ConfigureAwait(false);
        }

        private void VerCategoriasExecute()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.DAUtil.NavigationService.NavigateToAsync<CategoriasViewModel>(App.EstActual);
                    });
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void VerProductosExecute()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.DAUtil.NavigationService.NavigateToAsync<ArticulosViewModel>(App.EstActual);
                    });
            }
            catch (Exception ex)
            {
                // 
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

        private void CargaCodPostales()
        {
            CodPostales = new ObservableCollection<CodigosPostales>();
            foreach (PueblosModel p in poblaciones.Where(n => n.Provincia.ToUpper().Equals(Provincia.ToUpper()) && n.nombre.ToUpper().Equals(Poblacion.ToUpper())).OrderBy(pr => pr.codPostal))
            {
                foreach (string a in p.codPostal.Split(',')) {
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
            Pueblos = new ObservableCollection<string>();
            foreach (PueblosModel p in poblaciones.Where(n => n.Provincia.ToUpper().Equals(Provincia.ToUpper())).OrderBy(pr => pr.nombre))
            {
                if (!Pueblos.Contains(p.nombre))
                    Pueblos.Add(p.nombre);
            }
            Poblacion = Pueblos[0];
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

                            if (!esLogo)
                            {
                                Imagen = "";
                                path = file.Path;
                                NombreFoto = a[a.Length - 1];
                                g = Guid.NewGuid();
                                Imagen = file.Path;
                                subirFoto = true;
                            }
                            else
                            {
                                Logo = "";
                                pathLogo = file.Path;
                                NombreFotoLogo = a[a.Length - 1];
                                gLogo = Guid.NewGuid();
                                Logo = file.Path;
                                subirFotoLogo = true;
                            }

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
                        if (!esLogo)
                        {
                            Imagen = "";
                            path = file.Path;
                            NombreFoto = a[a.Length - 1];
                            g = Guid.NewGuid();
                            Imagen = file.Path;
                            subirFoto = true;
                        }
                        else
                        {
                            Logo = "";
                            pathLogo = file.Path;
                            NombreFotoLogo = a[a.Length - 1];
                            gLogo = Guid.NewGuid();
                            Logo = file.Path;
                            subirFotoLogo = true;
                        }
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
                        if (!esLogo)
                        {
                            Imagen = "";
                            path = file.Path;
                            NombreFoto = a[a.Length - 1];
                            g = Guid.NewGuid();
                            Imagen = file.Path;
                            subirFoto = true;
                        }
                        else
                        {
                            Logo = "";
                            pathLogo = file.Path;
                            NombreFotoLogo = a[a.Length - 1];
                            gLogo = Guid.NewGuid();
                            Logo = file.Path;
                            subirFotoLogo = true;
                        }
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
                    if (!esLogo)
                    {
                        Imagen = "";
                        path = file.Path;
                        NombreFoto = a[a.Length - 1];
                        g = Guid.NewGuid();
                        Imagen = file.Path;
                        subirFoto = true;
                    }
                    else
                    {
                        Logo = "";
                        pathLogo = file.Path;
                        NombreFotoLogo = a[a.Length - 1];
                        gLogo = Guid.NewGuid();
                        Logo = file.Path;
                        subirFotoLogo = true;
                    }
                    App.userdialog.HideLoading();
                }
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }
        }

        private async Task Guardar()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Guardando, MaskType.Black); } catch (Exception) { }
                await Task.Delay(200);
                await initGuardar();
                App.userdialog.HideLoading();
                await App.DAUtil.NavigationService.NavigateBackAsync();
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

        private async Task initGuardar()
        {
            try
            {
                if (_establecimiento == null)
                    {
                        _establecimiento = new Establecimiento();
                        _establecimiento.imagen = $"{ResponseServiceWS.urlPro}images/establecimientos/" + NombreFoto;
                        _establecimiento.nombre = Nombre;
                        _establecimiento.direccion = Direccion;
                        _establecimiento.visibleFuera = VisibleFuera;
                        _establecimiento.poblacion = Poblacion;
                        _establecimiento.provincia = Provincia;
                        _establecimiento.llevaAMesa = LlevaAMesa;
                        _establecimiento.tieneMenuDiario = false;
                        _establecimiento.recogeEnBarra = RecogeEnBarra;
                        _establecimiento.local = 0;
                        _establecimiento.idPueblo = Pueblo.id;
                        _establecimiento.idGrupo = Pueblo.idGrupo;
                        _establecimiento.codPostal = CodPostal.codPostal;
                        _establecimiento.longitud = double.Parse(Longitud.Replace(".", ","));
                        _establecimiento.latitud = double.Parse(Latitud.Replace(".", ","));
                        //_establecimiento.idZona = ZonaSeleccionada.idZona;
                        _establecimiento.idZona = 1;
                        cargarcamposBoolEstablecimiento();
                        _establecimiento.idTipo = 3;// TipoSeleccionado.Id;
                        _establecimiento.telefono = Telefono;
                        _establecimiento.telefono2 = Telefono2;
                        _establecimiento.whatsapp = Whatsapp;
                        _establecimiento.web = Web;
                        _establecimiento.tipoImpresora =1;
                        _establecimiento.emailContacto = EmailContacto;
                        _establecimiento.email = Email;
                    _establecimiento.idCategoria = 1;
                        cargarImagenLogoEstablecimiento();
                        _establecimiento.orden = Orden;
                        if (_establecimiento.latitud >= -90 && _establecimiento.latitud <= 90 && _establecimiento.longitud >= -180 && _establecimiento.longitud <= 180)
                        {
                            if (App.ResponseWS.nuevoEstablecimiento(_establecimiento, "adm" + Email.Split('@')[0]) != -1)
                            {
                                if (subirFoto)
                                {
                                    ResponseServiceWS.UploadImage(Imagen, g.ToString() + ".jpg", "establecimientos", "");
                                }
                                if (subirFotoLogo)
                                {
                                    ResponseServiceWS.UploadImage(Logo, gLogo.ToString() + ".jpg", "establecimientos", "");
                                }
                                App.userdialog.HideLoading();
                                await App.customDialog.ShowDialogAsync(AppResources.EstablecimientoOK, AppResources.App, AppResources.Aceptar);
                            }
                            else
                            {
                                App.userdialog.HideLoading();
                                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                            }
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync("La latitud y la longitud el establecimiento no son correctas", AppResources.SoloError, AppResources.Aceptar);
                        }
                    }
                    else
                    {
                        _establecimiento.nombre = Nombre;
                        _establecimiento.direccion = Direccion;
                        _establecimiento.poblacion = Poblacion;
                        _establecimiento.idPueblo = Pueblo.id;
                        _establecimiento.idGrupo = Pueblo.idGrupo;
                        _establecimiento.provincia = Provincia;
                        _establecimiento.visibleFuera = VisibleFuera;
                        _establecimiento.llevaAMesa = LlevaAMesa;
                        _establecimiento.recogeEnBarra = RecogeEnBarra;
                        _establecimiento.tieneMenuDiario = false;
                        _establecimiento.tipoImpresora = 1;
                        _establecimiento.local = 0;
                        _establecimiento.codPostal = CodPostal.codPostal;
                        //_establecimiento.idZona = ZonaSeleccionada.idZona;
                        _establecimiento.idZona = 1;
                        cargarcamposBoolEstablecimiento();
                        _establecimiento.idTipo = 3;// TipoSeleccionado.Id;
                        _establecimiento.telefono = Telefono;
                        _establecimiento.telefono2 = Telefono2;
                        _establecimiento.whatsapp = Whatsapp;
                        _establecimiento.web = Web;
                        _establecimiento.emailContacto = EmailContacto;
                        _establecimiento.email = Email;
                        _establecimiento.orden = Orden;
                        _establecimiento.imagen = Imagen;
                        _establecimiento.longitud = double.Parse(Longitud.Replace(".", ","));
                        _establecimiento.latitud = double.Parse(Latitud.Replace(".", ","));
                        _establecimiento.idCategoria = 1;
                        cargarImagenLogoEstablecimiento();
                        if (_establecimiento.latitud >= -90 && _establecimiento.latitud <= 90 && _establecimiento.longitud >= -180 && _establecimiento.longitud <= 180)
                        {
                            if (App.ResponseWS.actualizaEstablecimiento(_establecimiento))
                            {
                                if (subirFoto)
                                {
                                    ResponseServiceWS.UploadImage(Imagen, g.ToString() + ".jpg", "establecimientos", antiguo);
                                }
                                if (subirFotoLogo)
                                {
                                    ResponseServiceWS.UploadImage(Logo, gLogo.ToString() + ".jpg", "establecimientos", antiguoLogo);
                                }
                                App.userdialog.HideLoading();
                                await App.customDialog.ShowDialogAsync(AppResources.mEstablecimientoOK, AppResources.App, AppResources.Aceptar);
                            }
                            else
                            {
                                App.userdialog.HideLoading();
                                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                            }
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync("La latitud y la longitud el establecimiento no son correctas", AppResources.SoloError, AppResources.Aceptar);
                        }
                    }
            }
            catch (Exception ex)
            {
                // 
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
            }
        }

        private void cargarcamposBoolEstablecimiento()
        {
            if (Estado)
            _establecimiento.estado = 1;
            else
                _establecimiento.estado = 0;

            _establecimiento.esComercio = EsComercio;

            if (Envio)
                _establecimiento.envio = 1;
            else
                _establecimiento.envio = 0;

            if (Reserva)
                _establecimiento.puedeReservar = 1;
            else
                _establecimiento.puedeReservar = 0;

            if (Recogida)
                _establecimiento.recogida = 1;
            else
                _establecimiento.recogida = 0;
        }

        private void cargarImagenLogoEstablecimiento()
        {
            if (subirFoto)
                _establecimiento.imagen = $"{ResponseServiceWS.urlPro}images/establecimientos/" + g + ".jpg";
            else
                _establecimiento.imagen = Imagen;
            if (subirFotoLogo)
                _establecimiento.logo = $"{ResponseServiceWS.urlPro}images/establecimientos/" + gLogo + ".jpg";
            else
                _establecimiento.logo = Logo;
        }

        #endregion

        #region Propiedades
        string antiguo = "";
        string antiguoLogo = "";
        private bool subirFoto = false;
        private bool subirFotoLogo = false;
        private bool esLogo;
        Guid g;
        Guid gLogo;
        string path;
        string pathLogo;
        private static bool tienePermisoGaleria = false;
        private static bool tienePermisoCamara = false;
        private string NombreFoto = "";
        private string NombreFotoLogo = "";
        private bool haEntrado = false;
        private Establecimiento _establecimiento;
        List<PueblosModel> poblaciones;

        private bool llevaAMesa;
        public bool LlevaAMesa
        {
            get { return llevaAMesa; }
            set
            {
                if (llevaAMesa != value)
                {
                    llevaAMesa = value;
                    OnPropertyChanged(nameof(LlevaAMesa));
                    RecogeEnBarra = !LlevaAMesa;
                }
            }
        }
        private bool recogeEnBarra;
        public bool RecogeEnBarra
        {
            get { return recogeEnBarra; }
            set
            {
                if (llevaAMesa != value)
                {
                    recogeEnBarra = value;
                    OnPropertyChanged(nameof(RecogeEnBarra));
                    LlevaAMesa = !RecogeEnBarra;
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
        private string imagen = "logo_producto.png";
        public string Imagen
        {
            get { return imagen; }
            set
            {
                if (imagen != value)
                {
                    imagen = value;
                    OnPropertyChanged(nameof(Imagen));
                    checkEnableBtnGuardar();
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
                    checkEnableBtnGuardar();
                }
            }
        }
        private bool esAdmin;
        public bool EsAdmin
        {
            get
            {
                return esAdmin;
            }
            set
            {
                if (esAdmin != value)
                {
                    esAdmin = value;
                    OnPropertyChanged(nameof(EsAdmin));
                }
            }
        }
        private int numeroCategorias = 0;
        public int NumeroCategorias
        {
            get { return numeroCategorias; }
            set
            {
                if (numeroCategorias != value)
                {
                    numeroCategorias = value;
                    OnPropertyChanged(nameof(NumeroCategorias));
                }
            }
        }
        private string telefono2;
        public string Telefono2
        {
            get
            {
                return telefono2;
            }
            set
            {
                if (telefono2 != value)
                {
                    telefono2 = value;
                    OnPropertyChanged(nameof(Telefono2));
                }
            }
        }
        private string whatsapp;
        public string Whatsapp
        {
            get
            {
                return whatsapp;
            }
            set
            {
                if (whatsapp != value)
                {
                    whatsapp = value;
                    OnPropertyChanged(nameof(Whatsapp));
                }
            }
        }
        private string web;
        public string Web
        {
            get
            {
                return web;
            }
            set
            {
                if (web != value)
                {
                    web = value;
                    OnPropertyChanged(nameof(Web));
                }
            }
        }
        private string emailContacto;
        public string EmailContacto
        {
            get
            {
                return emailContacto;
            }
            set
            {
                if (emailContacto != value)
                {
                    emailContacto = value;
                    OnPropertyChanged(nameof(EmailContacto));
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
        private ZonaModel zonaSeleccionada;
        public ZonaModel ZonaSeleccionada
        {
            get
            {
                return zonaSeleccionada;
            }
            set
            {
                if (zonaSeleccionada != value)
                {
                    zonaSeleccionada = value;
                    OnPropertyChanged(nameof(ZonaSeleccionada));
                    checkEnableBtnGuardar();
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
                    checkEnableBtnGuardar();
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
                    checkEnableBtnGuardar();
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
                    checkEnableBtnGuardar();
                }
            }
        }

        private bool estado;
        public bool Estado
        {
            get { return estado; }
            set
            {
                if (estado != value)
                {
                    estado = value;
                    OnPropertyChanged(nameof(Estado));
                    checkEnableBtnGuardar();
                }
            }
        }
        private bool esComercio;
        public bool EsComercio
        {
            get
            {
                return esComercio;
            }
            set
            {
                if (esComercio != value)
                {
                    esComercio = value;
                    OnPropertyChanged(nameof(EsComercio));
                    checkEnableBtnGuardar();
                }
            }
        }
        private string latitud;
        public string Latitud
        {
            get
            {
                return latitud;
            }
            set
            {
                if (latitud != value)
                {
                    latitud = value;
                    OnPropertyChanged(nameof(Latitud));
                    checkEnableBtnGuardar();
                }
            }
        }
        private string longitud;
        public string Longitud
        {
            get
            {
                return longitud;
            }
            set
            {
                if (longitud != value)
                {
                    longitud = value;
                    OnPropertyChanged(nameof(Longitud));
                    checkEnableBtnGuardar();
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
                    checkEnableBtnGuardar();
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
                    checkEnableBtnGuardar();
                }
            }
        }
        private int orden;
        public int Orden
        {
            get
            {
                return orden;
            }
            set
            {
                if (orden != value)
                {
                    orden = value;
                    OnPropertyChanged(nameof(Orden));
                    checkEnableBtnGuardar();
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
                    checkEnableBtnGuardar();
                }
            }
        }

        private bool envio;

        public bool Envio
        {
            get { return envio; }
            set
            {
                envio = value;
                OnPropertyChanged();
            }
        }

        private bool recogida;

        public bool Recogida
        {
            get { return recogida; }
            set
            {
                recogida = value;
                OnPropertyChanged();
            }
        }
        private bool reserva;

        public bool Reserva
        {
            get { return reserva; }
            set
            {
                reserva = value;
                OnPropertyChanged();
            }
        }
        private bool visibleFuera;
        public bool VisibleFuera
        {
            get
            {
                return visibleFuera;
            }
            set
            {
                if (visibleFuera != value)
                {
                    visibleFuera = value;
                    OnPropertyChanged(nameof(VisibleFuera));
                }
            }
        }
        #endregion

        #region Comandos
        private void checkEnableBtnGuardar()
        {
            EnableBtnGuardar = !string.IsNullOrEmpty(Nombre) && !string.IsNullOrEmpty(Imagen) && !string.IsNullOrEmpty(Email) && Provincia != null && Poblacion != null && CodPostal != null && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Telefono);
        }
        public ICommand CmdCambiaImagen { get { return new Command(SacarFoto); } }
        public ICommand CmdCambiaImagenLogo { get { return new Command(SacarFotoLogo); } }
        public ICommand VerCategorias { get { return new Command(VerCategoriasExecute); } }
        public ICommand VerProductosBaja { get { return new Command(VerProductosBajaExecute); } }
        public ICommand VerProductos { get { return new Command(VerProductosExecute); } }
        public ICommand /*IAsyncRelayCommand*/ CommandGuardar { get { return new AsyncRelayCommand(async()=>await Guardar()); } }
        public ICommand CommandAjustes { get { return new Command(Ajustes); } }
        public ICommand CommandHorarios { get { return new Command(Horarios); } }
        #endregion


    }
}
