using Microsoft.AspNetCore.Http.Connections;

namespace BikeBuddy.Models
{
    public class Bike
    {
        public int BikeId { get; set; }

        public string BikeModel { get; set; }

        public string BikeNumber { get; set; }

        public string BikeLocation { get; set; }

        public string BikeAddress { get; set; }

        public KycStatus KycStatus { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime AvailableUpto{ get; set; }

        public byte[] BikeImageBytes { get; set; }

        public byte[] BikeDocumentsBytes { get; set; }

        // Foreign key to User (IdentityUser)
        public string UserId { get; set; }

        // Navigation property to User (One-to-Many)
        public User User { get; set; }
    }
}
