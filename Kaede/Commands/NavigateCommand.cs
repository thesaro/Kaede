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
    public static class NavigateCommand
    {
        public static RelayCommand Create<TViewModel>(NavigationService<TViewModel> _service)
            where TViewModel : ViewModelBase => new RelayCommand(_service.Navigate);
    }
}
