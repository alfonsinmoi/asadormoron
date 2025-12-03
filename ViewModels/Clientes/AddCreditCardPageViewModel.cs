using Mopups.Services;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.Utils;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Models;
using AsadorMoron.Interfaces;
using AsadorMoron.Recursos;

namespace AsadorMoron.ViewModels.Clientes
{
    public class AddCreditCardPageViewModel : ViewModelBase
    {
        public AddCreditCardPageViewModel()
        {
            AddCardCommand = new Command(async () => await ExecuteAddCardCommand());
            ClosePopUpCommand = new Command(async () => await ExecuteClosePopUpCommand());
        }

        public override Task InitializeAsync(object navigationData)
        {
            return base.InitializeAsync(navigationData);
        }

        #region Metodos
        private async Task ExecuteAddCardCommand()
        {
            if (string.IsNullOrEmpty(CardName.DefaultString()))
            {
                await App.customDialog.ShowDialogAsync( AppResources.IntroduzcaNombre, AppResources.Informacion,AppResources.OK);
                return;
            }

            if (string.IsNullOrEmpty(CardNumber.DefaultString()))
            {
                await App.customDialog.ShowDialogAsync(AppResources.IntroduzcaNumeroTarjeta, AppResources.Informacion, AppResources.OK);
                return;
            }

            if (CardNumber.DefaultString().Length < 16 ||
                CardNumber.DefaultString().Length < 15)
            {
                await App.customDialog.ShowDialogAsync(AppResources.NumeroTarjetaIncompleto, AppResources.Informacion, AppResources.OK);
                return;
            }

            if (!CreditCardHelper.IsValidCreditCardNumber(CardNumber))
            {
                await App.customDialog.ShowDialogAsync(AppResources.NumeroTarjetaInvalido, AppResources.Informacion, AppResources.OK);
                return;
            }

            if (string.IsNullOrEmpty(CardExpirationDate.DefaultString()))
            {
                await App.customDialog.ShowDialogAsync(AppResources.IntroduzcaFechaExpiracion, AppResources.Informacion, AppResources.OK);
                return;
            }

            if (string.IsNullOrEmpty(CardCVV.DefaultString()))
            {
                await App.customDialog.ShowDialogAsync(AppResources.IntroduzcaCCV, AppResources.Informacion, AppResources.OK);
                return;
            }
            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaAñadirTarjeta, AppResources.No, AppResources.No);
            InsertaTarjera(result);
        }

        private async void InsertaTarjera(bool hacer)
        {
            /*if (hacer)
            {
                var card = new TarjetaModel()
                {
                    nombre = CardName,
                    numero = CardNumber.RemoveNonNumbers(),
                    mes = int.Parse(CardExpirationDate.Split('/')[0]),
                    anyo = int.Parse(CardExpirationDate.Split('/')[1]),
                    cvv = CardCVV,
                    imagen = CardFlag,
                    numeroEncrypt = CardNumber.RemoveNonNumbers().Substring(0, 4) + " XXXX XXXX " + CardNumber.RemoveNonNumbers().Substring(CardNumber.RemoveNonNumbers().Length - 4, 4),
                    fondo = "mask.png",
                    idUsuario = App.DAUtil.Usuario.idUsuario
                };

                card.imagen = CreditCardHelper.FindFlagCard(card.numero.ToString());
                //App.ResponseWS.nuevaTarjeta(card);
                App.DAUtil.Usuario.tarjetas.Add(card);
                MessagingCenter.Send(this, "addCard", card);
                await MopupService.Instance.PopAsync(true);
            }*/
        }

        private async Task ExecuteClosePopUpCommand()
        {
            await MopupService.Instance.PopAsync(true);
        }

        #endregion

        #region Comandos
        public Command AddCardCommand { get; }
        public Command ClosePopUpCommand { get; }
        #endregion

        #region Properties
        private string _cardName;
        public string CardName
        {
            get
            {
                return _cardName;
            }
            set
            {
                if (_cardName != value)
                {
                    _cardName = value;
                    OnPropertyChanged(nameof(CardName));
                }
            }
        }

        private string _cardNumber;
        public string CardNumber
        {
            get
            {
                return _cardNumber;
            }
            set
            {
                if (_cardNumber != value)
                {
                    _cardNumber = value;
                    OnPropertyChanged(nameof(CardNumber));
                    CardFlag = CreditCardHelper.FindFlagCard(_cardNumber);
                }
            }
        }

        private string _cardExpirationDate;
        public string CardExpirationDate
        {
            get
            {
                return _cardExpirationDate;
            }
            set
            {
                if (_cardExpirationDate != value)
                {
                    _cardExpirationDate = value;
                    OnPropertyChanged(nameof(CardExpirationDate));
                }
            }
        }

        private string _cardCVV;
        public string CardCVV
        {
            get
            {
                return _cardCVV;
            }
            set
            {
                if (_cardCVV != value)
                {
                    _cardCVV = value;
                    OnPropertyChanged(nameof(CardCVV));
                }
            }
        }

        private string _cardFlag;
        public string CardFlag
        {
            get
            {
                return _cardFlag;
            }
            set
            {
                if (_cardFlag != value)
                {
                    _cardFlag = value;
                    OnPropertyChanged(nameof(CardFlag));
                }
            }
        }
        #endregion

    }
}
