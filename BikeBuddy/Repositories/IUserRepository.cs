using BikeBuddy.Models;

namespace BikeBuddy.Repositories
{
    public interface IUserRepository
    {
        int GetTotalUsers();
        int GetKycUsers();

        IEnumerable<User> GetAllUsers();
        void SaveChanges();

        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByNameAsync(string userName);
        Task<bool> UpdateUserAsync(User user);
        

    }
}
