using System;
using System.Threading.Tasks;

namespace AsadorMoron.Interfaces
{
    public enum MaskType
    {
        None,
        Clear,
        Black,
        Gradient
    }

    public interface IUserDialogs
    {
        void ShowLoading(string title = null, MaskType maskType = MaskType.None);
        void HideLoading();
        void ShowError(string message, int durationMillis = 2000);
        void ShowSuccess(string message, int durationMillis = 2000);
        void Toast(string message, int durationMillis = 2000);
        Task<bool> ConfirmAsync(string message, string title = null, string okText = "OK", string cancelText = "Cancel");
        Task AlertAsync(string message, string title = null, string okText = "OK");
        Task<string> ActionSheetAsync(string title, string cancel, string destructive, params string[] buttons);

        /// <summary>
        /// Shows a loading dialog that can be disposed to hide it (for use with 'using' statement)
        /// </summary>
        IDisposable Loading(string title = null, Action onCancel = null, string cancelText = null, bool show = true, MaskType maskType = MaskType.None);
    }
}
