namespace BikeBuddy.Services
{
    public class RideService : IRideService
    {
        private readonly IRideRepository _rideRepository;

        public RideService(IRideRepository rideRepository)
        {
            _rideRepository = rideRepository;
        }
        public async Task<IEnumerable<Ride>> GetRidesByUserIdAsync(string userId)
        {
            return await _rideRepository.GetRidesByUserIdAsync(userId);
        }

        public async Task<Ride> GetRideByIdAsync(int rideid)
        {
            return await _rideRepository.GetByIdAsync(rideid);
        }

        public async Task AddRideAsync(Ride ride)
        {
            await _rideRepository.AddAsync(ride);
        }

        public async Task UpdateRideAsync(Ride ride)
        {
            await _rideRepository.UpdateAsync(ride);
        }
        public async Task<IEnumerable<Ride>> GetRidesByBikeIdAsync(int bikeId)
        {
            return await _rideRepository.GetRidesByBikeIdAsync(bikeId);
        }
    }
}
