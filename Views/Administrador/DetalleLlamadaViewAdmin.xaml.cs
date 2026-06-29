using System;
using System.Net.Http;
using System.Threading.Tasks;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Administrador;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Plugin.Maui.Audio;

namespace AsadorMoron.Views.Administrador
{
    public partial class DetalleLlamadaViewAdmin : ContentPage
    {
        private IAudioPlayer? _player;
        private IDispatcherTimer? _timer;
        private bool _isDragging;
        private string? _currentSource;

        public DetalleLlamadaViewAdmin()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Cargar audio cuando el VM termine de inicializarse
            if (BindingContext is DetalleLlamadaViewModelAdmin vm && vm.Llamada != null)
            {
                await PrepararPlayerAsync(vm.Llamada.audio_url);
            }
            else
            {
                // VM aún no listo: esperar al primer cambio de Llamada
                if (BindingContext is DetalleLlamadaViewModelAdmin vm2)
                {
                    vm2.PropertyChanged += async (s, e) =>
                    {
                        if (e.PropertyName == nameof(DetalleLlamadaViewModelAdmin.Llamada) && vm2.Llamada != null)
                        {
                            await PrepararPlayerAsync(vm2.Llamada.audio_url);
                        }
                    };
                }
            }
        }

        protected override void OnDisappearing()
        {
            try
            {
                _timer?.Stop();
                _player?.Stop();
                _player?.Dispose();
                _player = null;
            }
            catch { }
            base.OnDisappearing();
        }

        private async Task PrepararPlayerAsync(string? url)
        {
            if (string.IsNullOrWhiteSpace(url) || url == _currentSource) return;
            _currentSource = url;

            try
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                using var resp = await http.GetAsync(url);
                if (!resp.IsSuccessStatusCode) return;

                var stream = await resp.Content.ReadAsStreamAsync();
                _player = AudioManager.Current.CreatePlayer(stream);

                if (_player != null)
                {
                    TotalTimeLabel.Text = FormatearTiempo(_player.Duration);
                    ProgressSlider.Maximum = Math.Max(0.001, _player.Duration);
                    _player.PlaybackEnded += (s, e) => Dispatcher.Dispatch(ResetPlayUI);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Audio] Error preparando player: {ex.Message}");
            }
        }

        private async void OnPlayPauseTapped(object sender, EventArgs e)
        {
            if (_player == null)
            {
                if (BindingContext is DetalleLlamadaViewModelAdmin vm && vm.Llamada != null)
                    await PrepararPlayerAsync(vm.Llamada.audio_url);
                if (_player == null) return;
            }

            if (_player.IsPlaying)
            {
                _player.Pause();
                PlayShape.IsVisible = true; PauseShape.IsVisible = false;
                _timer?.Stop();
            }
            else
            {
                _player.Play();
                PlayShape.IsVisible = false; PauseShape.IsVisible = true;
                ArrancarTimer();
            }
        }

        private void ArrancarTimer()
        {
            _timer ??= Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick -= OnTimerTick;
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            if (_player == null || _isDragging) return;
            CurrentTimeLabel.Text = FormatearTiempo(_player.CurrentPosition);
            if (_player.Duration > 0)
                ProgressSlider.Value = _player.CurrentPosition;
        }

        private void OnProgressDragCompleted(object? sender, EventArgs e)
        {
            if (_player == null) return;
            _player.Seek(ProgressSlider.Value);
            _isDragging = false;
        }

        private void ResetPlayUI()
        {
            PlayShape.IsVisible = true; PauseShape.IsVisible = false;
            ProgressSlider.Value = 0;
            CurrentTimeLabel.Text = "0:00";
            _timer?.Stop();
        }

        private static string FormatearTiempo(double seconds)
        {
            if (double.IsNaN(seconds) || seconds <= 0) return "0:00";
            var t = TimeSpan.FromSeconds(seconds);
            return $"{(int)t.TotalMinutes}:{t.Seconds:D2}";
        }

        private async void OnBackTapped(object sender, System.EventArgs e)
        {
            try { await Navigation.PopAsync(); } catch { }
        }
    }
}
