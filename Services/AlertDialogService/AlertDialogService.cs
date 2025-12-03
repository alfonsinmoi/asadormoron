using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using AsadorMoron.Interfaces;
using Mopups.Services;
using Mopups.Pages;

[assembly: Dependency(typeof(AsadorMoron.Services.AlertDialogService.AlertDialogService))]
namespace AsadorMoron.Services.AlertDialogService
{
    public class AlertDialogService : IAlertDialogService
    {
        private TaskCompletionSource<bool> _tcs;
        private Task<bool> _dialogTask;

        private readonly SemaphoreSlim _gate = new(1, 1);
        private LoadingPopup _loadingPopup;

        // ------------------ Diálogo simple ------------------
        public async Task ShowDialogAsync(string message, string title = "Asador Morón", string close = "Cerrar")
        {
            _tcs = new TaskCompletionSource<bool>();
            _dialogTask = _tcs.Task;

            AlertDialogPopup alertDialog = null;
            alertDialog = new AlertDialogPopup(title, message, null, close, result => Callback(result, alertDialog));

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await MopupService.Instance.PushAsync(alertDialog);
            });

            await _dialogTask;
        }

        // --------------- Diálogo confirmación ----------------
        public async Task<bool> ShowDialogConfirmationAsync(string title, string message, string cancel = "No", string ok = "Sí")
        {
            _tcs = new TaskCompletionSource<bool>();
            _dialogTask = _tcs.Task;

            AlertDialogPopup confirm = null;
            confirm = new AlertDialogPopup(title, message, cancel, ok, result => Callback(result, confirm));

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await MopupService.Instance.PushAsync(confirm);
            });

            return await _dialogTask;
        }

        // ------------------ ActionSheet ------------------
        public async Task<string> ActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            var page = Application.Current?.MainPage ?? throw new InvalidOperationException("No hay MainPage.");
            var choice = await MainThread.InvokeOnMainThreadAsync(() =>
                page.DisplayActionSheet(title, cancel, destruction, buttons));
            return choice ?? cancel;
        }

        // ------------------ Loading (Show/Hide) ------------------
        public void ShowLoading(string message = null) => _ = ShowLoadingInternalAsync(message);
        public void HideLoading() => _ = HideLoadingInternalAsync();

        private async Task ShowLoadingInternalAsync(string message)
        {
            await _gate.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_loadingPopup is null)
                {
                    _loadingPopup = new LoadingPopup(message);

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await MopupService.Instance.PushAsync(_loadingPopup);
                    });
                }
                else
                {
                    _loadingPopup.UpdateMessage(message);
                }
            }
            finally
            {
                _gate.Release();
            }
        }

        private async Task HideLoadingInternalAsync()
        {
            await _gate.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_loadingPopup is null) return;

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        await MopupService.Instance.RemovePageAsync(_loadingPopup);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"HideLoading RemovePopup error: {ex.Message}");
                        try { await MopupService.Instance.PopAllAsync(); } catch { /* ignore */ }
                    }
                    finally
                    {
                        _loadingPopup = null;
                    }
                });
            }
            finally
            {
                _gate.Release();
            }
        }

        // ------------------ Cierre de diálogos ------------------
        private async Task Callback(bool result, PopupPage popupToClose)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    await MopupService.Instance.RemovePageAsync(popupToClose);
                }
                catch { /* ignore */ }
            });

            if (!_tcs.Task.IsCompleted && !_tcs.Task.IsFaulted && !_tcs.Task.IsCanceled)
                _tcs.SetResult(result);
        }

        // ------------------ Popup de loading ------------------
        private sealed class LoadingPopup : PopupPage
        {
            private readonly Label _label;

            public LoadingPopup(string message)
            {
                CloseWhenBackgroundIsClicked = false;

                BackgroundColor = Color.FromArgb("#6245bc").WithAlpha(0.35f);

                var activity = new ActivityIndicator
                {
                    IsRunning = true,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    Color = Color.FromArgb("#6245bc"),
                    HorizontalOptions = LayoutOptions.Center
                };

                _label = new Label
                {
                    TextColor = Colors.White,
                    Text = string.IsNullOrWhiteSpace(message) ? "Cargando..." : message,
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontSize = 16
                };

                var card = new Frame
                {
                    CornerRadius = 14,
                    HasShadow = true,
                    Padding = 24,
                    BackgroundColor = Color.FromArgb("#44b176"),
                    Content = new VerticalStackLayout
                    {
                        Spacing = 12,
                        Children = { activity, _label }
                    }
                };

                Content = new Grid
                {
                    Padding = 24,
                    Children =
                    {
                        new Grid
                        {
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            Children = { card }
                        }
                    }
                };
            }

            public void UpdateMessage(string message)
            {
                _label.Text = string.IsNullOrWhiteSpace(message) ? "Cargando..." : message;
            }
        }
    }
}
