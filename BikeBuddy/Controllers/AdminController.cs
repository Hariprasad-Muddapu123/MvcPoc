using BikeBuddy.Services;
using BikeBuddy.Repositories;
using Microsoft.AspNetCore.Mvc;
using BikeBuddy.ViewModels;
using BikeBuddy.Models;

namespace BikeBuddy.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IBikeRepository _bikeRepository;

        public AdminController(IAdminDashboardService adminDashboardService, IBikeRepository bikeRepository)
        {
            _adminDashboardService = adminDashboardService;
            _bikeRepository = bikeRepository;
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

       

        public IActionResult BikeDetails()
        {
            // Retrieve dashboard data (if needed)
            var dashboardData = _adminDashboardService.GetDashboardData();

            // Fetch all bikes (you can modify this to get bikes that are pending for approval if needed)
            var bikes = _adminDashboardService.GetAllBikes();

            // Store bikes in ViewBag to pass to the view
            ViewBag.Bikes = bikes;

            // Return dashboard data if you want to display some metrics like total bikes, etc.
            return View(dashboardData); // You can return View(dashboardData) or View() depending on your requirements
        }
        [HttpGet]
        public IActionResult ViewBikeDetails(int bikeId)
        {
            // Fetch the bike details using the bikeId
            var bike = _bikeRepository.GetById(bikeId);

            // If no bike is found, return to the bike list with an error message
            if (bike == null)
            {
                TempData["Message"] = "Bike not found.";
                TempData["MessageType"] = "error"; // Indicates an error
                return RedirectToAction("BikeDetails"); // Or another action that lists bikes
            }

            // Return the bike details to the view
            return View(bike); // This will pass the bike object to the view for display
        }

        public IActionResult ViewBikeDocument(int bikeId, string type)
        {
            // Get the bike using the bikeId
            var bike = _bikeRepository.GetById(bikeId);

            if (bike == null)
            {
                return NotFound();
            }

            byte[] fileData = type switch
            {
                "BikeDocument" => bike.BikeDocumentsBytes,
                _ => null
            };

            if (fileData == null) return NotFound();

            // Assuming the document is a PDF. You can change this depending on the actual type of the document.
            string contentType = "application/pdf";

            return File(fileData, contentType); // This will open the PDF document directly in the browser.
        }


    }
}
