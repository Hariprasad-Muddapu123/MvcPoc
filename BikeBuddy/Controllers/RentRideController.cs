using BikeBuddy.Data;
using BikeBuddy.Models;
using BikeBuddy.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

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
            var user = await _userManager.GetUserAsync(User);
            var userBikes = await _context.Bikes
                   .Where(b => b.UserId.Equals(user.Id))
                   .ToListAsync();
            var viewModel = new RegisterBikeViewModel
            {
                NewBike = new BikeViewModel(),
                UserBikes = userBikes
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rent(RegisterBikeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var model = viewModel.NewBike;  

                if (model.BikeImage != null && model.BikeImage.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.BikeImage.CopyToAsync(memoryStream);
                        
                        model.BikeImageBytes = memoryStream.ToArray();
                    }
                }

                if (model.BikeDocuments != null && model.BikeDocuments.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.BikeDocuments.CopyToAsync(memoryStream);
                     
                        model.BikeDocumentsBytes = memoryStream.ToArray();
                    }
                }

                var user = await _userManager.GetUserAsync(User);

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
                    BikeImageBytes = model.BikeImageBytes,
                    BikeDocumentsBytes = model.BikeDocumentsBytes
                };

                _context.Bikes.Add(bike);
                var result = await _context.SaveChangesAsync();

                TempData["Message"] = result == 1 ? "Bike registered successfully!" : "Failed to upload bike details";

                return RedirectToAction("Rent");
            }
            TempData["ErrorMessage"] = "Failed to register the bike. Please check the inputs.";
            return View(viewModel);
        }

        [HttpGet]
        public async  Task<IActionResult> Ride()
        {
            var cities = await _context.Cities.ToListAsync();
            return View(cities);
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
        [HttpGet]
        public async Task<IActionResult> DisplayBikes(string SearchAddress, string SearchModel, string SearchLocation, string[] SelectedAddresses,string SelectedModels)
        {
            var bikes =await  _context.Bikes.ToListAsync();

            // Filter by City
            if (!string.IsNullOrEmpty(SearchLocation))
            {
                bikes =  bikes.Where(b => b.BikeLocation == SearchLocation & b.KycStatus == KycStatus.Approved).ToList();
            }

            if (!string.IsNullOrEmpty(SearchAddress))
            {
                bikes = bikes.Where(b => b.BikeAddress.Contains(SearchAddress, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(SearchModel))
            {
                bikes = bikes.Where(b => b.BikeModel.Contains(SearchModel, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (SelectedAddresses != null && SelectedAddresses.Any())
            {
                bikes = bikes.Where(b => SelectedAddresses.Contains(b.BikeAddress)).ToList();
            }

            if (SelectedModels != null && SelectedModels.Any())
            {
                bikes = bikes.Where(b => SelectedModels.Contains(b.BikeModel)).ToList();
            }

            var viewModel = new RegisteredBikeViewModel
            {
                Bikes = bikes,
                BikesAddress = bikes.Select(b => b.BikeAddress).Distinct().ToList(),
                BikeModels = bikes.Select(b => b.BikeModel).Distinct().ToList()
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Date(int BikeId)
        {
            HttpContext.Session.SetString("BikeId",BikeId.ToString());
            return View();
        }

        

        [HttpGet]
        public IActionResult SearchDate(RideSearchViewModel model)
        {
            // Combine Date and Time fields to create full DateTime objects
            DateTime pickupDateTime = model.PickupDate.Add(TimeSpan.Parse(model.PickupTime));
            DateTime dropoffDateTime = model.DropoffDate.Add(TimeSpan.Parse(model.DropoffTime));

            // Calculate the time difference in hours
            TimeSpan timeDifference = dropoffDateTime - pickupDateTime;

            // Calculate the total price based on the time difference
            double totalHours = CalculateHours(timeDifference);
            decimal totalPrice = (decimal)totalHours * 100;
            decimal gst = (totalPrice * 5 )/ 100;
            decimal totalBill = totalPrice + gst;

            // Store ride details and calculated price in session
            HttpContext.Session.SetString("PickupDateTime", pickupDateTime.ToString("yyyy-MM-dd HH:mm"));
            HttpContext.Session.SetString("DropoffDateTime", dropoffDateTime.ToString("yyyy-MM-dd HH:mm"));
            HttpContext.Session.SetString("RentedHours", totalHours.ToString(""));
            HttpContext.Session.SetString("TotalPrice", totalPrice.ToString("F2"));  // Store price as string
            HttpContext.Session.SetString("Gst", gst.ToString(""));
            HttpContext.Session.SetString("TotalBill", totalBill.ToString(""));

            // Pass the total price to the view (optional, if you want to show it immediately)
            ViewBag.TotalPrice = totalPrice;

            // Redirect or return the same view with the calculated price
            return RedirectToAction("BookBike");
        }

        private double CalculateHours(TimeSpan timeDifference)
        {
            double totalHours = timeDifference.TotalHours;
            if (totalHours % 1 != 0)
            {
                totalHours = Math.Ceiling(totalHours);
            }
            return totalHours;
        }
        [HttpGet]
        public async Task<IActionResult> BookBike()
        {
            var totalPrice = HttpContext.Session.GetString("TotalPrice");
            var rentedHours = HttpContext.Session.GetString("RentedHours");
            var gst = HttpContext.Session.GetString("Gst");
            var totalBill = HttpContext.Session.GetString("TotalBill");
            var bikeIdString = HttpContext.Session.GetString("BikeId");
            if (!int.TryParse(bikeIdString, out int bikeId))
            {
                // Handle invalid BikeId format
                return BadRequest("Invalid BikeId format.");
            }
            var bike = await _context.Bikes.FirstOrDefaultAsync(b => b.BikeId == bikeId);

            var model = new BookingViewModel
            {
                BikeDetails = bike,
                Amount = totalPrice,
                Gst = gst,
                RentedHours = rentedHours,
                TotalBill = totalBill

            };
            return View(model);
        }



        //Handle payment success and clear session
        //public IActionResult PaymentSucceeded()
        //{
        //    // Store ride details after successful payment (in DB or elsewhere)
        //    var rideDetails = new RideDetails
        //    {
        //        PickupDateTime = HttpContext.Session.GetString("PickupDateTime"),
        //        DropoffDateTime = HttpContext.Session.GetString("DropoffDateTime"),
        //        TotalPrice = decimal.Parse(HttpContext.Session.GetString("TotalPrice"))
        //    };

        //    // Clear session data after successful payment
        //    HttpContext.Session.Clear();

        //    // Redirect to a confirmation page or another view
        //    return View(rideDetails);
        //}
    }
}
