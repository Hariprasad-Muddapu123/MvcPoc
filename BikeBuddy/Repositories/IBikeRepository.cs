namespace BikeBuddy.Repositories
{
    public interface IBikeRepository
    {
        Task<IEnumerable<Bike>> GetAllBikes();
        Task<Bike> GetById(int bikeId); 
        Task Update(Bike bike);
        Task<IEnumerable<Bike>> GetAllByUserIdAsync(string userId);


    }   
}
