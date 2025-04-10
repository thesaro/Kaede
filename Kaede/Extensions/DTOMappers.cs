using Kaede.DTOs;
using Kaede.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Extensions
{
    public static class DTOMappers
    {
        public static UserDTO MapToDTO(this User user)
        {
            bool unameDecodeRes = User.TryDecodeUsername(user.UsernameHash, out string? uname);

            if (!unameDecodeRes)
            {
                throw new InvalidOperationException("Database table Users is corrupted!");
            }
            
            return new UserDTO
            {
                Username = uname!,
                Role = user.Role,
                CreationDate = user.CreationDate,
                LastPasswordChanged = user.LastPasswordChanged
            };
        }

    }
}
