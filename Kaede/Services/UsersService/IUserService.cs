using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaede.DTOs;


namespace Kaede.Services.UsersService
{
    public interface IUserService
    {
        Task CreateUser(UserDTO userDTO);
        Task<UserDTO?> GetUser(string username);
        Task<bool> ValidatePassword(string username, string password);
        Task RemoveUser(string username);
        Task ChangePassword(string username, string newPassword);
        Task<List<UserDTO>> GetAllBarbers();
        Task<bool> HasAdmin();
    }
}
