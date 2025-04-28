using Kaede.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Kaede.Models
{
    public class ShopItem : IFromDTO<ShopItemDTO, ShopItem>
    {
        [Key]
        public Guid ShopItemId { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public static ShopItem FromDTO(ShopItemDTO dto) =>
            new ShopItem { 
                Name = dto.Name, 
                Description = dto.Description, 
                Price = dto.Price,
                Duration = dto.Duration 
            };
    }
}
