namespace BikeBuddy.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByNameAsync(string userName);
        Task<bool> UpdateUserAsync(User user);

        Task BlockUserAsync(string userId, bool isBlocked);
        Task<bool> IsUserBlockedAsync(string userId);
    }
}
