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
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using System.Collections.Generic;

namespace AsadorMoron.ViewModels.Administrador
{

    public class DetalleRepartidorViewModel : ViewModelBase
    {
        public DetalleRepartidorViewModel() { }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                if (!haEntrado)
                {
                    haEntrado = true;
                    App.DAUtil.EnTimer = false;
                    if (navigationData is RepartidorModel)
                    {
                        try
                        {
                            _repartidor = (RepartidorModel)navigationData;
                            Nombre = _repartidor.nombre;
                            Foto = _repartidor.foto;
                            Telefono = _repartidor.telefono;
                            PIN = _repartidor.pin;
                            antiguo = _repartidor.foto.Replace(ResponseServiceWS.urlPro, "");
                            Activo = _repartidor.activo == 1;
                            EnableBtnGuardar = true;
                        }
                        catch (Exception)
                        {
                            EnableBtnGuardar = false;
                        }
                    }
                    else
                    {
                        Foto = "logocuadrado.png";
                        EnableBtnGuardar = false;
                        Activo = true;
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
                    string url = Foto;
                    var result = await App.userdialog.ActionSheetAsync(AppResources.ElegirFoto, AppResources.Cancelar, null, null, AppResources.Galeria, AppResources.Camara);

                    if (result.Equals(AppResources.Camara))
                    {

                        var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            PhotoSize = PhotoSize.MaxWidthHeight,
                            MaxWidthHeight = 300,
                            Directory = "Sample",
                            Name = "foto.jpg",
                            //CompressionQuality = 50
                        });
                        if (file == null)
                            return;
                        else
                        {
                            try { App.userdialog.ShowLoading(AppResources.Guardando); } catch (Exception) { }
                            Foto = "";

                            var a = file.Path.Split('/');
                            g = Guid.NewGuid();
                            string nFoto = "";
                            nFoto = g + ".jpg";
                            Foto = file.Path;
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
                            MaxWidthHeight = 300,
                            //CompressionQuality = 50

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
                        Foto = "";

                        var a = file.Path.Split('/');
                        g = Guid.NewGuid();
                        string nFoto = "";
                        nFoto = g + ".jpg";
                        Foto = file.Path;
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
                    string url = Foto;
                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 300,
                        Directory = "Sample",
                        Name = "foto.jpg",
                        //CompressionQuality = 50
                    });
                    if (file == null)
                        return;
                    else
                    {
                        try { App.userdialog.ShowLoading(AppResources.Guardando); } catch (Exception) { }
                        Foto = "";

                        var a = file.Path.Split('/');
                        g = Guid.NewGuid();
                        string nFoto = "";
                        nFoto = g + ".jpg";
                        Foto = file.Path;
                        subirFoto = true;

                    }
                    App.userdialog.HideLoading();

                }
                else if (tienePermisoGaleria)
                {
                    App.DAUtil.ConFoto = true;
                    string url = Foto;
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        return;
                    }
                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 300,
                        //CompressionQuality = 50

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
                    Foto = "";

                    var a = file.Path.Split('/');
                    g = Guid.NewGuid();
                    string nFoto = "";
                    nFoto = g + ".jpg";
                    Foto = file.Path;
                    subirFoto = true;
                    App.userdialog.HideLoading();
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void Cancelar()
        {
            //VisibleNormal = true;
            //VisibleEdicion = false;

            App.DAUtil.NavigationService.NavigateBackAsync();
        }
        private void checkEnableBtnGuardar()
        {
            EnableBtnGuardar = !string.IsNullOrEmpty(Nombre);
        }
        private async Task initGuardar()
        {
            try
            {
                if (string.IsNullOrEmpty(Nombre))
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.NombreObligatorio, AppResources.App, AppResources.Aceptar);
                }
                else if (string.IsNullOrEmpty(Foto))
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.ImagenObligatoria, AppResources.App, AppResources.Aceptar);
                }
                else
                {
                    if (_repartidor == null)
                    {
                        _repartidor = new RepartidorModel();
                        _repartidor.nombre = Nombre;
                        _repartidor.telefono = Telefono;

                        if (Activo)
                            _repartidor.activo = 1;
                        else
                            _repartidor.activo = 0;
                        if (subirFoto)
                        {
                            _repartidor.foto = $"{ResponseServiceWS.urlPro}images/repartidores/" + g + ".jpg";
                            _repartidor.pin = $"{ResponseServiceWS.urlPro}images/repartidores/" + g + ".png";
                        }
                        else
                        {
                            _repartidor.foto = Foto;
                            _repartidor.pin = PIN;
                        }
                        _repartidor.idPueblo = 1;
                        _repartidor.idGrupo = 1;
                        string idZ = "67";
                        if (_repartidor.pin == null)
                            _repartidor.pin = "";
                        if (App.ResponseWS.nuevoRepartidor(_repartidor, idZ) != -1)
                        {


                            if (subirFoto)
                            {
                                ResponseServiceWS.UploadImage(Foto, g.ToString() + ".jpg", "repartidores", "");
                            }
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync(AppResources.RepartidorOK, AppResources.App, AppResources.Aceptar);
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                        }
                    }
                    else
                    {
                        _repartidor.nombre = Nombre;
                        _repartidor.telefono = Telefono;
                        if (Activo)
                            _repartidor.activo = 1;
                        else
                            _repartidor.activo = 0;

                        if (subirFoto)
                        {

                            _repartidor.foto = $"{ResponseServiceWS.urlPro}images/repartidores/" + g + ".jpg";
                            _repartidor.pin = $"{ResponseServiceWS.urlPro}images/repartidores/" + g + ".png";
                        }
                        else
                        {
                            _repartidor.foto = Foto;
                            _repartidor.pin = PIN;
                        }
                        string idZ = "67";
                        _repartidor.idPueblo = 1;
                        _repartidor.idGrupo = 1;
                        if (App.ResponseWS.actualizaRepartidor(_repartidor, idZ))
                        {
                            if (subirFoto)
                            {
                                ResponseServiceWS.UploadImage(Foto, g.ToString() + ".jpg", "repartidores", antiguo);
                            }
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync(AppResources.mRepartidorOK, AppResources.App, AppResources.Aceptar);
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                        }
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
        public ICommand CommandGuardar { get { return new Command(Guardar); } }
        public ICommand CommandCancelar { get { return new Command(Cancelar); } }
        public ICommand CmdCambiaImagen { get { return new Command(SacarFoto); } }
        #endregion

        #region Propiedades
        string antiguo = "";
        private string PIN;
        private bool subirFoto = false;
        private static bool tienePermisoGaleria = false;
        private static bool tienePermisoCamara = false;
        private bool haEntrado = false;
        Guid g;
        private RepartidorModel _repartidor;
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
        private string foto;
        public string Foto
        {
            get
            {
                return foto;
            }
            set
            {
                if (foto != value)
                {
                    foto = value;
                    OnPropertyChanged(nameof(Foto));
                }
            }
        }
        private bool activo;
        public bool Activo
        {
            get
            {
                return activo;
            }
            set
            {
                if (activo != value)
                {
                    activo = value;
                    OnPropertyChanged(nameof(Activo));
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
        #endregion
    }
}
