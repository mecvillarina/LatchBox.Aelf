using Application.Common.Models;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.Managers
{
    public interface IManagerToolkit : IManager
    {
        Task SaveDataAsync<T>(string key, T data);
        Task<T> GetDataAsync<T>(string key);
    }
}