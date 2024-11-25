﻿using BikeBuddy.Models;
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

        public User GetUserById(Guid id)
        {
            return _userRepository.GetAllUsers().FirstOrDefault(u => Guid.Parse(u.Id )== id);
        }

        
        public bool UpdateKycStatus(String userId, bool approve)
        {
            var user = _userRepository.GetAllUsers().FirstOrDefault(u => u.Id == userId.ToString());
            if (user == null) return false;

            user.KycStatus = approve ? KycStatus.Approved : KycStatus.Rejected;
            _userRepository.SaveChanges();
            return true;
        }

        public IEnumerable<Bike> GetAllBikes()
        {
            return _bikeRepository.GetAll();
        }

        public bool UpdateBikeStatus(int bikeId, bool approve)
        {
            var bike = _bikeRepository.GetById(bikeId);
            if (bike == null) return false;

            bike.KycStatus = approve ? KycStatus.Approved : KycStatus.Rejected;
            _bikeRepository.Update(bike);
            return true;
        }
        public async Task LogoutAdminAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public Bike GetBikeByIdAsync(int bikeId)
        {
            return  _bikeRepository.GetById(bikeId);
        }
    }
}
