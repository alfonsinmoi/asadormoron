using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Controls
{
    public class AppleSignInButton : ImageButton
    {
        public AppleSignInButton()
        {
            Clicked += AppleSignInButton_Clicked;
            BorderWidth = 1;
            Source = "apple.png";
            switch (ButtonStyle)
            {
                case AppleSignInButtonStyle.White:
                    BackgroundColor = Colors.White;
                    BorderColor = Colors.White;
                    break;

            }

        }

        public AppleSignInButtonStyle ButtonStyle { get; set; } = AppleSignInButtonStyle.Black;

        private void AppleSignInButton_Clicked(object sender, EventArgs e)
        {
            SignIn?.Invoke(sender, e);
            Command?.Execute(CommandParameter);
        }

        public event EventHandler SignIn;

        public void InvokeSignInEvent(object sender, EventArgs e)
            => SignIn?.Invoke(sender, e);

        public void Dispose()
            => Clicked -= AppleSignInButton_Clicked;
    }

    public enum AppleSignInButtonStyle
    {
        Black,
        White,
        WhiteOutline
    }
}
