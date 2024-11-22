using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BikeBuddy.Models
{
    public class Ride
    {
        [Key]
        public int RideId { get; set; }

        [Required]
        public String UserId { get; set; }

        [Required]
        public int BikeId { get; set; }

        [Required]
        public String PickupDateTime { get; set; }

        [Required]
        public String DropoffDateTime { get; set; }

        [Required]
        [MaxLength(50)]
        public RentStatus RentalStatus { get; set; } // E.g., "Ongoing", "Completed", "Cancelled"

        [Required]
        public DateTime RideRegisteredDate { get; set; }

        [Required]
        public String RentedHours { get; set; }

        [Required]
        
        public String Amount { get; set; }

        [Required]
        public String Gst { get; set; }

        [Required]
        public String TotalAmount { get; set; }

        // Navigation properties
        public User User { get; set; } // Foreign Key Relationship with User
        public Bike Bike { get; set; } // Foreign Key Relationship with Bike

        public Payment Payment { get; set; }  // Navigation property

    }
}
