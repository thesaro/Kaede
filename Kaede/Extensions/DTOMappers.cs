using Kaede.DTOs;
using Kaede.Models;
using Serilog;
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
                Log.Fatal("Failed to decode username for {UsernameHash}. Possible database corruption.", user.UsernameHash);
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

        public static ShopItemDTO MapToDTO(this ShopItem item)
            => new ShopItemDTO()
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Duration = item.Duration
            };
    }
}
