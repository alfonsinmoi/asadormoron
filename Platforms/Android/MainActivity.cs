using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.View;

namespace AsadorMoron;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Window.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.ParseColor("#F5F5F5")));

        // Fix para Android 15: desactivar edge-to-edge forzado
        if ((int)Build.VERSION.SdkInt >= 35)
            WindowCompat.SetDecorFitsSystemWindows(Window, true);

        // Evitar que la app se cierre por excepciones no controladas en Android
        AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
        {
            System.Diagnostics.Debug.WriteLine($"[CRASH-ANDROID] {args.Exception}");
            args.Handled = true;
        };
    }
}
