using BikeBuddy.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeBuddy.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        public IActionResult Index()
        {
            var viewModel = _adminDashboardService.GetDashboardData();
            return View(viewModel);
        }

        public IActionResult UserDetails()
        {
            var dashboardData = _adminDashboardService.GetDashboardData();

            // Get list of all users
            var users = _adminDashboardService.GetAllUsers();

            // Pass data to the view using ViewBag
            ViewBag.Users = users;

            // Pass dashboard data as the model
            return View(dashboardData);
        }
    }
}
