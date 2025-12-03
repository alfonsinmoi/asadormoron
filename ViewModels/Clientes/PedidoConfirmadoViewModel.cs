using System;
using System.Threading.Tasks;
using System.Windows.Input;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Clientes
{

    public class PedidoConfirmadoViewModel : ViewModelBase
    {
        public PedidoConfirmadoViewModel() { }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                string datos = (string)navigationData;
                string[] datos2 = datos.Split(';');
                int tipo = int.Parse(datos2[2]);
                if (tipo == 1)
                {
                    Texto = AppResources.Recibira.Replace("%1", datos2[0]); ;
                }
                else if (tipo == 2)
                {
                    Texto = AppResources.PodraRecoger.Replace("%1", datos2[0]);
                }

                if (Preferences.Get("promocionAplicada", false)==true)
                {
                    TextoPromocion = Preferences.Get("textoPromocion", "");
                    App.DAUtil.Usuario.saldo += App.amigos.saldoAmigo;
                    Preferences.Set("promocionAplicada", false);
                    Preferences.Set("textoPromocion", "");
                }
                if (App.saldoGastado > 0)
                {
                    App.DAUtil.Usuario.saldo -= App.saldoGastado;
                    await App.ResponseWS.actualizaUsuario(App.DAUtil.Usuario);
                }
                if (!Preferences.Get("numerosSorteo", "").Equals(""))
                {
                    TextoPromocion = "Enhorabuena!!!, estos son los números que con los que jugarás para el sorteo de un iPhone 13 Pro Max" + Environment.NewLine + Preferences.Get("numerosSorteo", "");
                }
                Preferences.Set("numerosSorteo", "");
                App.saldoGastado = 0;
                App.amigos = null;
                App.PendientePromocion = false;
                //TimeSpan ts = new TimeSpan(int.Parse(datos2[1].Split(':')[0]), int.Parse(datos2[1].Split(':')[1]), 0);
                FechaEntrega = datos2[1];// ts.ToString(@"hh\:mm");
                //FechaEntrega = DateTime.Now.AddMinutes(App.EstActual.tiempoEntrega);
                NombreEst = datos2[3];
                await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.pedidoEnCurso = null;
                    App.carritoEnCurso = null;
                    App.userdialog.HideLoading();
                }));
            }
            catch (Exception ex)
            {
                // 
            }
        }

        public ICommand VolverCommand { get { return new Command(Volver); } }

        void Volver()
        {
            App.DAUtil.NavigationService.InitializeAsync();
        }
        private string textoPromocion = "";
        public string TextoPromocion
        {
            get
            {
                return textoPromocion;
            }
            set
            {
                if (textoPromocion != value)
                {
                    textoPromocion = value;
                    OnPropertyChanged(nameof(TextoPromocion));
                }
            }
        }
        private string texto;
        public string Texto
        {
            get
            {
                return texto;
            }
            set
            {
                if (texto != value)
                {
                    texto = value;
                    OnPropertyChanged(nameof(Texto));
                }
            }
        }
        private string dateTime;
        public string FechaEntrega
        {
            get
            {
                return dateTime;
            }
            set
            {
                if (dateTime != value)
                {
                    dateTime = value;
                    OnPropertyChanged(nameof(FechaEntrega));
                }
            }
        }

        private string nombreEst;
        public string NombreEst
        {
            get
            {
                return nombreEst;
            }
            set
            {
                if (nombreEst != value)
                {
                    nombreEst = value;
                    OnPropertyChanged(nameof(NombreEst));
                }
            }
        }
    }
}
