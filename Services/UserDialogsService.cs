using AsadorMoron.Interfaces;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using MauiToast = CommunityToolkit.Maui.Alerts.Toast;
using Mopups.Services;
using Mopups.Pages;

namespace AsadorMoron.Services
{
    /// <summary>
    /// Implementación de IUserDialogs para MAUI - Compatible con iOS, Android y Windows
    /// </summary>
    public class UserDialogsService : IUserDialogs
    {
        private static Page CurrentPage => Application.Current?.MainPage;
        private CancellationTokenSource _loadingCts;
        private IToast _currentToast;
        private LoadingPopupPage _loadingPopup;
        private bool _isLoadingVisible = false;

        public void ShowLoading(string title = null, MaskType maskType = MaskType.None)
        {
            if (_isLoadingVisible) return;
            _isLoadingVisible = true;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    _loadingPopup = new LoadingPopupPage(title ?? "Cargando...");
                    await MopupService.Instance.PushAsync(_loadingPopup, false);
                }
                catch { _isLoadingVisible = false; }
            });
        }

        public void HideLoading()
        {
            if (!_isLoadingVisible) return;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    if (_loadingPopup != null && MopupService.Instance.PopupStack.Contains(_loadingPopup))
                    {
                        await MopupService.Instance.RemovePageAsync(_loadingPopup, false);
                    }
                    _loadingPopup = null;
                }
                catch { }
                finally
                {
                    _isLoadingVisible = false;
                }
            });
        }

        public void ShowError(string message, int durationMillis = 2000)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var duration = durationMillis > 3000 ? ToastDuration.Long : ToastDuration.Short;
                    await MauiToast.Make($"❌ {message}", duration).Show();
                }
                catch { }
            });
        }

        public void ShowSuccess(string message, int durationMillis = 2000)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var duration = durationMillis > 3000 ? ToastDuration.Long : ToastDuration.Short;
                    await MauiToast.Make($"✓ {message}", duration).Show();
                }
                catch { }
            });
        }

        public void Toast(string message, int durationMillis = 2000)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var duration = durationMillis > 3000 ? ToastDuration.Long : ToastDuration.Short;
                    await MauiToast.Make(message, duration).Show();
                }
                catch { }
            });
        }

        public async Task<bool> ConfirmAsync(string message, string title = null, string okText = "OK", string cancelText = "Cancel")
        {
            try
            {
                if (CurrentPage != null)
                {
                    return await CurrentPage.DisplayAlert(title ?? "", message, okText, cancelText);
                }
            }
            catch { }
            return false;
        }

        public async Task AlertAsync(string message, string title = null, string okText = "OK")
        {
            try
            {
                if (CurrentPage != null)
                {
                    await CurrentPage.DisplayAlert(title ?? "", message, okText);
                }
            }
            catch { }
        }

        public async Task<string> ActionSheetAsync(string title, string cancel, string destructive, params string[] buttons)
        {
            try
            {
                if (CurrentPage != null)
                {
                    return await CurrentPage.DisplayActionSheet(title, cancel, destructive, buttons);
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Returns a disposable that shows loading and hides when disposed
        /// </summary>
        public IDisposable Loading(string title = null, Action onCancel = null, string cancelText = null, bool show = true, MaskType maskType = MaskType.None)
        {
            if (show)
            {
                ShowLoading(title, maskType);
            }
            return new LoadingDisposable(this);
        }

        private class LoadingDisposable : IDisposable
        {
            private readonly UserDialogsService _service;
            private bool _disposed;

            public LoadingDisposable(UserDialogsService service)
            {
                _service = service;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _service.HideLoading();
                }
            }
        }
    }

    /// <summary>
    /// Popup de loading con ActivityIndicator centrado
    /// </summary>
    public class LoadingPopupPage : PopupPage
    {
        public LoadingPopupPage(string message)
        {
            BackgroundColor = Color.FromArgb("#80000000");
            CloseWhenBackgroundIsClicked = false;

            Content = new Frame
            {
                CornerRadius = 15,
                BackgroundColor = Colors.White,
                Padding = new Thickness(30, 20),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HasShadow = true,
                Content = new StackLayout
                {
                    Spacing = 15,
                    Children =
                    {
                        new ActivityIndicator
                        {
                            IsRunning = true,
                            Color = Color.FromArgb("#C41E3A"),
                            HeightRequest = 50,
                            WidthRequest = 50,
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Label
                        {
                            Text = message,
                            TextColor = Colors.Black,
                            FontSize = 16,
                            FontFamily = "Nunito",
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                    }
                }
            };
        }
    }
}
