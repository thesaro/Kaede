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

namespace Kaede.ViewModels
{
    class UserRegistrationViewModel : ViewModelBase
    {
        private string _username = "";
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand SubmitCommand { get; }
        public ICommand NavigateLoginCommand { get; }

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

        private bool CanRegisterUser()
        {
            return true;
        }
    }
}
