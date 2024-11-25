using BikeBuddy.Data;
using BikeBuddy.Models;
using BikeBuddy.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BikeBuddy.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(ApplicationDbContext context,UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int GetTotalUsers()
        {
            return _context.Users.Count();
        }

        public int GetKycUsers()
        {
            return _context.Users.Count(u => u.KycStatus == KycStatus.Approved);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<User> GetUserByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        

    }
}
