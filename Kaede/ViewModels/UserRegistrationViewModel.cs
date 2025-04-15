using CommunityToolkit.Mvvm.Input;
using Kaede.Commands;
using Kaede.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kaede.Services;
using Kaede.Services.UsersService;
using System.Windows;
using System.ComponentModel.DataAnnotations;
using System.Windows.Controls;

using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using System.Diagnostics;
using System.Threading;
using Kaede.DTOs;
using Kaede.Services.RestorePointService;
using Microsoft.Win32;
using Kaede.Stores;
using Microsoft.Extensions.Logging;


namespace Kaede.ViewModels
{
    public class UserRegistrationViewModel : ViewModelBase
    {
        #region Services and Dependencies
        private readonly ILogger<UserRegistrationViewModel> _logger;
        private readonly IUserService _userService;
        private readonly IRestorePointService _restorePointService;
        private readonly UserSession _userSession;
        #endregion

        #region Commands 
        public IRelayCommand RegisterCommand { get; }
        public IRelayCommand RestoreCommand { get; }
        public IRelayCommand NavigateHomeCommand { get; }

        #endregion

        #region Properties

        private string _username = "";
        [Required]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters.")]
        [MaxLength(20, ErrorMessage = "Username must not be longer than 20 characters.")]
        [UsernameValidation]
        public string Username
        {
            get => _username;
            set
            {
                ClearErrors(nameof(Username));
                _logger.LogTrace("Username property being updated: {OldValue} -> {NewValue}", _username, value);
                SetProperty(ref _username, value, true);
                RegisterCommand.NotifyCanExecuteChanged();
            }
        }

        private string _password = "";
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [MaxLength(40, ErrorMessage = "Password must not be longer than 40 characters.")]
        [CustomValidation(typeof(UserRegistrationViewModel), nameof(ValidateMatchingPassword))]
        public string Password
        {
            get => _password;
            set
            {
                ClearErrors(nameof(Password));
                _logger.LogTrace("Password property being updated: {OldValue} -> {NewValue}", _password, value);
                SetProperty(ref _password, value, true);
                if (!string.IsNullOrEmpty(PasswordConfirm))
                    ValidateProperty(PasswordConfirm, nameof(PasswordConfirm));
                RegisterCommand.NotifyCanExecuteChanged();
            }
        }

        private string _passwordConfirm = "";
        [Required]
        [CustomValidation(typeof(UserRegistrationViewModel), nameof(ValidateMatchingPassword))]
        public string PasswordConfirm
        {
            get => _passwordConfirm;
            set
            {
                ClearErrors(nameof(PasswordConfirm));
                _logger.LogTrace("PasswordConfirm property being updated: {OldValue} -> {NewValue}", _passwordConfirm, value);
                SetProperty(ref _passwordConfirm, value, true);
                ValidateProperty(Password, nameof(Password));
                RegisterCommand.NotifyCanExecuteChanged();
            }
        }
        #endregion

        #region Constructor
        public UserRegistrationViewModel(
            ILogger<UserRegistrationViewModel> logger,
            NavigationService<DashboardViewModel> dashboardViewNavService, 
            IUserService userService,
            IRestorePointService restorePointService,
            UserSession userSession
        ) {
            _logger = logger;
            _userService = userService;
            _restorePointService = restorePointService;
            _userSession = userSession;

            NavigateHomeCommand = Commands.NavigateCommand.Create(dashboardViewNavService);
            RestoreCommand = new RelayCommand(RestorePrevBackup);
            RegisterCommand = new AsyncRelayCommand(RegisterUser, CanRegisterUser);
        }
        #endregion

        #region RegisterCommand Methods

        private async Task RegisterUser()
        {
            // No need to check for duplicate user here cos
            // admin is the first one to register
            UserDTO adminDTO = new UserDTO()
            {
                Username = this.Username,
                Password = this.Password,
                Role = UserRole.Admin
            };

            try
            {
                _logger.LogInformation("Attempting to register admin user: {Username}", adminDTO.Username);

                await _userService.CreateUser(adminDTO);

                _logger.LogInformation("Admin {Username} successfully registered", adminDTO.Username);

                MessageBox.Show($"Admin \"{adminDTO.Username}\" successfully registered", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register admin user {Username}. Reason: {Message}", adminDTO.Username, ex.Message);
                MessageBox.Show("Failed to register admin due to technical problems", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _logger.LogInformation("Assigning user session for {Username}", adminDTO.Username);
            _userSession.Assign(adminDTO);
            // need to resolve initially because of
            // creation date submition
            _logger.LogDebug("Resolving changes for user after assign");
            _userSession.ResolveChanges();
            ClearErrors();

            _logger.LogInformation("Navigating to Dashboard");
            NavigateHomeCommand.Execute(null);
        }

        private bool CanRegisterUser() =>
            !HasErrors &&
            !string.IsNullOrEmpty(Username) &&
            !string.IsNullOrEmpty(Password) &&
            !string.IsNullOrEmpty(PasswordConfirm);
        #endregion

        #region RestoreCommand Methods
        private void RestorePrevBackup()
        {
            _logger.LogDebug("RestorePrevBackup method invoked");

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open backup file",
                Filter = "SQLite Database Files (*.db)|*.db|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _logger.LogInformation("Backup file selected: {FileName}", openFileDialog.FileName);

                try
                {
                    _logger.LogDebug("Attempting to restore from backup file: {FileName}", openFileDialog.FileName);

                    _restorePointService.Restore(openFileDialog.FileName);

                    _logger.LogInformation("Restore successful from file: {FileName}. Will apply after relaunch.",
                        openFileDialog.FileName);

                    MessageBox.Show("Restore success.\nRelaunch the app for changes to take effect.",
                        "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to restore backup from file {FileName}", openFileDialog.FileName);
                    MessageBox.Show("Could not restore backup due to technical reasons.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                _logger.LogWarning("Restore operation canceled by the user");
            }
        }
        #endregion
        #region Validation Methods and Attributes
        public static ValidationResult? ValidateMatchingPassword(string _, ValidationContext context)
        {
            UserRegistrationViewModel instance = (UserRegistrationViewModel)context.ObjectInstance;
            bool isValid = instance.Password == instance.PasswordConfirm;

            if (isValid)
                return ValidationResult.Success;

            return new("Passwords do not match.");
        }

        internal sealed class UsernameValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                string uname = (string)value!;
                if (uname.Any(Char.IsWhiteSpace))
                    return new("Username must not contain whitespace.");
                if (!uname.All(Char.IsAscii))
                    return new("Username must only contain ASCII characters.");
                return ValidationResult.Success;
            }
        }
        #endregion

    }


}
