using Microsoft.Maui.Controls;

namespace AsadorMoron.Views.Administrador
{
    public partial class LlamadasViewAdmin : ContentPage
    {
        public LlamadasViewAdmin()
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
