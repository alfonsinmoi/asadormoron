using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Administrador
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnviarNotificacionView : ContentPage
    {
        public EnviarNotificacionView()
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
