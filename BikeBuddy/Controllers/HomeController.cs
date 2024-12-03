using System.Diagnostics;
using System.Security.Claims;

namespace BikeBuddy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Route()
        {
            var userRoles=User.FindAll(ClaimTypes.Role).Select(r=>r.Value).ToList();
            if (userRoles.Contains("Admin"))
                return RedirectToAction("Index", "Admin");
            else
                return RedirectToAction("Index", "Home");
        }
        public IActionResult Index(string type="owner")
        {
            ViewData["ServiceType"]=type;
            return View();
        }

        public IActionResult Safety()
        {
            return View();
        }
        public IActionResult Aboutus()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult HandleError(int statusCode)
        {
            if(statusCode == 404)
            {
                return View("Not Found");
            }
            return View("Error");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
