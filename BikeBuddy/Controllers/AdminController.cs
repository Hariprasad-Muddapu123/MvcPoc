﻿using BikeBuddy.Services;
using BikeBuddy.Repositories;
using Microsoft.AspNetCore.Mvc;
using BikeBuddy.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using BikeBuddy.Models;


namespace BikeBuddy.Controllers
{

    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IBikeRepository _bikeRepository;
        private readonly EmailSender _emailSender;
        public AdminController(IAdminDashboardService adminDashboardService, IBikeRepository bikeRepository, EmailSender emailSender)
        {
            _adminDashboardService = adminDashboardService;
            _bikeRepository = bikeRepository;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var viewModel = _adminDashboardService.GetDashboardData();
            return View(viewModel);
        }

        public IActionResult UserDetails()
        {
            var dashboardData = _adminDashboardService.GetDashboardData();

            var users = _adminDashboardService.GetAllUsers();

            ViewBag.Users = users;

            return View(dashboardData);
        }

        public IActionResult KycDetails()
        {
            var dashboardData = _adminDashboardService.GetDashboardData();
            var users = _adminDashboardService.GetAllUsers();

            ViewBag.Users=users;

            return View(dashboardData);

        }

        [HttpGet]
        public IActionResult SearchByUsername(string Username)
        {
            var dashboardData = _adminDashboardService.GetDashboardData();

            var users = _adminDashboardService.GetAllUsers();


            if (!string.IsNullOrEmpty(Username))
            {
                users = users.Where(u => u.UserName.Contains(Username, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.Users = users;

            return View("KycDetails", dashboardData);
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

            string contentType = "image/jpeg";

            return File(fileData, contentType);
        }



        [HttpPost]
        public async  Task<IActionResult> ApproveOrRejectKyc(String userId, string action)
        {
            var result = _adminDashboardService.UpdateKycStatus(userId, action == "approve");

            if (!result)
            {
                TempData["Message"] = "User not found or operation failed.";
                TempData["MessageType"] = "error"; 
            }
            else
            {

                TempData["Message"] = action == "approve"
                    ? "KYC successfully approved."
                    : "KYC rejected.";
                TempData["MessageType"] = "success";

                var userEmail = await _adminDashboardService.GetUserEmailAsync(userId);
                string subject = action == "approve" ? "KYC Approved" : "KYC Rejected";
                string body = action == "approve"
                    ? "Congratulations! Your KYC has been approved. You can now access all features of our platform."
                    : "Unfortunately, your KYC has been rejected. Please review your submission and try again.";
                await _emailSender.SendEmailAsync(userEmail, subject, body);
            }
            return RedirectToAction("KycDetails");
        }
        public IActionResult BikeDetails()
        { 
            var dashboardData = _adminDashboardService.GetDashboardData();

            var bikes = _adminDashboardService.GetAllBikes();

            ViewBag.Bikes = bikes;

            return View(dashboardData); 
        }
        [HttpGet]
        public IActionResult ViewBikeDetails(int bikeId)
        {
            var bike = _bikeRepository.GetById(bikeId);

            if (bike == null)
            {
                TempData["Message"] = "Bike not found.";
                TempData["MessageType"] = "error";
                return RedirectToAction("BikeDetails");
            }

            return View(bike);
        }

        public IActionResult ViewBikeDocument(int bikeId, string type)
        {
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

            string contentType = "application/pdf";

            return File(fileData, contentType);
        }
        [HttpPost]
        public async Task<IActionResult> ApproveOrRejectBike(int bikeId, string action)
        {
            var result = _adminDashboardService.UpdateBikeStatus(bikeId, action == "approve");

            if (!result)
            {
                TempData["Message"] = "Bike documnets are not valid upload valid documnets.";
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = action == "approve"
                    ? "bike approved."
                    : "bike rejected.";
                TempData["MessageType"] = "success";
                var bike =  _adminDashboardService.GetBikeByIdAsync(bikeId);
                var userEmail = bike?.User?.Email;
                string subject = action == "approve" ? "Bike Approved" : "Bike Rejected";
                string body = action == "approve"
                    ? "Congratulations! Your bike has been approved for renting. You can now list your bike and make it available for rental."
                    : "Unfortunately, your bike has been rejected. Please upload valid documents for review.";
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
