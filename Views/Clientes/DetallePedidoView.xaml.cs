using AsadorMoron.ViewModels.Clientes;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Clientes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetallePedidoView : Microsoft.Maui.Controls.ContentPage
    {
        DetallePedidoViewModel viewModel;
        public DetallePedidoView()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            viewModel = BindingContext as DetallePedidoViewModel;
            viewModel.SubscribeAddCard();
        }

        protected override void OnDisappearing()
        {
            viewModel.UnsubscribedAddCard();
        }
    }
}