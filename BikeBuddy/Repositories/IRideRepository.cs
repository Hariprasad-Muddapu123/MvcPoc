namespace BikeBuddy.Repositories
{
    public interface IRideRepository : IRepository<Ride>
    {
        Task<IEnumerable<Ride>> GetRidesByUserIdAsync(string userId);
        Task<IEnumerable<Ride>> GetAllOngoingRidesAsync();
    }
}