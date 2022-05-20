﻿using Client.Infrastructure.Managers.Interfaces;
using Client.Pages.Modals;
using Client.Shared.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Shared.Components
{
    public partial class AppBarContent
    {
        [Inject] public IFaucetManager FaucetManager { get; set; }

        public bool IsLoaded { get; set; }
        public bool IsPlatformTokenLoaded { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Network { get; set; }
        public string Node { get; set; }
        public string WalletAddress { get; set; }
        public bool IsClaimingElf { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    IsAuthenticated = await AuthManager.IsAuthenticated();
                    Network = BlockchainManager.Network;
                    Node = BlockchainManager.Node;

                    if (IsAuthenticated)
                    {
                        var wallet = await WalletManager.GetWalletInformationAsync();
                        WalletAddress = wallet.Address;
                    }

                    IsLoaded = true;
                    StateHasChanged();

                    //Network = BlockchainManager.Network;
                    //Node = BlockchainManager.Node;

                    //IsAuthenticated = await NightElfService.IsConnectedAsync();

                    //if (IsAuthenticated)
                    //{
                    //    var walletAddress = await NightElfService.GetAddressAsync();
                    //    WalletAddress = walletAddress;
                    //}

                    //IsLoaded = true;
                    //StateHasChanged();
                });
            }
        }

        private async Task InvokeConnectWalletModalAsync()
        {
            var options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall };
            DialogService.Show<ConnectWalletModal>("Connect Wallet (JSON)", options);

            //var hasNightElf = await NightElfService.HasNightElfAsync();

            //if (!hasNightElf)
            //{
            //    AppDialogService.ShowError("Install Night Elf Extension.");
            //    return;
            //}

            //await NightElfService.InitializeNightElfAsync("LatchBox", BlockchainManager.Node);

            //var isConnected = await NightElfService.IsConnectedAsync();

            //if (!isConnected)
            //{
            //    var result = await NightElfService.LoginAsync();
            //    if(result != null)
            //    {
            //        WalletAddress = await NightElfService.GetAddressAsync();
            //        IsAuthenticated = true;
            //    }
            //}
        }

        private async Task InvokeClaimELFAsync()
        {
            IsClaimingElf = true;

            var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

            if (authenticated)
            {
                try
                {
                    var result = await FaucetManager.TakeAsync("ELF", 100_00000000);

                    if (!string.IsNullOrEmpty(result.Error))
                    {
                        AppDialogService.ShowError(result.Error);
                    }
                    else
                    {
                        AppDialogService.ShowSuccess("Claim ELF Success.");
                    }
                }
                catch (Exception ex)
                {
                    AppDialogService.ShowError(ex.Message);
                }
            }

            IsClaimingElf = false;
        }

        private async Task InvokeDisconnectWalletDialogAsync()
        {
            var parameters = new DialogParameters
            {
                {nameof(DisconnectWalletDialog.ContentText), "Are you sure you want to disconnect your wallet?"},
                {nameof(DisconnectWalletDialog.ButtonText), "Disconnect"},
                {nameof(DisconnectWalletDialog.Color), Color.Error},
            };

            DialogService.Show<DisconnectWalletDialog>("Logout", parameters);
            //await NightElfService.LogoutAsync();
            //IsAuthenticated = false;
        }
    }
}