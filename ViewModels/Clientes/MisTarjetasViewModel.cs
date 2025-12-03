using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Interfaces;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.Utils;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Views.Clientes;
using Mopups.Services;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Clientes
{
    public class MisTarjetasViewModel : ViewModelBase
    {
        #region Propiedades
        private TarjetaModel tarjetaSeleccionada;
        public TarjetaModel TarjetaSeleccionada
        {
            get
            {
                return tarjetaSeleccionada;
            }
            set
            {
                if (tarjetaSeleccionada != value)
                {
                    tarjetaSeleccionada = value;
                    OnPropertyChanged(nameof(TarjetaSeleccionada));
                }
            }
        }
        private ObservableCollection<TarjetaModel> listadoTarjetas;
        public ObservableCollection<TarjetaModel> ListadoTarjetas
        {
            get
            {
                return listadoTarjetas;
            }
            set
            {
                if (listadoTarjetas != value)
                {
                    listadoTarjetas = value;
                    OnPropertyChanged(nameof(ListadoTarjetas));
                }
            }
        }
        #endregion


        public MisTarjetasViewModel()
        {
            NavigateToAddCreditCardPageCommand = new Command(() => ExecuteNavigateToAddCreditCardPageCommand());
        }
        public ICommand cmdEliminarTarjeta { get { return new Command(EliminarTarjeta); } }
        private async void EliminarTarjeta(object id)
        {
            try
            {
                bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaEliminarTarjeta, AppResources.No, AppResources.Si);
                
                if (result)
                {
                    int miId = (int)id;
                    try { App.userdialog.ShowLoading(AppResources.EliminandoTarjeta, MaskType.Black); } catch (Exception) { }
                    await Task.Delay(200);
                    await Task.Run(async () => { await App.ResponseWS.eliminaTarjeta(miId); }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ListadoTarjetas.Remove(ListadoTarjetas.Where(p => p.id == miId).FirstOrDefault());
                        App.userdialog.HideLoading();

                    }));
                }


            }
            catch (Exception ex)
            {
                // 
            }
        }
        public Command AddCardCommand { get; }
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                if (App.userdialog == null)
                {
                    try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
                App.DAUtil.EnTimer = false;
                ListadoTarjetas = new ObservableCollection<TarjetaModel>(ResponseServiceWS.getTarjetas(App.DAUtil.Usuario.idUsuario));
                await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.userdialog.HideLoading();
                }));

            }
            catch (Exception ex)
            {
                // 
            }
            finally
            {
                if (App.userdialog != null)
                {
                    App.userdialog.HideLoading();
                }
                else
                {
                    App.userdialog.HideLoading();
                }
            }
        }

        #region Comandos
        public Command NavigateToAddCreditCardPageCommand { get; }
        #endregion
        #region Métodos
        private async void ExecuteNavigateToAddCreditCardPageCommand()
        {
            // AddCreditCardPage is now a ContentPage, use regular navigation
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddCreditCardPage());
        }
        public void SubscribeAddCard()
        {
            MessagingCenter.Subscribe<AddCreditCardPageViewModel, TarjetaModel>(this, "addCard", (s, param) =>
            {
                ListadoTarjetas.Add(param);
                SetCardBackground();
            });
        }
        void SetCardBackground()
        {
            /*for (int i = 0; i < ListadoTarjetas.Count; i++)
            {
                if (Helper.IsOddNumber(i))
                    ListadoTarjetas[i].fondo = "mask.png";
                else
                    ListadoTarjetas[i].fondo = "mask2.png";
            }*/
        }
        public void UnsubscribedAddCard()
        {
            MessagingCenter.Unsubscribe<AddCreditCardPageViewModel>(this, "addCard");
        }
        #endregion
    }
}
