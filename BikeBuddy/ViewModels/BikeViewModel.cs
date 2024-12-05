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

        //public String? UserId {  get; set; }//Foriegn Key to user
    }
}
