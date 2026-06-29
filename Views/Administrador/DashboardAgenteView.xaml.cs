using Microsoft.Maui.Controls;

namespace AsadorMoron.Views.Administrador
{
    public partial class DashboardAgenteView : ContentPage
    {
        public DashboardAgenteView()
        {
            InitializeComponent();
        }

        private void OnMenuTapped(object sender, System.EventArgs e)
        {
            if (Application.Current?.MainPage is FlyoutPage fp)
                fp.IsPresented = true;
        }
    }
}
