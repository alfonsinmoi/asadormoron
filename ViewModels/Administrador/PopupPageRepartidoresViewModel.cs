using AsadorMoron.Interfaces;
// 
using Mopups.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Views.Administrador;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Linq;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;

namespace AsadorMoron.ViewModels.Administrador
{
    public class PopupPageRepartidoresViewModel : ViewModelBase
    {
        #region Propiedades
        private ObservableCollection<RepartidorModel> listadoRepartidores;

        public ObservableCollection<RepartidorModel> ListadoRepartidores
        {
            get { return listadoRepartidores; }
            set
            {
                listadoRepartidores = value;
                OnPropertyChanged(nameof(ListadoRepartidores));
            }
        }

        private RepartidorModel repartidorSeleccionado;

        public RepartidorModel RepartidorSeleccionado
        {
            get { return repartidorSeleccionado; }
            set
            {
                if (value != null)
                {
                    repartidorSeleccionado = value;
                    CambiarRepartidor();
                }
                //OnPropertyChanged(nameof(RepartidorSeleccionado));
            }
        }

        private async void CambiarRepartidor()
        {
            try
            {
                if (PopupPageRepartidores.l_Pedido != null)
                {
                    bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App,AppResources.PreguntaAsignarPedido.Replace("%1", PopupPageRepartidores.l_Pedido.codigoPedido).Replace("%2", repartidorSeleccionado.nombre), AppResources.No, AppResources.Si);
                    if (result)
                    {
                        if (PopupPageRepartidores.l_Pedido.idRepartidor != repartidorSeleccionado.id && PopupPageRepartidores.l_Pedido.idRepartidor != 0)
                        {
                            try
                            {
                                string tokenUser = App.ResponseWS.getTokenRepartidor(PopupPageRepartidores.l_Pedido.idRepartidor);
                                await App.ResponseWS.enviaNotificacion(PopupPageRepartidores.l_Pedido.nombreEstablecimiento, "El pedido " + PopupPageRepartidores.l_Pedido.codigoPedido + " de " + PopupPageRepartidores.l_Pedido.nombreEstablecimiento + " ha sido asignado a otro repartidor.", tokenUser);

                                List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(PopupPageRepartidores.l_Pedido.idEstablecimiento);
                                foreach (TokensModel to in tokens3)
                                    App.ResponseWS.enviaNotificacion(PopupPageRepartidores.l_Pedido.nombreEstablecimiento, "El pedido " + PopupPageRepartidores.l_Pedido.codigoPedido + " de " + PopupPageRepartidores.l_Pedido.nombreEstablecimiento + " ha sido cogido por un repartidor", to.token);

                            }
                            catch (Exception)
                            {

                            }
                        }
                        PopupPageRepartidores.l_Pedido.idRepartidor = repartidorSeleccionado.id;
                        PopupPageRepartidores.l_Pedido.FotoRepartidor = repartidorSeleccionado.foto;
                        await App.ResponseWS.pedidoConRepartidor(PopupPageRepartidores.l_Pedido.codigoPedido, repartidorSeleccionado.id).ContinueWith(res => MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            try
                            {
                                string tokenUser = App.ResponseWS.getTokenRepartidor(PopupPageRepartidores.l_Pedido.idRepartidor);
                                await App.ResponseWS.enviaNotificacion(PopupPageRepartidores.l_Pedido.nombreEstablecimiento, "El pedido " + PopupPageRepartidores.l_Pedido.codigoPedido + " de " + PopupPageRepartidores.l_Pedido.nombreEstablecimiento + " te ha sido asignado.", tokenUser);

                                List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(PopupPageRepartidores.l_Pedido.idEstablecimiento);
                                foreach (TokensModel to in tokens3)
                                    App.ResponseWS.enviaNotificacion(PopupPageRepartidores.l_Pedido.nombreEstablecimiento, "El pedido " + PopupPageRepartidores.l_Pedido.codigoPedido + " de " + PopupPageRepartidores.l_Pedido.nombreEstablecimiento + " ha sido cogido por un repartidor", to.token);
                            }
                            catch (Exception)
                            {

                            }
                            await MopupService.Instance.PopAsync();
                            //await NavigationService.InitializeAsync();
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }

        #endregion
        public PopupPageRepartidoresViewModel()
        {
            ListadoRepartidores = new ObservableCollection<RepartidorModel>(App.DAUtil.GetRepartidores().Where(t => t.activo == 1).ToList());
        }

        public override Task InitializeAsync(object navigationData)
        {

            //ListadoRepartidores = new ObservableCollection<RepartidorModel>( App.DAUtil.GetRepartidores());
            return base.InitializeAsync(navigationData);
        }
    }
}
