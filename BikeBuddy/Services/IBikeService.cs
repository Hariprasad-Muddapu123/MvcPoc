namespace BikeBuddy.Services
{
    public interface IBikeService
    {
        Task<IEnumerable<Bike>> GetAllBikesByUserIdAsync(string userId);
        Task<IEnumerable<Bike>> GetAllBikes();

        Task<Bike?> GetBikeByIdAsync(int bikeId);
        Task RegisterBikeAsync(Bike bike);
        Task UpdateBikeAsync(Bike bike);
        Task RemoveBikeAsync(int bikeId);
    }
}
