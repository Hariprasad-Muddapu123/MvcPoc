namespace BikeBuddy.Repositories
{
    public class RideRepository : Repository<Ride>, IRideRepository
    {
        private readonly ApplicationDbContext _context;

        public RideRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<Ride>> GetRidesByUserIdAsync(string userId)
        {
            return await _context.Rides
                                 .Include(r => r.Bike)
                                 .Where(r => r.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Ride>> GetAllOngoingRidesAsync()
        {
            return await Task.Run(() => _context.Rides
                .Where(ride =>
                    ride.RentalStatus == RentStatus.Ongoing)
                .AsEnumerable()
                .Where(ride =>
                    Convert.ToDateTime(ride.PickupDateTime) <= DateTime.Now &&
                    Convert.ToDateTime(ride.DropoffDateTime) >= DateTime.Now)
                .ToList()
                );
        }

        public async Task<IEnumerable<Ride>> GetRidesByBikeIdAsync(int bikeId)
        {
            return await _context.Rides
                                 .Where(r => r.BikeId == bikeId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Ride>> GetAllRidesAsync()
        {
            return await _context.Rides.Where(ride =>
                    ride.RentalStatus == RentStatus.Ongoing).Include(r => r.Bike).ToListAsync();
        }
    }
}
