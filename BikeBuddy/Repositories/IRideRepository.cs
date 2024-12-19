namespace BikeBuddy.Repositories
{
    public interface IRideRepository : IRepository<Ride>
    {
        Task<IEnumerable<Ride>> GetRidesByUserIdAsync(string userId);
        Task<IEnumerable<Ride>> GetAllOngoingRidesAsync();

        Task<IEnumerable<Ride>> GetAllRidesAsync();
        Task<IEnumerable<Ride>> GetRidesByBikeIdAsync(int bikeId);
    }
}