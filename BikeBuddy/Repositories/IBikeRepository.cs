namespace BikeBuddy.Repositories
{
    public interface IBikeRepository
    {
        Task<IEnumerable<Bike>> GetAllBikesAsync();
        Task<Bike> GetBikeByIdAsync(int bikeId); 
        Task UpdateBikeAsync(Bike bike);
        Task<IEnumerable<Bike>> GetAllByUserIdAsync(string userId);
        Task AddBikeAsync(Bike bike);

    }   
}
