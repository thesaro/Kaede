using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.DTOs
{
    public class CustomerDTO
    {
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; } = string.Empty;
    }
}
