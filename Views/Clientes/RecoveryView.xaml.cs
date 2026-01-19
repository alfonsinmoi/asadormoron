using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Views.Clientes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecoveryView : Microsoft.Maui.Controls.ContentPage
    {
        private bool _isPasswordVisible = false;
        private bool _isRePasswordVisible = false;

        public RecoveryView()
        {
            InitializeComponent();
        }

        private void OnTogglePasswordClicked(object sender, EventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            PasswordEntry.IsPassword = !_isPasswordVisible;
            TogglePasswordIcon.TextColor = _isPasswordVisible
                ? Color.FromArgb("#C41E3A")
                : Color.FromArgb("#999999");
        }

        private void OnToggleRePasswordClicked(object sender, EventArgs e)
        {
            _isRePasswordVisible = !_isRePasswordVisible;
            RePasswordEntry.IsPassword = !_isRePasswordVisible;
            ToggleRePasswordIcon.TextColor = _isRePasswordVisible
                ? Color.FromArgb("#C41E3A")
                : Color.FromArgb("#999999");
        }
    }
}
