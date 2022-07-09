using MudBlazor;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Shared
{
    public partial class MainLayout : IAsyncDisposable
    {
        private bool DrawerOpen { get; set; } = true;
        private MudTheme CurrentTheme { get; set; }

        protected override void OnInitialized()
        {
            CurrentTheme = _clientPreferenceManager.GetCurrentTheme();
            //FetchDataExecutor.StartExecuting();
        }

        protected async override Task OnInitializedAsync()
        {
            await AppBreakpointService.InitAsync();
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                StateHasChanged();
            }
        }
        public async ValueTask DisposeAsync()
        {
            await AppBreakpointService.DisposeAsync();
        }
    }
}