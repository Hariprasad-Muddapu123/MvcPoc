using BikeBuddy.Data;
using BikeBuddy.Models;

namespace BikeBuddy.Repositories
{
    public class BikeRepository : IBikeRepository
    {
        private readonly ApplicationDbContext _context;

        public BikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public int GetTotalBikes()
        {
            return _context.Bikes.Count();
        }

        public int GetApprovedBikes()
        {
            return _context.Bikes.Count(b => b.KycStatus == KycStatus.Approved);
        }

        public int GetRejectedBikes()
        {
            return _context.Bikes.Count(b => b.KycStatus == KycStatus.Rejected);
        }

        public int GetPendingBikes()
        {
            return _context.Bikes.Count(b=>b.KycStatus == KycStatus.Pending);
        }
    }
}
