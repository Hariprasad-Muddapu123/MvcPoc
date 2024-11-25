using BikeBuddy.Data;
using BikeBuddy.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeBuddy.Repositories
{
    public class RideRepository : IRideRepository
    {
        private readonly ApplicationDbContext _context;

        public RideRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ride>> GetRidesByUserIdAsync(string userId)
        {
            return await _context.Rides
                                 .Where(r => r.UserId == userId)
                                 .ToListAsync();
        }
    }
}
