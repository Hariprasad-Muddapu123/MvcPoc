﻿namespace BikeBuddy.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IBikeRepository _bikeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRideRepository _rideRepository;
        private readonly SignInManager<User> _signInManager;

        public AdminDashboardService(IBikeRepository bikeRepository, IUserRepository userRepository, IRideRepository rideRepository, SignInManager<User> signInManager)
        {
            _bikeRepository = bikeRepository;
            _userRepository = userRepository;
            _signInManager = signInManager;
            _rideRepository = rideRepository;
        }

        public async Task<string> GetUserEmailAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user?.Email;
        }

        public async Task<AdminDashboardViewModel> GetDashboardData()
        {
            var Bikes = await _bikeRepository.GetAllAsync();
            var Users=await _userRepository.GetAllAsync();
            return new AdminDashboardViewModel
            {
                Bikes = Bikes.ToList(),
                Users = Users.ToList(),
                TotalBikes = Bikes.Count(),
                ApprovedBikes = Bikes.Where(b => b.KycStatus == KycStatus.Approved).Count(),
                RejectedBikes = Bikes.Where(b => b.KycStatus == KycStatus.Rejected).Count(),
                PendingBikes = Bikes.Where(b => b.KycStatus == KycStatus.Pending).Count(),
                TotalUsers = Users.Count(),
                KycUsers = Users.Where(b => b.KycStatus == KycStatus.Approved).Count(),
                NonKycUsers = Users.Where(b => b.KycStatus == KycStatus.Rejected || b.KycStatus == KycStatus.Pending).Count()
            };
        }

        public async  Task<IEnumerable<User>> GetAllUsers()
        {
            return  await _userRepository.GetAllAsync();
        }

        public async  Task<User> GetUserById(Guid id)
        {
            var user = await _userRepository.GetAllAsync();
            return  user.FirstOrDefault(u => Guid.Parse(u.Id) == id);
        }

        
        public async  Task<bool> UpdateKycStatus(String userId, bool approve, string adminName)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Id == userId.ToString());
            if (user == null) return false;

            user.KycStatus = approve ? KycStatus.Approved : KycStatus.Rejected;
            user.ReviewedByAdmin = adminName;
            user.ApprovalOrRejectionDate = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async  Task<IEnumerable<Bike>> GetAllBikes()
        {
            return await _bikeRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Ride>> GetAllRides()
        {
            return await _rideRepository.GetAllAsync();
        }


        public  async Task<bool> UpdateBikeStatus(int bikeId, bool approve,string adminName)
        {
            var bike =  await _bikeRepository.GetByIdAsync(bikeId);
            if (bike == null) return false;

            bike.KycStatus = approve ? KycStatus.Approved : KycStatus.Rejected;
            bike.ReviewedByAdmin = adminName;
            bike.ApprovalOrRejectionDate=DateTime.Now;
            await  _bikeRepository.UpdateAsync(bike);
            return true;
        }
        public async Task LogoutAdminAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async  Task<Bike> GetBikeByIdAsync(int bikeId)
        {
            return await  _bikeRepository.GetByIdAsync(bikeId);
        }
    }
}
