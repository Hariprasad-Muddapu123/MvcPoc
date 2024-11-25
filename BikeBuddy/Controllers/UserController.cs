using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BikeBuddy.Models;
using BikeBuddy.ViewModels;

namespace BikeBuddy.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        

        // Admin action to update KYC status
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateKYCStatus(string userId, KycStatus status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            user.KycStatus = status;
            await _userManager.UpdateAsync(user);

            TempData["StatusMessage"] = $"KYC Status updated to {status}";
            return RedirectToAction("UserDetails", new { id = userId });
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var model = new ProfileViewModel
            {
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Address = user.Address,
                IsAadhaarUploaded = user.IsAadhaarUploaded,
                IsDrivingLicenseUploaded = user.IsDrivingLicenseUploaded,
                AadhaarUploadStatus = user.IsAadhaarUploaded ? "Aadhaar Uploaded Successfully" : "Aadhaar Not Uploaded",
                DrivingLicenseUploadStatus = user.IsDrivingLicenseUploaded ? "Driving License Uploaded Successfully" : "Driving License Not Uploaded",
                KYCStatus = user.KycStatus,
                ProfileImageBytes = user.ProfileImage,
                AadhaarImageBytes = user.AadhaarImage,
                DrivingLicenseImageBytes = user.DrivingLicenseImage
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Update other fields
            user.UserName = string.IsNullOrWhiteSpace(model.UserName) ? user.UserName : model.UserName;
            user.Email = string.IsNullOrWhiteSpace(model.Email) ? user.Email : model.Email;
            user.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? user.PhoneNumber : model.PhoneNumber;
            user.Address = string.IsNullOrWhiteSpace(model.Address) ? user.Address : model.Address;


            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.ProfileImage.CopyToAsync(memoryStream);
                    user.ProfileImage = memoryStream.ToArray();
                }
            }

            if (model.AadhaarFile != null && model.AadhaarFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.AadhaarFile.CopyToAsync(memoryStream);
                    user.AadhaarImage = memoryStream.ToArray(); 
                    user.IsAadhaarUploaded = true;
                }
            }
            if (model.DrivingLicenseFile != null && model.DrivingLicenseFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.DrivingLicenseFile.CopyToAsync(memoryStream);
                    user.DrivingLicenseImage = memoryStream.ToArray(); 
                    user.IsDrivingLicenseUploaded = true;
                }
            }
            var result = await _userManager.UpdateAsync(user);

            TempData["Message"] = result.Succeeded ? "Profile updated successfully!" : "Failed to update profile.";

            return RedirectToAction("Profile");
        }




    }
}
