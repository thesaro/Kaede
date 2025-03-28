using CommunityToolkit.Mvvm.ComponentModel;
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
        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
        public bool IsMainViews { get; private set; } = false;

        public MainViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

      
        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));

            IsMainViews = CurrentViewModel switch
            {
                HomeViewModel => true,
                UserLoginViewModel or UserRegistrationViewModel => false,
                _ => false
            };

            OnPropertyChanged(nameof(IsMainViews));
        }
    }
}
