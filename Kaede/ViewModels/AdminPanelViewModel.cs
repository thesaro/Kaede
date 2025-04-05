using CommunityToolkit.Mvvm.Input;
using Kaede.Models;
using Kaede.Services;
using Kaede.Services.RestorePointService;
using Kaede.Services.UsersService;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

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

            if (await _userService.GetUser(Username) != null)
            {
                MessageBox.Show($"Username \"{Username}\" already exists.", "Error",
                    MessageBoxButton.OK);
            }
            User barber = new User()
            {
                Username = this.Username,
                PasswordHash = User.HashPassword(this.Password),
                Role = UserRole.Barber
            };
            
            try
            {
                await _userService.CreateUser(barber);
                MessageBox.Show($"Barber \"{barber.Username}\" successfully registered", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register barber.\nReason: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ClearErrors();
            Username = Password = PasswordConfirm = string.Empty;
            SubmitCommand.NotifyCanExecuteChanged();
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

    public class BarberListingView : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly ObservableCollection<User> _barbers;
        public IEnumerable<User> Barbers => _barbers;

        public ICommand RemoveCommand { get; }
        public ICommand ChangePassCommand { get; }

        public BarberListingView(IUserService userService)
        {
            _userService = userService;

            List<User> res = userService.GetBarbers().GetAwaiter().GetResult();
            _barbers = new ObservableCollection<User>(res);

            RemoveCommand = new RelayCommand<object?>(_removeBarber);
            ChangePassCommand = new RelayCommand<object?>(_changeBarberPassword);
        }

        private void _removeBarber(object? item)
        {
            if (item != null && item is User barber)
            {
                try
                {
                    _userService.RemoveUser(barber);
                    _barbers.Remove(barber);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error occured while removing barber:\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void _changeBarberPassword(object? item)
        {
            throw new NotImplementedException();
        }
    }

    public class AdminPanelViewModel : ViewModelBase
    {
        private readonly IRestorePointService _restorePointService;
        public ICommand BackupCommand { get; }
        public ICommand RestoreCommand { get; }


        public AdminPanelViewModel(IRestorePointService restorePointService)
        {
            _restorePointService = restorePointService;

            BackupCommand = new RelayCommand(_createBackup);
            RestoreCommand = new RelayCommand(_restoreBackup);
        }

        private void _createBackup()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save backup as",
                Filter = "SQLite Database Files (*.db)|*.db|All Files (*.*)|*.*",
                FileName = "kdbasere.db"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _restorePointService.Backup(saveFileDialog.FileName);
                    MessageBox.Show("Successfully created backup.", "Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not create backup due to:\n{ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void _restoreBackup()
        {
            throw new NotImplementedException();
        }
    }
}
