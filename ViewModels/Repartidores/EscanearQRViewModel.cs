using AsadorMoron.ViewModels.Base;
using System;
using System.Threading.Tasks;

namespace AsadorMoron.ViewModels.Repartidores
{
    public class EscanearQRViewModel : ViewModelBase
    {
        // Callback que el caller (HomeViewModelRepartidor) registra antes de navegar.
        // La página scanner lo invoca con el código leído tras decodificar un QR válido.
        public static Action<string> OnCodigoLeido { get; set; }

        public override Task InitializeAsync(object navigationData)
        {
            return Task.CompletedTask;
        }
    }
}
