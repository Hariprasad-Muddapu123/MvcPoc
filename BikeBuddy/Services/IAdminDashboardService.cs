﻿using BikeBuddy.Models;
using BikeBuddy.ViewModels;

namespace BikeBuddy.Services
{
    public interface IAdminDashboardService
    {
        AdminDashboardViewModel GetDashboardData();

        IEnumerable<User> GetAllUsers();

        User GetUserById(Guid id);

        bool UpdateKycStatus(String userId, bool approve);
    }
}
