using CommunityToolkit.Mvvm.Input;
using Kaede.Services;
using Kaede.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Commands
{
    /// <summary>
    /// Provides factory methods for creating navigation commands.
    /// </summary>
    public static class NavigateCommand
    {
        /// <summary>
        /// Creates a new RelayCommand that navigates to the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of ViewModel to navigate to, must inherit from ViewModelBase.</typeparam>
        /// <param name="service">The navigation service that handles the navigation operation.</param>
        /// <returns>A new RelayCommand configured to execute the navigation.</returns>
        public static RelayCommand Create<TViewModel>(NavigationService<TViewModel> service)
            where TViewModel : ViewModelBase => new RelayCommand(service.Navigate);

        /// <summary>
        /// Creates a new RelayCommand that navigates to the specified view model with a predicate
        /// that determines whether the navigation can execute.
        /// </summary>
        /// <typeparam name="TViewModel">The type of ViewModel to navigate to, must inherit from ViewModelBase.</typeparam>
        /// <param name="service">The navigation service that handles the navigation operation.</param>
        /// <param name="pred">A function that returns a boolean value indicating whether navigation can execute.</param>
        /// <returns>A new RelayCommand configured to execute the navigation when the predicate allows it.</returns>
        public static RelayCommand CreateWithPredicate<TViewModel>(
            NavigationService<TViewModel> service,
            Func<bool> pred
        ) where TViewModel : ViewModelBase =>
            new RelayCommand(service.Navigate, pred);
    }
}
