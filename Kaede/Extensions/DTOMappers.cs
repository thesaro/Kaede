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
        public static UserDTO MapToDTO(this User user) =>
            new UserDTO
            {
                Username = user.Username,
                Role = user.Role,
                CreationDate = user.CreationDate,
                LastPasswordChanged = user.LastPasswordChanged,
            };
    }
}
