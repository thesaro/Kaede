using Kaede.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Stores
{
    /// <summary>
    /// Manages application navigation state and view model transitions.
    /// Handles the current view model and notifies subscribers when it changes.
    /// </summary>
    public class NavigationStore
    {
        private ViewModelBase? _currentViewModel;

        /// <summary>
        /// Gets or sets the current view model. 
        /// When setting a new value, disposes the previous view model if it implements IDisposable
        /// and raises the CurrentViewModelChanged event.
        /// </summary>
        /// <value>The currently active view model, or null if no view model is set.</value>
        public ViewModelBase? CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel is IDisposable disposableVM)
                    disposableVM.Dispose();
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        /// <summary>
        /// Occurs when the current view model changes.
        /// </summary>
        public event Action? CurrentViewModelChanged;
        
        private void OnCurrentViewModelChanged() =>
            CurrentViewModelChanged?.Invoke();
    }
}
