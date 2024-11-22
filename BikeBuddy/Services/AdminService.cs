namespace BikeBuddy.Services
{
    using BikeBuddy.Data;
    using Microsoft.EntityFrameworkCore;

    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<int> GetTotalBikesAsync() => await _context.Bikes.CountAsync();

        //public async Task<int> GetApprovedBikesAsync() => await _context.Bikes.CountAsync(b => b.IsApproved);

        //public async Task<int> GetRejectedBikesAsync() =>
        //    await _context.Bikes.CountAsync(b => !b.);

        //public async Task<int> GetTotalUsersAsync() => await _context.Users.CountAsync();

        //public async Task<int> GetKycUsersAsync() =>
        //    await _context.Users.CountAsync(u => u.IsKycApproved);

        //public async Task<int> GetNonKycUsersAsync() =>
        //    await _context.Users.CountAsync(u => !u.IsKycApproved);
    }

}
