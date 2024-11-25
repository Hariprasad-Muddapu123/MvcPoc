using BikeBuddy.Models;

namespace BikeBuddy.Repositories
{
    public interface IRideRepository
    {
        Task<IEnumerable<Ride>> GetRidesByUserIdAsync(string userId);
    }
}