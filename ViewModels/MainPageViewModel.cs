using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
// 
using AsadorMoron.ViewModels.Clientes;
using AsadorMoron.ViewModels.Repartidores;
using AsadorMoron.ViewModels.Administrador;
using AsadorMoron.ViewModels.Establecimientos;
using AsadorMoron.ViewModels.Informes;
using AsadorMoron.ViewModels.Contabilidad;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {

        private MenuLateralViewModel _menuLateralViewModel;

        public MainPageViewModel() : this(ViewModelLocator.Instance.Resolve<MenuLateralViewModel>())
        {
        }

        public MainPageViewModel(MenuLateralViewModel menuLateralViewModel)
        {
            _menuLateralViewModel = menuLateralViewModel;
        }

        public MenuLateralViewModel MenuViewModel
        {
            get
            {
                return _menuLateralViewModel;
            }

            set
            {
                _menuLateralViewModel = value;
                OnPropertyChanged(nameof(MenuLateralViewModel));
            }
        }

        public override Task InitializeAsync(object navigationData)
        {

            int rol = 0;
            if (App.DAUtil.Usuario != null)
                rol = App.DAUtil.Usuario.rol;
            //var rol = "Admin"; // GlobalSettings.usuarioLoged.rol.ToString();
            //rol = "Usuario";
            string PIN = Preferences.Get("PIN", "");
            bool versionBuena = false;
            Task result;
            try
            {
                if((DeviceInfo.Platform.ToString() == "Android" && Preferences.Get("VersionMinimaAndroid", 0) <= App.DAUtil.versionAppAndroid) || (DeviceInfo.Platform.ToString() == "iOS" && Preferences.Get("VersionMinimaiOS", 0) <= App.DAUtil.versionAppiOS))
                    versionBuena = true;

                if ((string.IsNullOrEmpty(PIN) && versionBuena) || DeviceInfo.Platform.ToString() == "WinUI")
                {
                    if (App.tieneShort)
                    {
                        App.tieneShort = false;
                        App.EstActual = App.EstShortCode;
                        result = Task.WhenAll(_menuLateralViewModel.InitializeAsync(navigationData), App.DAUtil.NavigationService.NavigateToAsync<CartaViewModel>(App.EstShortCode));
                    }
                    else
                    {
                        switch (rol)
                        {
                            case (int)RolesEnum.Cliente: //Usuario
                                if (string.IsNullOrEmpty(App.DAUtil.Usuario.provincia) || string.IsNullOrEmpty(App.DAUtil.Usuario.poblacion) || string.IsNullOrEmpty(App.DAUtil.Usuario.direccion) || App.DAUtil.Usuario.idZona == 0)
                                {
                                    result = Task.WhenAll(_menuLateralViewModel.InitializeAsync(navigationData), App.DAUtil.NavigationService.NavigateToAsync<PerfilViewModel>());
                                }
                                else
                                {
                                        result = Task.WhenAll(_menuLateralViewModel.InitializeAsync(navigationData), App.DAUtil.NavigationService.NavigateToAsync<CartaViewModel>(App.EstActual));
                                    
                                }
                                break;
                            case (int)RolesEnum.Establecimiento: //Restaurante
                                if (DeviceInfo.Platform.ToString() == "WinUI")
                                    result = Task.WhenAll(_menuLateralViewModel.InitializeAsync(navigationData), App.DAUtil.NavigationService.NavigateToAsync<HomeViewModelEst>());
                                else
                                    result = Task.WhenAll(_menuLateralViewModel.InitializeAsync(navigationData), App.DAUtil.NavigationService.NavigateToAsync<HomeViewModelEstMobile>());
                                break;
                            case (int)RolesEnum.Repartidor: //REpartidor
                                result = Task.WhenAll(_menuLateralViewModel.InitializeAsync(navigationData), App.DAUtil.NavigationService.NavigateToAsync<HomeViewModelRepartidor>());
                                break;
                            case (int)RolesEnum.Contable: //Contable
                                result = Task.WhenAll(_menuLateralViewModel.InitializeAsync(navigationData), App.DAUtil.NavigationService.NavigateToAsync<HomeContabilidadViewModel>());
                                break;
                            default:
                                result = Task.WhenAll(_menuLateralViewModel.InitializeAsync(navigationData), App.DAUtil.NavigationService.NavigateToAsync<CartaViewModel>(App.EstActual));
                                break;
                        }
                    }
                }
                else if (versionBuena)
                {
                    result = Task.WhenAll(_menuLateralViewModel.InitializeAsync(navigationData), App.DAUtil.NavigationService.NavigateToAsync<PinViewModel>());
                }
                else
                {
                    result = Task.WhenAll(_menuLateralViewModel.InitializeAsync(navigationData), App.DAUtil.NavigationService.NavigateToAsync<VersionMinimaViewModel>());
                }
            }
            catch (Exception ex)
            {
                // 
                result = Task.WhenAll(_menuLateralViewModel.InitializeAsync(navigationData), App.DAUtil.NavigationService.NavigateToAsync<CartaViewModel>(App.EstActual));
            }
            return result;
        }
    }
}
