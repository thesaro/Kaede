using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using Kaede.DbContexts;
using Kaede.Config;
using Microsoft.EntityFrameworkCore;

namespace Kaede;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;
    public App()
    {
        #if DEBUG
        AllocConsole();
        Console.WriteLine("Debug mode: Console attached.");
#endif


        var services = new ServiceCollection();
        services.AddDbContext<KaedeDbContext>(
            options => options.UseSqlite(Config.AppUtils.ConnectionString));
        services.AddDbContextFactory<KaedeDbContext>(options => 
            options.UseSqlite(Config.AppUtils.ConnectionString));
        _serviceProvider = services.BuildServiceProvider();

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<KaedeDbContext>();
        InitializeDb(context);
    }

    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();


    private void InitializeDb(KaedeDbContext context)
    {
        AppUtils.CreateLocalAppDir();
        context.Database.EnsureCreated();
        context.Database.Migrate();
    }
    

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        MainWindow mw = new MainWindow();
        mw.Show();
    }
}

