using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Models
{
    public class Appointment
    {
        [Key]
        public Guid AppointmentId { get; set; }

        [Required]
        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }
        public required Customer Customer { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Guid BarberId { get; set; }
        public required User Barber { get; set; }

        [Required]
        [ForeignKey(nameof(ShopItem))]
        public Guid ShopItemId { get; set; }
        public required ShopItem ShopItem { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
