using AsadorMoron.Interfaces;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using MauiToast = CommunityToolkit.Maui.Alerts.Toast;

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

        public void ShowLoading(string title = null, MaskType maskType = MaskType.None)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    _loadingCts = new CancellationTokenSource();
                    _currentToast = MauiToast.Make(title ?? "Cargando...", ToastDuration.Long);
                    await _currentToast.Show(_loadingCts.Token);
                }
                catch { }
            });
        }

        public void HideLoading()
        {
            try
            {
                _loadingCts?.Cancel();
                _loadingCts?.Dispose();
                _loadingCts = null;
            }
            catch { }
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
}
