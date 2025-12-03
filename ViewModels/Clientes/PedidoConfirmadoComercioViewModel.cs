using System;
using System.Threading.Tasks;
using System.Windows.Input;
// 
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Clientes
{
    public class PedidoConfirmadoComercioViewModel : ViewModelBase
    {
        public PedidoConfirmadoComercioViewModel()
        {
        }
        private string codigo;
        public string Codigo
        {
            get
            {
                return codigo;
            }
            set
            {
                if (codigo != value)
                {
                    codigo = value;
                    OnPropertyChanged(nameof(Codigo));
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
        private string textoPromocion="";
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
        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                string datos = (string)navigationData;
                string[] datos2 = datos.Split(';');
                Codigo = datos2[0];
                FechaEntrega = datos2[1];
                NombreEst = App.EstActual.nombre;
                if (Preferences.Get("promocionAplicada", false) == true)
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

                App.saldoGastado = 0;
                App.amigos = null;
                App.PendientePromocion = false;
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

        public ICommand VolverCommand { get { return new Command(Volver); } }

        void Volver()
        {
            App.DAUtil.NavigationService.InitializeAsync();
        }
    }
}
