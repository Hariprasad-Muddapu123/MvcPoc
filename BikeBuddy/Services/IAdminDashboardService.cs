namespace BikeBuddy.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetDashboardData();

        Task<IEnumerable<User>> GetAllUsers();

        Task<User> GetUserById(Guid id);

        Task<bool> UpdateKycStatus(String userId, bool approve);

        Task<IEnumerable<Bike>> GetAllBikes(); // Add this line
        Task<bool> UpdateBikeStatus(int bikeId, bool isApproved);
        Task<string> GetUserEmailAsync(string userId);
        Task LogoutAdminAsync();

        Task<Bike> GetBikeByIdAsync(int bikeId);
    }
}
