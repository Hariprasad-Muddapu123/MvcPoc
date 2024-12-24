using BikeBuddy.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BikeBuddy.Services
{
    public class WishlistService 
    {
        private readonly WishlistRepository _wishlistRepository;

        public WishlistService(WishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<bool> ToggleWishlistAsync(string userId, int bikeId)
        {
            var wishlistItem = await _wishlistRepository.GetWishlistAsync(userId);

            if (wishlistItem.Any())
            {
                await _wishlistRepository.RemoveFromWishlistAsync(userId, bikeId);
                return false;
            }
            else
            {
                await _wishlistRepository.AddToWishlistAsync(userId, bikeId);
                return true;
            }
        }
        public async Task RemoveFromWishlistAsync(string userId, int bikeId)
        {
            await _wishlistRepository.RemoveFromWishlistAsync(userId, bikeId);
        }

        public async Task<List<Bike>> GetWishlistAsync(string userId)
        {
            return await _wishlistRepository.GetWishlistAsync(userId);
        }
        

    }

}
