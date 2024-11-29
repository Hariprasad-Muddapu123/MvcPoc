using BikeBuddy.Data;
using BikeBuddy.Models;
using BikeBuddy.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task<int> GetTotalUsers()
        {
            return await  _context.Users.CountAsync();
        }

        public async Task<int> GetKycUsers()
        {
            return await  _context.Users.CountAsync(u => u.KycStatus == KycStatus.Approved);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await  _context.Users.ToListAsync();
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
