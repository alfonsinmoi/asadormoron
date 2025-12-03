using System.Threading.Tasks;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Models;
using System;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.Recursos;

namespace AsadorMoron.ViewModels.Clientes
{
    public class ValorarPedidoViewModel : ViewModelBase
    {

        public ValorarPedidoViewModel()
        {
        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                Pedido = (CabeceraPedido)navigationData;
            }
            catch (Exception)
            {
                App.userdialog.HideLoading();
            }
            finally
            {
                App.userdialog.HideLoading();
            }
            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));

        }


        private CabeceraPedido pedido;
        public CabeceraPedido Pedido
        {
            get
            {
                return pedido;
            }
            set
            {
                if (pedido != value)
                {
                    pedido = value;
                    OnPropertyChanged(nameof(Pedido));
                }
            }
        }
        private double valoracionPedido;
        public double ValoracionPedido
        {
            get
            {
                return valoracionPedido;
            }
            set
            {
                if (valoracionPedido != value)
                {
                    valoracionPedido = value;
                    OnPropertyChanged(nameof(ValoracionPedido));
                }
            }
        }
        private double valoracionRepartidor;
        public double ValoracionRepartidor
        {
            get
            {
                return valoracionRepartidor;
            }
            set
            {
                if (valoracionRepartidor != value)
                {
                    valoracionRepartidor = value;
                    OnPropertyChanged(nameof(ValoracionRepartidor));
                }
            }
        }
        private double valoracionPuntualidad;
        public double ValoracionPuntualidad
        {
            get
            {
                return valoracionPuntualidad;
            }
            set
            {
                if (valoracionPuntualidad != value)
                {
                    valoracionPuntualidad = value;
                    OnPropertyChanged(nameof(ValoracionPuntualidad));
                }
            }
        }
        private double valoracionEstablecimiento;
        public double ValoracionEstablecimiento
        {
            get
            {
                return valoracionEstablecimiento;
            }
            set
            {
                if (valoracionEstablecimiento != value)
                {
                    valoracionEstablecimiento = value;
                    OnPropertyChanged(nameof(ValoracionEstablecimiento));
                }
            }
        }
        private string comentario;
        public string Comentario
        {
            get
            {
                return comentario;
            }
            set
            {
                if (comentario != value)
                {
                    comentario = value;
                    OnPropertyChanged(nameof(Comentario));
                }
            }
        }
        public ICommand cmdEnviarValoracion { get { return new Command(EnviarValoracion); } }
        private async void EnviarValoracion()
        {
            ValoracionPedido v = new ValoracionPedido();
            v.comentario = Comentario;
            v.idPedido = Pedido.idPedido;
            v.idUsuario = App.DAUtil.Usuario.idUsuario;
            v.valoracionEstablecimiento = ValoracionEstablecimiento;
            v.valoracionPuntualidad = ValoracionPuntualidad;
            v.valoracionRepartidor = ValoracionRepartidor;
            v.valoracionServicio = ValoracionPedido;
            v.fecha = DateTime.Now;
            App.ResponseWS.ValoraPedido(v);
            await App.customDialog.ShowDialogAsync(AppResources.GraciasValoracion, AppResources.App, AppResources.Cerrar);
            await App.DAUtil.NavigationService.InitializeAsync();
        }
    }
}
