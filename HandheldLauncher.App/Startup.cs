using HandheldLauncher.App.Data;
using HandheldLauncher.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radzen;

namespace HandheldLauncher.App
{
    public static class Startup
    {
        public static IServiceProvider? Services { get; private set; }

        public static void Init()
        {
            var host = Host.CreateDefaultBuilder()
                           .ConfigureServices(WireupServices)
                           .Build();
            Services = host.Services;
        }

        private static void WireupServices(IServiceCollection services)
        {
            services.AddWindowsFormsBlazorWebView();
            services.AddRadzenComponents();
            services.AddSingleton<WeatherForecastService>();
            var launcherSettingsService = new LauncherSettingsService("HandheldLauncher");
            services.AddSingleton(launcherSettingsService);
            launcherSettingsService.LoadAsync().GetAwaiter().GetResult();


#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif
        }
    }
}
