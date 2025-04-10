using CommunityToolkit.Mvvm.Input;
using Kaede.DTOs;
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
        #region Services and Dependencies
        private readonly IUserService _userService;
        #endregion

        #region Commands
        public IRelayCommand SubmitCommand { get; }
        #endregion

        #region Properties
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
        #endregion

        #region Constructor
        public BarberRegistrationViewModel(IUserService userService)
        {
            _userService = userService;

            SubmitCommand = new AsyncRelayCommand(RegisterBarber, CanRegisterBarber);
        }
        #endregion

        #region RegisterBarberCommand Methods
        private async Task RegisterBarber()
        {

            if (await _userService.GetUser(Username) != null)
            {
                MessageBox.Show($"Username \"{Username}\" already exists.", "Error",
                    MessageBoxButton.OK);
                return;
            }

            UserDTO barberDTO = new UserDTO()
            {
                Username = this.Username,
                Password = this.Password,
                Role = UserRole.Barber
            };
            
            try
            {
                await _userService.CreateUser(barberDTO);
                // need to resolve the barber immediately because of creation dates
                OnBarberAdded((await _userService.GetUser(barberDTO.Username))!);
                MessageBox.Show($"Barber \"{barberDTO.Username}\" successfully registered", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register barber.\nReason: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ClearErrors();
            SubmitCommand.NotifyCanExecuteChanged();
        }

        private bool CanRegisterBarber() =>
            !HasErrors &&
            !string.IsNullOrEmpty(Username) &&
            !string.IsNullOrEmpty(Password) &&
            !string.IsNullOrEmpty(PasswordConfirm);
        #endregion

        #region Events
        public Action<UserDTO>? BarberAdded;

        private void OnBarberAdded(UserDTO barberDTO)
        {
            BarberAdded?.Invoke(barberDTO);
        }
        #endregion

        #region Validation Methods
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
        #endregion
    }

    public class BarberListingViewModel : ViewModelBase
    {
        #region Services and Dependencies
        private readonly IUserService _userService;
        #endregion

        #region Commands
        public ICommand RemoveBarberCommand { get; }
        public ICommand ChangePassCommand { get; }
        #endregion

        #region Properties
        private readonly ObservableCollection<UserDTO> _barbers;
        public IEnumerable<UserDTO> Barbers => _barbers;
        #endregion


        #region Constructor
        public BarberListingViewModel(IUserService userService)
        {
            _userService = userService;

            List<UserDTO> res = userService.GetBarbers().GetAwaiter().GetResult();
            _barbers = new ObservableCollection<UserDTO>(res);

            RemoveBarberCommand = new RelayCommand<object?>(RemoveBarber);
            ChangePassCommand = new RelayCommand<object?>(ChangeBarberPassword);
        }
        #endregion

        #region Methods
        public void AddBarber(UserDTO barberDTO)
        {
            _barbers.Add(barberDTO);
        }
        private void RemoveBarber(object? item)
        {
            // This check 100% succeds in runtime if the xaml setup is correct.
            if (item != null && item is UserDTO barberDTO)
            {
                MessageBoxResult removeRes = MessageBox.Show($"Do you really want to remove \"{barberDTO.Username}\"?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (removeRes != MessageBoxResult.Yes) 
                    return;

                try
                {
                    _userService.RemoveUser(barberDTO.Username);
                    _barbers.Remove(barberDTO);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error occured while removing barber:\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ChangeBarberPassword(object? item)
        {
            MessageBox.Show("devs were busy so they left me out :3", "Not Implemented",
                MessageBoxButton.OK, MessageBoxImage.None);
        }
        #endregion
    }

    public class AdminPanelViewModel : ViewModelBase
    {
        #region Services and Dependencies
        private readonly IRestorePointService _restorePointService;
        #endregion

        #region Commands
        public IRelayCommand BackupCommand { get; }
        public IRelayCommand RestoreCommand { get; }
        #endregion

        #region Child ViewModels
        public BarberListingViewModel BarberListingVM { get; }
        public BarberRegistrationViewModel BarberRegistrationVM { get; }
        #endregion

        #region Constructor
        public AdminPanelViewModel(
            IRestorePointService restorePointService,
            BarberListingViewModel barberListingVM,
            BarberRegistrationViewModel barberRegistrationVM)
        {
            _restorePointService = restorePointService;

            BackupCommand = new RelayCommand(CreateBackup);
            RestoreCommand = new RelayCommand(RestoreBackup);

            BarberListingVM = barberListingVM;
            BarberRegistrationVM = barberRegistrationVM;

            // maybe abstract this eventing stuff into a IWhateverService if it proves to 
            // be recurring in the program structure.
            BarberRegistrationVM.BarberAdded += BarberListingVM.AddBarber;
        }
        #endregion

        #region Methods
        private void CreateBackup()
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

        private void RestoreBackup()
        {
            // confirm app data overwrite
            MessageBoxResult result = MessageBox.Show("Restoring another app data instance will REMOVE the current data.\nAre you sure you want to proceed?",
                "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.OK) return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open backup file",
                Filter = "SQLite Database Files (*.db)|*.db|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _restorePointService.Restore(openFileDialog.FileName);
                    MessageBox.Show("Restore success.\nRelaunch the app for changes to take effect.",
                        "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not restore backup due to:\n{ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
    }
}
