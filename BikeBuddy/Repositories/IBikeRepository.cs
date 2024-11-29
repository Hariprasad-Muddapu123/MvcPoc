using BikeBuddy.Models;

namespace BikeBuddy.Repositories
{
    public interface IBikeRepository
    {
        Task<int> GetTotalBikes();
        Task<int> GetApprovedBikes();
        Task<int> GetRejectedBikes();
        Task<int> GetPendingBikes();

        Task<IEnumerable<Bike>> GetAllBikes();
        Task<Bike> GetById(int bikeId); 
        Task Update(Bike bike);
        Task<IEnumerable<Bike>> GetAllByUserIdAsync(string userId);


    }   
}
