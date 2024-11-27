using BikeBuddy.Models;

namespace BikeBuddy.Repositories
{
    public interface IUserRepository
    {
        Task<int> GetTotalUsers();
        Task<int> GetKycUsers();

        Task<IEnumerable<User>> GetAllUsers();
        void SaveChanges();

        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByNameAsync(string userName);
        Task<bool> UpdateUserAsync(User user);
        

    }
}
