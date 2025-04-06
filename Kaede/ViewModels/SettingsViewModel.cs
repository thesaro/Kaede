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
        #region Properties and Dependencies
        private readonly NavigationService<UserLoginViewModel> _userLoginNavService;
        public UserSession USession { get; }
        #endregion

        #region Commands
        public IRelayCommand NavigateChangePasswordCommand { get; }
        public IRelayCommand LogoutCommand { get; }
        #endregion

        #region Constructor
        public SettingsViewModel(
            UserSession userSession,
            NavigationService<UserLoginViewModel> userLoginNavService,
            NavigationService<ChangePasswordViewModel> changePasswordNavService)
        {
            USession = userSession;
            _userLoginNavService = userLoginNavService;

            LogoutCommand = new RelayCommand(Logout);
            NavigateChangePasswordCommand = Commands.NavigateCommand.Create(changePasswordNavService);
        }
        #endregion

        #region LogoutCommand Methods
        private void Logout()
        {
            USession.Remove();
            _userLoginNavService.Navigate();
        }
        #endregion
    }
}
