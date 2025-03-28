using CommunityToolkit.Mvvm.Input;
using Kaede.Services;
using Kaede.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.ViewModels
{
    public class HomeViewModelBase : ViewModelBase
    {
        private readonly NavigationService<UserLoginViewModel> _userLoginNavService;
        private readonly NavigationService<DashboardViewModel> _dashboardNavService;

        public UserSession USession { get; }
        public IRelayCommand LogoutCommand { get; }
        public IRelayCommand NavigateDashboardCommand { get; }
        public IRelayCommand NavigateSettingsCommand { get;  }

        public HomeViewModelBase(
            NavigationStore navigationStore,
            UserSession userSession,
            NavigationService<UserLoginViewModel> userLoginNavService,
            NavigationService<DashboardViewModel> dashboardNavService,
            NavigationService<SettingsViewModel> settingsNavService
        )
        {
            _userLoginNavService = userLoginNavService;
            _dashboardNavService = dashboardNavService;

            USession = userSession;

            LogoutCommand = new RelayCommand(_logout);

            NavigateDashboardCommand = Commands.NavigateCommand.CreateWithPredicate(
                dashboardNavService,
                () => navigationStore.CurrentViewModel is not DashboardViewModel
            );

            NavigateSettingsCommand = Commands.NavigateCommand.CreateWithPredicate(
                settingsNavService,
                () => navigationStore.CurrentViewModel is not SettingsViewModel
            );
        }

        void _logout()
        {
            USession.Logout();
            _userLoginNavService.Navigate();
        }
    }
}
