using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BikeBuddy.Models;
using BikeBuddy.Services;
using BikeBuddy.ViewModels;

namespace BikeBuddy.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBikeService _bikeService;
        private readonly IRideService _rideService;

        public UserController(IUserService userService,IBikeService bikeService,IRideService rideService)
        {
            _userService = userService;
            _bikeService = bikeService; 
            _rideService = rideService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userService.GetCurrentUserAsync(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var model = await _userService.GetUserProfileAsync(user.Id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _userService.GetCurrentUserAsync(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var result = await _userService.UpdateUserProfileAsync(user.Id, model);

            TempData["Message"] = result ? "Profile updated successfully!" : "Failed to update profile.";
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> MyRents()
        {
            var user = await _userService.GetCurrentUserAsync(User.Identity.Name);
            var bikes= await _bikeService.GetAllBikesByUserIdAsync(user.Id);
            if(bikes==null)
            {
                return NotFound("No Bikes found for this User");
            }
            return View(bikes);
        }

        public async Task<IActionResult> MyRides()
        {
            var user = await _userService.GetCurrentUserAsync(User.Identity.Name);
            var rides = await _rideService.GetRidesByUserIdAsync(user.Id);
            if (rides == null)
            {
                return NotFound("No Rides found for this User");
            }

            return View(rides);
        }

    }
}
