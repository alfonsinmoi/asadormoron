using AsadorMoron.ViewModels.Clientes;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Clientes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartaView : Microsoft.Maui.Controls.ContentPage
    {
        public CartaView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Refrescar carrito al volver de DetalleArticulo
            if (BindingContext is CartaViewModel vm)
            {
                vm.RefrescarCarrito();
            }
        }

        private void OnMenuTapped(object sender, EventArgs e)
        {
            if (Application.Current?.MainPage is FlyoutPage flyoutPage)
            {
                flyoutPage.IsPresented = !flyoutPage.IsPresented;
            }
        }
    }
}
