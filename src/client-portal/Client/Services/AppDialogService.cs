using Client.Parameters;
using Client.Shared.Dialogs;
using MudBlazor;

namespace Client.Services
{
    public class AppDialogService : IAppDialogService
    {
        private readonly ISnackbar _snackbar;
        private readonly IDialogService _dialogService;
        public AppDialogService(ISnackbar snackBar, IDialogService dialogService)
        {
            _snackbar = snackBar;
            _dialogService = dialogService;
        }

        public void ShowSuccess(string message)
        {
            _snackbar.Add(message, Severity.Success);
        }

        public void ShowWarning(string message)
        {
            _snackbar.Add(message, Severity.Warning);
        }

        public void ShowError(string message)
        {
            if (message.Contains("[LatchBoxLockTokenVaultContract]"))
            {
                var messages = message.Split("[LatchBoxLockTokenVaultContract]", StringSplitOptions.RemoveEmptyEntries);
                if (messages.Length == 2)
                {
                    message = messages.Last().Trim();
                }
            }
            else if (message.Contains("[LatchBoxVestingTokenVaultContract]"))
            {
                var messages = message.Split("[LatchBoxVestingTokenVaultContract]", StringSplitOptions.RemoveEmptyEntries);
                if (messages.Length == 2)
                {
                    message = messages.Last().Trim();
                }
            }

            _snackbar.Add(message, Severity.Error);
        }

        public void ShowErrors(List<string> messages)
        {
            foreach (var message in messages)
            {
                _snackbar.Add(message, Severity.Error);
            }
        }

    }
}
