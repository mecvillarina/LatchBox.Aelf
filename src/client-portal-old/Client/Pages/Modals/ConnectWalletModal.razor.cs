using Blazored.FluentValidation;
using Client.Infrastructure.Extensions;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Client.Pages.Modals
{
    public partial class ConnectWalletModal
    {
        private ConnectWalletParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        private string WalletFilename = "Please select json wallet file location";
        private IBrowserFile WalletBrowserFile;
        private bool PasswordVisibility;
        private InputType PasswordInput = InputType.Password;
        private string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        public string MainChainNode { get; set; }
        public string SideChainNode { get; set; }
        public string MainChain { get; set; }
        public string SideChain { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    MainChainNode = BlockchainManager.MainChainNode;
                    MainChain = $"Main {BlockchainManager.GetMainChainId().ToChainName()}";

                    StateHasChanged();

                    SideChainNode = BlockchainManager.SideChainNode;
                    SideChain = $"Side {BlockchainManager.GetSideChainId().ToChainName()}";
                    StateHasChanged();
                });
            }
        }

        private async Task SubmitAsync()
        {
            if (Validated)
            {
                if (WalletBrowserFile == null)
                {
                    AppDialogService.ShowError("Please select json wallet file path");
                    return;
                }

                try
                {
                    IsProcessing = true;

                    await AuthManager.ConnectWalletAsync(WalletBrowserFile, Model.Password);
                    var isAuthenticated = await AuthManager.IsAuthenticated();

                    if (isAuthenticated)
                    {
                        NavigationManager.NavigateTo("/", true);
                        return;
                    }
                    else
                    {
                        await AuthManager.ClearKeyStoreAsync();
                        Model.Password = string.Empty;
                        AppDialogService.ShowError("Open wallet error, please check wallet file or password and retry");
                    }
                }
                catch
                {
                    await AuthManager.ClearKeyStoreAsync();
                    Model.Password = string.Empty;
                    AppDialogService.ShowError("Open wallet error, please check wallet file or password and retry");
                }

                IsProcessing = false;
            }
        }

        private void TogglePasswordVisibility()
        {
            if (PasswordVisibility)
            {
                PasswordVisibility = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                PasswordVisibility = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }

        private void OnWalletFileChanged(InputFileChangeEventArgs e)
        {
            foreach (var file in e.GetMultipleFiles(1))
            {
                if (file.Name.EndsWith(".json"))
                {
                    WalletBrowserFile = file;
                    WalletFilename = file.Name;
                }
                else
                {
                    AppDialogService.ShowError("Selected wallet file is not supported. Wallet filename extension must be .json.");
                }
            }
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}