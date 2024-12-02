namespace BikeBuddy.Services
{
    public interface IBikeService
    {
        Task<IEnumerable<Bike>> GetAllBikesByUserIdAsync(string userId);
        Task<IEnumerable<Bike>> GetAllBikes();
    }
}
