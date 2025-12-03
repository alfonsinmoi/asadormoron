using AsadorMoron.ViewModels.Clientes;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Clientes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TarjetasView : Microsoft.Maui.Controls.ContentPage
    {
        MisTarjetasViewModel viewModel;
        public TarjetasView()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            viewModel = BindingContext as MisTarjetasViewModel;
            viewModel.SubscribeAddCard();
        }

        protected override void OnDisappearing()
        {
            viewModel.UnsubscribedAddCard();
        }
    }
}
