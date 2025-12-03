using System;
using System.Collections.Generic;
using System.Diagnostics;
using AsadorMoron.Models;
using AsadorMoron.Models.PayComet;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels;
using AsadorMoron.ViewModels.Clientes;
using Microsoft.Maui.Devices;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Views
{
    public partial class MyWebView : Microsoft.Maui.Controls.ContentPage
    {
        public MyWebView()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            webView.Source = App.urlChallengue;
            webView.Navigated += WebView_Navigated;
        }
        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            /*string resultado = await webView.EvaluateJavaScriptAsync("document.body.innerHTML");
            if (resultado.StartsWith("Proceso correcto"))
            {
                ResponseAddCardModel info = JsonConvert.DeserializeObject<ResponseAddCardModel>(resultado.Replace("Proceso correcto", "").Replace("\\", ""));
                TarjetaModel tarjera = await App.ResponseWS.infoTarjetaPaycomet(info.DS_IDUSER, info.DS_TOKEN_USER);
                if (tarjera != null)
                {
                    await App.ResponseWS.nuevaTarjeta(tarjera);
                    MessagingCenter.Send(BindingContext, "addCard", tarjera);
                }

                await MopupService.Instance.PopAsync(true);

            }
            else if (resultado.StartsWith("Proceso Incorrecto") || resultado.StartsWith("Error, no se ha obtenido token"))
            {
                webView.Reload();
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
            }*/
            if (e.Url.Contains("url-") && e.Url.Contains("-ok"))
            {
                
                        int idCodigoPedido = ResponseServiceWS.NuevoPedido(0);
                        if (idCodigoPedido > 0)
                        {
                            List<TokensModel> tokens = App.ResponseWS.getTokenMultiAdministrador(App.EstActual.idPueblo);
                            foreach (TokensModel to in tokens)
                                App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + App.pedidoEnCurso.codigoPedido, to.token);

                            if (App.pedidoEnCurso.tipoVenta.StartsWith("Envío"))
                            {
                                List<TokensModel> tokens2 = App.ResponseWS.getTokenRepartidores(App.EstActual.idEstablecimiento);
                                foreach (TokensModel to in tokens2)
                                    App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido para " + App.EstActual.nombre + ": " + App.pedidoEnCurso.codigoPedido, to.token);
                            }

                            List<TokensModel> tokens3 = App.ResponseWS.getTokenEstablecimiento(App.EstActual.idEstablecimiento);
                            foreach (TokensModel to in tokens3)
                                App.ResponseWS.enviaNotificacion(App.EstActual.nombre, "Nuevo Pedido: " + App.pedidoEnCurso.codigoPedido, to.token);

                            List<PedidoModel> pedidosModel = new List<PedidoModel>();
                            DateTime fechaPedido = DateTime.Now;
                            foreach (var item in App.carritoEnCurso)
                            {
                                PedidoModel p = new PedidoModel();
                                p.idArticulo = item.idArticulo;
                                p.idPedido = App.pedidoEnCurso.idPedido;
                                p.idEstablecimiento = item.idEstablecimiento;
                                p.imagen = item.imagen;
                                p.nombreCantidad = item.nombreCantidad;
                                p.observaciones = item.observaciones.Trim();
                                p.precio = item.precio;
                                p.cantidad = item.cantidad;
                                p.comida = item.comida;
                                p.precioTotal = item.precioTotal;
                                p.fechaPedido = fechaPedido;
                                p.nombreEstablecimiento = App.EstActual.nombre;
                                p.horaEntrega = App.pedidoEnCurso.horaEntrega;
                                pedidosModel.Add(p);
                            }
                            App.DAUtil.GuardarPedido(pedidosModel);
                            App.DAUtil.VaciaCarrito();
                            await App.DAUtil.NavigationService.NavigateToAsyncWithoutMenu<PedidoConfirmadoComercioViewModel>(App.pedidoEnCurso.codigoPedido + ";" + Preferences.Get("dia", ""));
                        }
                        else
                        {
                            OperationInfoModel info = await App.ResponseWS.infoTransactionPaycomet(App.pedidoEnCurso.codigoPedido);
                            await App.ResponseWS.refundPaycomet(App.pedidoEnCurso.codigoPedido, App.TarjetaSeleccionada.idUser, App.TarjetaSeleccionada.tokenUser, Preferences.Get("totalPedido", ""), info.payment.authCode);
                            App.userdialog.HideLoading();
                            await App.DAUtil.NavigationService.NavigateToAsyncWithoutMenu<PagoErroneoViewModel>(App.pedidoEnCurso.codigoPedido + ";" + Preferences.Get("dia", ""));
                        }
            }
            else if (e.Url.Contains("url-") && e.Url.Contains("-ko"))
            {
                /*try
                {
                    if (!Preferences.Get("EsPedidoLocal", false))
                        await ResponseServiceWS.eliminaPedido(App.pedidoEnCurso.idPedido);
                }catch (Exception)
                {

                }*/
                await App.DAUtil.NavigationService.NavigateToAsyncWithoutMenu<PagoErroneoViewModel>(App.pedidoEnCurso.codigoPedido + ";" + Preferences.Get("dia", ""));
            }
            else if (!e.Url.Contains("https://api.paycomet.com/gateway/sca_challenge.php?"))
            {
                //await ResponseServiceWS.eliminaPedido(App.pedidoEnCurso.idPedido);
                //await App.DAUtil.NavigationService.NavigateToAsyncMenu<PagoErroneoViewModel>(App.pedidoEnCurso.codigoPedido + ";" + Preferences.Get("dia", ""));
            }
        }
    }
}
