using BikeBuddy.Data;
using BikeBuddy.Models;
using BikeBuddy.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BikeBuddy.Controllers
{
    public class RentRideController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public RentRideController(UserManager<User> userManager, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
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
            var userBikes = await _context.Bikes
                   .Where(b => b.UserId.Equals(user.Id))
                   .ToListAsync();
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
                    AvailableUpto = model.AvailableUpto,
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

        [HttpGet]
        public IActionResult Ride()
        {
            var cities = _context.Cities.ToList();
            return View(cities);
        }


        public async Task<IActionResult> DisplayByCity(String SearchAddress)
        {
            IQueryable<string> BikeLocation = from m in _context.Bikes orderby m.BikeLocation select m.BikeLocation;
            return View();
        }

        //public async Task<IActionResult> DisplayBikes(string SearchModel, string SearchAddress)
        //{
        //    if (_context.Bikes == null)
        //    {
        //        return Problem("Entity set 'Bikes' is null.");
        //    }

        //    // Fetch bike addresses and models
        //    IQueryable<string> BikesAddress = from m in _context.Bikes
        //                                      orderby m.BikeAddress
        //                                      select m.BikeAddress;

        //    IQueryable<string> BikeModels = from m in _context.Bikes
        //                                    orderby m.BikeModel
        //                                    select m.BikeModel;

        //    // Filter the list of bikes based on the search criteria
        //    var bikesQuery = _context.Bikes.AsQueryable();

        //    if (!string.IsNullOrEmpty(SearchModel))
        //    {
        //        bikesQuery = bikesQuery.Where(b => b.BikeModel.ToUpper().Contains(SearchModel.ToUpper()));
        //    }

        //    if (!string.IsNullOrEmpty(SearchAddress))
        //    {
        //        bikesQuery = bikesQuery.Where(b => b.BikeAddress.ToUpper().Contains(SearchAddress.ToUpper()));
        //    }

        //    var bikes = await bikesQuery.ToListAsync();

        //    // Create a view model with filtered results
        //    var RegisteredBikeviewModel = new RegisteredBikeViewModel
        //    {
        //        BikeModels = await BikeModels.Distinct().ToListAsync(),
        //        BikesAddress = await BikesAddress.Distinct().ToListAsync(),
        //        Bikes = bikes // Add the filtered list of bikes
        //    };

        //    // Return the view with the filtered data
        //    return View(RegisteredBikeviewModel);
        //}
        public IActionResult DisplayBikes(string SearchAddress, string SearchModel, string SearchLocation)
        {
            var bikes = _context.Bikes.ToList();

            // Filter by City
            if (!string.IsNullOrEmpty(SearchLocation))
            {
                bikes = bikes.Where(b => b.BikeLocation == SearchLocation).ToList();
            }

            // Filter by Address
            if (!string.IsNullOrEmpty(SearchAddress))
            {
                bikes = bikes.Where(b => b.BikeAddress.Contains(SearchAddress, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filter by Bike Model
            if (!string.IsNullOrEmpty(SearchModel))
            {
                bikes = bikes.Where(b => b.BikeModel.Contains(SearchModel, StringComparison.OrdinalIgnoreCase)).ToList();
            }


            var viewModel = new RegisteredBikeViewModel
            {
                Bikes = bikes,
                BikesAddress = bikes.Select(b => b.BikeAddress).Distinct().ToList(),
                BikeModels = bikes.Select(b => b.BikeModel).Distinct().ToList()
            };

            return View(viewModel);
        }




    }
}
