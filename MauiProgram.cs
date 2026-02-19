using System.Diagnostics;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Mopups.Hosting;
using FFImageLoading.Maui;
using Syncfusion.Maui.Core.Hosting;
using ZXing.Net.Maui.Controls;
using Plugin.Maui.Audio;
using Microsoft.Maui.Handlers;

namespace AsadorMoron;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Manejador global de excepciones no controladas
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            Debug.WriteLine($"[CRASH] UnhandledException: {ex}");
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            Debug.WriteLine($"[CRASH] UnobservedTaskException: {args.Exception}");
            args.SetObserved();
        };

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

        // Quitar borde de los Entry
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS || MACCATALYST
            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
            handler.PlatformView.Layer.BorderWidth = 0;
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
        });

#if DEBUG
        // builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
