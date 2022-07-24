using Application.Common.Extensions;
using Client.App.Models;
using Client.App.Pages.Locks.Modals;
using Client.App.Parameters;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.Launchpad;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Pages.Launchpads.Modals
{
    public partial class LaunchpadPreviewerModal
    {
        [Parameter] public long CrowdSaleId { get; set; }
        [Parameter] public TokenInfo NativeTokenInfo { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        public bool IsLoaded { get; set; }
        public bool IsInvestmentsLoaded { get; set; }
        public LaunchpadModel Model { get; set; }
        public List<CrowdSaleInvestment> Investments { get; set; }
        public string ShareLink { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    await FetchDataAsync();
                });
            }
        }

        private async Task FetchDataAsync()
        {
            IsLoaded = false;
            IsInvestmentsLoaded = false;
            StateHasChanged();


            var launchpad = await LaunchpadService.GetCrowdSaleAsync(new CrowdSaleGetCrowdSaleInput() { CrowdSaleId = CrowdSaleId });
            Model = new LaunchpadModel(launchpad);
            ShareLink = $"{NavigationManager.BaseUri}launchpads/{CrowdSaleId}";

            MudDialog.SetTitle(Model.Launchpad.Name);
            IsLoaded = true;
            StateHasChanged();

            var investmentOutput = await LaunchpadService.GetCrowdSaleInvestmentsAsync(new CrowdSaleGetCrowdSaleInvestorsInput() { CrowdSaleId = CrowdSaleId });
            investmentOutput ??= new();
            Investments = investmentOutput.List.ToList();
            IsInvestmentsLoaded = true;
            StateHasChanged();
        }

        private async Task OnCopyShareLinkAsync()
        {
            await ClipboardService.WriteTextAsync(ShareLink);
            AppDialogService.ShowSuccess("Launchpad Link copied to clipboard.");
        }

        private async Task InvokeInvestOnLaunchpadModalAsync()
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
            var parameters = new DialogParameters()
            {
                 { nameof(InvestOnLaunchpadConfirmationModal.LaunchpadModel), Model},
                 { nameof(InvestOnLaunchpadConfirmationModal.NativeTokenInfo), NativeTokenInfo},
                 { nameof(InvestOnLaunchpadConfirmationModal.Model), new InvestOnLaunchpadParameter() { LimitAmount = (double)Model.Launchpad.NativeTokenPurchaseLimitPerBuyerAddress.ToAmount(NativeTokenInfo.Decimals) } },
            };

            var dialog = DialogService.Show<InvestOnLaunchpadConfirmationModal>($"{Model.Launchpad.Name} invest confirmation", parameters, options);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }

        private void InvokeLockPreviewerModal()
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(LockPreviewerModal.LockId), Model.Launchpad.LockId},
            };

            DialogService.Show<LockPreviewerModal>($"Lock #{Model.Launchpad.LockId}", parameters, options);
        }
    }
}