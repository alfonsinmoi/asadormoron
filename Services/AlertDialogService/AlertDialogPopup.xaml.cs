using System;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AsadorMoron.Services.AlertDialogService
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlertDialogPopup
    {
        private Func<bool, Task> callback;

        public AlertDialogPopup(string title, string message, string cancel, string ok, Func<bool, Task> callback)
        {
            this.callback = callback;

            InitializeComponent();
            LbTitle.Text = title;
            LbMessage.Text = message;

            // Si no hay botón cancelar, ocultar y hacer el OK de ancho completo
            if (string.IsNullOrWhiteSpace(cancel))
            {
                BorderCancel.IsVisible = false;
                BorderOk.SetValue(Grid.ColumnProperty, 0);
                BorderOk.SetValue(Grid.ColumnSpanProperty, 2);
            }
            else
            {
                BtCancel.Text = cancel;
            }

            BtOk.Text = ok;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            FrContent.Opacity = 1;
        }

        private async void BtCancel_Clicked(object sender, EventArgs e)
        {
            await callback.Invoke(false);
        }

        private async void BtOk_Clicked(object sender, EventArgs e)
        {
            await callback.Invoke(true);
        }

        private async void BtClose_Clicked(object sender, EventArgs e)
        {
            await callback.Invoke(false);
        }
    }
}