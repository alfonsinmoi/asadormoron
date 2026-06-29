using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Models;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Administrador
{
    /// <summary>
    /// Detalle de una llamada con transcripción concatenada y reproducción del audio.
    /// </summary>
    public class DetalleLlamadaViewModelAdmin : ViewModelBase
    {
        public DetalleLlamadaViewModelAdmin()
        {
            Turnos = new ObservableCollection<TranscripcionTurnoModel>();
        }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                if (navigationData is int idLlamada) IdLlamada = idLlamada;

                if (IdLlamada > 0)
                {
                    try { App.userdialog.ShowLoading(Recursos.AppResources.Cargando); } catch { }

                    var detalle = await App.ResponseWS.GetLlamadaAgenteDetalleAsync(IdLlamada);
                    if (detalle != null)
                    {
                        Llamada = detalle;
                        Turnos.Clear();
                        if (detalle.transcripcion != null)
                        {
                            foreach (var t in detalle.transcripcion)
                                Turnos.Add(t);
                        }
                        TieneTranscripcion = Turnos.Count > 0;
                        TieneAudio = !string.IsNullOrEmpty(detalle.audio_url);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error DetalleLlamada: " + ex.Message);
            }
            finally
            {
                try { App.userdialog.HideLoading(); } catch { }
            }

            await base.InitializeAsync(navigationData);
        }

        #region Comandos
        public ICommand cmdReproducir => new Command(async () => await ReproducirAudio());
        public ICommand cmdAbrirAudio => new Command(async () => {
            if (Llamada != null && !string.IsNullOrEmpty(Llamada.audio_url))
            {
                try { await Browser.Default.OpenAsync(Llamada.audio_url, BrowserLaunchMode.SystemPreferred); }
                catch { }
            }
        });
        #endregion

        private async Task ReproducirAudio()
        {
            if (Llamada == null || string.IsNullOrEmpty(Llamada.audio_url)) return;
            // Por simplicidad, abrimos en navegador. Si se quiere reproductor inline,
            // hay que añadir CommunityToolkit.Maui.MediaElement en la View.
            await Browser.Default.OpenAsync(Llamada.audio_url, BrowserLaunchMode.SystemPreferred);
        }

        #region Propiedades
        public int IdLlamada { get; private set; }

        private LlamadaModel llamada;
        public LlamadaModel Llamada { get => llamada; set { llamada = value; OnPropertyChanged(nameof(Llamada)); } }

        public ObservableCollection<TranscripcionTurnoModel> Turnos { get; }

        private bool tieneTranscripcion;
        public bool TieneTranscripcion { get => tieneTranscripcion; set { tieneTranscripcion = value; OnPropertyChanged(nameof(TieneTranscripcion)); } }

        private bool tieneAudio;
        public bool TieneAudio { get => tieneAudio; set { tieneAudio = value; OnPropertyChanged(nameof(TieneAudio)); } }
        #endregion
    }
}
