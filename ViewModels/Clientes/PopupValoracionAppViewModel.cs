using AsadorMoron.Interfaces;
using Mopups.Services;
using System;
using System.Threading.Tasks;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.Services;
using AsadorMoron.Recursos;

namespace AsadorMoron.ViewModels.Clientes
{
    public class PopupValoracionAppViewModel : ViewModelBase
    {
        #region Propiedades
        private double valoracion;
        public double Valoracion
        {
            get
            {
                return valoracion;
            }
            set
            {
                if (valoracion != value)
                {
                    valoracion = value;
                    OnPropertyChanged(nameof(Valoracion));
                }
            }
        }
        private string comentario = "";
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
        #endregion
        public PopupValoracionAppViewModel()
        {
            AceptarCommand = new Command(async () => await ExecuteAceptarCommand());
            ClosePopUpCommand = new Command(async () => await ExecuteClosePopUpCommand());
        }

        public override Task InitializeAsync(object navigationData)
        {

            //ListadoRepartidores = new ObservableCollection<RepartidorModel>( App.DAUtil.GetRepartidores());

            return base.InitializeAsync(navigationData);
        }

        #region Metodos
        private async Task ExecuteAceptarCommand()
        {
            bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaValoracion, AppResources.No, AppResources.Si);
            Valora(result);
        }

        private async void Valora(bool hacer)
        {
            if (hacer)
            {
                ValoracionModel v = new ValoracionModel();
                v.codigoPedido = "";
                v.comentario = Comentario;
                v.fecha = DateTime.Now;
                v.idUsuario = App.DAUtil.Usuario.idUsuario;
                v.idValoracion = 0;
                v.rechazada = 0;
                v.tipoValoracion = 3;
                v.valoracion = Valoracion;
                ResponseServiceWS.insertaValoracion(v);

                await MopupService.Instance.PopAsync(true);
            }
        }

        private async Task ExecuteClosePopUpCommand()
        {
            await MopupService.Instance.PopAsync(true);
        }

        #endregion

        #region Comandos
        public Command AceptarCommand { get; }
        public Command ClosePopUpCommand { get; }
        #endregion
    }
}
