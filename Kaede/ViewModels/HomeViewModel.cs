using CommunityToolkit.Mvvm.Input;
using Kaede.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly UserSession _userSession;
 
        public HomeViewModel(UserSession userSession)
        {
            _userSession = userSession;
            Console.WriteLine("LOGGED IN AS " + _userSession.CurrentUser.Username);
        }
    }
}
