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
        public IRelayCommand LogoutCommand { get; }
        public SettingsViewModel(
            UserSession userSession,
            NavigationService<UserLoginViewModel> userLoginNavService)
        {
            USession = userSession;
            _userLoginNavService = userLoginNavService;

            LogoutCommand = new RelayCommand(_logout);
        }

        private void _logout()
        {
            USession.Logout();
            _userLoginNavService.Navigate();
        }
    }
}
