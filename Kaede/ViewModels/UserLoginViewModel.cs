using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaede.Services;
using Kaede.Models;
using Kaede.Services.UsersService;
using System.Security.Permissions;
using System.Security.Cryptography;
using System.Printing.IndexedProperties;

namespace Kaede.ViewModels
{
    public class UserLoginViewModel : ViewModelBase
    {
        private readonly IUserService _userSerivce;
        private readonly UserSession _userSession;

        public IRelayCommand NavigateHomeCommand { get; }
        public IRelayCommand SubmitCommand { get; }

        private string _username = "";
        public string Username
        {
            get => _username;
            set
            {
                ClearErrors(nameof(Username));
                SetProperty(ref _username, value);
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
                SetProperty(ref _password, value);
                SubmitCommand.NotifyCanExecuteChanged();
            }
        }

        private string _loginError = "";
        public string LoginError
        {
            get => _loginError;
            set => SetProperty(ref _loginError, value);
        }


        public UserLoginViewModel(
            NavigationService<DashboardViewModel> dashboardViewNavService, 
            IUserService userService,
            UserSession userSession)
        {
            _userSerivce = userService;
            _userSession = userSession;
            NavigateHomeCommand = Commands.NavigateCommand.Create(dashboardViewNavService);
            SubmitCommand = new AsyncRelayCommand(LoginUser, CanLoginUser);
        }

        private async Task LoginUser()
        {
            User? user = await _userSerivce.GetUser(Username);
            if (user is null)
            {
                _loginError = "Username does not exist";
                return;
            }

            if (user.PasswordHash != User.HashPassword(this.Password))
            {
                _loginError = "Invalid password";
                return;
            }

            _userSession.Login(user);
            NavigateHomeCommand.Execute(null);
        }

        private bool CanLoginUser() =>
            !string.IsNullOrEmpty(Username) &&
            !string.IsNullOrEmpty(Password);
    }
}
