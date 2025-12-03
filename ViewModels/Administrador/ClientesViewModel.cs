using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Services;
using AsadorMoron.Recursos;
using AsadorMoron.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using System.Linq;
using System.Diagnostics;

namespace AsadorMoron.ViewModels.Administrador
{
    public class ClientesViewModel : ViewModelBase
    {
        private List<UsuarioModel> listadoClientesOriginal;
        private CancellationTokenSource searchCancellationToken;

        public ClientesViewModel()
        {
        }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }

                var clientes = await ResponseServiceWS.GetClientesAsync();

                // Ordenar por nombre y apellidos
                listadoClientesOriginal = clientes
                    .OrderBy(c => c.nombre ?? "")
                    .ThenBy(c => c.apellidos ?? "")
                    .ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListadoClientes = new ObservableCollection<UsuarioModel>(listadoClientesOriginal);
                    App.userdialog.HideLoading();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error ClientesViewModel: " + ex.Message);
                MainThread.BeginInvokeOnMainThread(() => App.userdialog.HideLoading());
            }

            await base.InitializeAsync(navigationData);
        }

        #region Propiedades

        private ObservableCollection<UsuarioModel> listadoClientes;
        public ObservableCollection<UsuarioModel> ListadoClientes
        {
            get { return listadoClientes; }
            set
            {
                if (listadoClientes != value)
                {
                    listadoClientes = value;
                    OnPropertyChanged(nameof(ListadoClientes));
                }
            }
        }

        private UsuarioModel clienteSeleccionado;
        public UsuarioModel ClienteSeleccionado
        {
            get { return clienteSeleccionado; }
            set
            {
                if (clienteSeleccionado != value)
                {
                    clienteSeleccionado = value;
                    OnPropertyChanged(nameof(ClienteSeleccionado));

                    if (clienteSeleccionado != null)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                            await App.DAUtil.NavigationService.NavigateToAsync<DetalleClienteViewModel>(ClienteSeleccionado);
                            // Limpiar selección después de navegar
                            clienteSeleccionado = null;
                            OnPropertyChanged(nameof(ClienteSeleccionado));
                        });
                    }
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
                    OnPropertyChanged(nameof(TextoBusqueda));
                    FiltrarClientesConDebounce();
                }
            }
        }

        #endregion

        #region Comandos

        public ICommand BuscarCommand { get { return new Command(() => FiltrarClientes()); } }
        public ICommand LimpiarBusquedaCommand { get { return new Command(LimpiarBusqueda); } }
        public ICommand RefrescarCommand { get { return new Command(async () => await Refrescar()); } }

        #endregion

        #region Métodos

        private void FiltrarClientesConDebounce()
        {
            // Cancelar búsqueda anterior
            searchCancellationToken?.Cancel();
            searchCancellationToken = new CancellationTokenSource();
            var token = searchCancellationToken.Token;

            // Debounce de 300ms para evitar búsquedas mientras el usuario escribe
            Task.Delay(300, token).ContinueWith(t =>
            {
                if (!t.IsCanceled)
                {
                    MainThread.BeginInvokeOnMainThread(() => FiltrarClientes());
                }
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void FiltrarClientes()
        {
            try
            {
                if (listadoClientesOriginal == null) return;

                if (string.IsNullOrWhiteSpace(TextoBusqueda))
                {
                    ListadoClientes = new ObservableCollection<UsuarioModel>(listadoClientesOriginal);
                }
                else
                {
                    var filtro = TextoBusqueda.ToLowerInvariant();

                    // Búsqueda optimizada con StringComparison
                    var clientesFiltrados = listadoClientesOriginal.Where(c =>
                        (!string.IsNullOrEmpty(c.nombre) && c.nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(c.apellidos) && c.apellidos.Contains(filtro, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(c.email) && c.email.Contains(filtro, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(c.telefono) && c.telefono.Contains(filtro, StringComparison.OrdinalIgnoreCase))
                    ).ToList();

                    ListadoClientes = new ObservableCollection<UsuarioModel>(clientesFiltrados);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error FiltrarClientes: " + ex.Message);
            }
        }

        private void LimpiarBusqueda()
        {
            TextoBusqueda = "";
        }

        private async Task Refrescar()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }

                var clientes = await ResponseServiceWS.GetClientesAsync();

                // Ordenar por nombre y apellidos
                listadoClientesOriginal = clientes
                    .OrderBy(c => c.nombre ?? "")
                    .ThenBy(c => c.apellidos ?? "")
                    .ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    FiltrarClientes();
                    App.userdialog.HideLoading();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Refrescar: " + ex.Message);
                MainThread.BeginInvokeOnMainThread(() => App.userdialog.HideLoading());
            }
        }

        #endregion
    }
}
