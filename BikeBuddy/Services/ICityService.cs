namespace BikeBuddy.Services
{
    public interface ICityService
    {
        Task<List<City>> GetAllCitiesAsync();
    }
}
