using Kaede.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.DTOs
{
    public class AppointmentDTO
    {
        public Guid? AppointmentId { get; init; }
        public required CustomerDTO CustomerDTO { get; set; }
        public required UserDTO BarberDTO { get; set; }  
        public required ShopItemDTO ShopItemDTO { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required AppointmentStatus Status { get; set; }
    }
}
