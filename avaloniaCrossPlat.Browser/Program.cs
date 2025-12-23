using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using avaloniaCrossPlat;
using avaloniaCrossPlat.Browser.Services;
using avaloniaCrossPlat.Services;
using avaloniaCrossPlat.ViewModels;
using Microsoft.Extensions.DependencyInjection;

internal sealed partial class Program
{
        private static Task Main(string[] args) => BuildAvaloniaApp()
                .WithInterFont()
                .AfterSetup(RegisterServices)
                .StartBrowserAppAsync("out");

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>();


        private static void RegisterServices(AppBuilder builder)
        {
                if (builder.Instance is App app)
                {
                        var services = new ServiceCollection();

                        services.AddSingleton<IClientContextStorage, BrowserClientContextStorage>();
                        services.AddSingleton<MainViewModel>();
                        app.Services = services.BuildServiceProvider();
                }
        }
}