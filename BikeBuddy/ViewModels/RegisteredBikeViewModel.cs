using BikeBuddy.Models;

namespace BikeBuddy.ViewModels
{
    public class RegisteredBikeViewModel
    {
        public List<string> BikeModels { get; set; }
        public List<string> BikesAddress { get; set; }

        public List<Bike> Bikes {  get; set; }
    }
}
