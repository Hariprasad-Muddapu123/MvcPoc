namespace BikeBuddy.Repositories
{
    public class BikeRepository : IBikeRepository
    {
        private readonly ApplicationDbContext _context;

        public BikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async  Task<IEnumerable<Bike>> GetAllBikesAsync()
        {
            return await  _context.Bikes.Include(b => b.User).ToListAsync();
        }

        public async Task<Bike> GetBikeByIdAsync(int bikeId)
        {
            return await _context.Bikes.Include(b => b.User).FirstOrDefaultAsync(b => b.BikeId == bikeId);
        }

        public async Task AddBikeAsync(Bike bike)
        {
            _context.Bikes.Add(bike);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBikeAsync(Bike bike)
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
