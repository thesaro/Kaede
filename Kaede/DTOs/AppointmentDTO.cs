using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.DTOs
{
    public class AppointmentDTO
    {
        public Guid AppointmentId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid BarberId { get; set; }
        public string BarberName { get; set; } = string.Empty;
        public Guid ShopItemId { get; set; }
        public string ShopItemName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
