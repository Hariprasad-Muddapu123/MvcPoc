namespace BikeBuddy.ViewModels
{
    public class RegisterBikeViewModel
    {
        public BikeViewModel NewBike { get; set; }
        public IEnumerable<Bike>? UserBikes { get; set; }
    }
}
