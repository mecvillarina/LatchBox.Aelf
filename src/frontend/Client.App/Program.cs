using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Client.App.Extensions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Client.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder
                    .CreateDefault(args)
                    .AddRootComponents()
                    .AddClientServices();

            var host = builder.Build();
           
            await host.RunAsync();
        }
    }
}
