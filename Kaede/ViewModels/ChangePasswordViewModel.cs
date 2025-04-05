using CommunityToolkit.Mvvm.Input;
using Kaede.Services;
using Kaede.Services.UsersService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kaede.ViewModels
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly NavigationService<SettingsViewModel> _settingsNavService;
        public IRelayCommand ChangePasswordCommand { get; }
        private string _currentPassword = "";
        public string CurrentPassword
        {
            get => _currentPassword;
            set
            {
                ClearErrors(nameof(CurrentPassword));
                SetProperty(ref _currentPassword, value);
                ChangePasswordCommand.NotifyCanExecuteChanged();
            }
        }

        private string _newPassword = "";
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                ClearErrors(nameof(NewPassword));
                SetProperty(ref _newPassword, value);
                ChangePasswordCommand.NotifyCanExecuteChanged();
            }
        }

        private string _newPasswordConfirm = "";
        public string NewPasswordConfirm
        {
            get => _newPasswordConfirm;
            set
            {
                ClearErrors(nameof(NewPasswordConfirm));
                SetProperty(ref _newPasswordConfirm, value);
                ChangePasswordCommand.NotifyCanExecuteChanged();
            }
        }

        public ChangePasswordViewModel(
            IUserService userService, 
            NavigationService<SettingsViewModel> settingsNavService
        )
        {
            _userService = userService;
            _settingsNavService = settingsNavService;

            ChangePasswordCommand = new RelayCommand(_changePWD, _canChangePWD);
        }

        private void _changePWD()
        {
            // do the change pwd stuff then relocate to settings

            _settingsNavService.Navigate();
        }

        private bool _canChangePWD() =>
            !string.IsNullOrEmpty(CurrentPassword) &&
            !string.IsNullOrEmpty(NewPassword) &&
            !string.IsNullOrEmpty(NewPasswordConfirm);
    }
}
