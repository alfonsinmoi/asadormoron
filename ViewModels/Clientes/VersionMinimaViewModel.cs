using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace AsadorMoron.ViewModels.Clientes
{
    public class VersionMinimaViewModel : ViewModelBase
    {
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
        public ICommand cmdActualizar { get { return new Command(Actualizar); } }
        private async void Actualizar()
        {
            try
            {
                switch (DeviceInfo.Platform.ToString())
                {
                    case "iOS":
                        await Browser.Default.OpenAsync(new Uri("https://apps.apple.com/es/app/pollo-andaluz/id1210357759"), BrowserLaunchMode.SystemPreferred);
                        break;
                    case "Android":
                        await Browser.Default.OpenAsync(new Uri("https://play.google.com/store/apps/details?id=net.polloandaluz.PolloAndaluz&gl=ES"), BrowserLaunchMode.SystemPreferred);
                        break;
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        public VersionMinimaViewModel()
        {

        }

        public override Task InitializeAsync(object navigationData)
        {

            if (App.userdialog == null)
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { };
            }
            try
            {
                Texto = App.MensajesGlobal.Where(p => p.clave.Equals("version_minima")).FirstOrDefault<MensajesModel>().valor;
                return base.InitializeAsync(navigationData);

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
            return base.InitializeAsync(navigationData);
        }
    }
}
