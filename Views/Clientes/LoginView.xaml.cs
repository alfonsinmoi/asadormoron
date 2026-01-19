using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Clientes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : Microsoft.Maui.Controls.ContentPage
    {
        private bool _isPasswordVisible = false;

        public LoginView()
        {
            InitializeComponent();
        }

        private void OnTogglePasswordClicked(object sender, EventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            PasswordEntry.IsPassword = !_isPasswordVisible;

            // Cambiar color del icono segun estado
            TogglePasswordIcon.TextColor = _isPasswordVisible
                ? Color.FromArgb("#C41E3A")  // Rojo cuando visible
                : Color.FromArgb("#999999"); // Gris cuando oculto
        }
    }
}
