using BikeBuddy.Models;

namespace BikeBuddy.Repositories
{
    public interface IUserRepository
    {
        int GetTotalUsers();
        int GetKycUsers();

        IEnumerable<User> GetAllUsers();
        void SaveChanges();
    }
}
