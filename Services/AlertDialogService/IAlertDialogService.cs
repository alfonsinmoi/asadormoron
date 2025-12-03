using System.Threading.Tasks;

namespace AsadorMoron.Interfaces
{
    public interface IAlertDialogService
    {
        Task ShowDialogAsync(string message, string title = "Asador Morón", string close = "Cerrar");
        Task<bool> ShowDialogConfirmationAsync(string title, string message, string cancel = "No", string ok = "Sí");
        Task<string> ActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);
        void ShowLoading(string message = null);
        void HideLoading();
    }
}
