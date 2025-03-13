using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.ViewModels
{
    public class UserLoginViewModel : ViewModelBase
    {
        private string _username = "";
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
    }
}
