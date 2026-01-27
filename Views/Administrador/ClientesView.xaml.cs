using Microsoft.Maui.Controls;

namespace AsadorMoron.Views.Administrador
{
    public partial class ClientesView : ContentPage
    {
        public ClientesView()
        {
            InitializeComponent();
        }

        private void OnMenuTapped(object sender, EventArgs e)
        {
            if (Application.Current?.MainPage is FlyoutPage flyoutPage)
            {
                flyoutPage.IsPresented = true;
            }
        }
    }
}
