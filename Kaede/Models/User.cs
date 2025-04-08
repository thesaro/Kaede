using Kaede.DTOs;
using Microsoft.EntityFrameworkCore;
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
    public class User : IFromDTO<UserDTO, User>
    {
        #region Properties
        [Key]
        public Guid UserId { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastPasswordChanged { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Username must have min length of 5 and max length of 50.")]
        public required string Username { get; init; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }
        #endregion

        #region Static Methods
        public static User FromDTO(UserDTO dto) => new User
        {
            Username = dto.Username,
            PasswordHash = HashPassword(dto.Password),
            Role = dto.Role,
        };

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
