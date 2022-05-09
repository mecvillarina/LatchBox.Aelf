using Client.Infrastructure.Models;

namespace Client.Services
{
    public interface IAppDialogService
    {
        void ShowSuccess(string message);
        void ShowWarning(string message);
        void ShowError(string message);
        void ShowErrors(List<string> messages);
        Task<bool> ShowConfirmWalletTransactionAsync();
    }
}