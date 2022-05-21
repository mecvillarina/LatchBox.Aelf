using Blazored.LocalStorage;
using Client.Extensions;
using Client.Infrastructure.Managers;
using Client.Infrastructure.Models;
using Client.Infrastructure.Services;
using Client.Infrastructure.Services.Interfaces;
using Client.Services;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
      .AddHubOptions(options =>
      {
          options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
          options.EnableDetailedErrors = true;
          options.HandshakeTimeout = TimeSpan.FromSeconds(15);
          options.KeepAliveInterval = TimeSpan.FromSeconds(15);
          options.MaximumParallelInvocationsPerClient = 1;
          options.MaximumReceiveMessageSize = 32 * 1024;
          options.StreamBufferCapacity = 10;
      });

#if RELEASE
builder.Services.AddSignalR().AddAzureSignalR();
#endif
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));
builder.Services.Configure<AElfSettings>(builder.Configuration.GetSection("AELF"));
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMudServices(configuration =>
{
    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
    configuration.SnackbarConfiguration.VisibleStateDuration = 5000;
    configuration.SnackbarConfiguration.ShowCloseIcon = true;
});

builder.Services.AddScoped<AElfClientFactory>();
builder.Services.AddScoped<IKeyStore, AElfKeyStore>();
builder.Services.AddScoped<IAccountsService, AccountsService>();
builder.Services.AddScoped<IBlockChainService, BlockChainService>();
builder.Services.AddScoped<IDateTime, DateTimeService>();
builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();

builder.Services.AddScoped<ClientPreferenceManager>();
builder.Services.AddScoped<IAppDialogService, AppDialogService>();
builder.Services.AddManagers();
builder.Services.AddScoped<AppBreakpointService>();
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddScoped<PageService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();