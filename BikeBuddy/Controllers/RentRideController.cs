
using BikeBuddy.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace BikeBuddy.Controllers
{
    public class RentRideController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IBikeService _bikeService;
        private readonly IRideService _rideService;
        private readonly IPaymentService _paymentService;
        private readonly ICityService _cityService;
        public RentRideController(UserManager<User> userManager,IBikeService bikeService, IRideService rideService, IPaymentService paymentService, ICityService cityService)
        {
            this._userManager = userManager;
            this._bikeService = bikeService;
            this._rideService = rideService;
            this._paymentService = paymentService;
            this._cityService = cityService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ServiceFilter(typeof(BlockedUserFilter))]
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
                if (user == null)
                {
                    TempData["Message"] = "Please log in to continue.";
                    return RedirectToAction("Login", "Account");
                }
                var bike = CreateBike(model, user);

                await _bikeService.RegisterBikeAsync(bike);
                TempData["Message"] = "Bike registered successfully!";
                return RedirectToAction("Rent");
            }
            TempData["ErrorMessage"] = "Failed to register the bike. Please check the inputs.";
            return View(viewModel);
        }
        private Bike CreateBike(BikeViewModel model, User user)
        {
            return new Bike
            {
                BikeModel = model.BikeModel,
                BikeNumber = model.BikeNumber,
                BikeLocation = model.BikeLocation,
                BikeAddress = model.BikeAddress,
                FullAddress =model.FullAddress,
                ContactNo =model.ContactNo,
                KycStatus = model.KycStatus,
                BikeRentPrice = model.BikeRentPrice,
                UserId = user.Id,
                Available = true,
                RegistrationDate = DateTime.UtcNow,
                AvailableUpto = model.AvailableUpto,
                BikeImageBytes = model.BikeImageBytes,
                BikeDocumentsBytes = model.BikeDocumentsBytes
            };
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBike(int bikeId)
        {
            IEnumerable<Bike> bikes = await _bikeService.GetAllBikes();
            Bike? bike = bikes.FirstOrDefault(b => b.BikeId == bikeId);
            if (bike != null)
            {
                bike.RemovedDate = DateTime.UtcNow;
                bike.IsRemoved = true;
                await _bikeService.RemoveBikeAsync(bikeId);
                TempData["Message"] = "Bike removed successfully.";
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
        [ServiceFilter(typeof(BlockedUserFilter))]
        public async  Task<IActionResult> Ride()
        {
            var cities = await _cityService.GetAllCitiesAsync();
            return View(cities);
        }

        [HttpGet]
        public async Task<IActionResult> DisplayBikes(string SearchAddress, string SearchModel, string SearchLocation, string[] SelectedAddresses,string[] SelectedModels)
        {
            List<Bike> bikes = (List<Bike>)await _bikeService.GetAllBikes();
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bikes = bikes.Where(bike => bike.UserId != currentUserId).ToList();
            if (SearchAddress == null && SearchLocation ==null && SearchModel ==null && SelectedAddresses.Length==0 && SelectedModels.Length==0)
            {
                bikes = bikes.Where(bike => bike.KycStatus != KycStatus.Rejected).ToList();
            }

            bikes = bikes.Where(b =>
                (string.IsNullOrEmpty(SearchLocation) || (b.BikeLocation == SearchLocation && b.KycStatus == KycStatus.Approved)) &&
                (string.IsNullOrEmpty(SearchAddress) || b.BikeAddress.Contains(SearchAddress, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(SearchModel) || b.BikeModel.Contains(SearchModel, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedAddresses == null || !SelectedAddresses.Any() || SelectedAddresses.Contains(b.BikeAddress)) &&
                (SelectedModels == null || !SelectedModels.Any() || SelectedModels.Contains(b.BikeModel)) &&
                (b.AvailableUpto>=DateTime.Now)
            ).ToList();
            var viewModel = new RegisteredBikeViewModel
            {
                Bikes = bikes,
                BikesAddress = bikes.Select(b => b.BikeAddress).Distinct().ToList(),
                BikeModels = bikes.Select(b => b.BikeModel).Distinct().ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Date(int bikeId)
        {
            HttpContext.Session.SetString("BikeId",bikeId.ToString());
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
            if (bike == null)
                return NotFound("Bike not found.");
            if (bike.AvailableUpto<model.DropoffDate)
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

            var rides= await _rideService.GetRidesByBikeIdAsync(bikeId);
            foreach(var ride in rides)
            {
                var existingPickup = DateTime.Parse(ride.PickupDateTime);
                var existingDropoff = DateTime.Parse(ride.DropoffDateTime);
                if (!(dropoffDateTime <= existingPickup || pickupDateTime >= existingDropoff))
                {
                    ViewData["Message"] = $"This bike is already booked from {existingPickup} to {existingDropoff}.";
                    return View("Date");
                }
            }

            TimeSpan timeDifference = dropoffDateTime - pickupDateTime;  
            double bikeRentPrice = bikes
                .Where(b => b.BikeId == bikeId)
                .Select(b => b.BikeRentPrice)
                .FirstOrDefault();
            double totalHours = timeDifference.TotalHours;
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
            var bike = await _bikeService.GetBikeByIdAsync(bikeId);
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

            var sessionData = GetSessionData();
            if (sessionData == null || !int.TryParse(HttpContext.Session.GetString("BikeId"), out int bikeId))
            {
                return BadRequest("Session data is incomplete or BikeId is invalid.");
            }

            var bikes = await _bikeService.GetAllBikes();
            var bike = bikes.FirstOrDefault(b => b.BikeId == bikeId);
            if (bike == null)
            {
                TempData["Message"] = "Bike not found.";
                return RedirectToAction("DisplayBikes");
            }

            try
            {
                var ride = new Ride
                {
                    UserId = user.Id,
                    BikeId = bikeId,
                    PickupDateTime = sessionData.PickupDateTime,
                    DropoffDateTime = sessionData.DropoffDateTime,
                    RentalStatus = RentStatus.Ongoing,
                    RideRegisteredDate = DateTime.UtcNow,
                    RentedHours = sessionData.RentedHours,
                    Amount = sessionData.TotalPrice,
                    Gst = sessionData.Gst,
                    TotalAmount = sessionData.TotalBill
                };

                await _rideService.AddRideAsync(ride);

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


                await _paymentService.AddPaymentAsync(payment);
                HttpContext.Session.Clear();
                return View(bike);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing the payment. Please try again.";
                return RedirectToAction("DisplayBikes");
            }
        }

        private SessionData GetSessionData()
        {
            var bikeId = HttpContext.Session.GetString("BikeId");
            var pickupDateTime = HttpContext.Session.GetString("PickupDateTime");
            var dropoffDateTime = HttpContext.Session.GetString("DropoffDateTime");
            var totalPrice = HttpContext.Session.GetString("TotalPrice");
            var rentedHours = HttpContext.Session.GetString("RentedHours");
            var gst = HttpContext.Session.GetString("Gst");
            var totalBill = HttpContext.Session.GetString("TotalBill");
            if (string.IsNullOrEmpty(bikeId) || string.IsNullOrEmpty(pickupDateTime) ||
                string.IsNullOrEmpty(dropoffDateTime) || string.IsNullOrEmpty(totalPrice) ||
                string.IsNullOrEmpty(rentedHours) || string.IsNullOrEmpty(gst) ||
                string.IsNullOrEmpty(totalBill))
            {
                return null;
            }

            return new SessionData
            {
                BikeId = int.Parse(bikeId),
                PickupDateTime = pickupDateTime,
                DropoffDateTime = dropoffDateTime,
                TotalPrice = totalPrice,
                RentedHours = rentedHours,
                Gst = gst,
                TotalBill = totalBill
            };
        }
        private string GenerateTransactionId()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }

    }
}
