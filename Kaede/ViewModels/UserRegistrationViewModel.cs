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


namespace Kaede.ViewModels
{
    public class UserRegistrationViewModel : ViewModelBase
    {
        #region Commands 
        public IRelayCommand RegisterCommand { get; }
        public IRelayCommand NavigateHomeCommand { get; }
        #endregion

        #region Services and Dependencies
        private readonly IUserService _userService;
        private readonly UserSession _userSession;
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
                SetProperty(ref _passwordConfirm, value, true);
                ValidateProperty(Password, nameof(Password));
                RegisterCommand.NotifyCanExecuteChanged();
            }
        }
        #endregion

        #region Constructor
        public UserRegistrationViewModel(
            NavigationService<DashboardViewModel> dashboardViewNavService, 
            IUserService userService,
            UserSession userSession
        ) {
            _userService = userService;
            _userSession = userSession;
            NavigateHomeCommand = Commands.NavigateCommand.Create(dashboardViewNavService);
            RegisterCommand = new AsyncRelayCommand(_registerUser, _canRegisterUser);
        }
        #endregion

        #region RegisterCommand Methods

        private async Task _registerUser()
        {
            // No need to check for duplicate user here cos
            // admin is the first one to register
            UserDTO admin = new UserDTO()
            {
                Username = this.Username,
                Password = this.Password,
                Role = UserRole.Admin
            };

            try
            {
                await _userService.CreateUser(admin);
                MessageBox.Show($"Barber \"{admin.Username}\" successfully registered", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register admin.\nReason: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _userSession.Assign(admin);
            ClearErrors();

            NavigateHomeCommand.Execute(null);
        }

        private bool _canRegisterUser() =>
            !HasErrors &&
            !string.IsNullOrEmpty(Username) &&
            !string.IsNullOrEmpty(Password) &&
            !string.IsNullOrEmpty(PasswordConfirm);
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
