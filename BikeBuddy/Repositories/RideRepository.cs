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
        public async Task<List<Ride>> GetAllRidesAsync()
        {
            return await _context.Rides.ToListAsync();
        }

        public async Task UpdateAsync(Ride ride)
        {
            _context.Rides.Update(ride);
            await _context.SaveChangesAsync();
        }
    }
}
