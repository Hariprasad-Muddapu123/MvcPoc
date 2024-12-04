namespace BikeBuddy.Controllers
{
    /// <summary>
    /// Controller to manage user-related actions, including profile management, rented bikes, and ride history.
    /// </summary>
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBikeService _bikeService;
        private readonly IRideService _rideService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">Service for managing user-related operations.</param>
        /// <param name="bikeService">Service for managing bike-related operations.</param>
        /// <param name="rideService">Service for managing ride-related operations.</param>
        public UserController(IUserService userService, IBikeService bikeService, IRideService rideService)
        {
            _userService = userService;
            _bikeService = bikeService;
            _rideService = rideService;
        }

        /// <summary>
        /// Displays the user's profile information.
        /// </summary>
        /// <returns>A View with the profile information if the user is logged in; otherwise, redirects to the login page.</returns>
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Retrieve the current user based on the logged-in username.
            var user = await _userService.GetCurrentUserAsync(User.Identity.Name);

            if (user == null)
                // Redirect to Login if no user is found.
                return RedirectToAction("Login", "Account");

            // Get user profile data for the logged-in user.
            var model = await _userService.GetUserProfileAsync(user.Id);

            return View(model);
        }

        /// <summary>
        /// Updates the user's profile information.
        /// </summary>
        /// <param name="model">The profile data to update.</param>
        /// <returns>Redirects to the Profile view with a success or failure message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            // Retrieve the current user based on the logged-in username.
            var user = await _userService.GetCurrentUserAsync(User.Identity.Name);

            if (user == null)
                // Redirect to Login if no user is found.
                return RedirectToAction("Login", "Account");

            // Update the user's profile data.
            var result = await _userService.UpdateUserProfileAsync(user.Id, model);

            // Store the result message in TempData to show on the next view.
            TempData["Message"] = result ? "Profile updated successfully!" : "Failed to update profile.";

            return RedirectToAction("Profile");
        }

        /// <summary>
        /// Displays the list of bikes rented by the user.
        /// </summary>
        /// <returns>A View with the user's rented bikes or a NotFound result if no bikes are found.</returns>
        public async Task<IActionResult> MyRents()
        {
            // Retrieve the current user based on the logged-in username.
            var user = await _userService.GetCurrentUserAsync(User.Identity.Name);

            // Get all bikes associated with the current user.
            var bikes = await _bikeService.GetAllBikesByUserIdAsync(user.Id);

            if (bikes == null)
            {
                // Return NotFound if no bikes are associated with the user.
                return NotFound("No Bikes found for this User");
            }

            return View(bikes);
        }

        /// <summary>
        /// Displays the list of rides booked by the user.
        /// </summary>
        /// <returns>A View with the user's ride history or a NotFound result if no rides are found.</returns>
        public async Task<IActionResult> MyRides()
        {
            // Retrieve the current user based on the logged-in username.
            var user = await _userService.GetCurrentUserAsync(User.Identity.Name);

            // Get all rides associated with the current user.
            var rides = await _rideService.GetRidesByUserIdAsync(user.Id);

            if (rides == null)
            {
                // Return NotFound if no rides are associated with the user.
                return NotFound("No Rides found for this User");
            }

            return View(rides);
        }
    }
}
