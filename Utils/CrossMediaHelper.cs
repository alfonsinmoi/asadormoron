using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace AsadorMoron.Utils
{
    /// <summary>
    /// Helper para migrar de Plugin.Media a MAUI MediaPicker
    /// TODO: Completar implementación
    /// </summary>
    public static class CrossMedia
    {
        public static CrossMediaInstance Current { get; } = new CrossMediaInstance();
    }

    public class CrossMediaInstance
    {
        public bool IsCameraAvailable => MediaPicker.Default.IsCaptureSupported;
        public bool IsTakePhotoSupported => MediaPicker.Default.IsCaptureSupported;
        public bool IsPickPhotoSupported => true;

        public Task Initialize() => Task.CompletedTask;

        public async Task<MediaFile> TakePhotoAsync(StoreCameraMediaOptions options)
        {
            try
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    return new MediaFile(photo.FullPath, await photo.OpenReadAsync());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TakePhotoAsync error: {ex.Message}");
            }
            return null;
        }

        public async Task<MediaFile> PickPhotoAsync(PickMediaOptions options = null)
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    return new MediaFile(photo.FullPath, await photo.OpenReadAsync());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PickPhotoAsync error: {ex.Message}");
            }
            return null;
        }
    }

    public class MediaFile : IDisposable
    {
        public string Path { get; }
        private readonly Stream _stream;

        public MediaFile(string path, Stream stream)
        {
            Path = path;
            _stream = stream;
        }

        public Stream GetStream() => _stream;
        public string AlbumPath => Path;

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }

    public class StoreCameraMediaOptions
    {
        public PhotoSize PhotoSize { get; set; } = PhotoSize.Medium;
        public int CompressionQuality { get; set; } = 92;
        public string Directory { get; set; }
        public string Name { get; set; }
        public bool SaveToAlbum { get; set; }
        public int MaxWidthHeight { get; set; } = 1024;
        public int CustomPhotoSize { get; set; } = 100;
    }

    public class PickMediaOptions
    {
        public PhotoSize PhotoSize { get; set; } = PhotoSize.Medium;
        public int CompressionQuality { get; set; } = 92;
        public int MaxWidthHeight { get; set; } = 1024;
        public int CustomPhotoSize { get; set; } = 100;
    }

    public enum PhotoSize
    {
        Small,
        Medium,
        Large,
        Full,
        MaxWidthHeight,
        Custom
    }
}
