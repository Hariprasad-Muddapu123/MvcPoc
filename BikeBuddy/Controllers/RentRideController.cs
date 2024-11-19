using BikeBuddy.Data;
using BikeBuddy.Models;
using BikeBuddy.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BikeBuddy.Controllers
{
    public class RentRideController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public RentRideController(UserManager<User> userManager,IWebHostEnvironment webHostEnvironment,ApplicationDbContext context)
        {
            this._userManager = userManager;
            this._webHostEnvironment = webHostEnvironment;
            this._context = context;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Rent()
        {
            //TempData["Message"] = "";
            var user = await _userManager.GetUserAsync(User);
           // var model = new BikeViewModel();
            var userBikes = _context.Bikes
                   .Where(b => b.UserId.Equals(user.Id))
                   .ToList();
            var viewModel = new RegisterBikeViewModel
            {
                NewBike = new BikeViewModel(),
                UserBikes = userBikes
            };

            return View(viewModel);

            //return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rent(RegisterBikeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var model = viewModel.NewBike;  // Access the NewBike model

                // Handle BikeImage upload
                if (model.BikeImage != null && model.BikeImage.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.BikeImage.CopyToAsync(memoryStream);
                        // Assuming your Bike class has a BikeImageBytes property
                        model.BikeImageBytes = memoryStream.ToArray();
                    }
                }

                // Handle BikeDocuments upload
                if (model.BikeDocuments != null && model.BikeDocuments.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.BikeDocuments.CopyToAsync(memoryStream);
                        // Assuming your Bike class has a BikeDocumentsBytes property
                        model.BikeDocumentsBytes = memoryStream.ToArray();
                    }
                }

                // Retrieve the logged-in user
                var user = await _userManager.GetUserAsync(User);

                // Create the bike object from the model
                var bike = new Bike
                {
                    BikeModel = model.BikeModel,
                    BikeNumber = model.BikeNumber,
                    BikeLocation = model.BikeLocation,
                    BikeAddress = model.BikeAddress,
                    KycStatus = model.KycStatus,
                    UserId = user.Id,
                    RegistrationDate = DateTime.UtcNow,
                    // Save the byte arrays for images and documents
                    BikeImageBytes = model.BikeImageBytes,
                    BikeDocumentsBytes = model.BikeDocumentsBytes
                };

                // Save to the database
                _context.Bikes.Add(bike);
                var result = await _context.SaveChangesAsync();

                // Set TempData message for success/failure
                TempData["Message"] = result == 1 ? "Bike registered successfully!" : "Failed to upload bike details";

                // Redirect to the Rent page
                return RedirectToAction("Rent");
            }

            // If model validation fails, show the form with errors
            TempData["ErrorMessage"] = "Failed to register the bike. Please check the inputs.";
            return View(viewModel);  // Return the RegisterBikeViewModel back to the view
        }

    }
}
