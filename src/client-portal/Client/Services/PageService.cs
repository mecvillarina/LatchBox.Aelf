using Client.Infrastructure.Managers.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Client.Services
{
    public class PageService
    {
        private readonly NavigationManager _navigationManager;
        private readonly IAuthManager _authManager;
        private readonly IAppDialogService _appDialogService;
        private readonly IBlockchainManager _blockchainManager;
        private readonly NightElfService _nightElfService;

        public PageService(NavigationManager navigationManager, IAuthManager authManager, IAppDialogService appDialogService, NightElfService nightElfService, IBlockchainManager blockchainManager)
        {
            _navigationManager = navigationManager;
            _authManager = authManager;
            _appDialogService = appDialogService;
            _nightElfService = nightElfService;
            _blockchainManager = blockchainManager;
        }

        public async Task EnsureAuthenticatedAsync(Action<bool> callback)
        {
            var isAuthenticated = await _authManager.IsAuthenticated();

            if (!isAuthenticated)
            {
                _navigationManager.NavigateTo("/");
                _appDialogService.ShowError("Connect your wallet first.");
            }

            callback?.Invoke(isAuthenticated);

            //var hasNightElf = await _nightElfService.HasNightElfAsync();

            //if (!hasNightElf)
            //{
            //    _appDialogService.ShowError("Install Night Elf Extension.");
            //    callback?.Invoke(false);
            //}

            //await _nightElfService.InitializeNightElfAsync("LatchBox", _blockchainManager.Node);

            //var isConnected = await _nightElfService.IsConnectedAsync();

            //if (!isConnected)
            //{
            //    _navigationManager.NavigateTo("/");
            //    _appDialogService.ShowError("Connect your wallet first.");
            //}

            //callback?.Invoke(isConnected);
        }
    }
}
