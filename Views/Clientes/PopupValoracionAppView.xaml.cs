using AsadorMoron.ViewModels.Clientes;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Clientes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupValoracionAppView : ContentPage
    {
        public PopupValoracionAppView()
        {
            BindingContext = new PopupValoracionAppViewModel();
            InitializeComponent();
        }
    }
}
