namespace BikeBuddy.Services
{
    public interface IRideService
    {
        Task AddRideAsync(Ride ride);
        Task<IEnumerable<Ride>> GetRidesByUserIdAsync(string userId);
    }
}