using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kaede.Services;
using Kaede.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Kaede.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Services and Dependencies
        private readonly NavigationStore _navigationStore;
        private readonly UserSession _userSession;
        #endregion

        #region Commands
        public IRelayCommand NavigateDashboardCommand { get; }
        public IRelayCommand NavigateSettingsCommand { get; }
        public IRelayCommand NavigateAdminPanelCommand { get; }
        #endregion

        #region Properties
        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
        public bool IsHomeView =>
            CurrentViewModel 
            is DashboardViewModel 
            or SettingsViewModel
            or AdminPanelViewModel
            or ChangePasswordViewModel;
        public bool IsAdminLogged =>
            _userSession.CurrentUser?.Role == Models.UserRole.Admin;
        #endregion

        #region Constructor
        public MainViewModel(NavigationStore navigationStore, UserSession userSession)
        {
            _userSession = userSession;
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            // we guarantee non-nullness here cos how did we even 
            // get here if we don't have a running app instance?
            NavigateDashboardCommand = Commands.NavigateCommand.CreateWithPredicate(
                App.RunningInstance().FetchProviderService<NavigationService<DashboardViewModel>>()!, 
                () => CurrentViewModel is not DashboardViewModel);
            NavigateSettingsCommand = Commands.NavigateCommand.CreateWithPredicate(
                App.RunningInstance().FetchProviderService<NavigationService<SettingsViewModel>>()!,
                () => CurrentViewModel is not SettingsViewModel);
            NavigateAdminPanelCommand = Commands.NavigateCommand.CreateWithPredicate(
                App.RunningInstance().FetchProviderService<NavigationService<AdminPanelViewModel>>()!,
                () => CurrentViewModel is not AdminPanelViewModel);
        }
        #endregion

        #region Event Listeners
        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
            OnPropertyChanged(nameof(IsHomeView));
            OnPropertyChanged(nameof(IsAdminLogged));

            Console.WriteLine($"Current VM is {CurrentViewModel} and ADMIN is {IsAdminLogged}");
        }
        #endregion
    }
}
