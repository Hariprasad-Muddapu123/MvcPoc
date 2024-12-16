using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
namespace BikeBuddy.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public readonly EmailSender emailSender;
		private readonly IMemoryCache memoryCache;
        private static DateTime expirationTime;

        /// <summary>
        /// Constructor to initialize dependencies for the AccountController.
        /// </summary>
        /// <param name="userManager">Provides user management operations.</param>
        /// <param name="signInManager">Handles user sign-in operations.</param>
        /// <param name="emailSender">Handles email sending functionality.</param>
        /// <param name="roleManager">Manages roles in the application.</param>
        /// <param name="memoryCache">Provides in-memory caching functionality.</param>
        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,EmailSender emailSender,RoleManager<IdentityRole> roleManager, IMemoryCache memoryCache)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.roleManager = roleManager;
            this.memoryCache = memoryCache;
        }

        /// <summary>
        /// Displays the sign-up page to the user.
        /// </summary>
        /// <returns>The signup view.</returns>
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        /// <summary>
        /// Handles the sign-up form submission to register a new user.
        /// </summary>
        /// <param name="model">The registration details submitted by the user.</param>
        /// <returns>The signup view with success or error messages.</returns>
        [HttpPost]
        public async Task<IActionResult> Signup(RegisterViewModel model)
        {
            // Check if the submitted model is valid.
            if (ModelState.IsValid)
            {
                // Check if the email is already taken.
                var existingEmail = await userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "This email address is already taken.");
                    return View(model);  
                }
                // Check if the mobile number is already taken.
                var existingMobile = await userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == model.Mobile);
                if (existingMobile != null)
                {
                    ModelState.AddModelError("Mobile", "This mobile number is already taken.");
                    return View(model); 
                }

                // Create a new user object with the submitted details.
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.Mobile
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Ensure the "User" role exists, and create it if it doesn't.
                    if (!await roleManager.RoleExistsAsync("User"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("User"));
                    }
                    // Assign the "User" role to the newly created user.
                    var roleResult = await userManager.AddToRoleAsync(user, "User");
                    if (!roleResult.Succeeded)
                    {
                        // Handle errors that occurred while adding the role.
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return View(model);
                    }
                    // Show a success message after successful registration.
                    ViewBag.SuccessMessage = "Registration successful! Please log in to continue.";
                    return View();
                }
                // Handle errors that occurred during user creation.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // Return the view with any validation or creation errors.
            return View(model);
        }

        /// <summary>
        /// Displays the login page with external authentication schemes.
        /// </summary>
        /// <returns>The login view with external schemes.</returns>
        [HttpGet]
        public async Task<IActionResult> Login(String? returnUrl)
        {
            var loginVM = new LoginViewModel()
            {
                Schemes = await signInManager.GetExternalAuthenticationSchemesAsync(),
                ReturnUrl=returnUrl
            };
            return View(loginVM);
        }

        /// <summary>
        /// Handles the login form submission to authenticate the user.
        /// </summary>
        /// <param name="model">The login details submitted by the user.</param>
        /// <param name="ReturnUrl">The URL to redirect after successful login.</param>
        /// <returns>The appropriate view or redirect based on the login result.</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Check if the submitted model is valid.
            if (ModelState.IsValid)
            {
                // Find the user by username
                var user = await userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    // Check if the user's email is confirmed
                    if (!user.EmailConfirmed)
                    {
                        await SendOtpAsync(user.Email);
                        expirationTime = DateTime.Now.AddMinutes(10);
                        var countdownTime = expirationTime - DateTime.Now;
                      
                        ViewBag.Message = "An OTP has been sent to your email for verification.";
                        return RedirectToAction("VerifyOtp", new { Email = user.Email, ExpirationTime = countdownTime});
                    }
                    else
                    {
                        // Authenticate the user
                        var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, true, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            // Get the user's roles
                            var roles = await userManager.GetRolesAsync(user);

                            // Redirect based on role
                            if (roles.Contains("Admin"))
                            {
                                return RedirectToAction("Index", "Admin");
                            }
                            else if (roles.Contains("User"))
                            {
                                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                                {
                                    return Redirect(model.ReturnUrl);
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                            else
                            {
                                ViewBag.Message = "Unauthorized role.";
                                model.Schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
                                return View(model);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid Password.");
                            model.Schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
                            return View(model);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid User Name.");
                    ViewBag.Message = "User does not exist.";
                }
            }
            // Populate external authentication schemes and re-display the login view
            model.Schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
            return View(model);
        }
        /// <summary>
        /// Displays the OTP verification page to the user.
        /// This action is triggered after the user tries to log in, and their email is not confirmed.
        /// </summary>
        /// <param name="email">The email address of the user to verify.</param>
        /// <returns>The OTP verification view with the user's email passed as a model.</returns>
        [HttpGet]
		public IActionResult VerifyOtp(string email,TimeSpan expirationTime)
		{
			var model = new VerifyOtpViewModel { Email = email, ExpirationTime= expirationTime };
            return View(model);
		}

        /// <summary>
        /// Handles the OTP verification process when the user submits the OTP.
        /// Verifies the OTP against the one stored in memory cache and updates the user's email confirmation status.
        /// </summary>
        /// <param name="model">The model containing the OTP entered by the user and their email address.</param>
        /// <returns>The appropriate view based on whether the OTP is valid or expired.</returns>
        [HttpPost]
		public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
		{
            // Check if the OTP exists in memory cache.
            if (!memoryCache.TryGetValue($"OTP_{model.Email}", out string cachedOtp))
			{
                // If OTP doesn't exist or has expired, show an error message and return to the view.
                ModelState.AddModelError("", "The OTP has expired. Please request a new one.");
				return View(model);
			}

            // Compare the OTP entered by the user with the cached OTP.
            if (cachedOtp != model.Otp)
			{
                // If the OTP doesn't match, show an error message and return to the view.
                ModelState.AddModelError("", "The OTP is invalid.");
				return View(model);
			}
            var countdownTime = expirationTime - DateTime.Now;
            model.ExpirationTime = countdownTime;

            // Find the user by email and mark their email as confirmed.
            var user = await userManager.FindByEmailAsync(model.Email);
			user.EmailConfirmed = true;

            // Update the user's email confirmation status in the database.
            await userManager.UpdateAsync(user);

            // Remove the OTP from the memory cache after successful verification.
            memoryCache.Remove($"OTP_{model.Email}");

            // Show a success message
            ViewBag.Message = "Your email has been successfully verified.";

            // Redirect the user to the login page after successful verification.
            return RedirectToAction("Login");
		}

        /// <summary>
        /// Handles the process of resending the OTP to the user's email.
        /// This is typically used when the user requests a new OTP after the previous one expired or was not received.
        /// </summary>
        /// <param name="email">The email address of the user to resend the OTP to.</param>
        /// <returns>A JSON response indicating success or failure with an appropriate message.</returns>
        [HttpPost]
        public async Task<IActionResult> ResendOtp(string email)
        {
            // Check if the email parameter is not provided or is empty.
            if (string.IsNullOrWhiteSpace(email))
            {
                // Return a JSON response with failure message if email is missing.
                return Json(new { success = false, message = "Email is required." });
            }

            // Find the user by their email address.
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Return a JSON response with failure message if the user does not exist.
                return Json(new { success = false, message = "Email does not exist." });
            }

            // Check if the user's email is already confirmed.
            if (user.EmailConfirmed)
            {
                // Return a JSON response indicating that the email is already verified.
                return Json(new { success = false, message = "Email is already verified." });
            }


            try
            {
                // Attempt to send the OTP to the user's email.
                await SendOtpAsync(email);

                // Return a JSON response indicating success when OTP is resent.
                return Json(new { success = true, message = "OTP resent successfully." });
            }
            catch (Exception ex)
            {
                // Return a JSON response with failure message if an error occurs while sending OTP.
                return Json(new { success = false, message = "An error occurred while resending OTP." });
            }
        }

        /// <summary>
        /// Generates and sends a One-Time Password (OTP) to the user's email for verification.
        /// The OTP is stored in memory cache for 10 minutes.
        /// </summary>
        /// <param name="email">The email address where the OTP will be sent.</param>
        private async Task SendOtpAsync(string email)
        {
            // Generate a random 6-digit OTP.
            var otp = new Random().Next(100000, 999999).ToString();

            // Store the OTP in memory cache with a 10-minute expiration time.
            memoryCache.Set($"OTP_{email}", otp, TimeSpan.FromMinutes(10));

            // Send the OTP via email using the EmailSender service.
            await emailSender.SendEmailAsync(
                email,
                "Email Verification OTP",
                $"Your OTP for email verification is: <strong>{otp}</strong>");
        }

        /// <summary>
        /// Sends a confirmation email to the user with a link to confirm their email address.
        /// The link contains a token for email verification.
        /// </summary>
        /// <param name="model">The login model submitted by the user.</param>
        /// <param name="user">The user object containing the user's information.</param>
        private async void SendEmail(LoginViewModel model, User user)
        {
            // Generate an email confirmation token for the user.
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            // Create a callback URL for the user to confirm their email.
            var callbackUrl = Url.Action(
                "ConfirmEmail", "Account",
                new { token, UserId = user.Id },
                protocol: HttpContext.Request.Scheme);

            // Send the email with the confirmation link to the user's email address.
            await emailSender.SendEmailAsync(
                user.Email,
                "Confirm Email",
                $"Please confirm your email by clicking here: <a href='{callbackUrl}'>link</a>");
        }

        /// <summary>
        /// Logs the user out by signing them out using the SignInManager and redirects to the Home page.
        /// </summary>
        /// <returns>A redirect to the Home page after successful logout.</returns>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user.
            await signInManager.SignOutAsync();

            // Redirect to the Home page after logout.
            return RedirectToAction("index", "Home");
        }

        /// <summary>
        /// Displays the Forgot Password page where users can enter their email to reset their password.
        /// </summary>
        /// <returns>The Forgot Password view.</returns>
        [HttpGet]
        public IActionResult Forgot()
        {
            return View();
        }

        /// <summary>
        /// Handles the Forgot Password form submission by verifying the email and sending a reset link.
        /// If the email is valid and confirmed, an email with a password reset link is sent to the user.
        /// </summary>
        /// <param name="model">The Forgot Password view model containing the user's email.</param>
        /// <returns>The Forgot Password view with a success or error message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotViewModel model)
        {
            // Check if the model state is valid (email provided).
            if (ModelState.IsValid)
            {
                // Find the user by email.
                var user = await userManager.FindByEmailAsync(model.Email);

                // If user not found or email not confirmed, return an error message.
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    ViewBag.Message = "The email you entered is not registered. Please enter a valid email to reset your password.";
                    return View("Forgot");
                }

                // Generate a password reset token for the user.
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                // Create a callback URL that the user will use to reset their password.
                var callbackUrl = Url.Action(
                    "ResetPassword", "Account",
                    new { token, email = user.Email },
                    protocol: HttpContext.Request.Scheme);

                // Send an email with the password reset link.
                await emailSender.SendEmailAsync(
                    model.Email,
                    "Reset Password",
                    $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                // Provide a success message.
                ViewBag.Message = "Password reset link has been sent to your email.";
                return View("Forgot");
            }

            // If the model state is invalid, return the view with the errors.
            return View(model);
        }


        /// <summary>
        /// Displays the Reset Password page with the token and email passed as query parameters.
        /// </summary>
        /// <param name="token">The password reset token sent to the user's email.</param>
        /// <param name="email">The email of the user requesting the password reset.</param>
        /// <returns>The Reset Password view with the token and email for the user to set a new password.</returns>
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            // If either the token or email is missing, show an error message and redirect to the ForgotPassword page.
            if (token == null || email == null)
            {
                ViewBag.Message = "Invalid password reset token.";
                return RedirectToAction("ForgotPassword");
            }

            // Create a view model with the token and email, then return the Reset Password view.
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        /// <summary>
        /// Handles the submission of the Reset Password form. It resets the user's password if the model is valid.
        /// </summary>
        /// <param name="model">The ResetPasswordViewModel containing the new password and token.</param>
        /// <returns>A redirect to the Login page if successful or the Reset Password view with errors if failed.</returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            // If the model state is invalid (e.g., password does not meet criteria), return the view with errors.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find the user by email.
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // If the user is not found, show a message that the password reset was successful and redirect to login.
                ViewBag.Message = "Password has been reset successfully. Please log in.";
                return RedirectToAction("Login");
            }

            // Reset the user's password using the provided token and new password.
            var resetPassResult = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            // If the password reset fails, add the error messages to the ModelState and return the view with errors.
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // If successful, show a success message and redirect the user to the Login page.
            ViewBag.Message = "Password has been reset successfully. Please log in.";
            return RedirectToAction("Login");
        }


        /// <summary>
        /// Initiates the Google login process by redirecting the user to the Google authentication provider.
        /// </summary>
        /// <param name="provider">The authentication provider (e.g., Google).</param>
        /// <param name="returnUrl">The URL to redirect to after a successful login. Optional parameter.</param>
        /// <returns>A ChallengeResult to initiate the external authentication process.</returns>
        [HttpGet]
        public IActionResult GoogleLogin(string provider, string returnUrl = "")
        {
            // Generate the URL where the user will be redirected after a successful login (callback action).
            var redirectUrl = Url.Action("GoogleLoginCallBack", "Account", new { ReturnUrl = returnUrl });

            // Configure the external authentication properties, including the redirect URL and prompt option.
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.Items["prompt"] = "select_account";  // Forces Google login to display account selection if multiple accounts exist.

            // Redirect to the external authentication provider (Google).
            return new ChallengeResult(provider, properties);
        }

        /// <summary>
        /// Handles the callback from the Google authentication provider after a user has authenticated.
        /// </summary>
        /// <param name="remoteError">Any error returned by the external provider, if applicable.</param>
        /// <param name="returnUrl">The URL to redirect to after a successful login. Optional parameter.</param>
        /// <returns>A View (Login) if there is an error, or redirects the user based on their role after successful authentication.</returns>
        public async Task<IActionResult> GoogleLoginCallBack(string remoteError, string returnUrl = "")
        {
            var loginVM = new LoginViewModel()
            {
                // Get all available external authentication schemes (e.g., Google).
                Schemes = await signInManager.GetExternalAuthenticationSchemesAsync()
            };

            // If there was an error from the external provider, display the error message.
            if (remoteError != null)
            {
                ModelState.AddModelError("", $"Error from google login provider: {remoteError}");
                return View("Login", loginVM);
            }

            // Get external login information after the user successfully authenticated.
            var info = await signInManager.GetExternalLoginInfoAsync();

            // If there was an issue retrieving the external login info, show an error.
            if (info == null)
            {
                ModelState.AddModelError("", "Error from google login provider: Unable to retrieve user info.");
                return View("Login", loginVM);
            }

            // Find the user by the email associated with the external login.
            var user = await userManager.FindByEmailAsync(info.Principal?.FindFirst(ClaimTypes.Email)?.Value);
            if (user == null)
            {
                ModelState.AddModelError("", "The external login was unsuccessful.");
                return View("Login", loginVM);
            }

            // Check if the user is assigned to the "Admin" or "User" role.
            var roles = await userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                // Sign in the user as an Admin and redirect to the Admin dashboard.
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Admin");
            }
            else if (roles.Contains("User"))
            {
                // Sign in the user as a regular user and redirect to the Home page.
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // If the user doesn't have an authorized role, display an error message.
                TempData["Message"] = "Unauthorized role.";
                return View("Login", loginVM);
            }
        }

    }
}
