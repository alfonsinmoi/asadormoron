using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using AsadorMoron.Services;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using AsadorMoron.Recursos;
using AsadorMoron.Messages;
// 
using AsadorMoron.ViewModels.Informes;
using AsadorMoron.ViewModels.Establecimientos;
using AsadorMoron.ViewModels.Administrador;
using AsadorMoron.ViewModels.Clientes;
using AsadorMoron.ViewModels.Repartidores;
using System.Linq;

namespace AsadorMoron.ViewModels
{
    public class MenuLateralViewModel : ViewModelBase
    {
        private ObservableCollection<ItemMenuLateralModel> listaMenuByRol;
        public ObservableCollection<ItemMenuLateralModel> ListaMenuByRol
        {
            get { return listaMenuByRol; }
            set
            {
                if (listaMenuByRol != value)
                {
                    listaMenuByRol = value;
                    OnPropertyChanged(nameof(ListaMenuByRol));
                }
            }
        }
        private int totalUsuarios;
        public int TotalUsuarios
        {
            get
            {
                return totalUsuarios;
            }
            set
            {
                if (totalUsuarios != value)
                {
                    totalUsuarios = value;
                    OnPropertyChanged(nameof(TotalUsuarios));
                }
            }
        }
        public ICommand PerfilCommand { get { return new Command(IrAPerfil); } }
        public ICommand ContadorCommand { get { return new Command(ActualizaContador); } }
        public ICommand ItemSelected { get { return new Command((parametro) => Seleccionado(parametro)); } }



        void Seleccionado(object param)
        {
            ItemMenuLateralModel item = (ItemMenuLateralModel)param;
            if (item != null)
            {
                try
                {
                    if (!item.TieneHijos)
                    {
                        bool pasa = true;

                        if (item.Title.Equals(AppResources.CerrarSesion))
                        {
                            if (App.DAUtil.DoIHaveInternet())
                                App.ResponseWS.BorraToken(App.DAUtil.Usuario.email);
                            App.DAUtil.DeleteUsuarioSQLite();
                            App.DAUtil.VaciaConfig();
                            App.DAUtil.VaciaCarrito();
                            if (App.DAUtil.Usuario.rol == (int)RolesEnum.Cliente)
                                ResponseServiceWS.GuardaOnline(2);
                            App.DAUtil.Usuario = null;
                            GetListByRol();
                            Preferences.Set("Social", false);
                            Preferences.Set("RedSocial", "");
                            try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await App.DAUtil.NavigationService.InitializeAsync();
                            });

                        }
                        else if (item.Title.ToUpper().Equals(AppResources.MiEstablecimiento.ToUpper()))
                        {
                            if (App.DAUtil.Usuario != null)
                            {
                                if (App.DAUtil.Usuario.rol == 2)
                                {

                                    try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                                    MainThread.BeginInvokeOnMainThread(async () =>
                                    {
                                        await App.DAUtil.NavigationService.NavigateToAsyncMenu<DetalleEstablecimientoViewModel>(App.EstActual);
                                    });
                                }
                                else
                                {
                                    MainThread.BeginInvokeOnMainThread(async () =>
                                    {
                                        await App.DAUtil.NavigationService.NavigateToAsyncMenu(item.TargetType, false);
                                    });
                                }
                            }
                            else
                            {
                                MainThread.BeginInvokeOnMainThread(async () =>
                                {
                                    await App.DAUtil.NavigationService.NavigateToAsyncMenu(item.TargetType, false);
                                });
                            }
                        }
                        else
                        {
                            if (App.DAUtil.Usuario != null)
                            {
                                if (string.IsNullOrEmpty(App.DAUtil.Usuario.provincia) || string.IsNullOrEmpty(App.DAUtil.Usuario.poblacion) || string.IsNullOrEmpty(App.DAUtil.Usuario.direccion) || App.DAUtil.Usuario.idZona == 0)
                                    pasa = false;
                            }
                            if (pasa)
                            {
                                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                                MainThread.BeginInvokeOnMainThread(async () =>
                                {
                                    await App.DAUtil.NavigationService.NavigateToAsyncMenu(item.TargetType, false);
                                });
                            }
                            else
                            {
                                try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

                                MainThread.BeginInvokeOnMainThread(async () =>
                                {
                                    await App.customDialog.ShowDialogAsync(AppResources.CompleteCampos, AppResources.App, AppResources.Cerrar);
                                    await App.DAUtil.NavigationService.NavigateToAsyncMenu(typeof(PerfilViewModel), false);
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 
                }
            }
        }
        void ActualizaContador()
        {
            TotalUsuarios = ResponseServiceWS.contadorUsuarios();
        }
        void IrAPerfil()
        {
            try
            {
                if (App.DAUtil.Usuario != null)
                {
                    App.userdialog.ShowLoading(AppResources.Cargando);
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        App.DAUtil.Idioma = "ES";
                        await App.DAUtil.NavigationService.NavigateToAsyncMenu<PerfilViewModel>();
                    });
                }
            }
            catch (Exception)
            {

            }
        }

        private String foto = "logo_producto.png";
        public String Foto
        {
            get { return foto; }
            set
            {
                if (foto != value)
                {
                    foto = value;
                    OnPropertyChanged(nameof(Foto));
                }
            }
        }

        private String nombre;
        public String Nombre
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
        private String codigo = "";
        public String Codigo
        {
            get { return codigo; }
            set
            {
                if (codigo != value)
                {
                    codigo = value;
                    OnPropertyChanged(nameof(Codigo));
                }
            }
        }
        private bool tieneCodigoAmigo = false;
        public bool TieneCodigoAmigo
        {
            get { return tieneCodigoAmigo; }
            set
            {
                if (tieneCodigoAmigo != value)
                {
                    tieneCodigoAmigo = value;
                    OnPropertyChanged(nameof(TieneCodigoAmigo));
                }
            }
        }
        public MenuLateralViewModel()
        {
            if (App.DAUtil.Usuario != null)
            {
                if (App.DAUtil.Usuario.rol == (int)RolesEnum.Administrador || App.DAUtil.Usuario.rol == (int)RolesEnum.SuperAdmin)
                {
                    TotalUsuarios = ResponseServiceWS.contadorUsuarios();
                    EsAdmin = true;
                }
                else
                    EsAdmin = false;
            }
        }
        public ObservableCollection<ItemMenuLateralModel> GetListByRol()
        {
            int rol = 0;
            if (App.DAUtil.Usuario != null)
                rol = App.DAUtil.Usuario.rol;
            var menus = ResponseServiceWS.getMenu(rol);

            var menuItems = new ObservableCollection<ItemMenuLateralModel>();
            try
            {
                string nombreMenu = "";
                foreach (MenuModel m in menus)
                {
                    m.viewmodel = m.viewmodel.Replace("PolloAndaluz", "AsadorMoron");
                    m.imagen = m.imagen.Replace("Images/", "");
                    nombreMenu = m.nombre;
                    if (m.viewmodel.Contains("ClientesView"))
                    {
                        string a = "";
                    }
                    if (m.idParent == 0)
                    {
                        List<MenuModel> hijos = menus.Where(p => p.idParent == m.id).ToList();
                        if (hijos == null)
                            hijos = new List<MenuModel>();
                        ItemMenuLateralModel ml = (new ItemMenuLateralModel
                        {
                            Title = nombreMenu,
                            IconSource = m.imagen,
                            TargetType = Type.GetType(m.viewmodel),
                            TieneHijos = hijos.Count > 0,
                            Hijos = new List<ItemMenuLateralModel>()
                        });
                        foreach (MenuModel m2 in hijos)
                        {
                            nombreMenu = m2.nombre;
                            ml.Hijos.Add(new ItemMenuLateralModel
                            {
                                Title = nombreMenu,
                                IconSource = m2.imagen,
                                TargetType = Type.GetType(m2.viewmodel),
                                TieneHijos = false
                            });
                        }
                        menuItems.Add(ml);
                    }
                }
            }
            catch (Exception ex)
            {
                // 
                menuItems.Add(new ItemMenuLateralModel
                {
                    Title = AppResources.Inicio,
                    IconSource = "home.png",
                    TargetType = typeof(CartaViewModel)
                });
                menuItems.Add(new ItemMenuLateralModel
                {
                    Title = AppResources.Acceder,
                    IconSource = "login.png",
                    TargetType = typeof(LoginViewModel)
                });
                CerrarSesion();
            }
            return menuItems;
        }
        private void CerrarSesion()
        {
            if (App.DAUtil.DoIHaveInternet())
                App.ResponseWS.BorraToken(App.DAUtil.Usuario.email);
            App.DAUtil.DeleteUsuarioSQLite();
            App.DAUtil.VaciaConfig();
            App.DAUtil.VaciaCarrito();
            App.DAUtil.Usuario = null;
            Preferences.Set("Social", false);
            Preferences.Set("RedSocial", "");
            try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await App.DAUtil.NavigationService.InitializeAsync();
            });
        }
        private bool esAdmin = false;
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
        private string version = "";
        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                if (version != value)
                {
                    version = value;
                    OnPropertyChanged(nameof(Version));
                }
            }
        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                if (DeviceInfo.Platform.ToString() == "Android")
                    Version = AppResources.Version + ": " + App.DAUtil.versionAndroid;
                else if (DeviceInfo.Platform.ToString() == "iOS")
                    Version = AppResources.Version + ": " + App.DAUtil.versioniOS;
                else
                    Version = AppResources.Version + ": " + App.DAUtil.versionUWP;
                ListaMenuByRol = GetListByRol();
                if (App.DAUtil.Usuario != null)
                {
                    if (App.DAUtil.Usuario.rol == (int)RolesEnum.Administrador || App.DAUtil.Usuario.rol == (int)RolesEnum.SuperAdmin)
                    {
                        TotalUsuarios = ResponseServiceWS.contadorUsuarios();
                        EsAdmin = true;
                    }
                    else
                        EsAdmin = false;

                    if (App.DAUtil.Usuario.rol == (int)RolesEnum.Cliente)
                    {
                        if (App.promocionAmigo != null)
                        {
                            if (App.promocionAmigo.activo && DateTime.Now >= App.promocionAmigo.fechaInicio && DateTime.Now <= App.promocionAmigo.fechaFin)
                            {
                                Codigo = App.DAUtil.Usuario.codigo;
                                TieneCodigoAmigo = true;
                            }
                        }
                    }

                    Nombre = App.DAUtil.Usuario.nombre;
                    if (App.DAUtil.Usuario.foto.Contains("platform") || App.DAUtil.Usuario.foto.Contains("pbs.twimg.com"))
                    {
                        Foto = App.DAUtil.Usuario.foto;
                    }
                    else if (!App.DAUtil.Usuario.foto.Equals(""))
                    {
                        Foto = App.DAUtil.Usuario.foto;
                    }
                    else
                    {
                        Foto = "perfil.png";
                    }
                }
                else
                    Nombre = AppResources.Invitado;
            }
            catch (Exception ex)
            {
                // 
            }
            await base.InitializeAsync(navigationData);
            //await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }
    }
}
