using System.Diagnostics;
using System.Security.Claims;

namespace BikeBuddy.Controllers
{
    /// <summary>
    /// Controller to handle the main application pages such as home, safety, about us, contact us, and privacy policy.
    /// It also handles role-based redirection and error handling.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging application events.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Redirects the user based on their roles.
        /// </summary>
        /// <returns>
        /// Redirects to the Admin Index page if the user has the Admin role, otherwise redirects to the Home Index page.
        /// </returns>
        public IActionResult Route()
        {
            // Retrieve all roles of the current user.
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            if (userRoles.Contains("Admin"))
                // Redirect to the Admin Index if the user is an Admin.
                return RedirectToAction("Index", "Admin");
            else
                // Redirect to the Home Index otherwise.
                return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Displays the Home Index view.
        /// </summary>
        /// <param name="type">The type of service (default is "owner").</param>
        /// <returns>The Index view.</returns>
        public IActionResult Index(string type = "owner")
        {
            // Pass the service type to the view via ViewData.
            ViewData["ServiceType"] = type;
            return View();
        }

        /// <summary>
        /// Displays the Safety page.
        /// </summary>
        /// <returns>The Safety view.</returns>
        public IActionResult Safety()
        {
            return View();
        }

        /// <summary>
        /// Displays the About Us page.
        /// </summary>
        /// <returns>The About Us view.</returns>
        public IActionResult Aboutus()
        {
            return View();
        }

        /// <summary>
        /// Displays the Contact Us page.
        /// </summary>
        /// <returns>The Contact Us view.</returns>
        public IActionResult ContactUs()
        {
            return View();
        }

        /// <summary>
        /// Displays the Privacy Policy page.
        /// </summary>
        /// <returns>The Privacy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Handles errors based on the HTTP status code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code of the error.</param>
        /// <returns>
        /// The appropriate error view for the given status code.
        /// For 404, it returns the Not Found view.
        /// For 500, it returns the Server Error view.
        /// For other status codes, it returns the generic Error view.
        /// </returns>
        public IActionResult HandleError(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    // Handle 404 Not Found errors.
                    return View("Not Found");
                case 500:
                    // Handle 500 Server Error.
                    return View("ServerError");
                case 440:
                    return View("SessionExpired");
                default:
                    // Handle other errors.
                    return View("Error");
            }
        }

        [HttpGet]
        public IActionResult BlockedAccount(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        /// <summary>
        /// Displays the generic error page.
        /// </summary>
        /// <returns>The Error view with a request ID for tracking.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Return the error view with the request ID for tracing.
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
