namespace BikeBuddy.Repositories
{
    public interface ICityRepository
    {
        Task<List<City>> GetAllCitiesAsync();
    }
}
