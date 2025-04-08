using CommunityToolkit.Mvvm.Input;
using Kaede.Services;
using Kaede.Services.UsersService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kaede.ViewModels
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        #region Services and Dependencies
        private readonly IUserService _userService;
        private readonly UserSession _userSession;
        private readonly NavigationService<SettingsViewModel> _settingsNavService;
        #endregion

        #region Commands
        public IRelayCommand NavigateBackCommand { get; }
        public IRelayCommand ChangePasswordCommand { get; }
        #endregion

        #region Properties
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
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [MaxLength(40, ErrorMessage = "Password must not be longer than 40 characters.")]
        [CustomValidation(typeof(ChangePasswordViewModel), nameof(ValidateMatchingPassword))]
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                ClearErrors(nameof(NewPassword));
                SetProperty(ref _newPassword, value, true);
                if (!string.IsNullOrEmpty(NewPasswordConfirm))
                    ValidateProperty(NewPasswordConfirm, nameof(NewPasswordConfirm));
                ChangePasswordCommand.NotifyCanExecuteChanged();
            }
        }

        private string _newPasswordConfirm = "";
        [Required]
        [CustomValidation(typeof(ChangePasswordViewModel), nameof(ValidateMatchingPassword))]
        public string NewPasswordConfirm
        {
            get => _newPasswordConfirm;
            set
            {
                ClearErrors(nameof(NewPasswordConfirm));
                SetProperty(ref _newPasswordConfirm, value, true);
                ValidateProperty(NewPassword, nameof(NewPassword));
                ChangePasswordCommand.NotifyCanExecuteChanged();
            }
        }
        #endregion

        #region Constructor
        public ChangePasswordViewModel(
            UserSession userSession,
            IUserService userService, 
            NavigationService<SettingsViewModel> settingsNavService
        )
        {
            _userSession = userSession;
            _userService = userService;
            _settingsNavService = settingsNavService;

            ChangePasswordCommand = new AsyncRelayCommand(ChangePWD, CanChangePWD);
            NavigateBackCommand = Commands.NavigateCommand.Create(_settingsNavService);
        }
        #endregion

        #region ChangePasswordCommand Methods
        private async Task ChangePWD()
        {
            if (!await _userService
                .ValidatePassword(_userSession.CurrentUser!.Username, CurrentPassword))
            {
                MessageBox.Show("Incorrect password", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                if (_userSession.CurrentUser == null)
                    throw new ArgumentNullException();
                await _userService.ChangePassword(_userSession.CurrentUser.Username, NewPassword);
                _userSession.ResolveChanges();
                MessageBox.Show("Your password was changed successfully.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to change password due to:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            _settingsNavService.Navigate();
        }

        private bool CanChangePWD() =>
            !HasErrors &&
            !string.IsNullOrEmpty(CurrentPassword) &&
            !string.IsNullOrEmpty(NewPassword) &&
            !string.IsNullOrEmpty(NewPasswordConfirm);
        #endregion

        #region Validation Methods
        public static ValidationResult? ValidateMatchingPassword(string _, ValidationContext context)
        {
            ChangePasswordViewModel instance = (ChangePasswordViewModel)context.ObjectInstance;
            bool isValid = instance.NewPassword == instance.NewPasswordConfirm;

            if (isValid)
                return ValidationResult.Success;

            return new("Passwords do not match.");
        }
        #endregion
    }
}
