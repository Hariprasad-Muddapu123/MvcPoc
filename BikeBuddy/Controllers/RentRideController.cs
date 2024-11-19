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
        public IActionResult Rent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rent(BikeViewModel model)
        {
            if (ModelState.IsValid)
            {


                // Handle BikeImage upload
                //if (model.BikeImage != null)
                //{
                //    var bikeImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "BikeImages");
                //    Directory.CreateDirectory(bikeImagePath);
                //    var imageFileName = Guid.NewGuid() + Path.GetExtension(model.BikeImage.FileName);
                //    var imagePath = Path.Combine(bikeImagePath, imageFileName);

                //    using (var stream = new FileStream(imagePath, FileMode.Create))
                //    {
                //        await model.BikeImage.CopyToAsync(stream);
                //    }

                //    model.BikeImageBytes = System.IO.File.ReadAllBytes(imagePath);
                //}

                //// Handle BikeDocuments upload
                //if (model.BikeDocuments != null)
                //{
                //    var bikeDocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "BikeDocuments");
                //    Directory.CreateDirectory(bikeDocumentPath);
                //    var documentFileName = Guid.NewGuid() + Path.GetExtension(model.BikeDocuments.FileName);
                //    var documentPath = Path.Combine(bikeDocumentPath, documentFileName);

                //    using (var stream = new FileStream(documentPath, FileMode.Create))
                //    {
                //        await model.BikeDocuments.CopyToAsync(stream);
                //    }

                //    model.BikeDocumentsBytes = System.IO.File.ReadAllBytes(documentPath);
                //}

                var user = await _userManager.GetUserAsync(User);
                var bike = new Bike
                {
                    BikeModel = model.BikeModel,
                    BikeNumber = model.BikeNumber,
                    BikeLocation = model.BikeLocation,
                    BikeAddress = model.BikeAddress,
                    KycStatus = model.KycStatus,
                    UserId = user.Id,
                    RegistrationDate = DateTime.UtcNow
                };
                if (model.BikeImage != null && model.BikeImage.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.BikeImage.CopyToAsync(memoryStream);
                        bike.BikeImageBytes = memoryStream.ToArray();
                    }
                }
                if (model.BikeDocuments != null && model.BikeDocuments.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.BikeDocuments.CopyToAsync(memoryStream);
                        bike.BikeDocumentsBytes = memoryStream.ToArray();
                    }
                }

                //Save to the database(example only, replace with your actual DbContext logic)
                _context.Bikes.Add(bike);
                var result=await _context.SaveChangesAsync();
                
                TempData["Message"] = result==1 ? "Bike registered successfully!" : "Failed to upload bike details";
                return RedirectToAction("Rent");
            }

            TempData["ErrorMessage"] = "Failed to register the bike. Please check the inputs.";
            return View(model);
        }

    }
}
