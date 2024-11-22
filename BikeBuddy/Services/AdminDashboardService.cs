using BikeBuddy.Models;
using BikeBuddy.Repositories;
using BikeBuddy.ViewModels;

namespace BikeBuddy.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IBikeRepository _bikeRepository;
        private readonly IUserRepository _userRepository;

        public AdminDashboardService(IBikeRepository bikeRepository, IUserRepository userRepository)
        {
            _bikeRepository = bikeRepository;
            _userRepository = userRepository;
        }

        public AdminDashboardViewModel GetDashboardData()
        {
            var totalBikes = _bikeRepository.GetTotalBikes();
            var approvedBikes = _bikeRepository.GetApprovedBikes();
            var pendingBikes = _bikeRepository.GetPendingBikes();
            var rejectedBikes = _bikeRepository.GetRejectedBikes();

            var totalUsers = _userRepository.GetTotalUsers();
            var kycUsers = _userRepository.GetKycUsers();
            var nonKycUsers = totalUsers - kycUsers;

            return new AdminDashboardViewModel
            {
                TotalBikes = totalBikes,
                ApprovedBikes = approvedBikes,
                RejectedBikes = rejectedBikes,
                PendingBikes = pendingBikes,
                TotalUsers = totalUsers,
                KycUsers = kycUsers,
                NonKycUsers = nonKycUsers
            };
        }

            public IEnumerable<User> GetAllUsers()
            {
                return _userRepository.GetAllUsers();
            }
        
    }
}
