namespace BikeBuddy.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(ApplicationDbContext context,UserManager<User> userManager) : base(context) 
        {
            _context = context;
            _userManager = userManager;
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

        public async Task BlockUserAsync(string userId, bool isBlocked)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsBlocked = isBlocked;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<bool> IsUserBlockedAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && user.IsBlocked;
        }
    }
}
