using Kaede.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Models
{
    public class User : ITryFromDTO<UserDTO, User>
    {
        #region Properties
        [Key]
        public Guid UserId { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastPasswordChanged { get; set; }

        [Required]
        public required string UsernameHash { get; init; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        #endregion

        #region Static Methods
        public static bool TryFromDTO(UserDTO dto, out User? user)
        {
            if (TryEncodeUsername(dto.Username, out string? uHash) &&
                dto.Password.Length >= 6 && dto.Password.Length <= 50)
            {
                user = new User
                {
                    // visual studio can't realize uHash is not null here lol
                    UsernameHash = uHash!,
                    PasswordHash = HashPassword(dto.Password),
                    Role = dto.Role
                };
                return true;
            }
            else
            {
                user = null;
                return false;
            }
        }
  
        public static bool TryEncodeUsername(string username, out string? encodedUsername)
        {
            if (username.Length < 5 || username.Length > 50)
            {
                encodedUsername = null;
                return false;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(username);
            encodedUsername = Convert.ToBase64String(bytes);
            return true;

        }

        public static bool TryDecodeUsername(string encodedUsername, out string? decodedUsername)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(encodedUsername);
                decodedUsername = Encoding.UTF8.GetString(bytes);
                return true;
            }
            catch (FormatException)
            {
                decodedUsername = null;
                return false;
            }

        }

        public static string HashPassword(string password)
        {
            using var sha256Hasher = SHA256.Create();
            byte[] hashedBytes = sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashedBytes)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        public static void OnDataSaving(IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry>? entityEntries)
        {
            if (entityEntries is null) return;

            foreach (var userAddedEntry in entityEntries.Where(e => e.Entity is User && e.State == EntityState.Added))
            {
                var u = (User)userAddedEntry.Entity;
                u.CreationDate = u.LastPasswordChanged = DateTime.Now;
            }
           
            foreach (var userModifiedEntry in entityEntries.Where(e => e.Entity is User && e.State == EntityState.Modified))
            {
                var u = (User)userModifiedEntry.Entity;
                u.LastPasswordChanged = DateTime.Now;
            }
        }
        #endregion

    }
    public enum UserRole
    {
        Admin,
        Barber
    }
}
