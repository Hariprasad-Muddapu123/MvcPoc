namespace BikeBuddy.ViewModels
{
    public class RegisteredBikeViewModel
    {
        public List<string> BikeModels { get; set; }
        public List<string> BikesAddress { get; set; }
        public List<Bike> Bikes {  get; set; }
        public List<String> BikeLocation { get; set; }
        public List<int> UserWishlist { get; set; }
    }
}
