using Kaede.DTOs;
using Kaede.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Kaede.Models
{
    public class Appointment : ITryFromDTO<AppointmentDTO, Appointment>
    {
        [Key]
        public Guid AppointmentId { get; set; }

        [Required]
        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }
        public required Customer? Customer { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Guid BarberId { get; set; }
        public required User? Barber { get; set; }

        [Required]
        [ForeignKey(nameof(ShopItem))]
        public Guid ShopItemId { get; set; }
        public required ShopItem? ShopItem { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public static bool TryFromDTO(AppointmentDTO dto, out Appointment? entity)
        {
            if (dto.StartDate >= dto.EndDate)
            {
                entity = null;
                return false;
            }

            entity = new Appointment
            {
                AppointmentId = dto.AppointmentId,
                CustomerId = dto.CustomerId,
                BarberId = dto.BarberId,
                ShopItemId = dto.ShopItemId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Customer = null,
                Barber = null,
                ShopItem = null
            };

            return false;
        }
    }
}


