using BikeBuddy.Models;
using BikeBuddy.Repositories;
using BikeBuddy.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BikeBuddy.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IBikeRepository _bikeRepository;
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;

        public AdminDashboardService(IBikeRepository bikeRepository, IUserRepository userRepository, SignInManager<User> signInManager)
        {
            _bikeRepository = bikeRepository;
            _userRepository = userRepository;
            _signInManager = signInManager;

        }

        public async Task<string> GetUserEmailAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user?.Email;
        }

        public async Task<AdminDashboardViewModel> GetDashboardData()
        {
            var totalBikes = await  _bikeRepository.GetTotalBikes();
            var approvedBikes =await  _bikeRepository.GetApprovedBikes();
            var pendingBikes = await  _bikeRepository.GetPendingBikes();
            var rejectedBikes = await _bikeRepository.GetRejectedBikes();

            var totalUsers = await  _userRepository.GetTotalUsers();
            var kycUsers = await _userRepository.GetKycUsers();
            var nonKycUsers = totalUsers - kycUsers;

            return new AdminDashboardViewModel
            {
                TotalBikes =totalBikes,
                ApprovedBikes = approvedBikes,
                RejectedBikes = rejectedBikes,
                PendingBikes = pendingBikes,
                TotalUsers = totalUsers,
                KycUsers = kycUsers,
                NonKycUsers = nonKycUsers
            };
        }

        public async  Task<IEnumerable<User>> GetAllUsers()
        {
            return  await _userRepository.GetAllUsers();
        }

        public async  Task<User> GetUserById(Guid id)
        {
            var user = await _userRepository.GetAllUsers();
            return  user.FirstOrDefault(u => Guid.Parse(u.Id) == id);
        }

        
        public async  Task<bool> UpdateKycStatus(String userId, bool approve)
        {
            var users = await _userRepository.GetAllUsers();
            var user = users.FirstOrDefault(u => u.Id == userId.ToString());
            if (user == null) return false;

            user.KycStatus = approve ? KycStatus.Approved : KycStatus.Rejected;
            _userRepository.SaveChanges();
            return true;
        }

        public async  Task<IEnumerable<Bike>> GetAllBikes()
        {
            return await _bikeRepository.GetAll();
        }

        public  async Task<bool> UpdateBikeStatus(int bikeId, bool approve)
        {
            var bike =  await _bikeRepository.GetById(bikeId);
            if (bike == null) return false;

            bike.KycStatus = approve ? KycStatus.Approved : KycStatus.Rejected;
            await  _bikeRepository.Update(bike);
            return true;
        }
        public async Task LogoutAdminAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async  Task<Bike> GetBikeByIdAsync(int bikeId)
        {
            return await  _bikeRepository.GetById(bikeId);
        }
    }
}
