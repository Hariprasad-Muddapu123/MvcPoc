namespace BikeBuddy.Services
{
    public interface IRideService
    {
        Task<IEnumerable<Ride>> GetRidesByUserIdAsync(string userId);
    }
}