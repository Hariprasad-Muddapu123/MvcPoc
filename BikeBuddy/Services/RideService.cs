namespace BikeBuddy.Services
{
    public class RideService : IRideService
    {
        private readonly IRideRepository _rideRepository;

        public RideService(IRideRepository rideRepository)
        {
            _rideRepository = rideRepository;
        }
        // Method to get rides by user ID
        public async Task<IEnumerable<Ride>> GetRidesByUserIdAsync(string userId)
        {
            return await _rideRepository.GetRidesByUserIdAsync(userId);
        }
    }
}
