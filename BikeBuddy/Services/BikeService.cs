namespace BikeBuddy.Services
{
    public class BikeService :IBikeService
    {
        private readonly IBikeRepository _bikeRepository;

        public BikeService(IBikeRepository bikeRepository)
        {
            _bikeRepository = bikeRepository;
        }
        public async Task<IEnumerable<Bike>> GetAllBikes()
        {
            return await _bikeRepository.GetAllBikes();
        }

        public async Task<IEnumerable<Bike>> GetAllBikesByUserIdAsync(string userId)
        {
            return await _bikeRepository.GetAllByUserIdAsync(userId);
        }
    }
}
