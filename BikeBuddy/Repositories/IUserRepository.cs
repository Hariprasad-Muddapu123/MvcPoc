namespace BikeBuddy.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers();
        void SaveChanges();

        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByNameAsync(string userName);
        Task<bool> UpdateUserAsync(User user);
        

    }
}
