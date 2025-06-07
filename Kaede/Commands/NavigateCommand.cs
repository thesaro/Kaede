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
        /// Creates a RelayCommand to navigate to the specified ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type to navigate to, inheriting from ViewModelBase.</typeparam>
        /// <param name="service">The navigation service handling the operation.</param>
        /// <returns>A RelayCommand for navigation.</returns>
        public static RelayCommand Create<TViewModel>(NavigationService<TViewModel> service)
            where TViewModel : ViewModelBase => new RelayCommand(service.Navigate);

        /// <summary>
        /// Creates a RelayCommand with a predicate to control navigation execution.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type to navigate to, inheriting from ViewModelBase.</typeparam>
        /// <param name="service">The navigation service handling the operation.</param>
        /// <param name="pred">A function determining if navigation can execute.</param>
        /// <returns>A RelayCommand with conditional navigation.</returns>
        public static RelayCommand CreateWithPredicate<TViewModel>(
            NavigationService<TViewModel> service,
            Func<bool> pred
        ) where TViewModel : ViewModelBase =>
            new RelayCommand(service.Navigate, pred);
    }
}