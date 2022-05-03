using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IAuthManager : IManager
    {
        Task<bool> IsAuthenticated();
        Task ConnectWalletAsync(IBrowserFile file, string password);
        Task DisconnectWalletAsync();
    }
}