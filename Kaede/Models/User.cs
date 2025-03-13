using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters long.")]
        public required string Username { get; init; }

        [Required]
        public required string PasswordHash { get; set; }

        public UserRole Role { get; set; }
    }
    public enum UserRole
    {
        Admin,
        Barber
    }
}
