using System;
using System.Threading.Tasks;
// 
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Establecimientos
{
    public class MisFacturasViewModel:ViewModelBase
    {
        public MisFacturasViewModel()
        {
        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                if (!haEntrado)
                {
                    haEntrado = true;
                    App.DAUtil.EnTimer = false;
                }
            }
            catch (Exception ex)
            {

                // 
            }
            finally
            {
                App.userdialog.HideLoading();
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }
        #region Propiedades
        private bool haEntrado = false;
        #endregion
    }
}
