using BikeBuddy.Models;
using Microsoft.AspNetCore.Identity;

namespace BikeBuddy.ViewModels
{
    public class BikeViewModel
    {
        public int BikeId { get; set; }

        public String BikeModel { get; set; }

        public String BikeNumber { get; set; }

        public String BikeLocation { get; set; }

        public String BikeAddress { get; set; }

        public KycStatus KycStatus { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime AvailableUpto { get; set; }

        public IFormFile BikeImage { get; set; }
        public byte[]? BikeImageBytes { get; set; }

        public IFormFile BikeDocuments { get; set; }

        public byte[]? BikeDocumentsBytes { get; set; }

        //public String? UserId {  get; set; }//Foriegn Key to user
    }
}
