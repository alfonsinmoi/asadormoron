using AsadorMoron.Interfaces;
using AsadorMoron.ViewModels;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.ViewModels.Informes;
using AsadorMoron.Views;
using AsadorMoron.Views.Informes;
using AsadorMoron.Views.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.ViewModels.Establecimientos;
using AsadorMoron.Views.Establecimientos;
using AsadorMoron.ViewModels.Clientes;
using AsadorMoron.ViewModels.Administrador;
using AsadorMoron.ViewModels.Repartidores;
using AsadorMoron.Views.Administrador;
using AsadorMoron.Views.Clientes;
using AsadorMoron.Views.Repartidores;
using AsadorMoron.ViewModels.Contabilidad;
using AsadorMoron.Views.Contabilidad;
using AppMainPage = AsadorMoron.Views.MainPage;

namespace AsadorMoron.Services
{
    public class NavigationService : INavigationService
    {
        public Lazy<Dictionary<Type, Type>> _mappings = new Lazy<Dictionary<Type, Type>>();

        private CustomNavigationPage navigationPage;

        private AppMainPage mainPage;

        internal Application CurrentApplication
        {
            get
            {
                return Application.Current;
            }
        }

        public NavigationService()
        {
            if(_mappings == null || !_mappings.IsValueCreated)
                _mappings = CreatePageViewModelMappings();
        }

        public Task InitializeAsync()
        {
            return NavigateToAsync<MainPageViewModel>();
        }

        public async Task NavigateBackAsync()
        {
            if (CurrentApplication.MainPage != null)
            {
                var mainPage = CurrentApplication.MainPage as AppMainPage;
                var navigationPage = mainPage.Detail as CustomNavigationPage;
                if (navigationPage != null)
                {
                    await navigationPage.PopAsync(true);
                }
                else
                {
                    //Podria poner throw 
                }
            }
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase
        {
            return NavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return NavigateToAsync(typeof(TViewModel), parameter);
        }

        public Task NavigateToAsync(Type viewModelType)
        {
            return NavigateToAsync(viewModelType, null);
        }

        public Task NavigateToAsyncWithoutMenu<TViewModel>() where TViewModel : ViewModelBase
        {
            return NavigateToAsync(typeof(TViewModel), null,false);
        }

        public Task NavigateToAsyncWithoutMenu<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return NavigateToAsync(typeof(TViewModel), parameter,false);
        }

        public Task NavigateToAsyncWithoutMenu(Type viewModelType)
        {
            return NavigateToAsync(viewModelType, null,false);
        }

        public Task NavigateToAsyncMenu<TViewModel>() where TViewModel : ViewModelBase
        {
            return NavigateToAsyncMenu(typeof(TViewModel), null);
        }

        public Task NavigateToAsyncMenu<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return NavigateToAsyncMenu(typeof(TViewModel), parameter);
        }

        public Task NavigateToAsyncMenu(Type viewModelType)
        {
            return NavigateToAsyncMenu(viewModelType, null);
        }

        Task INavigationService.NavigateToAsyncMenu(Type viewModelType, object parameter)
        {
            return NavigateToAsyncMenu(viewModelType, parameter);
        }

        public Task LogOutAppAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return LogOutAppAsync(typeof(TViewModel), parameter);
        }

        

        protected virtual async Task NavigateToAsync(Type viewModelType, object parameter,bool visibleMenu=true)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine($"[PERF] NavigateToAsync INICIO: {viewModelType.Name}");

            if (!App.ViendoDocumento)
            {
                Page page = CreateAndBindPage(viewModelType, parameter);
                System.Diagnostics.Debug.WriteLine($"[PERF] CreateAndBindPage completado: {sw.ElapsedMilliseconds}ms");

                if (page is AppMainPage)
                {
                    CurrentApplication.MainPage = page;
                }
                else if (CurrentApplication.MainPage is AppMainPage)
                {

                    mainPage = CurrentApplication.MainPage as AppMainPage;
                    mainPage.IsGestureEnabled = visibleMenu;
                    navigationPage = mainPage.Detail as CustomNavigationPage;

                    if (navigationPage != null)
                    {
                        if (!App.DAUtil.ConFoto && App.DAUtil.ExportandoExcel == false)
                        {
                            navigationPage.BarTextColor = Color.FromArgb("#f0dd49");
                            navigationPage.BackgroundColor = Colors.Black;
                            // Kiosko: sin animación para mayor velocidad
                            bool animado = (App.DAUtil.Usuario?.kiosko ?? 0) != 1;
                            System.Diagnostics.Debug.WriteLine($"[PERF] Antes PushAsync (animado={animado}): {sw.ElapsedMilliseconds}ms");
                            await navigationPage.PushAsync(page, animado);
                            System.Diagnostics.Debug.WriteLine($"[PERF] Después PushAsync: {sw.ElapsedMilliseconds}ms");
                        }
                        else
                        {
                            App.DAUtil.ConFoto = false;
                            App.DAUtil.ExportandoExcel = false;
                        }
                    }
                    else
                    {
                        navigationPage = new CustomNavigationPage(page);
                        navigationPage.BarTextColor = Color.FromArgb("#f0dd49");
                        navigationPage.BackgroundColor = Colors.Black;
                        mainPage.Detail = navigationPage;
                    }
                    mainPage.IsPresented = false;
                }
                else
                {
                    if (CurrentApplication.MainPage is CustomNavigationPage navigationPage)
                    {
                        navigationPage.BarTextColor = Color.FromArgb("#f0dd49");
                        navigationPage.BackgroundColor = Colors.Black;
                        // Kiosko: sin animación para mayor velocidad
                        bool animado = (App.DAUtil.Usuario?.kiosko ?? 0) != 1;
                        await navigationPage.PushAsync(page, animado).ConfigureAwait(false);
                    }
                    else
                    {

                        CurrentApplication.MainPage = new CustomNavigationPage(page);
                    }
                }
            }
            else
                App.ViendoDocumento = false;

            System.Diagnostics.Debug.WriteLine($"[PERF] NavigateToAsync FIN: {sw.ElapsedMilliseconds}ms");
        }

        protected virtual async Task NavigateToAsyncMenu(Type viewModelType, object parameter)
        {
            try
            {
                Page page = CreateAndBindPage(viewModelType, parameter);
                if (page == null)
                {
                    Console.WriteLine($"Failed to create page for {viewModelType.Name}");
                    return;
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    mainPage = CurrentApplication.MainPage as AppMainPage;
                    if (mainPage == null)
                        mainPage = new AppMainPage();

                    navigationPage = new CustomNavigationPage(page);
                    navigationPage.BarTextColor = Color.FromArgb("#f0dd49");
                    navigationPage.BackgroundColor = Colors.Black;
                    mainPage.Detail = navigationPage;

                    mainPage.IsPresented = false;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NavigateToAsyncMenu error: {ex.Message}");
            }
        }

        protected Type GetPageTypeForViewModel(Type viewModelType)
        {
            if (!_mappings.Value.ContainsKey(viewModelType))
            {
                throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings");
            }
            return _mappings.Value[viewModelType];
        }

        private Page CreateAndBindPage(Type viewModelType, object parameter)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine($"[PERF] CreateAndBindPage INICIO: {viewModelType.Name}");
            try
            {
                Type pageType = GetPageTypeForViewModel(viewModelType);
                System.Diagnostics.Debug.WriteLine($"[PERF] GetPageTypeForViewModel: {sw.ElapsedMilliseconds}ms");

                if (pageType == null)
                {
                    throw new Exception($"Mapping type for {viewModelType} is not a page");
                }

                Page page = Activator.CreateInstance(pageType) as Page;
                System.Diagnostics.Debug.WriteLine($"[PERF] Activator.CreateInstance (Page): {sw.ElapsedMilliseconds}ms");

                ViewModelBase viewModel = ViewModelLocator.Instance.Resolve(viewModelType) as ViewModelBase;
                System.Diagnostics.Debug.WriteLine($"[PERF] ViewModelLocator.Resolve: {sw.ElapsedMilliseconds}ms");

                App.DAUtil.WriteLine(page.GetType().Name, viewModel.GetType().Name);
                page.BindingContext = viewModel;
                System.Diagnostics.Debug.WriteLine($"[PERF] BindingContext set: {sw.ElapsedMilliseconds}ms");

                page.Appearing += async (object sender, EventArgs e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[PERF] Page.Appearing DISPARADO para {viewModelType.Name}");
                    try
                    {
                        await viewModel.InitializeAsync(parameter);
                        System.Diagnostics.Debug.WriteLine($"[PERF] Page.Appearing InitializeAsync COMPLETADO para {viewModelType.Name}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PERF] Page.Appearing ERROR para {viewModelType.Name}: {ex.Message}");
                    }
                };
                System.Diagnostics.Debug.WriteLine($"[PERF] CreateAndBindPage FIN: {sw.ElapsedMilliseconds}ms");
                return page;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Task InsertPageBefore(Type viewModelType)
        {
            mainPage = CurrentApplication.MainPage as AppMainPage;
            return NavigateToAsync(viewModelType, null);
        }

        public void InsertPageBefore<T>()
        {
            Page page = CreateAndBindPage(typeof(T), null);
            CurrentApplication.MainPage = new CustomNavigationPage(page);
        }

        private Lazy<Dictionary<Type, Type>> CreatePageViewModelMappings()
        {
            _mappings.Value.Add(typeof(MenuLateralViewModel), typeof(MenuLateral));
            _mappings.Value.Add(typeof(MainPageViewModel), typeof(AppMainPage));
            _mappings.Value.Add(typeof(LoginViewModel), typeof(LoginView));
            _mappings.Value.Add(typeof(HomeViewModelEstMobile), typeof(HomeViewEstMobile));
            _mappings.Value.Add(typeof(HomeViewModelEst), typeof(HomeViewEst));
            _mappings.Value.Add(typeof(PerfilViewModel), typeof(PerfilView));
            _mappings.Value.Add(typeof(DetalleComidaViewModel), typeof(DetalleComidaView));
            _mappings.Value.Add(typeof(CategoriasViewModel), typeof(CategoriasView));
            _mappings.Value.Add(typeof(DetalleEstablecimientoViewModel), typeof(DetalleEstablecimientoView));
            _mappings.Value.Add(typeof(DetalleEstablecimientoParaClienteViewModel), typeof(DetalleEstablecimientoParaClienteView));
            _mappings.Value.Add(typeof(DetalleCategoriaViewModel), typeof(DetalleCategoriaView));
            _mappings.Value.Add(typeof(DetalleArticuloViewModel), typeof(DetalleArticulosView));
            _mappings.Value.Add(typeof(CartaViewModel), typeof(CartaView));
            _mappings.Value.Add(typeof(PinViewModel), typeof(PinView));
            _mappings.Value.Add(typeof(QuienViewModel), typeof(QuienView));
            _mappings.Value.Add(typeof(DetallePedidoViewModel), typeof(DetallePedidoView));
            _mappings.Value.Add(typeof(HistoricoPedidosViewModelClient), typeof(HistoricoPedidosView));
            _mappings.Value.Add(typeof(HistoricoPedidosViewModelAdmin), typeof(HistoricoPedidosViewAdmin));
            _mappings.Value.Add(typeof(HistoricoPedidosViewModelEst), typeof(HistoricoPedidosPolloAndaluzEstView));
            _mappings.Value.Add(typeof(ConfiguracionEstablecimientoViewModel), typeof(ConfiguracionEstablecimientoView));
            _mappings.Value.Add(typeof(ArticulosViewModel), typeof(ArticulosView));
            _mappings.Value.Add(typeof(RecoveryViewModel), typeof(RecoveryView));
            _mappings.Value.Add(typeof(PedidoConfirmadoViewModel), typeof(PedidoConfirmadoView));
            _mappings.Value.Add(typeof(VersionMinimaViewModel), typeof(VersionMinimaPopUp));
            _mappings.Value.Add(typeof(FranjasHorariasViewModel), typeof(FranjasHorariasView));
            _mappings.Value.Add(typeof(ZonasViewModel), typeof(ZonasView));
            _mappings.Value.Add(typeof(DetalleZonaViewModel), typeof(DetalleZonaView));
            _mappings.Value.Add(typeof(IngredientesViewModel), typeof(IngredientesView));
            _mappings.Value.Add(typeof(DetalleIngredienteViewModel), typeof(DetalleIngredienteView));
            _mappings.Value.Add(typeof(HomeViewModelRepartidor), typeof(HomeViewRepartidor));
            _mappings.Value.Add(typeof(DetalleRepartidorViewModel), typeof(DetalleRepartidorView));
            _mappings.Value.Add(typeof(RepartidoresViewModel), typeof(RepartidoresView));
            _mappings.Value.Add(typeof(PopupPageRepartidoresViewModel), typeof(PopupPageRepartidores));
            _mappings.Value.Add(typeof(UsuariosViewModel), typeof(UsuariosView));
            _mappings.Value.Add(typeof(EnviarNotificacionViewModel), typeof(EnviarNotificacionView));
            _mappings.Value.Add(typeof(PedidoConfirmadoComercioViewModel), typeof(PedidoConfirmadoComercioView));
            _mappings.Value.Add(typeof(MisTarjetasViewModel), typeof(TarjetasView));
            _mappings.Value.Add(typeof(AddCreditCardPageViewModel), typeof(AddCreditCardPage));
            _mappings.Value.Add(typeof(ArticulosBajaViewModel), typeof(ArticulosBajaView));
            _mappings.Value.Add(typeof(InformeBeneficiosViewModel), typeof(InformeBeneficiosView));
            _mappings.Value.Add(typeof(InformeTicketMedioViewModel), typeof(InformeTicketMedioView));
            _mappings.Value.Add(typeof(ClientesInactivosViewModel), typeof(ClientesInactivosView));
            _mappings.Value.Add(typeof(MejoresClientesViewModel), typeof(MejoresClientesView));
            _mappings.Value.Add(typeof(ProductosMasVendidosViewModel), typeof(ProductosMasVendidosView));
            _mappings.Value.Add(typeof(InformesViewModel), typeof(informesView));
            _mappings.Value.Add(typeof(RegistroViewModel), typeof(RegistroView));
            _mappings.Value.Add(typeof(ValorarPedidoViewModel), typeof(ValorarPedidoView));
            _mappings.Value.Add(typeof(WebViewModel), typeof(MyWebView));
            _mappings.Value.Add(typeof(PagoErroneoViewModel), typeof(PagoErroneoView));
            _mappings.Value.Add(typeof(DatosFiscalesEstablecimientoViewModel), typeof(DatosFiscalesEstablecimientosView));
            _mappings.Value.Add(typeof(AutoPedidoViewModel), typeof(AutoPedidoView));
            _mappings.Value.Add(typeof(FranjasHorariasEncargoViewModel), typeof(FranjasHorariasEncargoView));
            _mappings.Value.Add(typeof(CuentasEstablecimientoViewModel), typeof(CuentasEstablecimientoView));
            _mappings.Value.Add(typeof(CuentasAdministradorViewModel), typeof(CuentasAdministradorView));
            _mappings.Value.Add(typeof(HomeContabilidadViewModel), typeof(HomeContabilidadView));
            _mappings.Value.Add(typeof(HorariosEstablecimientoViewModel), typeof(HorariosEstablecimientoView));
            _mappings.Value.Add(typeof(CuponesViewModel), typeof(CuponesView));
            _mappings.Value.Add(typeof(CuponesEstViewModel), typeof(CuponesEstView));
            _mappings.Value.Add(typeof(DetalleCuponViewModel), typeof(DetalleCuponView));
            _mappings.Value.Add(typeof(DetalleCuponesEstViewModel), typeof(DetalleCuponesEstView));
            _mappings.Value.Add(typeof(AutoPedidoMinViewModel), typeof(AutoPedidoMinView));
            _mappings.Value.Add(typeof(HistoricoPedidosPuntosPolloAndaluzViewModelAdmin), typeof(HistoricoPedidosPuntosPolloAndaluzViewAdmin));
            _mappings.Value.Add(typeof(FacturasEstablecimientoViewModel), typeof(FacturasEstablecimientoView));
            _mappings.Value.Add(typeof(AutoPedidoAdminViewModel), typeof(AutoPedidoAdminView));
            _mappings.Value.Add(typeof(FacturasAdministradorViewModel), typeof(FacturasAdministradorView));
            _mappings.Value.Add(typeof(InvitaAmigoViewModel), typeof(InvitaAmigoView));
            _mappings.Value.Add(typeof(NumerosSorteoViewModel), typeof(NumerosSorteoView));
            _mappings.Value.Add(typeof(CartaProductosViewModel), typeof(CartaProductosView));
            _mappings.Value.Add(typeof(CartaProductosNavidadViewModel), typeof(CartaProductosNavidadView));
            _mappings.Value.Add(typeof(GastoViewModel), typeof(GastoView));
            _mappings.Value.Add(typeof(HistoricoPedidosAnuladosViewModelAdmin), typeof(HistoricoPedidosAnuladosViewAdmin));
            _mappings.Value.Add(typeof(DetalleArticuloNavidadViewModel), typeof(DetalleArticuloNavidadView));
            _mappings.Value.Add(typeof(ClientesViewModel), typeof(ClientesView));
            _mappings.Value.Add(typeof(DetalleClienteViewModel), typeof(DetalleClienteView));
            return _mappings;
        }
        public void LogOutApp(Type viewModelType, object parameter)
        {
            Page page = CreateAndBindPage(viewModelType, parameter);
            CurrentApplication.MainPage = page;
        }

        protected virtual async Task LogOutAppAsync(Type viewModelType, object parameter)
        {
            Page page = CreateAndBindPage(viewModelType, parameter);
            CurrentApplication.MainPage = page;
        }

    }
}
