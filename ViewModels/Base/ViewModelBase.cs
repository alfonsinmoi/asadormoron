using AsadorMoron.Interfaces;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Base
{
    public class ViewModelBase : BindableObject
    {
        //internal readonly INavigationService NavigationService;

        private double opacity;

        public double Opacity
        {
            get { return opacity; }
            set
            {
                if (opacity != value)
                {
                    opacity = value;
                    OnPropertyChanged(nameof(Opacity));
                }

            }
        }


        private bool isActivityRunning;

        public bool IsActivityRunning
        {
            get { return isActivityRunning; }
            set
            {
                if (isActivityRunning != value)
                {

                    isActivityRunning = value;
                    if (isActivityRunning)
                    {
                        Opacity = 0.5;
                    }
                    else
                    {
                        Opacity = 1;
                    }
                    OnPropertyChanged(nameof(IsActivityRunning));
                }
            }
        }

        public ViewModelBase()
        {
           // NavigationService = ViewModelLocator.Instance.Resolve<INavigationService>();
        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(true);
        }
        public virtual Task FinalizePage(object navigationData)
        {
            return Task.FromResult(false);
        }
        public virtual void FinalizePage()
        {

        }
    }
}
