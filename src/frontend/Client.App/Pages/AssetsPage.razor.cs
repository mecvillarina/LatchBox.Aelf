using Application.Common.Dtos;
using Client.App.Pages.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Pages
{
    public partial class AssetsPage : IPageBase
    {
        public List<TokenBalanceInfoDto> TokenBalances { get; set; } = new();
        public bool IsLoaded { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchData();
            }
        }

        private async Task FetchData()
        {
            IsLoaded = false;
            StateHasChanged();

            var result = await TokenManager.GetTokenBalancesAsync("tdVV", "61bTPDbBwfB2abbB8oquerLyD3tmRyqjUk4YVN9QQvLJkN2mN");

            if (result.Succeeded)
            {
                TokenBalances = result.Data;
            }

            IsLoaded = true;
            StateHasChanged();
        }

    }
}