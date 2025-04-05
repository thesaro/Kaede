using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kaede.Models;

namespace Kaede.Services.UsersService
{
    public interface IUserService
    {
        Task CreateUser(User user);
        Task<User?> GetUser(string username);

        Task RemoveUser(User user);
        Task ChangePassword(User user, string newPassword);
        Task<List<User>> GetBarbers();
        Task<bool> HasAdmin();
    }
}
