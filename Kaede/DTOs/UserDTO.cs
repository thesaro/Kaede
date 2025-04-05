using Kaede.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.DTOs
{
    public class UserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreationDate { get; set; } 
        public DateTime LastPasswordChanged { get; set; }
    }
}
