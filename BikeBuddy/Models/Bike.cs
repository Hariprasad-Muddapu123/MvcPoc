using Microsoft.AspNetCore.Http.Connections;
using System.ComponentModel.DataAnnotations;

namespace BikeBuddy.Models
{
    public class Bike
    {
        public int BikeId { get; set; }

        public string BikeModel { get; set; }

        public string BikeNumber { get; set; }

        public string BikeLocation { get; set; }

        public string BikeAddress { get; set; }

        public string FullAddress {  get; set; }

        public string ContactNo { get; set; }

        public double BikeRentPrice { get; set; }

        public bool Available { get; set; } 

        

        public KycStatus KycStatus { get; set; }


        public DateTime RegistrationDate { get; set; }

        public DateTime AvailableUpto{ get; set; }

        public byte[] BikeImageBytes { get; set; }

        public byte[] BikeDocumentsBytes { get; set; }

        // Foreign key to User (IdentityUser)
        public string UserId { get; set; }

        public DateTime? RemovedDate { get; set; }
        public bool IsRemoved { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string ReviewedByAdmin { get; set; }
        public DateTime? ApprovalOrRejectionDate { get; set; }

        // Navigation property to User (One-to-Many)
        public User User { get; set; }

        public ICollection<Ride> Rides { get; set; }

        public ICollection<Payment> Payments { get; set; } 

    }
}
