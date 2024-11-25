using BikeBuddy.Models;
using BikeBuddy.ViewModels;

namespace BikeBuddy.Services
{
    public interface IUserService
    {
        Task<User> GetCurrentUserAsync(string userName);
        Task<ProfileViewModel> GetUserProfileAsync(string userId);
        Task<bool> UpdateUserProfileAsync(string userId, ProfileViewModel model);

    }
}
