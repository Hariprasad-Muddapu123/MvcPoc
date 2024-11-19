using Microsoft.AspNetCore.Mvc;

namespace BikeBuddy.Controllers
{
    public class RentRideController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Rent()
        {
            return View();
        }
    }
}
