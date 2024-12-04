﻿namespace BikeBuddy.Services
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
            return await _bikeRepository.GetAllBikesAsync();
        }

        public async Task<IEnumerable<Bike>> GetAllBikesByUserIdAsync(string userId)
        {
            return await _bikeRepository.GetAllByUserIdAsync(userId);
        }

        public async Task<Bike?> GetBikeByIdAsync(int bikeId)
        {
            return await _bikeRepository.GetBikeByIdAsync(bikeId);
        }
        public async Task RegisterBikeAsync(Bike bike)
        {
            await _bikeRepository.AddBikeAsync(bike);
        }

        public async Task RemoveBikeAsync(int bikeId)
        {
            var bike = await _bikeRepository.GetBikeByIdAsync(bikeId);
            if (bike != null)
            {
                bike.IsRemoved = true;
                bike.RemovedDate = DateTime.UtcNow;
                await _bikeRepository.UpdateBikeAsync(bike);
            }
        }
        public async Task UpdateBikeAsync(Bike bike)
        {
            if (bike == null)
            {
                throw new ArgumentNullException(nameof(bike), "Bike cannot be null.");
            }

            if (string.IsNullOrEmpty(bike.BikeModel) || string.IsNullOrEmpty(bike.BikeAddress))
            {
                throw new ArgumentException("Bike model and address must not be empty.");
            }
            await _bikeRepository.UpdateBikeAsync(bike);
        }
    }
}
