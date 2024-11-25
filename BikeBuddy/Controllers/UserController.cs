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

        public UserController(IUserService userService)
        {
            _userService = userService;
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
    }
}
