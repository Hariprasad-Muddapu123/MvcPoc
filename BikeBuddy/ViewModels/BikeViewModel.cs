using System.ComponentModel.DataAnnotations;

namespace BikeBuddy.ViewModels
{
    public class BikeViewModel
    {
        public int BikeId { get; set; }

        public string BikeModel { get; set; }

        public string BikeNumber { get; set; }

        public string BikeLocation { get; set; }

        public string BikeAddress { get; set; }

        public string FullAddress { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression("^[6-9][0-9]{9}$", ErrorMessage = "Contact number must be 10 digits and start with 6, 7, 8, or 9.")]
        public string ContactNo {  get; set; }

        public double BikeRentPrice { get; set; }

        public bool Available { get; set; } = true;

        public KycStatus KycStatus { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime AvailableUpto { get; set; }

        public IFormFile BikeImage { get; set; }
        public byte[]? BikeImageBytes { get; set; }

        public IFormFile BikeDocuments { get; set; }

        public byte[]? BikeDocumentsBytes { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //public String? UserId {  get; set; }//Foriegn Key to user
    }
}
