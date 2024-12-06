namespace BikeBuddy.Services
{
    public interface IUserService
    {
        Task<User> GetCurrentUserAsync(string userName);
        Task<ProfileViewModel> GetUserProfileAsync(string userId);

        Task<User> GetUserByIdAsync(String userId);
        Task<bool> UpdateUserProfileAsync(string userId, ProfileViewModel model);

        Task BlockUserAsync(string userId, bool isBlocked);
        Task<bool> IsUserBlockedAsync(string userId);
    }
}
