﻿using System.Threading.Tasks;

namespace Client.App.Pages
{
    public partial class AssetsPage
    {

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var chains = ChainManager.FetchSupportedChainsAsync();
            }
        }

        private async Task FetchChainData()
        {

        }

    }
}