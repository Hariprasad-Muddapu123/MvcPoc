namespace BikeBuddy.Repositories
{
    public class RideRepository : Repository<Ride>, IRideRepository
    {
        private readonly ApplicationDbContext _context;

        public RideRepository(ApplicationDbContext context):base(context) 
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
