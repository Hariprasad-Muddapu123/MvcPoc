using BikeBuddy.Models;

namespace BikeBuddy.Repositories
{
    public interface IBikeRepository
    {
        int GetTotalBikes();
        int GetApprovedBikes();
        int GetRejectedBikes();
        int GetPendingBikes();

        IEnumerable<Bike> GetAll();
        Bike GetById(int bikeId); 
        void Update(Bike bike);

        
    }   
}
