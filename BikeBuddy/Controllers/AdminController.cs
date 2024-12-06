using BikeBuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace BikeBuddy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IUserService _userService;
        private readonly IBikeService _bikeService;
        private readonly IRideService _rideService;
        private readonly EmailSender _emailSender;

        public AdminController(IAdminDashboardService adminDashboardService,IUserService userService,IBikeService bikeService,IRideService rideService, EmailSender emailSender)
        {
            _adminDashboardService = adminDashboardService;
            _userService = userService;
            _rideService = rideService;
            _bikeService = bikeService;
            _emailSender = emailSender;
        }
        private async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            var dashboardData = await _adminDashboardService.GetDashboardData();

            TempData["DashBoardData"] = JsonConvert.SerializeObject(dashboardData, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            TempData.Keep("DashBoardData");
            return dashboardData;
        }

        private AdminDashboardViewModel GetDashboardDataFromTempData()
        {
            if (TempData.ContainsKey("DashBoardData"))
            {
                var jsonData = TempData["DashBoardData"]?.ToString();
                TempData.Keep("DashBoardData");
                return JsonConvert.DeserializeObject<AdminDashboardViewModel>(jsonData);
            }
            return null;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dashboardData = await GetDashboardDataAsync();
            return View(dashboardData);
        }
        [HttpGet]
        public async Task<IActionResult> UserDetails()
        {
            var dashboardData = GetDashboardDataFromTempData() ?? await GetDashboardDataAsync();
            return View(dashboardData);
        }

        [HttpGet]
        public async Task<IActionResult> MoreDetails(String userid)
        {
            User user = await _userService.GetUserByIdAsync(userid);
            var bikes = await _bikeService.GetAllBikesByUserIdAsync(userid);
            var rides = await _rideService.GetRidesByUserIdAsync(userid);

            AdminOverviewViewModel adminOverviewViewModel = new AdminOverviewViewModel()
            {
                User = user,
                Bikes = bikes,
                Rides = rides
            };
            return View(adminOverviewViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> KycDetails()
        {
            var dashboardData = TempData["FilteredData"] != null
               ? JsonConvert.DeserializeObject<AdminDashboardViewModel>((string)TempData["FilteredData"])
               : await GetDashboardDataAsync();
                    return View(dashboardData);
        }

        [HttpGet]
        public async Task<IActionResult> ByStatus(KycStatus? kycStatus = null)
        {
            var dashboardData = GetDashboardDataFromTempData() ?? await GetDashboardDataAsync();
            if (kycStatus.HasValue)
            {
                dashboardData.Users = dashboardData.Users
                    .Where(b => b.KycStatus == kycStatus.Value);
            }
            TempData["FilteredData"] = JsonConvert.SerializeObject(dashboardData,new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return RedirectToAction("KycDetails");
        }
        [HttpGet]
        public async Task<IActionResult> SearchByUsername(string username, string targetView)
        {
            var dashboardData = GetDashboardDataFromTempData() ?? await GetDashboardDataAsync();
            var users = dashboardData.Users;

            if (!string.IsNullOrEmpty(username))
            {
                users = users.Where(u => u.UserName.Contains(username, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            dashboardData.Users = users;
            switch (targetView?.ToLower())
            {
                case "kycdetails":
                    return PartialView("KycDetails", dashboardData);

                case "userdetails":
                    return PartialView("UserDetails", dashboardData);

                case "bikedetails":
                    return PartialView("BikeDetails", dashboardData);

                default:
                    return BadRequest("Invalid target view.");
            }
            return View(dashboardData);
        }

        public async Task<IActionResult> ViewUserDetails(Guid id)
        {
            var user = await _adminDashboardService.GetUserById(id);
            if (user == null) return NotFound("User not found.");

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

            return View(userViewModel);
        }

        public async Task<IActionResult> ViewDocument(Guid userId, string type)
        {
            var user = await _adminDashboardService.GetUserById(userId);
            if (user == null) return NotFound();

            byte[]? fileData = type switch
            {
                "DrivingLicense" => user.DrivingLicenseImage,
                "Aadhaar" => user.AadhaarImage,
                _ => null
            };

            if (fileData == null) return NotFound();

            return File(fileData, "image/jpeg");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveOrRejectKyc(string userId, string action, string? rejectionReason)
        {
            bool isApproved = action == "approve";
            var result = await _adminDashboardService.UpdateKycStatus(userId, isApproved);

            if (result)
            {
                GetDashboardDataAsync();
                var userEmail = await _adminDashboardService.GetUserEmailAsync(userId);
                string subject = isApproved ? "KYC Approved" : "KYC Rejected";
                string body = isApproved
                    ? "Congratulations! Your KYC has been approved."
                    : $"Unfortunately, your KYC has been rejected.Reason: {rejectionReason}";
                await _emailSender.SendEmailAsync(userEmail, subject, body);
            }

            return RedirectToAction("KycDetails");
        }

        public async Task<IActionResult> BikeDetails()
        {
            var dashboardData = TempData["BikeFilteredData"] != null
               ? JsonConvert.DeserializeObject<AdminDashboardViewModel>((string)TempData["BikeFilteredData"])
               : await GetDashboardDataAsync();
            return View(dashboardData);
        }
        [HttpGet]
        public async Task<IActionResult> BikeStatus(KycStatus? kycStatus = null)
        {

            var dashboardData = GetDashboardDataFromTempData() ?? await GetDashboardDataAsync();

            if (kycStatus.HasValue)
            {
                dashboardData.Users = dashboardData.Users
                    .Where(user => user.Bikes != null && user.Bikes.Any(bike => bike.KycStatus == kycStatus.Value))
                    .Select(user => new User
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Bikes = user.Bikes.Where(bike => bike.KycStatus == kycStatus.Value).ToList()
                    })
                    .ToList();
            }

            TempData["BikeFilteredData"] = JsonConvert.SerializeObject(dashboardData, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return RedirectToAction("BikeDetails");
        }



        [HttpGet]
        public async Task<IActionResult> ViewBikeDetails(int bikeId)
        {
            var bike = await _adminDashboardService.GetBikeByIdAsync(bikeId);
            if (bike == null)
            {
                TempData["Message"] = "Bike not found.";
                TempData["MessageType"] = "error";
                return RedirectToAction("BikeDetails");
            }

            return View(bike);
        }

        public async Task<IActionResult> ViewBikeDocument(int bikeId, string type)
        {
            var bike=await _adminDashboardService.GetBikeByIdAsync(bikeId);
            if (bike == null) return NotFound();

            byte[]? fileData = type switch
            {
                "BikeDocument" => bike.BikeDocumentsBytes,
                _ => null
            };

            if (fileData == null) return NotFound();

            return File(fileData, "application/pdf");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveOrRejectBike(int bikeId, string action,string? rejectionReason)
        {
            bool isApproved = action == "approve";
            var result = await _adminDashboardService.UpdateBikeStatus(bikeId, isApproved);

            if (result)
            {
                var bike = await _adminDashboardService.GetBikeByIdAsync(bikeId);
                var userEmail = await _adminDashboardService.GetUserEmailAsync(bike.UserId);
                string subject = isApproved ? "Bike Approved" : "Bike Rejected";
                string body = isApproved
                    ? "Congratulations! Your bike has been approved for renting."
                    : $"Unfortunately, your bike has been rejected. Reason: {rejectionReason}";
                await _emailSender.SendEmailAsync(userEmail, subject, body);
            }

            return RedirectToAction("BikeDetails");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _adminDashboardService.LogoutAdminAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
