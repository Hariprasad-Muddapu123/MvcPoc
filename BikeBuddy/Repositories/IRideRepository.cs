namespace BikeBuddy.Repositories
{
    public interface IRideRepository
    {
        Task<IEnumerable<Ride>> GetRidesByUserIdAsync(string userId);
        Task<List<Ride>> GetAllRidesAsync();
        Task UpdateAsync(Ride ride);
    }
}