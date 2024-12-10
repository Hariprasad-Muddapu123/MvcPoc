namespace BikeBuddy.Services
{
    public interface IRideService
    {
        Task AddRideAsync(Ride ride);

        Task<Ride> GetRideByIdAsync(int rideid);
        Task<IEnumerable<Ride>> GetRidesByUserIdAsync(string userId);

        Task<IEnumerable<Ride>> GetRidesByBikeIdAsync(int bikeId);
        Task UpdateRideAsync(Ride ride);
    }
}