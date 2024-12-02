﻿using System.Security.Claims;
namespace BikeBuddy.Controllers
{
    public class RentRideController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IBikeService _bikeService;

        public RentRideController(UserManager<User> userManager, IWebHostEnvironment webHostEnvironment,IBikeService bikeService, ApplicationDbContext context)
        {
            this._userManager = userManager;
            this._webHostEnvironment = webHostEnvironment;
            this._bikeService = bikeService;
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
            if (user == null)
            {
                TempData["Message"] = "Please log in to continue.";
                return RedirectToAction("Login", "Account");
            }
            List<Bike> bikes=(List<Bike>) await _bikeService.GetAllBikes();
            var userBikes = bikes
                   .Where(b => b.UserId.Equals(user.Id) && !b.IsRemoved)
                   .ToList();
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
                    BikeRentPrice = model.BikeRentPrice,
                    UserId = user.Id,
                    Available=true,
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
        [HttpPost]
        public async Task<IActionResult> RemoveBike(int bikeId)
        {
            IEnumerable<Bike> bikes = await _bikeService.GetAllBikes();
            Bike bike = bikes.FirstOrDefault(b => b.BikeId == bikeId);
            if (bike != null)
            {
                bike.RemovedDate = DateTime.UtcNow;
                bike.IsRemoved = true;
                _context.Bikes.Update(bike);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Bike removed successfully and archived!";
            }
            else
            {
                TempData["ErrorMessage"] = "Bike not found!";
            }

            return RedirectToAction("Rent");
        }

        [HttpGet]
        public async Task<IActionResult> RemovedBikes()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Bike> bikes= (List<Bike>)await _bikeService.GetAllBikes(); 
            var removedBikes = bikes
                .Where(b => b.UserId.Equals(user.Id) && b.IsRemoved)
                .ToList();
            return View(removedBikes);
        }

        [HttpGet]
        public async  Task<IActionResult> Ride()
        {
            var cities = await _context.Cities.ToListAsync();
            return View(cities);
        }
        [HttpGet]
        public async Task<IActionResult> DisplayBikes(string SearchAddress, string SearchModel, string SearchLocation, string[] SelectedAddresses,string SelectedModels)
        {
            List<Bike> bikes = (List<Bike>) await _bikeService.GetAllBikes();
            String currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(currentUserId);
            bikes=bikes.Where(bike=>bike.UserId != currentUserId).ToList();
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
        public async Task<IActionResult> SearchDate(RideSearchViewModel model)
        {
            var bikeIdString = HttpContext.Session.GetString("BikeId");
            if (!int.TryParse(bikeIdString, out int bikeId))
            {

                return BadRequest("Invalid BikeId format.");
            }
            List<Bike> bikes = (List<Bike>)await _bikeService.GetAllBikes();
            var bike = bikes.FirstOrDefault(b => b.BikeId == bikeId);
            if(bike.AvailableUpto<model.DropoffDate)
            {
                ViewData["Message"] = $"Not Available upto that time.Available upto {@bike.AvailableUpto}";
                return View("Date");
            }
            
            DateTime pickupDateTime = model.PickupDate.Add(TimeSpan.Parse(model.PickupTime));
            DateTime dropoffDateTime = model.DropoffDate.Add(TimeSpan.Parse(model.DropoffTime));

            if (pickupDateTime <= DateTime.Now)
            {
                ViewData["Message"] = "Pickup date and time must be in the future.";
                return View("Date");
            }
            if (dropoffDateTime <= pickupDateTime)
            {
                ViewData["Message"] = "Dropoff date and time must be later than pickup date and time.";
                return View("Date");
            }
            TimeSpan timeDifference = dropoffDateTime - pickupDateTime;  
            double bikeRentPrice = bikes
                .Where(b => b.BikeId == bikeId)
                .Select(b => b.BikeRentPrice)
                .FirstOrDefault();
            double totalHours = CalculateHours(timeDifference);
            decimal totalPrice = (decimal)totalHours * (decimal)bikeRentPrice;
            decimal gst = (totalPrice * 5 )/ 100;
            decimal totalBill = totalPrice + gst;
            HttpContext.Session.SetString("PickupDateTime", pickupDateTime.ToString("yyyy-MM-dd HH:mm"));
            HttpContext.Session.SetString("DropoffDateTime", dropoffDateTime.ToString("yyyy-MM-dd HH:mm"));
            HttpContext.Session.SetString("RentedHours", totalHours.ToString(""));
            HttpContext.Session.SetString("TotalPrice", totalPrice.ToString("F2"));  
            HttpContext.Session.SetString("Gst", gst.ToString(""));
            HttpContext.Session.SetString("TotalBill", totalBill.ToString(""));
            ViewBag.TotalPrice = totalPrice;
            return RedirectToAction("BookBike");
        }

        private double CalculateHours(TimeSpan timeDifference)
        {
            double totalHours = timeDifference.TotalHours;
            
            return totalHours;
        }
        [HttpGet]
        public async Task<IActionResult> BookBike()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Message"] = "Please log in to continue.";
                return RedirectToAction("Login", "Account");
            }

            if (user.KycStatus != KycStatus.Approved)
            {
                TempData["Message"] = "Your KYC status is pending approval. You cannot book a bike until all required documents are uploaded and verified.";
                return RedirectToAction("Profile", "User");
            }

            var totalPrice = HttpContext.Session.GetString("TotalPrice");
            var rentedHours = HttpContext.Session.GetString("RentedHours");
            var gst = HttpContext.Session.GetString("Gst");
            var totalBill = HttpContext.Session.GetString("TotalBill");
            var bikeIdString = HttpContext.Session.GetString("BikeId");
            if (!int.TryParse(bikeIdString, out int bikeId))
            {
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

        [HttpGet]
        public async Task<IActionResult> PaymentSucess()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(HttpContext.Session.GetString("BikeId"), out int bikeId))
            {
                return BadRequest("Invalid BikeId format.");
            }
            var pickupdatetime = HttpContext.Session.GetString("PickupDateTime");
            var dropoffdatetime = HttpContext.Session.GetString("PickupDateTime");
            var totalPrice = HttpContext.Session.GetString("TotalPrice");
            var rentedHours = HttpContext.Session.GetString("RentedHours");
            var gst = HttpContext.Session.GetString("Gst");
            var totalBill = HttpContext.Session.GetString("TotalBill");
            
            if (string.IsNullOrEmpty(pickupdatetime) || string.IsNullOrEmpty(dropoffdatetime) ||
        string.IsNullOrEmpty(totalPrice) || string.IsNullOrEmpty(rentedHours) ||
        string.IsNullOrEmpty(gst) || string.IsNullOrEmpty(totalBill))
            {
                return BadRequest("Session data is incomplete.");
            }

            var ride = new Ride
            {
                UserId = user.Id,
                BikeId = bikeId,
                PickupDateTime = pickupdatetime,
                DropoffDateTime = dropoffdatetime,
                RentalStatus = RentStatus.Ongoing, 
                RideRegisteredDate = DateTime.UtcNow,
                RentedHours=rentedHours,
                Amount = totalPrice, 
                Gst = gst, 
                TotalAmount = totalBill, 
            };

            _context.Rides.Add(ride);
            await _context.SaveChangesAsync();

            var payment = new Payment
            {
                UserId = user.Id,
                RideId = ride.RideId,
                BikeId = ride.BikeId, 
                TransactionId = GenerateTransactionId(),
                TransactionDateTime = DateTime.UtcNow,
                TotalAmount = Convert.ToDecimal(ride.TotalAmount),
                PaymentStatus = "Success", 
                Currency = "INR" 
            };
            
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(); 
            ride.TotalAmount = payment.TotalAmount.ToString();

            _context.Rides.Update(ride);
            await _context.SaveChangesAsync();
            var bikes = await _bikeService.GetAllBikes();

            var bike =  bikes.FirstOrDefault(b => b.BikeId == bikeId);
            if(bike!=null)
            {
                bike.Available = false;
                _context.Bikes.Update(bike);
                await _context.SaveChangesAsync();
                HttpContext.Session.Clear();
                return View(bike);
            }
            else
            {
                TempData["Message"] = "Bike not found.";
                return View("DisplayBikes");
            }
            
        }

        private string GenerateTransactionId()
        {
            return Guid.NewGuid().ToString("N");
        }

    }
}
