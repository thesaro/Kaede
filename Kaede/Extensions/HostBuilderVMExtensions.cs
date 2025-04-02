using Kaede.Services;
using Kaede.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.HostBuilderExt
{
    public static class HostBuilderVMExtensions
    {
        public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                RegisterVMService<UserLoginViewModel>(services);
                RegisterVMService<UserRegistrationViewModel>(services);
                RegisterVMService<DashboardViewModel>(services);
                RegisterVMService<SettingsViewModel>(services);
                RegisterVMService<AdminPanelViewModel>(services);
                RegisterVMService<BarberRegistrationViewModel>(services);
                RegisterVMService<BarberListingView>(services);
            });

            return hostBuilder;
        }

        private static void RegisterVMService<T>(IServiceCollection services)
            where T: ViewModelBase
        {
            services.AddTransient<T>();
            services.AddSingleton<Func<T>>
                ((s) => () => s.GetRequiredService<T>());
            services.AddSingleton<NavigationService<T>>();
        }
    }
}
