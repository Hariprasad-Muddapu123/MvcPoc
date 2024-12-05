namespace BikeBuddy.Repositories
{
    public class BikeRepository : Repository<Bike>, IBikeRepository
    {
        private readonly ApplicationDbContext _context;

        public BikeRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<Bike>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Bikes
                                 .Where(b => b.UserId == userId)
                                 .ToListAsync();
        }
    }
}
