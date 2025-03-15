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


namespace Kaede.ViewModels
{
    class UserRegistrationViewModel : ViewModelBase
    {

        private string _username = "";
        [Required]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters.")]
        [MaxLength(20, ErrorMessage = "Username must not be longer than 20 characters.")]
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
        public string Password
        {
            get => _password;
            set
            {
                ClearErrors(nameof(Password));
                SetProperty(ref _password, value, true);
                SubmitCommand.NotifyCanExecuteChanged();
            }
        }

        private string _passwordConfirm = "";
        public string PasswordConfirm
        {
            get => _passwordConfirm;
            set
            {
                ClearErrors(nameof(PasswordConfirm));
                SetProperty(ref _passwordConfirm, value, true);
                SubmitCommand.NotifyCanExecuteChanged();
            }
        } 
        public IRelayCommand SubmitCommand { get; }
        public IRelayCommand NavigateLoginCommand { get; }

        private readonly IUserService _userService;

        public UserRegistrationViewModel
            (NavigationService<UserLoginViewModel> userLoginViewNavigationService, IUserService userService) 
        {
            _userService = userService;
            NavigateLoginCommand = Commands.NavigateCommand.Create(userLoginViewNavigationService);
            SubmitCommand = new AsyncRelayCommand(RegisterUser, CanRegisterUser);
        }

        private async Task RegisterUser()
        {
            User user = new User() { Username = this.Username, PasswordHash = this.Password };
            await _userService.CreateUser(user);

            var res = MessageBox.Show("User actaully got registered!\nRedirecting to login...", "NICE!!", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            if (res == MessageBoxResult.OK)
                NavigateLoginCommand.Execute(null);
        }

        private bool CanRegisterUser() =>
            !HasErrors &&
            !string.IsNullOrEmpty(Username) &&
            !string.IsNullOrEmpty(Password) &&
            !string.IsNullOrEmpty(PasswordConfirm);
    }


}
