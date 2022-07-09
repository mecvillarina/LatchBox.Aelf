using Blazored.LocalStorage;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.Managers
{
    public class ManagerToolkit : IManagerToolkit
    {
        private readonly ILocalStorageService _localStorage;

        public ManagerToolkit(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SaveDataAsync<T>(string key, T data) => await _localStorage.SetItemAsync(key, data);
        public async Task<T> GetDataAsync<T>(string key)
        {
            var data = await _localStorage.GetItemAsync<T>(key);
            return data;
        }
    }
}
