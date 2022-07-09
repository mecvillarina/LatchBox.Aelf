using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application;
using WebApi;
using Infrastructure;

[assembly: FunctionsStartup(typeof(Startup))]

namespace WebApi
{ 
    public class Startup : FunctionsStartup
    {
        public IConfigurationRoot Configuration { get; private set; }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            services.AddHttpContextAccessor();
            services.AddApplication();
            services.AddInfrastructure();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            Configuration = builder.ConfigurationBuilder.Build();
            base.ConfigureAppConfiguration(builder);
        }
    }
}