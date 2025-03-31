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

namespace Kaede.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly UserSession _userSession;

        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        public IRelayCommand NavigateDashboardCommand { get; }
        public IRelayCommand NavigateSettingsCommand { get; }
        public IRelayCommand NavigateAdminPanelCommand { get; }

        public bool IsHomeView =>
            CurrentViewModel 
            is DashboardViewModel 
            or SettingsViewModel
            or AdminPanelViewModel;

        public bool IsAdminLogged =>
            _userSession.CurrentUser?.Role == Models.UserRole.Admin;

        public MainViewModel(
            NavigationStore navigationStore,
            UserSession userSession,
            NavigationService<DashboardViewModel> dashboardNavService,
            NavigationService<SettingsViewModel> settingsNavService,
            NavigationService<AdminPanelViewModel> adminPanelNavService)
        {
            _userSession = userSession;
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            NavigateDashboardCommand = Commands.NavigateCommand.CreateWithPredicate(dashboardNavService, 
                () => CurrentViewModel is not DashboardViewModel);
            NavigateSettingsCommand = Commands.NavigateCommand.CreateWithPredicate(settingsNavService,
                () => CurrentViewModel is not SettingsViewModel);
            NavigateAdminPanelCommand = Commands.NavigateCommand.CreateWithPredicate(adminPanelNavService,
                () => CurrentViewModel is not AdminPanelViewModel);
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
            OnPropertyChanged(nameof(IsHomeView));
            OnPropertyChanged(nameof(IsAdminLogged));

            Console.WriteLine($"Current VM is {CurrentViewModel} and ADMIN is {IsAdminLogged}");
        }
    }
}
