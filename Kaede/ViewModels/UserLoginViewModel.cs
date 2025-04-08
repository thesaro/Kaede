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
using Kaede.DTOs;
using System.Windows;

namespace Kaede.ViewModels
{
    public class UserLoginViewModel : ViewModelBase
    {
        #region Services and Dependencies
        private readonly IUserService _userSerivce;
        private readonly UserSession _userSession;
        #endregion

        #region Commands
        public IRelayCommand NavigateHomeCommand { get; }
        public IRelayCommand LoginCommand { get; }
        #endregion

        #region Properties
        private string _username = "";
        public string Username
        {
            get => _username;
            set
            {
                ClearErrors(nameof(Username));
                SetProperty(ref _username, value);
                LoginCommand.NotifyCanExecuteChanged();
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
                LoginCommand.NotifyCanExecuteChanged();
            }
        }
        #endregion

        #region Constructor
        public UserLoginViewModel(
            NavigationService<DashboardViewModel> dashboardViewNavService, 
            IUserService userService,
            UserSession userSession)
        {
            _userSerivce = userService;
            _userSession = userSession;
            NavigateHomeCommand = Commands.NavigateCommand.Create(dashboardViewNavService);
            LoginCommand = new AsyncRelayCommand(LoginUser, CanLoginUser);
        }
        #endregion

        #region LoginCommand Methods
        private async Task LoginUser()
        {
            UserDTO? userDTO = await _userSerivce.GetUser(Username);


            if (userDTO != null && await _userSerivce.ValidatePassword(Username, Password))
            {
                _userSession.Assign(userDTO);
                NavigateHomeCommand.Execute(null);
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanLoginUser() =>
            !string.IsNullOrEmpty(Username) &&
            !string.IsNullOrEmpty(Password);
        #endregion
    }
}
