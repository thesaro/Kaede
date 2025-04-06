using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using Kaede.DbContexts;
using Kaede.Config;
using Microsoft.EntityFrameworkCore;
using Kaede.ViewModels;
using Kaede.Stores;
using Kaede.Views;
using Kaede.Services;
using Kaede.Services.UsersService;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Kaede.HostBuilderExt;
using Kaede.Services.RestorePointService;

namespace Kaede;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App : Application
{

    private readonly IHost _host;

    public static App? RunningInstance() => Application.Current as App;
    public S? FetchProviderService<S>() => _host.Services.GetService<S>();
    public App()
    {
        #if DEBUG
            AllocConsole();
            Console.WriteLine("Debug mode: Console attached.");
        #endif

        Config.AppUtils.LoadAppData();

        _host = Host.CreateDefaultBuilder()
            .AddViewModels()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContextFactory<KaedeDbContext>(options =>
                    options.UseSqlite(Config.AppUtils.ConnectionString));

                services.AddSingleton<NavigationStore>();
                services.AddSingleton<IUserService, DatabaseUserService>();
                services.AddSingleton<IRestorePointService, DatabaseRestorePointService>();

                services.AddSingleton<UserSession>();

                services.AddSingleton(s => new MainWindow()
                {
                    DataContext = new MainViewModel(
                        s.GetRequiredService<NavigationStore>(),
                        s.GetRequiredService<UserSession>()
                    )
                });
            })
            .Build();
    }

    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IUserService userService = _host.Services.GetRequiredService<IUserService>();
        bool hasAdmin = userService.HasAdmin().GetAwaiter().GetResult();

        if (hasAdmin)
        {
            NavigationService<UserLoginViewModel> loginNavService =
                _host.Services.GetRequiredService<NavigationService<UserLoginViewModel>>();
            loginNavService.Navigate();
        }
        else
        {
            NavigationService<UserRegistrationViewModel> registerNavService = 
                _host.Services.GetRequiredService<NavigationService<UserRegistrationViewModel>>();
            registerNavService.Navigate();
        }


        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }
}

