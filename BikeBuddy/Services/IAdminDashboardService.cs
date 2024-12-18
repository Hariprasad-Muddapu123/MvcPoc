namespace BikeBuddy.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetDashboardData();

        Task<IEnumerable<User>> GetAllUsers();
        Task<IEnumerable<Ride>> GetAllRides();
        Task<User> GetUserById(Guid id);

        Task<bool> UpdateKycStatus(String userId, bool approve, string adminName);

        Task<IEnumerable<Bike>> GetAllBikes();
        Task<bool> UpdateBikeStatus(int bikeId, bool isApproved,string adminName);
        Task<string> GetUserEmailAsync(string userId);
        Task LogoutAdminAsync();

        Task<Bike> GetBikeByIdAsync(int bikeId);
    }
}
