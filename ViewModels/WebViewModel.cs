using System;
using System.Threading.Tasks;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels
{
    public class WebViewModel : ViewModelBase
    {
        public WebViewModel()
        {
        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                await base.InitializeAsync(navigationData);
            }
            catch
            {

            }
            finally
            {
                App.userdialog.HideLoading();
            }
        }
        private string url;
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                if (url != value)
                {
                    url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }
    }
}
