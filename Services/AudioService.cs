using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AsadorMoron.Interfaces;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Plugin.Maui.Audio;

[assembly: Dependency(typeof(AsadorMoron.Services.AudioService))]
namespace AsadorMoron.Services
{
    /// <summary>
    /// Servicio de audio multiplataforma usando Plugin.Maui.Audio
    /// </summary>
    public class AudioService : IAppAudio
    {
        private readonly IAudioManager _audioManager;

        public AudioService()
        {
            _audioManager = AudioManager.Current;
        }

        public async void PlayAudioFile(string fileName)
        {
            try
            {
                // Plugin.Maui.Audio requiere que el archivo esté en Resources/Raw
                using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                var player = _audioManager.CreatePlayer(stream);
                if (player != null)
                {
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AudioService] Error playing {fileName}: {ex.Message}");
            }
        }
    }
}
