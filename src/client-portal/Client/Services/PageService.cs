using Client.Infrastructure.Managers.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Client.Services
{
    public class PageService
    {
        private readonly NavigationManager _navigationManager;
        private readonly IAuthManager _authManager;
        private readonly IAppDialogService _appDialogService;

        public PageService(NavigationManager navigationManager, IAuthManager authManager, IAppDialogService appDialogService)
        {
            _navigationManager = navigationManager;
            _authManager = authManager;
            _appDialogService = appDialogService;
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
        }
    }
}
