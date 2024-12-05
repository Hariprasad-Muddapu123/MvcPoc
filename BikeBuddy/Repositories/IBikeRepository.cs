namespace BikeBuddy.Repositories
{
    public interface IBikeRepository : IRepository<Bike>
    {
        Task<IEnumerable<Bike>> GetAllByUserIdAsync(string userId);
    }   
}
