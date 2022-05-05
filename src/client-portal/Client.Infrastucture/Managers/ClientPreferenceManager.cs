using Blazored.LocalStorage;
using Client.Infrastructure.Constants;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Settings;
using MudBlazor;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class ClientPreferenceManager : IClientPreferenceManager
    {
        public ClientPreferenceManager()
        {
        }

        public MudTheme GetCurrentTheme()
        {
            return AppTheme.GetDefaultTheme();
        }
    }
}