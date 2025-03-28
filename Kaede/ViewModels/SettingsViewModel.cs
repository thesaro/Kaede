using Kaede.Services;
using Kaede.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.ViewModels
{
    public class SettingsViewModel : HomeViewModelBase
    {
        public SettingsViewModel(
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
