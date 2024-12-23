namespace BikeBuddy.Repositories
{
    public class WishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddToWishlistAsync(string userId, int bikeId)
        {
            
                var wishlist = new Wishlist
                {
                    UserId = userId,
                    BikeId = bikeId
                };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
        }

        public async Task RemoveFromWishlistAsync(string userId, int bikeId)
        {
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.BikeId == bikeId);
            if (wishlist != null)
            {
                _context.Wishlists.Remove(wishlist);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Bike>> GetWishlistAsync(string userId)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Select(w => w.Bike)
                .ToListAsync();
        }

    }

}
