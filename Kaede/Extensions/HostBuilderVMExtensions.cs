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
    /// <summary>
    /// Provides extension methods for <see cref="IHostBuilder"/> to register ViewModel services.
    /// </summary>
    public static class HostBuilderVMExtensions
    {
        /// <summary>
        /// Registers all required ViewModel services and their associated navigation services into the dependency injection container.
        /// This method adds transient registrations for each ViewModel type, singleton factories for resolving ViewModels, 
        /// and singleton navigation services to facilitate navigation within the application.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IHostBuilder"/> instance to configure.</param>
        /// <returns>The same <see cref="IHostBuilder"/> instance with ViewModel services registered.</returns>
        public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                RegisterVMService<UserLoginViewModel>(services);
                RegisterVMService<UserRegistrationViewModel>(services);
                RegisterVMService<ShopItemSubmitionViewModel>(services);
                RegisterVMService<ShopItemListingViewModel>(services);
                RegisterVMService<AppointmentSubmitionViewModel>(services);
                RegisterVMService<AppointmentListingViewModel>(services);
                RegisterVMService<DashboardViewModel>(services);
                RegisterVMService<SettingsViewModel>(services);
                RegisterVMService<AdminPanelViewModel>(services);
                RegisterVMService<BarberRegistrationViewModel>(services);
                RegisterVMService<BarberListingViewModel>(services);
                RegisterVMService<ChangePasswordViewModel>(services);
            });

            return hostBuilder;
        }

        /// <summary>
        /// Registers a ViewModel service of type <typeparamref name="T"/> as transient, 
        /// along with a singleton factory delegate and a singleton navigation service for the ViewModel.
        /// </summary>
        /// <typeparam name="T">The type of the ViewModel to register. Must inherit from <see cref="ViewModelBase"/>.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        private static void RegisterVMService<T>(IServiceCollection services)
            where T : ViewModelBase
        {
            services.AddTransient<T>();
            services.AddSingleton<Func<T>>((s) => () => s.GetRequiredService<T>());
            services.AddSingleton<NavigationService<T>>();
        }
    }
}
