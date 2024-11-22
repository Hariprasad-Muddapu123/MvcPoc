using BikeBuddy.Data;
using BikeBuddy.Models;

namespace BikeBuddy.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
