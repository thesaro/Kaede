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

namespace Kaede;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App : Application
{

    private readonly IHost _host;


    public App()
    {
        #if DEBUG
            AllocConsole();
            Console.WriteLine("Debug mode: Console attached.");
        #endif

        InitializeDb();

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContextFactory<KaedeDbContext>(options =>
                    options.UseSqlite(Config.AppUtils.ConnectionString));

                services.AddSingleton<NavigationStore>();
                services.AddSingleton<IUserService, DatabaseUserService>();

                services.AddTransient<UserLoginViewModel>();
                services.AddSingleton<Func<UserLoginViewModel>>
                    ((s) => () => s.GetRequiredService<UserLoginViewModel>());
                services.AddSingleton<NavigationService<UserLoginViewModel>>();

                services.AddTransient<UserRegistrationViewModel>();
                services.AddSingleton<Func<UserRegistrationViewModel>>
                    ((s) => () => s.GetRequiredService<UserRegistrationViewModel>());
                services.AddSingleton<NavigationService<UserRegistrationViewModel>>();

                services.AddSingleton(s => new MainWindow()
                {
                    DataContext = new MainViewModel(s.GetRequiredService<NavigationStore>())
                });
            })
            .Build();
    }

    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();


    private void InitializeDb()
    {
        AppUtils.CreateLocalAppDir();
        var options = new DbContextOptionsBuilder<KaedeDbContext>()
            .UseSqlite(Config.AppUtils.ConnectionString).Options;
        using var context = new KaedeDbContext(options);

        context.Database.EnsureCreated();
        context.Database.Migrate();
    }
    

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        NavigationService<UserRegistrationViewModel> navService = _host.Services.GetRequiredService<NavigationService<UserRegistrationViewModel>>();
        navService.Navigate();

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }
}

