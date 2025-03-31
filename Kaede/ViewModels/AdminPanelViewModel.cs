using CommunityToolkit.Mvvm.Input;
using Kaede.Models;
using Kaede.Services;
using Kaede.Services.UsersService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.ViewModels
{
    public class BarberRegistrationViewModel : ViewModelBase
    { 
        private readonly IUserService _userService;
        public IRelayCommand SubmitCommand { get; }

        public BarberRegistrationViewModel(IUserService userService)
        {
            _userService = userService;

            SubmitCommand = new AsyncRelayCommand(RegisterBarber, CanRegisterBarber);
        }

        private string _username = "";
        [Required]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters.")]
        [MaxLength(20, ErrorMessage = "Username must not be longer than 20 characters.")]
        [UserRegistrationViewModel.UsernameValidation]
        public string Username
        {
            get => _username;
            set
            {
                ClearErrors(nameof(Username));
                SetProperty(ref _username, value, true);
                SubmitCommand.NotifyCanExecuteChanged();
            }
        }

        private string _password = "";
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [MaxLength(40, ErrorMessage = "Password must not be longer than 40 characters.")]
        [CustomValidation(typeof(BarberRegistrationViewModel), nameof(ValidateMatchingPassword))]
        public string Password
        {
            get => _password;
            set
            {
                ClearErrors(nameof(Password));
                SetProperty(ref _password, value, true);
                if (!string.IsNullOrEmpty(PasswordConfirm))
                    ValidateProperty(PasswordConfirm, nameof(PasswordConfirm));
                SubmitCommand.NotifyCanExecuteChanged();
            }
        }

        private string _passwordConfirm = "";
        [Required]
        [CustomValidation(typeof(BarberRegistrationViewModel), nameof(ValidateMatchingPassword))]
        public string PasswordConfirm
        {
            get => _passwordConfirm;
            set
            {
                ClearErrors(nameof(PasswordConfirm));
                SetProperty(ref _passwordConfirm, value, true);
                ValidateProperty(Password, nameof(Password));
                SubmitCommand.NotifyCanExecuteChanged();
            }

        }
        private async Task RegisterBarber()
        {
            User user = new User()
            {
                Username = this.Username,
                PasswordHash = User.HashPassword(this.Password),
                Role = UserRole.Barber
            };
            await _userService.CreateUser(user);
            // TODO: Show some message box for like confirmation or other stuff
            ClearErrors();
        }

        private bool CanRegisterBarber() =>
            !HasErrors &&
            !string.IsNullOrEmpty(Username) &&
            !string.IsNullOrEmpty(Password) &&
            !string.IsNullOrEmpty(PasswordConfirm);


        // This is uhh a bit replicated from the UserRegistrationViewModel maybe
        // find a way to abstract it later.
        public static ValidationResult? ValidateMatchingPassword(string _, ValidationContext context)
        {
            BarberRegistrationViewModel instance = (BarberRegistrationViewModel)context.ObjectInstance;
            bool isValid = instance.Password == instance.PasswordConfirm;

            if (isValid)
                return ValidationResult.Success;

            return new("Passwords do not match.");
        }

    }
    public class AdminPanelViewModel : ViewModelBase
    {
    }
}
