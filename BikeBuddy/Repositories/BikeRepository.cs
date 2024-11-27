using BikeBuddy.Data;
using BikeBuddy.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeBuddy.Repositories
{
    public class BikeRepository : IBikeRepository
    {
        private readonly ApplicationDbContext _context;

        public BikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalBikes()
        {
            return await _context.Bikes.CountAsync();
        }

        public async Task<int> GetApprovedBikes()
        {
            return await _context.Bikes.CountAsync(b => b.KycStatus == KycStatus.Approved);
        }

        public async Task<int> GetRejectedBikes()
        {
            return await _context.Bikes.CountAsync(b => b.KycStatus == KycStatus.Rejected);
        }

        public async Task<int> GetPendingBikes()
        {
            return await _context.Bikes.CountAsync(b=>b.KycStatus == KycStatus.Pending);
        }
        public async  Task<IEnumerable<Bike>> GetAll()
        {
            return await  _context.Bikes.Include(b => b.User).ToListAsync();
        }

        public async Task<Bike> GetById(int bikeId)
        {
            return await _context.Bikes.Include(b => b.User).FirstOrDefaultAsync(b => b.BikeId == bikeId);
        }

        public async Task Update(Bike bike)
        {
            _context.Bikes.Update(bike);
           await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Bike>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Bikes
                                 .Where(b => b.UserId == userId) // Assuming Bike model has UserId property
                                 .ToListAsync();
        }
    }
}
