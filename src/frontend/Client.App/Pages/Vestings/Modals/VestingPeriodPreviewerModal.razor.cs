using Client.App.SmartContractDto;
using Client.App.SmartContractDto.VestingTokenVault;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;

namespace Client.App.Pages.Vestings.Modals
{
    public partial class VestingPeriodPreviewerModal
    {
        [Parameter] public TokenInfo TokenInfo { get; set; } = new();
        [Parameter] public VestingOutput Vesting { get; set; }
        [Parameter] public VestingPeriodOutput Period { get; set; } = new();
        [Parameter] public List<VestingReceiverOutput> Receivers { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    }
}