using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.DTOs
{
    public class AppointmentDTO
    {
        public CustomerDTO CustomerDTO { get; set; }
        public UserDTO BarberDTO { get; set; }  
        public ShopItemDTO ShopItemDTO { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
