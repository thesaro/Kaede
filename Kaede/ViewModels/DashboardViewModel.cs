using CommunityToolkit.Mvvm.Input;
using Kaede.Services;
using Kaede.Stores;

namespace Kaede.ViewModels
{

    public class DashboardViewModel : HomeViewModelBase
    {
        public DashboardViewModel(
            NavigationStore navigationStore,
            UserSession userSession, 
            NavigationService<UserLoginViewModel> userLoginNavService,
            NavigationService<DashboardViewModel> dashboardNavService,
            NavigationService<SettingsViewModel> settingsNavService
        ) : base(navigationStore, userSession, userLoginNavService, dashboardNavService, settingsNavService)
        {
            
        }
    }
}
