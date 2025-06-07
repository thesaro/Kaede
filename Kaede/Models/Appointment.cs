using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaede.Models
{
    public class Appointment : IValidatableObject
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

        [Required]
        public AppointmentStatus Status { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate < DateTime.Now)
            {
                yield return new ValidationResult(
                    "Start date cannot be in the past.",
                    new[] { nameof(StartDate) });
            }

            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "End date must be after start date.",
                    new[] { nameof(EndDate) });
            }
        }
    }

    public enum AppointmentStatus
    {
        Pending,
        Canceled,
        Done
    }
}
