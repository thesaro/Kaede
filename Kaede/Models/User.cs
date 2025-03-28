using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Username must have min length of 5 and max length of 50.")]
        public required string Username { get; init; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public static string HashPassword(string password)
        {
            using var sha256Hasher = SHA256.Create();
            byte[] hashedBytes = sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashedBytes)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }
    public enum UserRole
    {
        Admin,
        Barber
    }
}
