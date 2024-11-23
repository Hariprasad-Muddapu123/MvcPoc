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
        public IEnumerable<Bike> GetAll()
        {
            return _context.Bikes.Include(b => b.User).ToList();
        }

        public Bike GetById(int bikeId)
        {
            return _context.Bikes.Include(b => b.User).FirstOrDefault(b => b.BikeId == bikeId);
        }

        public void Update(Bike bike)
        {
            _context.Bikes.Update(bike);
            _context.SaveChanges();
        }
    }
}
