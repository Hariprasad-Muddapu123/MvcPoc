namespace BikeBuddy.ViewModels
{
    public class AdminOverviewViewModel
    {
        public User User { get; set; }
        public IEnumerable<Bike> Bikes { get; set; }

        public IEnumerable<Ride> Rides { get; set; }    
    }
}
