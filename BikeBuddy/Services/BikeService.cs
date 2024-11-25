using BikeBuddy.Models;
using BikeBuddy.Repositories;

namespace BikeBuddy.Services
{
    public class BikeService :IBikeService
    {
        private readonly IBikeRepository _bikeRepository;

        public BikeService(IBikeRepository bikeRepository)
        {
            _bikeRepository = bikeRepository;
        }

        public async Task<IEnumerable<Bike>> GetAllBikesByUserIdAsync(string userId)
        {
            return await _bikeRepository.GetAllByUserIdAsync(userId);
        }
    }
}
