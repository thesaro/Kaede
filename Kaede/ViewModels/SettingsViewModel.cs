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
    public class SettingsViewModel : ViewModelBase
    {
        private readonly NavigationService<UserLoginViewModel> _userLoginNavService;
        public UserSession USession { get; }
        public IRelayCommand NavigateChangePasswordCommand { get; }
        public IRelayCommand LogoutCommand { get; }
        public SettingsViewModel(
            UserSession userSession,
            NavigationService<UserLoginViewModel> userLoginNavService,
            NavigationService<ChangePasswordViewModel> changePasswordNavService)
        {
            USession = userSession;
            _userLoginNavService = userLoginNavService;

            LogoutCommand = new RelayCommand(_logout);
            NavigateChangePasswordCommand = Commands.NavigateCommand.Create(changePasswordNavService);
        }

        private void _logout()
        {
            USession.Logout();
            _userLoginNavService.Navigate();
        }

    }
}
