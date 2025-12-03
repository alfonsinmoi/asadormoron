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
using AsadorMoron.Services;
using System.Collections.ObjectModel;
using FFImageLoading.Maui;
using System.Linq;
using AsadorMoron.Recursos;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using System.Diagnostics;

namespace AsadorMoron.ViewModels.Establecimientos
{

    public class DetalleComidaViewModel : ViewModelBase
    {
        public DetalleComidaViewModel()
        {
            App.DAUtil.ConFoto = false;
            if (App.DAUtil.NotificacionPantalla.Equals(""))
            {
                if (App.userdialog == null)
                {
                    try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
            }
        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                if (!App.DAUtil.ConFoto)
                {
                    App.DAUtil.EnTimer = false;
                    if (navigationData != null && navigationData is Establecimiento)
                    {
                        if (_establecimiento == null)
                        {
                            _establecimiento = (Establecimiento)navigationData;
                            _articulo = new ArticuloModel();
                            _articulo.idEstablecimiento = _establecimiento.idEstablecimiento;
                            _articulo.idArticulo = 0;
                            ListadoCategorias = new ObservableCollection<Categoria>(ResponseServiceWS.getListadoCategorias(_articulo.idEstablecimiento));
                            ListadoAlergenos = new ObservableCollection<AlergenosModel>();
                            EsComida = true;
                        }
                    }
                    else if (navigationData != null && navigationData is ArticuloModel)
                    {
                        TieneLocal = App.EstActual.local==1;
                        if (App.EstActual.configuracion == null)
                            App.EstActual.configuracion =  ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);
                        SistemaPuntos = App.EstActual.configuracion.sistemaPuntos;
                        TieneEncargos = App.EstActual.configuracion.aceptaEncargos;
                        if (_articulo == null)
                        {
                            _articulo = (ArticuloModel)navigationData;
                            Nombre = _articulo.nombre;
                            Descripcion = _articulo.descripcion;
                            Nombre_eng = _articulo.nombre_eng;
                            Descripcion_eng = _articulo.descripcion_eng;
                            Nombre_ger = _articulo.nombre_ger;
                            Descripcion_ger = _articulo.descripcion_ger;
                            Nombre_fr = _articulo.nombre_fr;
                            Puntos = _articulo.puntos;
                            Descripcion_fr = _articulo.descripcion_fr;
                            PorEncargo = _articulo.porEncargo;
                            Precio = _articulo.precio.ToString();
                            PrecioLocal = _articulo.precioLocal.ToString();
                            Imagen = _articulo.imagen;
                            NumeroIngredientes = _articulo.numeroIngredientes;
                            FuerzaIngredientes = _articulo.fuerzaIngredientes;
                            antiguo = _articulo.imagen.Replace(ResponseServiceWS.urlPro, "");
                            ListadoCategorias = new ObservableCollection<Categoria>(ResponseServiceWS.getListadoCategorias(_articulo.idEstablecimiento));
                            CategoriaSeleccionada = ListadoCategorias.Where((obj) => obj.id == _articulo.idCategoria).FirstOrDefault();
                            if (CategoriaSeleccionada.idTipo == 0)
                                EsComida = false;
                            else
                                EsComida = true;
                            Estado = _articulo.estado;
                            VistaEnvios = _articulo.vistaEnvios;
                            VistaLocal = _articulo.vistaLocal;
                            if (_articulo.listadoOpciones != null)
                            {
                                ListadoOpciones = _articulo.listadoOpciones;
                                SetHeightListView = listadoOpciones.Count * 40;
                            }
                            if (_articulo.listadoAlergenos == null)
                                _articulo.listadoAlergenos = new ObservableCollection<AlergenosModel>();
                            ListadoAlergenos = _articulo.listadoAlergenos;
                            if (_articulo.listadoAlergenos.Count == 0 && !string.IsNullOrEmpty(_articulo.alergenos))
                            {
                                _articulo.listadoAlergenos = new ObservableCollection<AlergenosModel>();
                                string[] al = _articulo.alergenos.Split('|');
                                foreach (string al2 in al)
                                {
                                    string[] al3 = al2.Split(';');
                                    AlergenosModel a = new AlergenosModel();
                                    a.id = int.Parse(al3[0]);
                                    a.nombre = al3[1];
                                    a.imagen = al3[2];
                                    _articulo.listadoAlergenos.Add(a);
                                }
                            }
                            ListadoAlergenos = _articulo.listadoAlergenos;
                            foreach (AlergenosModel a in ListadoAlergenos)
                            {
                                if (a.nombre.ToUpper().Equals("APIO"))
                                    Apio = "apiosel.png";
                                else if (a.nombre.ToUpper().Equals("ALTRAMUZ"))
                                    Altramuces = "altramucessel.png";
                                else if (a.nombre.ToUpper().Equals("CACAHUETES"))
                                    Cacahuetes = "cacahuetessel.png";
                                else if (a.nombre.ToUpper().Equals("CRUSTACEOS"))
                                    Crustaceos = "crustaceossel.png";
                                else if (a.nombre.ToUpper().Equals("FRUTOS SECOS"))
                                    FrutosSecos = "frutossecossel.png";
                                else if (a.nombre.ToUpper().Equals("CEREALES CON GLUTEN"))
                                    Gluten = "glutensel.png";
                                else if (a.nombre.ToUpper().Equals("HUEVOS"))
                                    Huevos = "huevossel.png";
                                else if (a.nombre.ToUpper().Equals("LACTEOS"))
                                    Lacteos = "lacteossel.png";
                                else if (a.nombre.ToUpper().Equals("MOLUSCOS"))
                                    Moluscos = "moluscossel.png";
                                else if (a.nombre.ToUpper().Equals("MOSTAZA"))
                                    Mostaza = "mostazasel.png";
                                else if (a.nombre.ToUpper().Equals("PESCADO"))
                                    Pescado = "pescadosel.png";
                                else if (a.nombre.ToUpper().Equals("SÉSAMO"))
                                    Sesamo = "sesamosel.png";
                                else if (a.nombre.ToUpper().Equals("SOJA"))
                                    Soja = "sojasel.png";
                                else if (a.nombre.ToUpper().Equals("SULFITOS"))
                                    Sulfitos = "sulfitossel.png";
                            }
                        }
                    }
                    else
                    {
                        _articulo = new ArticuloModel();
                        _articulo.idEstablecimiento = App.EstActual.idEstablecimiento;
                        ListadoCategorias = new ObservableCollection<Categoria>(ResponseServiceWS.getListadoCategorias(App.EstActual.idEstablecimiento));
                        Imagen = "logocuadrado.png";
                    }
                    List<IngredientesListModel> lis = new List<IngredientesListModel>();
                    List<IngredientesModel> ings = new List<IngredientesModel>(App.ResponseWS.listadoIngredientes(App.EstActual.idEstablecimiento).Where(pp => pp.estado == 1).OrderBy(p => p.nombre));
                    List<IngredienteProductoModel> ingsP = await App.ResponseWS.listadoIngredientesProducto(_articulo.idArticulo);
                    foreach (IngredientesModel i in ings)
                    {
                        IngredientesListModel il = new IngredientesListModel();
                        il.id = i.id;
                        il.nombre = i.nombre;
                        il.estado = 1;
                        il.puntos = i.puntos;
                        il.seleccionado = ingsP.Find(p => p.idIngrediente == i.id) != null;
                        if (il.seleccionado)
                            il.precio = ingsP.Find(p => p.idIngrediente == i.id).precio.ToString();
                        else
                            il.precio = i.precio.ToString();

                        lis.Add(il);
                    }
                    ListadoIngredientes = new ObservableCollection<IngredientesListModel>(lis);

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
        private void CambiaAltramuces()
        {
            if (Altramuces.Equals("altramuces.png"))
                Altramuces = "altramucessel.png";
            else
                Altramuces = "altramuces.png";
        }

        private void CambiaApio()
        {
            if (Apio.Equals("apio.png"))
                Apio = "apiosel.png";
            else
                Apio = "apio.png";
        }

        private void CambiaCacahuetes()
        {
            if (Cacahuetes.Equals("cacahuetes.png"))
                Cacahuetes = "cacahuetessel.png";
            else
                Cacahuetes = "cacahuetes.png";
        }

        private void CambiaCrustaceos()
        {
            if (Crustaceos.Equals("crustaceos.png"))
                Crustaceos = "crustaceossel.png";
            else
                Crustaceos = "crustaceos.png";
        }

        private void CambiaFrutosSecos()
        {
            if (FrutosSecos.Equals("frutossecos.png"))
                FrutosSecos = "frutossecossel.png";
            else
                FrutosSecos = "frutossecos.png";
        }

        private void CambiaGluten()
        {
            if (Gluten.Equals("gluten.png"))
                Gluten = "glutensel.png";
            else
                Gluten = "gluten.png";
        }

        private void CambiaHuevos()
        {
            if (Huevos.Equals("huevos.png"))
                Huevos = "huevossel.png";
            else
                Huevos = "huevos.png";
        }

        private void CambiaMoluscos()
        {
            if (Moluscos.Equals("moluscos.png"))
                Moluscos = "moluscossel.png";
            else
                Moluscos = "moluscos.png";
        }

        private void CambiaLacteos()
        {
            if (Lacteos.Equals("lacteos.png"))
                Lacteos = "lacteossel.png";
            else
                Lacteos = "lacteos.png";
        }

        private void CambiMostaza()
        {
            if (Mostaza.Equals("mostaza.png"))
                Mostaza = "mostazasel.png";
            else
                Mostaza = "mostaza.png";
        }

        private void CambiaPescado()
        {
            if (Pescado.Equals("pescado.png"))
                Pescado = "pescadosel.png";
            else
                Pescado = "pescado.png";
        }

        private void CambiaSesamo()
        {
            if (Sesamo.Equals("sesamo.png"))
                Sesamo = "sesamosel.png";
            else
                Sesamo = "sesamo.png";
        }

        private void CambiaSoja()
        {
            if (Soja.Equals("soja.png"))
                Soja = "sojasel.png";
            else
                Soja = "soja.png";
        }

        private void CambiaSulfitos()
        {
            if (Sulfitos.Equals("sulfitos.png"))
                Sulfitos = "sulfitossel.png";
            else
                Sulfitos = "sulfitos.png";
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
                        try
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
                                Imagen = "";
                                subirFotos = true;
                                var a = file.Path.Split('/');
                                NombreFoto = a[a.Length - 1];
                                //UploadImage(file.Path);
                                string nFoto = "";
                                if (_articulo.idArticulo != 0)
                                    nFoto = _articulo.idArticulo.ToString() + ".jpg";
                                else
                                    nFoto = NombreFoto;
                                //App.ResponseWS.mueveFichero(NombreFoto, _articulo.idEstablecimiento, nFoto);
                                try
                                {
                                    if (_articulo.idArticulo != 0)
                                        // TODO: FFImageLoading.Maui - cache invalidation
                                        await Task.CompletedTask;
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }
                                Imagen = file.Path;
                                NombreFoto = nFoto;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
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
                        Imagen = "";
                        subirFotos = true;
                        var a = file.Path.Split('/');
                        NombreFoto = a[a.Length - 1];
                        //UploadImage(file.Path);
                        string nFoto = "";
                        if (_articulo.idArticulo != 0)
                            nFoto = _articulo.idArticulo.ToString() + ".jpg";
                        else
                            nFoto = NombreFoto;

                        //App.ResponseWS.mueveFichero(NombreFoto, _articulo.idEstablecimiento, nFoto);
                        try
                        {
                            if (_articulo.idArticulo != 0)
                                // TODO: FFImageLoading.Maui - cache invalidation
                                        await Task.CompletedTask;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                        Imagen = file.Path;
                        NombreFoto = nFoto;
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
                        Name = "foto.jpg",
                        //CompressionQuality = 50
                    });
                    if (file == null)
                        return;
                    else
                    {
                        try { App.userdialog.ShowLoading(AppResources.Guardando); } catch (Exception) { }
                        Imagen = "";
                        subirFotos = true;
                        var a = file.Path.Split('/');
                        NombreFoto = a[a.Length - 1];
                        //UploadImage(file.Path);
                        string nFoto = "";
                        if (_articulo.idArticulo != 0)
                            nFoto = _articulo.idArticulo.ToString() + ".jpg";
                        else
                            nFoto = NombreFoto;
                        //App.ResponseWS.mueveFichero(NombreFoto, _articulo.idEstablecimiento, nFoto);

                        try
                        {
                            if (_articulo.idArticulo != 0)
                                // TODO: FFImageLoading.Maui - cache invalidation
                                        await Task.CompletedTask;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                        Imagen = file.Path;
                        NombreFoto = nFoto;
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
                    Imagen = "";
                    subirFotos = true;
                    var a = file.Path.Split('/');
                    NombreFoto = a[a.Length - 1];
                    //UploadImage(file.Path);
                    string nFoto = "";
                    if (_articulo.idArticulo != 0)
                        nFoto = _articulo.idArticulo.ToString() + ".jpg";
                    else
                        nFoto = NombreFoto;
                    //App.ResponseWS.mueveFichero(NombreFoto, _articulo.idEstablecimiento, nFoto);
                    try
                    {
                        if (_articulo.idArticulo != 0)
                            // TODO: FFImageLoading.Maui - cache invalidation
                                        await Task.CompletedTask;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    Imagen = file.Path;
                    NombreFoto = nFoto;
                    App.userdialog.HideLoading();
                }
                else
                {
                    App.userdialog.HideLoading();
                    string mensaje = "";
                    if (App.idioma.Equals("EN") || App.idioma.Equals("US") || App.idioma.Equals("GB"))
                        mensaje = App.MensajesGlobal.Where(p => p.clave.Equals("no_permiso_fotos")).FirstOrDefault<MensajesModel>().valor_eng;
                    else if (App.idioma.Equals("FR"))
                        mensaje = App.MensajesGlobal.Where(p => p.clave.Equals("no_permiso_fotos")).FirstOrDefault<MensajesModel>().valor_fr;
                    else if (App.idioma.Equals("DE"))
                        mensaje = App.MensajesGlobal.Where(p => p.clave.Equals("no_permiso_fotos")).FirstOrDefault<MensajesModel>().valor;
                    else
                        mensaje = App.MensajesGlobal.Where(p => p.clave.Equals("no_permiso_fotos")).FirstOrDefault<MensajesModel>().valor;
                    await App.customDialog.ShowDialogAsync(mensaje, AppResources.App, AppResources.Cerrar);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async void Guardar()
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
        }
        private async void Cancelar()
        {
            await App.DAUtil.NavigationService.NavigateBackAsync();

        }
        private void checkComida()
        {
            if (CategoriaSeleccionada != null)
            {
                if (CategoriaSeleccionada.idTipo == 0)
                    EsComida = false;
                else
                    EsComida = true;
            }
            else
                EsComida = true;
        }

        private async Task initGuardar()
        {
            try
            {
                if (_articulo.idArticulo == 0)
                {
                    if (subirFotos)
                        _articulo.imagen = $"{ResponseServiceWS.urlPro}images/productos/establecimiento/" + _articulo.idEstablecimiento + "/" + NombreFoto;
                    else
                        _articulo.imagen = $"{ResponseServiceWS.urlPro}images/logo_producto.png";
                    //UploadImage();
                    if (string.IsNullOrEmpty(Descripcion_eng))
                        Descripcion_eng = Descripcion;
                    if (string.IsNullOrEmpty(Descripcion_ger))
                        Descripcion_ger = Descripcion;
                    if (string.IsNullOrEmpty(Descripcion_fr))
                        Descripcion_fr = Descripcion;
                    if (string.IsNullOrEmpty(Nombre_eng))
                        Nombre_eng = Nombre;
                    if (string.IsNullOrEmpty(Nombre_ger))
                        Nombre_ger = Nombre;
                    if (string.IsNullOrEmpty(Nombre_fr))
                        Nombre_fr = Nombre;

                    _articulo.porEncargo = PorEncargo;
                    _articulo.numeroIngredientes = NumeroIngredientes;
                    _articulo.fuerzaIngredientes = FuerzaIngredientes;
                    _articulo.nombre = Nombre;
                    _articulo.descripcion_eng = Descripcion_eng;
                    _articulo.nombre_eng = Nombre_eng;
                    _articulo.descripcion_ger = Descripcion_ger;
                    _articulo.nombre_ger = Nombre_ger;
                    _articulo.descripcion_fr = Descripcion_fr;
                    _articulo.nombre_fr = Nombre_fr;
                    _articulo.descripcion = Descripcion;
                    try
                    {
                        _articulo.puntos = Puntos;
                    }
                    catch (Exception)
                    {
                        _articulo.puntos = 0;
                    }
                    _articulo.precio = double.Parse(Precio.Replace(".", ","));
                    _articulo.puntos = Puntos;
                    _articulo.precioLocal = double.Parse(PrecioLocal.Replace(".", ","));
                    _articulo.idCategoria = CategoriaSeleccionada.id;
                    _articulo.estado = Estado;
                    _articulo.vistaEnvios = VistaEnvios;
                    _articulo.vistaLocal = VistaLocal;
                    _articulo.descripcion = Descripcion;
                    _articulo.listadoOpciones = ListadoOpciones;
                    _articulo.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>();
                    if (ListadoIngredientes != null)
                    {
                        foreach (IngredientesListModel l in ListadoIngredientes.Where(p => p.seleccionado))
                        {
                            IngredienteProductoModel pro = new IngredienteProductoModel();
                            pro.id = l.idIngPro;
                            pro.idIngrediente = l.id;
                            pro.idProducto = _articulo.idArticulo;
                            pro.nombre = l.nombre;
                            pro.precio = double.Parse(l.precio.Replace(".", ","));
                            pro.puntos = l.puntos;
                            _articulo.listadoIngredientes.Add(pro);
                        }
                    }
                    _articulo.listadoAlergenos = new ObservableCollection<AlergenosModel>();
                    List<AlergenosModel> alergenos = App.DAUtil.GetAlergenos();
                    if (Pescado.Equals("pescadosel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 1).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (FrutosSecos.Equals("frutossecossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 2).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Lacteos.Equals("lacteossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 3).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Moluscos.Equals("moluscossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 4).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Gluten.Equals("glutensel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 5).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Crustaceos.Equals("crustaceossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 6).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Huevos.Equals("huevossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 7).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Cacahuetes.Equals("cacahuetessel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 8).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Soja.Equals("sojasel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 9).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Apio.Equals("apiosel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 10).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Mostaza.Equals("mostazasel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 11).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Sesamo.Equals("sesamosel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 12).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Altramuces.Equals("altramucessel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 13).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Sulfitos.Equals("sulfitossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 14).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (App.ResponseWS.nuevoProducto(_articulo))
                    {
                        if (subirFotos)
                            ResponseServiceWS.UploadImage(Imagen, NombreFoto, "productos/establecimiento/" + _articulo.idEstablecimiento, "");
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.ProductoOK, AppResources.App, AppResources.Aceptar);
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                    }
                }
                else
                {
                    _articulo.numeroIngredientes = NumeroIngredientes;
                    _articulo.fuerzaIngredientes = FuerzaIngredientes;
                    _articulo.nombre = Nombre;
                    _articulo.descripcion = Descripcion;
                    _articulo.precio = double.Parse(Precio.Replace(".", ","));
                    _articulo.puntos = Puntos;
                    _articulo.precioLocal = double.Parse(PrecioLocal.Replace(".", ","));
                    _articulo.idCategoria = CategoriaSeleccionada.id;
                    _articulo.estado = Estado;
                    _articulo.vistaLocal = VistaLocal;
                    _articulo.vistaEnvios = VistaEnvios;
                    try
                    {
                        _articulo.puntos = Puntos;
                    }catch (Exception)
                    {
                        _articulo.puntos = 0;
                    }
                    _articulo.porEncargo = PorEncargo;
                    _articulo.listadoOpciones = ListadoOpciones;
                    _articulo.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>();
                    _articulo.listadoAlergenos = new ObservableCollection<AlergenosModel>();
                    _articulo.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>();
                    if (ListadoIngredientes != null)
                    {
                        foreach (IngredientesListModel l in ListadoIngredientes.Where(p => p.seleccionado))
                        {
                            IngredienteProductoModel pro = new IngredienteProductoModel();
                            pro.id = l.idIngPro;
                            pro.idIngrediente = l.id;
                            pro.idProducto = _articulo.idArticulo;
                            pro.nombre = l.nombre;
                            pro.precio = double.Parse(l.precio.Replace(".", ","));
                            pro.puntos = l.puntos;
                            _articulo.listadoIngredientes.Add(pro);
                        }
                    }
                    List<AlergenosModel> alergenos = App.DAUtil.GetAlergenos();
                    if (Pescado.Equals("pescadosel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 1).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (FrutosSecos.Equals("frutossecossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 2).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Lacteos.Equals("lacteossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 3).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Moluscos.Equals("moluscossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 4).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Gluten.Equals("glutensel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 5).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Crustaceos.Equals("crustaceossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 6).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Huevos.Equals("huevossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 7).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Cacahuetes.Equals("cacahuetessel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 8).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Soja.Equals("sojasel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 9).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Apio.Equals("apiosel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 10).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Mostaza.Equals("mostazasel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 11).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Sesamo.Equals("sesamosel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 12).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Altramuces.Equals("altramucessel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 13).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (Sulfitos.Equals("sulfitossel.png"))
                    {
                        AlergenosModel a = alergenos.Where(p => p.id == 14).FirstOrDefault();
                        _articulo.listadoAlergenos.Add(a);
                    }
                    if (subirFotos)
                    {
                        _articulo.imagen = $"{ResponseServiceWS.urlPro}images/productos/establecimiento/" + _articulo.idEstablecimiento + "/" + NombreFoto;
                    }
                    if (App.ResponseWS.actualizaProducto(_articulo))
                    {
                        if (subirFotos)
                            ResponseServiceWS.UploadImage(Imagen, NombreFoto, "productos/establecimiento/" + _articulo.idEstablecimiento, antiguo);
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.ProductoOK, AppResources.App, AppResources.Aceptar);
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
        private void NuevaOpcion()
        {
            try
            {
                OpcionesModel op = new OpcionesModel();
                op.id = 0;
                op.opcion = "";
                op.opcion_eng = "";
                op.opcion_ger = "";
                op.opcion_fr = "";
                op.precio = Precio.ToString().Replace(".", ",");
                op.puntos = 0;
                op.seleccionado = false;
                op.tipoIncremento = 0;
                if (ListadoOpciones == null)
                    ListadoOpciones = new ObservableCollection<OpcionesModel>();
                ListadoOpciones.Add(op);
                SetHeightListView = listadoOpciones.Count * 50;
            }
            catch (Exception ex)
            {
                // 
            }
        }
        #endregion

        #region Comandos
        public ICommand CmdCambiaAltramuces { get { return new Command(CambiaAltramuces); } }
        public ICommand CmdCambiaApio { get { return new Command(CambiaApio); } }
        public ICommand CmdCambiaCacahuetes { get { return new Command(CambiaCacahuetes); } }
        public ICommand CmdCambiaCrustaceos { get { return new Command(CambiaCrustaceos); } }
        public ICommand CmdCambiaFrutosSecos { get { return new Command(CambiaFrutosSecos); } }
        public ICommand CmdCambiaGluten { get { return new Command(CambiaGluten); } }
        public ICommand CmdCambiaHuevos { get { return new Command(CambiaHuevos); } }
        public ICommand CmdCambiaMoluscos { get { return new Command(CambiaMoluscos); } }
        public ICommand CmdCambiaLacteos { get { return new Command(CambiaLacteos); } }
        public ICommand CmdCambiaMostaza { get { return new Command(CambiMostaza); } }
        public ICommand CmdCambiaPescado { get { return new Command(CambiaPescado); } }
        public ICommand CmdCambiaSesamo { get { return new Command(CambiaSesamo); } }
        public ICommand CmdCambiaSoja { get { return new Command(CambiaSoja); } }
        public ICommand CmdCambiaSulfitos { get { return new Command(CambiaSulfitos); } }
        public ICommand CmdCambiaImagen { get { return new Command(SacarFoto); } }
        public ICommand CommandGuardar { get { return new Command(Guardar); } }
        public ICommand CommandCancelar { get { return new Command(Cancelar); } }
        public ICommand CmdNuevaOpcion { get { return new Command(NuevaOpcion); } }
        #endregion

        #region Propiedades
        string antiguo = "";
        private ArticuloModel _articulo;
        private string NombreFoto = "";
        Establecimiento _establecimiento;
        bool subirFotos = false;
        private static bool tienePermisoGaleria = false;
        private static bool tienePermisoCamara = false;

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
        private string nombre_eng;
        public string Nombre_eng
        {
            get { return nombre_eng; }
            set
            {
                if (nombre_eng != value)
                {
                    nombre_eng = value;
                    OnPropertyChanged(nameof(Nombre_eng));
                }
            }
        }
        private string nombre_ger;
        public string Nombre_ger
        {
            get { return nombre_ger; }
            set
            {
                if (nombre_ger != value)
                {
                    nombre_ger = value;
                    OnPropertyChanged(nameof(Nombre_ger));
                }
            }
        }
        private string nombre_fr;
        public string Nombre_fr
        {
            get { return nombre_fr; }
            set
            {
                if (nombre_fr != value)
                {
                    nombre_fr = value;
                    OnPropertyChanged(nameof(Nombre_fr));
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
        private bool tieneLocal;
        public bool TieneLocal
        {
            get { return tieneLocal; }
            set
            {
                if (tieneLocal != value)
                {
                    tieneLocal = value;
                    OnPropertyChanged(nameof(TieneLocal));
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
        private bool tieneEncargos;
        public bool TieneEncargos
        {
            get { return tieneEncargos; }
            set
            {
                if (tieneEncargos != value)
                {
                    tieneEncargos = value;
                    OnPropertyChanged(nameof(TieneEncargos));
                }
            }
        }
        private bool esComida;
        public bool EsComida
        {
            get { return esComida; }
            set
            {
                if (esComida != value)
                {
                    esComida = value;
                    OnPropertyChanged(nameof(EsComida));
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
                }
            }
        }
        private string descripcion;
        public string Descripcion
        {
            get { return descripcion; }
            set
            {
                if (descripcion != value)
                {
                    descripcion = value;
                    OnPropertyChanged(nameof(Descripcion));
                }
            }
        }
        private string descripcion_eng;
        public string Descripcion_eng
        {
            get { return descripcion_eng; }
            set
            {
                if (descripcion_eng != value)
                {
                    descripcion_eng = value;
                    OnPropertyChanged(nameof(Descripcion_eng));
                }
            }
        }
        private string descripcion_ger;
        public string Descripcion_ger
        {
            get { return descripcion_ger; }
            set
            {
                if (descripcion_ger != value)
                {
                    descripcion_ger = value;
                    OnPropertyChanged(nameof(Descripcion_ger));
                }
            }
        }
        private string descripcion_fr;
        public string Descripcion_fr
        {
            get { return descripcion_fr; }
            set
            {
                if (descripcion_fr != value)
                {
                    descripcion_fr = value;
                    OnPropertyChanged(nameof(Descripcion_fr));
                }
            }
        }
        private bool porEncargo;
        public bool PorEncargo
        {
            get { return porEncargo; }
            set
            {
                if (porEncargo != value)
                {
                    porEncargo = value;
                    OnPropertyChanged(nameof(PorEncargo));
                }
            }
        }
        private string precio = "0";
        public string Precio
        {
            get { return precio; }
            set
            {
                if (precio != value)
                {
                    precio = value;
                    OnPropertyChanged(nameof(Precio));
                }
            }
        }
        private string precioLocal = "0";
        public string PrecioLocal
        {
            get { return precioLocal; }
            set
            {
                if (precioLocal != value)
                {
                    precioLocal = value;
                    OnPropertyChanged(nameof(PrecioLocal));
                }
            }
        }
        private int estado;
        public int Estado
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
        private int vistaLocal;
        public int VistaLocal
        {
            get { return vistaLocal; }
            set
            {
                if (vistaLocal != value)
                {
                    vistaLocal = value;
                    OnPropertyChanged(nameof(VistaLocal));
                }
            }
        }
        private int fuerzaIngredientes;
        public int FuerzaIngredientes
        {
            get { return fuerzaIngredientes; }
            set
            {
                if (fuerzaIngredientes != value)
                {
                    fuerzaIngredientes = value;
                    OnPropertyChanged(nameof(FuerzaIngredientes));
                }
            }
        }
        private int vistaEnvios;
        public int VistaEnvios
        {
            get { return vistaEnvios; }
            set
            {
                if (vistaEnvios != value)
                {
                    vistaEnvios = value;
                    OnPropertyChanged(nameof(VistaEnvios));
                }
            }
        }
        private int numeroIngredientes;
        public int NumeroIngredientes
        {
            get { return numeroIngredientes; }
            set
            {
                if (numeroIngredientes != value)
                {
                    numeroIngredientes = value;
                    OnPropertyChanged(nameof(NumeroIngredientes));
                }
            }
        }
        private ObservableCollection<Categoria> listadoCategorias;
        public ObservableCollection<Categoria> ListadoCategorias
        {
            get { return listadoCategorias; }
            set
            {
                if (listadoCategorias != value)
                {
                    listadoCategorias = value;
                    OnPropertyChanged(nameof(ListadoCategorias));
                }
            }
        }
        private Categoria categoriaSeleccionada;
        public Categoria CategoriaSeleccionada
        {
            get { return categoriaSeleccionada; }
            set
            {
                if (categoriaSeleccionada != value)
                {
                    categoriaSeleccionada = value;
                    OnPropertyChanged(nameof(CategoriaSeleccionada));
                    checkComida();
                }
            }
        }
        private ObservableCollection<OpcionesModel> listadoOpciones;
        public ObservableCollection<OpcionesModel> ListadoOpciones
        {
            get { return listadoOpciones; }
            set
            {
                if (listadoOpciones != value)
                {
                    listadoOpciones = value;
                    OnPropertyChanged(nameof(ListadoOpciones));
                }
            }
        }
        private ObservableCollection<IngredientesListModel> listadoIngredientes;
        public ObservableCollection<IngredientesListModel> ListadoIngredientes
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
        private string altramuces = "altramuces.png";
        public string Altramuces
        {
            get { return altramuces; }
            set
            {
                if (altramuces != value)
                {
                    altramuces = value;
                    OnPropertyChanged(nameof(Altramuces));
                }
            }
        }
        private string apio = "apio.png";
        public string Apio
        {
            get { return apio; }
            set
            {
                if (apio != value)
                {
                    apio = value;
                    OnPropertyChanged(nameof(Apio));
                }
            }
        }
        private string cacahuetes = "cacahuetes.png";
        public string Cacahuetes
        {
            get { return cacahuetes; }
            set
            {
                if (cacahuetes != value)
                {
                    cacahuetes = value;
                    OnPropertyChanged(nameof(Cacahuetes));
                }
            }
        }
        private string crustaceos = "crustaceos.png";
        public string Crustaceos
        {
            get { return crustaceos; }
            set
            {
                if (crustaceos != value)
                {
                    crustaceos = value;
                    OnPropertyChanged(nameof(Crustaceos));
                }
            }
        }
        private string frutosSecos = "frutossecos.png";
        public string FrutosSecos
        {
            get { return frutosSecos; }
            set
            {
                if (frutosSecos != value)
                {
                    frutosSecos = value;
                    OnPropertyChanged(nameof(FrutosSecos));
                }
            }
        }
        private string gluten = "gluten.png";
        public string Gluten
        {
            get { return gluten; }
            set
            {
                if (gluten != value)
                {
                    gluten = value;
                    OnPropertyChanged(nameof(Gluten));
                }
            }
        }
        private string huevos = "huevos.png";
        public string Huevos
        {
            get { return huevos; }
            set
            {
                {
                    huevos = value;
                    OnPropertyChanged(nameof(Huevos));
                }
            }
        }
        private string lacteos = "lacteos.png";
        public string Lacteos
        {
            get { return lacteos; }
            set
            {
                if (lacteos != value)
                {
                    lacteos = value;
                    OnPropertyChanged(nameof(Lacteos));
                }
            }
        }
        private string moluscos = "moluscos.png";
        public string Moluscos
        {
            get { return moluscos; }
            set
            {
                if (moluscos != value)
                {
                    moluscos = value;
                    OnPropertyChanged(nameof(Moluscos));
                }
            }
        }
        private string mostaza = "mostaza.png";
        public string Mostaza
        {
            get { return mostaza; }
            set
            {
                if (mostaza != value)
                {
                    mostaza = value;
                    OnPropertyChanged(nameof(Mostaza));
                }
            }
        }
        private string pescado = "pescado.png";
        public string Pescado
        {
            get { return pescado; }
            set
            {
                if (pescado != value)
                {
                    pescado = value;
                    OnPropertyChanged(nameof(Pescado));
                }
            }
        }
        private string sesamo = "sesamo.png";
        public string Sesamo
        {
            get { return sesamo; }
            set
            {
                if (sesamo != value)
                {
                    sesamo = value;
                    OnPropertyChanged(nameof(Sesamo));
                }
            }
        }
        private string soja = "soja.png";
        public string Soja
        {
            get { return soja; }
            set
            {
                if (soja != value)
                {
                    soja = value;
                    OnPropertyChanged(nameof(Soja));
                }
            }
        }
        private ObservableCollection<AlergenosModel> listadoAlergenos;
        public ObservableCollection<AlergenosModel> ListadoAlergenos
        {
            get { return listadoAlergenos; }
            set
            {
                if (listadoAlergenos != value)
                {
                    listadoAlergenos = value;
                    OnPropertyChanged(nameof(ListadoAlergenos));
                }
            }
        }
        private string sulfitos = "sulfitos.png";
        public string Sulfitos
        {
            get { return sulfitos; }
            set
            {
                if (sulfitos != value)
                {
                    sulfitos = value;
                    OnPropertyChanged(nameof(Sulfitos));
                }
            }
        }

        private int setHeightListView;

        public int SetHeightListView
        {
            get { return setHeightListView; }
            set
            {
                if (setHeightListView != value)
                {
                    setHeightListView = value;
                    OnPropertyChanged(nameof(SetHeightListView));
                }
            }
        }
        #endregion

    }
}