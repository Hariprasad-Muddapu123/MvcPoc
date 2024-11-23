using BikeBuddy.Services;
using Microsoft.AspNetCore.Mvc;
using BikeBuddy.ViewModels;

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

        public IActionResult KycDetails()
        {
            var dashboardData = _adminDashboardService.GetDashboardData();
            var users = _adminDashboardService.GetAllUsers();

            ViewBag.Users=users;

            return View(dashboardData);

        }

        public IActionResult ViewUserDetails(Guid id)
        {
            var user = _adminDashboardService.GetUserById(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userViewModel = new UserDetailsViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                MobileNo = user.PhoneNumber,
                Email = user.Email,
                AadhaarImage = user.AadhaarImage,
                DrivingLicenseImage = user.DrivingLicenseImage,
                KycStatus = user.KycStatus
            };

            return View("ViewUserDetails",userViewModel);
        }

        public IActionResult ViewDocument(Guid userId, string type)
        {
            var user = _adminDashboardService.GetUserById(userId);
            if (user == null) return NotFound();

            byte[] fileData = type switch
            {
                "DrivingLicense" => user.DrivingLicenseImage,
                "Aadhaar" => user.AadhaarImage,
                _ => null
            };

            if (fileData == null) return NotFound();

            // Set MIME type to display the image in the browser
            string contentType = "image/jpeg";

            return File(fileData, contentType);
        }



        [HttpPost]
        public IActionResult ApproveOrRejectKyc(String userId, string action)
        {
            var result = _adminDashboardService.UpdateKycStatus(userId, action == "approve");

            if (!result)
            {
                // Store failure message in TempData
                TempData["Message"] = "User not found or operation failed.";
                TempData["MessageType"] = "error"; // Indicates an error
            }
            else
            {
                // Store success message in TempData
                TempData["Message"] = action == "approve"
                    ? "KYC successfully approved."
                    : "KYC successfully rejected.";
                TempData["MessageType"] = "success"; // Indicates success
            }

            // Redirect to the same KYC details page
            return RedirectToAction("KycDetails");
        }





    }
}
