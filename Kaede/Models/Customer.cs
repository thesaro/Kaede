using Kaede.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Models
{
    public class Customer : IFromDTO<CustomerDTO, Customer>
    {
        [Key]
        public Guid CustomerId { get; set; }

        [Required]
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; } = string.Empty;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public static Customer FromDTO(CustomerDTO dto)
            => new Customer
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
            };
    }
}
