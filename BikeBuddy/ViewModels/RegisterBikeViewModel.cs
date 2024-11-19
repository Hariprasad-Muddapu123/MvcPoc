using BikeBuddy.Models;

namespace BikeBuddy.ViewModels
{
    public class RegisterBikeViewModel
    {
        public BikeViewModel NewBike { get; set; }

        // List of already registered bikes
        public IEnumerable<Bike>? UserBikes { get; set; }
    }
}
