namespace BikeBuddy.Services
{
    public class CityService
    {
        //private readonly ICityRepository _cityRepository;

        //public CityService(ICityRepository cityRepository)
        //{
        //    _cityRepository = cityRepository;
        //}

        //public async Task<List<City>> GetAllCitiesAsync()
        //{
        //    return await _cityRepository.GetAllCitiesAsync();
        //}
        private readonly ApplicationDbContext _context;

        public CityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            return await _context.Cities.ToListAsync();
        }
    }

}
