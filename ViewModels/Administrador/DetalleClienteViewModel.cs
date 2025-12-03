using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Services;
using AsadorMoron.Recursos;
using AsadorMoron.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using System.Linq;
using System.Diagnostics;

namespace AsadorMoron.ViewModels.Administrador
{
    public class DetalleClienteViewModel : ViewModelBase
    {
        private UsuarioModel clienteOriginal;

        public DetalleClienteViewModel()
        {
        }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;

                if (navigationData is UsuarioModel cliente)
                {
                    clienteOriginal = cliente;
                    CargarDatosCliente(cliente);

                    Debug.WriteLine($"DetalleClienteViewModel - Cargando datos para cliente ID: {cliente.idUsuario}");

                    // Cargar histórico de pedidos y puntos en paralelo
                    var taskPedidos = ResponseServiceWS.GetHistoricoPedidosClienteAsync(cliente.idUsuario);
                    var taskPuntos = ResponseServiceWS.GetPuntosClienteAsync(cliente.idUsuario);

                    await Task.WhenAll(taskPedidos, taskPuntos);

                    var pedidos = taskPedidos.Result;
                    var puntos = taskPuntos.Result;

                    Debug.WriteLine($"DetalleClienteViewModel - Pedidos recibidos: {pedidos?.Count ?? 0}");
                    Debug.WriteLine($"DetalleClienteViewModel - Puntos recibidos: {puntos?.Count ?? 0}");

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Pedidos
                        if (pedidos != null && pedidos.Count > 0)
                        {
                            HistoricoPedidos = new ObservableCollection<CabeceraPedido>(pedidos);
                            TotalPedidos = HistoricoPedidos.Count;
                            TotalGastado = HistoricoPedidos.Sum(p => p.precioTotalPedido);
                            Debug.WriteLine($"DetalleClienteViewModel - HistoricoPedidos asignado: {HistoricoPedidos.Count}");
                        }
                        else
                        {
                            HistoricoPedidos = new ObservableCollection<CabeceraPedido>();
                            Debug.WriteLine("DetalleClienteViewModel - No hay pedidos");
                        }

                        // Puntos
                        if (puntos != null && puntos.Count > 0)
                        {
                            ListadoPuntos = new ObservableCollection<PuntosUsuarioModel>(puntos);
                            TotalPuntos = ListadoPuntos.Sum(p => p.puntos);
                            Debug.WriteLine($"DetalleClienteViewModel - ListadoPuntos asignado: {ListadoPuntos.Count}");
                        }
                        else
                        {
                            ListadoPuntos = new ObservableCollection<PuntosUsuarioModel>();
                            Debug.WriteLine("DetalleClienteViewModel - No hay puntos");
                        }

                        App.userdialog.HideLoading();
                    });
                }
                else
                {
                    Debug.WriteLine("DetalleClienteViewModel - navigationData no es UsuarioModel");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error DetalleClienteViewModel: " + ex.Message);
                App.userdialog.HideLoading();
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
            {
                App.userdialog.HideLoading();
            }));
        }

        private void CargarDatosCliente(UsuarioModel cliente)
        {
            IdUsuario = cliente.idUsuario;
            Nombre = cliente.nombre;
            Apellidos = cliente.apellidos;
            Email = cliente.email;
            Telefono = cliente.telefono;
            Direccion = cliente.direccion;
            Poblacion = cliente.poblacion;
            Provincia = cliente.provincia;
            CodigoPostal = cliente.codPostal;
            FechaNacimiento = cliente.fechaNacimiento;
            Estado = cliente.estado == 1;
            Saldo = cliente.saldo;
            Kiosko = cliente.kiosko;
        }

        #region Propiedades Cliente

        private int idUsuario;
        public int IdUsuario
        {
            get { return idUsuario; }
            set
            {
                if (idUsuario != value)
                {
                    idUsuario = value;
                    OnPropertyChanged(nameof(IdUsuario));
                }
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
                }
            }
        }

        private string codigoPostal;
        public string CodigoPostal
        {
            get { return codigoPostal; }
            set
            {
                if (codigoPostal != value)
                {
                    codigoPostal = value;
                    OnPropertyChanged(nameof(CodigoPostal));
                }
            }
        }

        private DateTime? fechaNacimiento;
        public DateTime? FechaNacimiento
        {
            get { return fechaNacimiento; }
            set
            {
                if (fechaNacimiento != value)
                {
                    fechaNacimiento = value;
                    OnPropertyChanged(nameof(FechaNacimiento));
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
                }
            }
        }

        private double saldo;
        public double Saldo
        {
            get { return saldo; }
            set
            {
                if (saldo != value)
                {
                    saldo = value;
                    OnPropertyChanged(nameof(Saldo));
                }
            }
        }

        private int kiosko;
        public int Kiosko
        {
            get { return kiosko; }
            set
            {
                if (kiosko != value)
                {
                    kiosko = value;
                    OnPropertyChanged(nameof(Kiosko));
                }
            }
        }

        #endregion

        #region Propiedades Histórico

        private ObservableCollection<CabeceraPedido> historicoPedidos;
        public ObservableCollection<CabeceraPedido> HistoricoPedidos
        {
            get { return historicoPedidos; }
            set
            {
                if (historicoPedidos != value)
                {
                    historicoPedidos = value;
                    OnPropertyChanged(nameof(HistoricoPedidos));
                }
            }
        }

        private int totalPedidos;
        public int TotalPedidos
        {
            get { return totalPedidos; }
            set
            {
                if (totalPedidos != value)
                {
                    totalPedidos = value;
                    OnPropertyChanged(nameof(TotalPedidos));
                }
            }
        }

        private double totalGastado;
        public double TotalGastado
        {
            get { return totalGastado; }
            set
            {
                if (totalGastado != value)
                {
                    totalGastado = value;
                    OnPropertyChanged(nameof(TotalGastado));
                }
            }
        }

        private CabeceraPedido pedidoSeleccionado;
        public CabeceraPedido PedidoSeleccionado
        {
            get { return pedidoSeleccionado; }
            set
            {
                if (pedidoSeleccionado != value)
                {
                    pedidoSeleccionado = value;
                    OnPropertyChanged(nameof(PedidoSeleccionado));

                    if (pedidoSeleccionado != null)
                    {
                        // Expandir/contraer detalles del pedido
                        pedidoSeleccionado.IsDetailVisible = !pedidoSeleccionado.IsDetailVisible;
                    }
                }
            }
        }

        #endregion

        #region Propiedades Puntos

        private ObservableCollection<PuntosUsuarioModel> listadoPuntos;
        public ObservableCollection<PuntosUsuarioModel> ListadoPuntos
        {
            get { return listadoPuntos; }
            set
            {
                if (listadoPuntos != value)
                {
                    listadoPuntos = value;
                    OnPropertyChanged(nameof(ListadoPuntos));
                }
            }
        }

        private int totalPuntos;
        public int TotalPuntos
        {
            get { return totalPuntos; }
            set
            {
                if (totalPuntos != value)
                {
                    totalPuntos = value;
                    OnPropertyChanged(nameof(TotalPuntos));
                }
            }
        }

        #endregion

        #region Comandos

        public ICommand GuardarCommand { get { return new Command(async () => await Guardar()); } }
        public ICommand CambiarEstadoCommand { get { return new Command(async () => await CambiarEstado()); } }
        public ICommand EditarPuntosCommand { get { return new Command<PuntosUsuarioModel>(async (puntos) => await EditarPuntos(puntos)); } }
        public ICommand EditarPuntosDirectoCommand { get { return new Command(async () => await EditarPuntosDirecto()); } }

        #endregion

        #region Métodos

        private async Task Guardar()
        {
            try
            {
                bool confirmar = await App.customDialog.ShowDialogConfirmationAsync(
                    AppResources.App,
                    "¿Desea guardar los cambios del cliente?",
                    AppResources.No,
                    AppResources.Si);

                if (!confirmar) return;

                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }

                var clienteActualizado = new UsuarioModel
                {
                    idUsuario = IdUsuario,
                    nombre = Nombre,
                    apellidos = Apellidos,
                    email = Email,
                    telefono = Telefono,
                    direccion = Direccion,
                    poblacion = Poblacion,
                    provincia = Provincia,
                    codPostal = CodigoPostal,
                    fechaNacimiento = FechaNacimiento ?? DateTime.MinValue,
                    estado = Estado ? 1 : 0,
                    saldo = Saldo,
                    kiosko = Kiosko
                };

                bool resultado = await Task.Run(() => ResponseServiceWS.ActualizarCliente(clienteActualizado));

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    App.userdialog.HideLoading();

                    if (resultado)
                    {
                        await App.customDialog.ShowDialogAsync(
                            "Cliente actualizado correctamente",
                            AppResources.App,
                            AppResources.Aceptar);

                        await App.DAUtil.NavigationService.NavigateBackAsync();
                    }
                    else
                    {
                        await App.customDialog.ShowDialogAsync(
                            "Error al actualizar el cliente",
                            AppResources.SoloError,
                            AppResources.Aceptar);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Guardar: " + ex.Message);
                App.userdialog.HideLoading();
                await App.customDialog.ShowDialogAsync(
                    "Error al guardar: " + ex.Message,
                    AppResources.SoloError,
                    AppResources.Aceptar);
            }
        }

        private async Task CambiarEstado()
        {
            string accion = Estado ? "desactivar" : "activar";
            bool confirmar = await App.customDialog.ShowDialogConfirmationAsync(
                AppResources.App,
                $"¿Desea {accion} este cliente?",
                AppResources.No,
                AppResources.Si);

            if (confirmar)
            {
                Estado = !Estado;
            }
        }

        private async Task EditarPuntos(PuntosUsuarioModel puntosItem)
        {
            try
            {
                if (puntosItem == null) return;

                string resultado = await Application.Current.MainPage.DisplayPromptAsync(
                    "Editar Puntos",
                    $"Establecimiento: {puntosItem.nombreEstablecimiento}\nPuntos actuales: {puntosItem.puntos}",
                    "Guardar",
                    "Cancelar",
                    placeholder: "Nuevos puntos",
                    keyboard: Keyboard.Numeric,
                    initialValue: puntosItem.puntos.ToString());

                if (string.IsNullOrEmpty(resultado)) return;

                if (int.TryParse(resultado, out int nuevosPuntos))
                {
                    if (nuevosPuntos < 0)
                    {
                        await App.customDialog.ShowDialogAsync(
                            "Los puntos no pueden ser negativos",
                            AppResources.SoloError,
                            AppResources.Aceptar);
                        return;
                    }

                    try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }

                    bool success = await Task.Run(() => ResponseServiceWS.ActualizarPuntosCliente(
                        IdUsuario,
                        puntosItem.idEstablecimiento,
                        nuevosPuntos));

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        App.userdialog.HideLoading();

                        if (success)
                        {
                            // Actualizar localmente
                            puntosItem.puntos = nuevosPuntos;
                            var index = ListadoPuntos.IndexOf(puntosItem);
                            if (index >= 0)
                            {
                                ListadoPuntos[index] = puntosItem;
                            }
                            TotalPuntos = ListadoPuntos.Sum(p => p.puntos);

                            await App.customDialog.ShowDialogAsync(
                                "Puntos actualizados correctamente",
                                AppResources.App,
                                AppResources.Aceptar);
                        }
                        else
                        {
                            await App.customDialog.ShowDialogAsync(
                                "Error al actualizar los puntos",
                                AppResources.SoloError,
                                AppResources.Aceptar);
                        }
                    });
                }
                else
                {
                    await App.customDialog.ShowDialogAsync(
                        "Por favor, introduce un número válido",
                        AppResources.SoloError,
                        AppResources.Aceptar);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error EditarPuntos: " + ex.Message);
                App.userdialog.HideLoading();
            }
        }

        private async Task EditarPuntosDirecto()
        {
            try
            {
                // Si hay puntos en la lista, editar el primero (único establecimiento)
                if (ListadoPuntos != null && ListadoPuntos.Count > 0)
                {
                    await EditarPuntos(ListadoPuntos[0]);
                }
                else
                {
                    // No hay puntos registrados, mostrar mensaje
                    await App.customDialog.ShowDialogAsync(
                        "Este cliente no tiene puntos registrados",
                        AppResources.App,
                        AppResources.Aceptar);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error EditarPuntosDirecto: " + ex.Message);
            }
        }

        #endregion
    }
}
