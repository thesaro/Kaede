using Kaede.Stores;
using Kaede.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Services
{
    /// <summary>
    /// Provides navigation functionality for switching between ViewModels of type <typeparamref name="TViewModel"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel to navigate to. Must inherit from <see cref="ViewModelBase"/>.</typeparam>
    public class NavigationService<TViewModel> where TViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<TViewModel> _createViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService{TViewModel}"/> class.
        /// </summary>
        /// <param name="navigationStore">The navigation store that holds the current ViewModel.</param>
        /// <param name="createViewModel">A factory function to create an instance of the target ViewModel.</param>
        public NavigationService(NavigationStore navigationStore, Func<TViewModel> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        /// <summary>
        /// Navigates to the ViewModel of type <typeparamref name="TViewModel"/> by creating a new instance
        /// and setting it as the current ViewModel in the <see cref="NavigationStore"/>.
        /// </summary>
        public void Navigate() => _navigationStore.CurrentViewModel = _createViewModel();
    }
}
