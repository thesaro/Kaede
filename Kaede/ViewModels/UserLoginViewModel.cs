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
using Kaede.Stores;
using Microsoft.Extensions.Logging;

namespace Kaede.ViewModels
{
    public class UserLoginViewModel : ViewModelBase
    {
        #region Services and Dependencies
        private readonly ILogger _logger;
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
                _logger.LogTrace("Username property being updated: {OldValue} -> {NewValue}", _username, value);
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
                _logger.LogTrace("Password property being updated: {OldValue} -> {NewValue}", _password, value);
                SetProperty(ref _password, value);
                LoginCommand.NotifyCanExecuteChanged();
            }
        }
        #endregion

        #region Constructor
        public UserLoginViewModel(
            ILogger<UserLoginViewModel> logger,
            NavigationService<DashboardViewModel> dashboardViewNavService, 
            IUserService userService,
            UserSession userSession)
        {
            _logger = logger;
            _userSerivce = userService;
            _userSession = userSession;
            NavigateHomeCommand = Commands.NavigateCommand.Create(dashboardViewNavService);
            LoginCommand = new AsyncRelayCommand(LoginUser, CanLoginUser);
        }
        #endregion

        #region LoginCommand Methods
        private async Task LoginUser()
        {
            _logger.LogInformation("Login attempt for user: {Username}", Username);
            _logger.LogDebug("Fetching user {Username} from DB", Username);

            bool loginSuccess = false;
            UserDTO? userDTO = await _userSerivce.GetUser(Username);

            if (userDTO == null)
            {
                _logger.LogWarning("Login failed: User {Username} not found.", Username);
            }
            else
            {
                _logger.LogDebug("Validating password for user {Username}", Username);
                if (await _userSerivce.ValidatePassword(Username, Password))
                {
                    _logger.LogInformation("Login successful for user {Username}", Username);

                    loginSuccess = true;
                    _userSession.Assign(userDTO);

                    _logger.LogInformation("Navigating to HomeView");

                    NavigateHomeCommand.Execute(null);
                }
                else
                {
                    _logger.LogWarning("Login failed: Incorrect password for {Username}", Username);
                }
            }

            if (!loginSuccess)
                MessageBox.Show("Invalid username or password.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool CanLoginUser() =>
            !string.IsNullOrEmpty(Username) &&
            !string.IsNullOrEmpty(Password);
        #endregion
    }
}
