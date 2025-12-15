using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Mopups.Hosting;
using FFImageLoading.Maui;
using Syncfusion.Maui.Core.Hosting;
using ZXing.Net.Maui.Controls;
using Plugin.Maui.Audio;

namespace AsadorMoron;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Registrar licencia de Syncfusion
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDE0NTgxM0AzMjM0MmUzMDJlMzBLTUFsajQ2VnI3VkZGRE1zaGRWWnlzcmQzdjV5THpGNHBoUVpZejMvNXdNPQ==");

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement()
            .ConfigureMopups()
            .UseFFImageLoading()
            .ConfigureSyncfusionCore()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Syne-Bold.ttf", "Syne");
                fonts.AddFont("Syne-Medium.ttf", "Syne_Medium");
                fonts.AddFont("Syne-Regular.ttf", "Syne_Regular");
                fonts.AddFont("NunitoSans-Regular.ttf", "Nunito");
                fonts.AddFont("NunitoSans-Bold.ttf", "Nunito_bold");
            });

        // Registrar servicios
        builder.Services.AddSingleton(AudioManager.Current);

#if DEBUG
        // builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
