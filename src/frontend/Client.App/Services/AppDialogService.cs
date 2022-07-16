using MudBlazor;
using System.Collections.Generic;

namespace Client.App.Services
{
    public class AppDialogService
    {
        private readonly ISnackbar _snackbar;

        public AppDialogService(ISnackbar snackBar)
        {
            _snackbar = snackBar;
        }

        public void Show(string message)
        {
            _snackbar.Add(message, Severity.Normal, config => config.HideIcon = true);
        }

        public void ShowTxSend(string explorer, string txId, string defaultMessage = "Transaction sent")
        {
            string url = $"{explorer}/tx/{txId}";
            string message = $"{defaultMessage}. View it <a href=\"{url}\" target=\"_blank\"><u>here</u></a>.";
            _snackbar.Add(message, Severity.Normal, config => config.HideIcon = true);
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
            message ??= string.Empty;
            message = message.Replace("AElf.Sdk.CSharp.AssertionException:", "");
            message = message.Trim();
            _snackbar.Add(message, Severity.Error);
        }

        public void ShowErrors(List<string> messages)
        {
            foreach(var message in messages)
            {
                _snackbar.Add(message, Severity.Error);
            }
        }
    }
}
