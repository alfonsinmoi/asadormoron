using System;
using AsadorMoron.Interfaces;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using AsadorMoron.Services;
using System.Collections.Generic;

namespace AsadorMoron.ViewModels.Repartidores
{
    public class GastoViewModel:ViewModelBase
    {
        public GastoViewModel()
        {
        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {

                App.DAUtil.EnTimer = false;
                try
                {
                    Concepto = "";
                    Precio = "";
                    Tipo = new ObservableCollection<string>();
                    Tipo.Add("Gasto");
                    Tipo.Add("Ingreso");
                    TipoSeleccionado = "Ingreso";
                }
                catch (Exception)
                {
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
        private async Task initGuardar()
        {
            try
            {
                if (double.Parse(Precio.Replace(".", ",")) == 0)
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync("El precio debe ser distinto de 0", AppResources.App, AppResources.Aceptar);
                }
                else if (string.IsNullOrEmpty(Concepto))
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync("Debe indicar el concepto", AppResources.App, AppResources.Aceptar);
                }
                else
                {
                    if (TipoSeleccionado.Equals("Gasto"))
                    {
                        GastoModel Gasto = new GastoModel();
                        Gasto.concepto = Concepto;
                        Gasto.precio = double.Parse(Precio.Replace(".", ","));
                        if (TipoSeleccionado.Equals("Gasto"))
                            Gasto.precio = Gasto.precio * -1;
                        Gasto.idRepartidor = App.DAUtil.Usuario.Repartidor.id;
                        Gasto.fecha = DateTime.Now;

                        if (App.ResponseWS.nuevoGasto(Gasto) == 1)
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync("La nota se ha creado correctamente", AppResources.App, AppResources.Aceptar);
                            MainThread.BeginInvokeOnMainThread(async () => { await App.DAUtil.NavigationService.InitializeAsync(); });
                        }
                        else
                        {
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                        }
                    }
                    else
                    {
                        ConfiguracionAdmin configuracionAdmin = ResponseServiceWS.getConfiguracionAdmin();
                        AutoPedidoModel auto = new AutoPedidoModel();
                        auto.apellidos = App.DAUtil.Usuario.apellidos;
                        auto.codigo = App.DAUtil.GetCodigo();
                        auto.direccion = "";
                        auto.hora = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        auto.idEstablecimiento = 67;
                        auto.idZona = App.EstActual.idZona;
                        auto.importe = double.Parse(Precio.Replace(".", ","));
                        auto.nombre = App.DAUtil.Usuario.nombre;
                        auto.telefono = "";
                        auto.tipoPago = "Efectivo";
                        auto.idPueblo = 1;
                        auto.codPostal = "41530";
                        auto.poblacion = "Morón de la Frontera";
                        auto.provincia = "Sevilla";
                        auto.comentario = Concepto;
                        auto.idUsuario = App.DAUtil.Usuario.idUsuario;
                        int idCodigoPedido = ResponseServiceWS.NuevoAutoPedidoRep(auto);
                        if (idCodigoPedido > 0)
                        {
                            string pedido = string.Empty;
                            List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(67);
                            foreach (TokensModel to in tokens3)
                                App.ResponseWS.enviaNotificacion("Asador Morón", "Nuevo Pedido: " + auto.codigo, to.token);

                            List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(1);
                            foreach (TokensModel to in tokens)
                                App.ResponseWS.enviaNotificacion("Asador Morón", "Nuevo Pedido para " + "Asador Morón" + ": " + auto.codigo, to.token);

                            List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(67);
                            foreach (TokensModel to in tokens2)
                                App.ResponseWS.enviaNotificacion("Asador Morón", "Nuevo Pedido para " + "Asador Morón" + ": " + auto.codigo, to.token);
                        }
                        else
                        {
                            await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
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
        #endregion

        #region Propiedades

        private string precio;
        public string Precio
        {
            get
            {
                return precio;
            }
            set
            {
                if (precio != value)
                {
                    precio = value;
                    OnPropertyChanged(nameof(Precio));
                }
            }
        }
        private string concepto;
        public string Concepto
        {
            get { return concepto; }
            set
            {
                if (concepto != value)
                {
                    concepto = value;
                    OnPropertyChanged(nameof(Concepto));
                }
            }
        }
        private string tipoSeleccionado;
        public string TipoSeleccionado
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
        private ObservableCollection<string> tipo;
        public ObservableCollection<string> Tipo
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
        #endregion

    }
}
