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
		public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,EmailSender emailSender,RoleManager<IdentityRole> roleManager, IMemoryCache memoryCache)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.roleManager = roleManager;
            this.memoryCache = memoryCache;
        }
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingEmail = await userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "This email address is already taken.");
                    return View(model);  
                }
                var existingMobile = await userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == model.Mobile);

                if (existingMobile != null)
                {
                    ModelState.AddModelError("Mobile", "This mobile number is already taken.");
                    return View(model); 
                }
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.Mobile
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await roleManager.RoleExistsAsync("User"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("User"));
                    }
                    var roleResult = await userManager.AddToRoleAsync(user, "User");
                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return View(model);
                    }
                    ViewBag.SuccessMessage = "Registration successful! Please log in to continue.";
                    return View();
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var loginVM = new LoginViewModel()
            {
                Schemes = await signInManager.GetExternalAuthenticationSchemesAsync()
            };
            return View(loginVM);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        await SendOtpAsync(user.Email);
                        ViewBag.Message = "An OTP has been sent to your email for verification.";
                        return RedirectToAction("VerifyOtp", new { Email = user.Email});
                    }
                    else
                    {
                        var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, true, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            var roles = await userManager.GetRolesAsync(user);

                            if (roles.Contains("Admin"))
                            {
                                return RedirectToAction("Index", "Admin");
                            }
                            else if (roles.Contains("User"))
                            {
                                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                                {
                                    return Redirect(ReturnUrl);
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
            model.Schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
            return View(model);
        }

		[HttpGet]
		public IActionResult VerifyOtp(string email)
		{
			var model = new VerifyOtpViewModel { Email = email };
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
		{
			if (!memoryCache.TryGetValue($"OTP_{model.Email}", out string cachedOtp))
			{
				ModelState.AddModelError("", "The OTP has expired. Please request a new one.");
				return View(model);
			}

			if (cachedOtp != model.Otp)
			{
				ModelState.AddModelError("", "The OTP is invalid.");
				return View(model);
			}
			var user = await userManager.FindByEmailAsync(model.Email);
			user.EmailConfirmed = true;
			await userManager.UpdateAsync(user);
			memoryCache.Remove($"OTP_{model.Email}");
			ViewBag.Message = "Your email has been successfully verified.";
			return RedirectToAction("Login");
		}
        [HttpPost]
        public async Task<IActionResult> ResendOtp(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { success = false, message = "Email is required." });
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(new { success = false, message = "Email does not exist." });
            }

            if (user.EmailConfirmed)
            {
                return Json(new { success = false, message = "Email is already verified." });
            }

            try
            {
                await SendOtpAsync(email);
                return Json(new { success = true, message = "OTP resent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while resending OTP." });
            }
        }


        private async Task SendOtpAsync(String email)
		{
			var otp = new Random().Next(100000, 999999).ToString();
		    memoryCache.Set($"OTP_{email}", otp, TimeSpan.FromMinutes(10));
			await emailSender.SendEmailAsync(
				email,
				"Email Verification OTP",
				$"Your OTP for email verification is: <strong>{otp}</strong>");
		}

		private async void SendEmail(LoginViewModel model, User user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(
                "ConfirmEmail", "Account",
                new { token, UserId = user.Id },
                protocol: HttpContext.Request.Scheme);

            await emailSender.SendEmailAsync(
                user.Email,
                "Confirm Email",
                $"Please confirm your email by clicking here: <a href='{callbackUrl}'>link</a>");
        }


        [HttpGet]
        public IActionResult GoogleLogin(String provider, String returnUrl = "")
        {
            var redirectUrl = Url.Action("GoogleLoginCallBack", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.Items["prompt"] = "select_account";
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> GoogleLoginCallBack(string remoteError, String returnUrl = "")
        {
            var loginVM = new LoginViewModel()
            {
                Schemes = await signInManager.GetExternalAuthenticationSchemesAsync()
            };
            if (remoteError != null)
            {
                ModelState.AddModelError("", $"Error from google login provider : {remoteError}");
                return View("Login", loginVM);
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError("", $"Error from google login provider : {remoteError}");
                return View("Login", loginVM);
            }
            var user = await userManager.FindByEmailAsync(info.Principal?.FindFirst(ClaimTypes.Email)?.Value);
            var roles = await userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Admin");
            }
            else if (roles.Contains("User"))
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Message"] = "Unauthorized role.";

                return View("Login", loginVM);
            }   
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            
            return RedirectToAction("index", "Home");
        }

        [HttpGet]
        public IActionResult Forgot()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    ViewBag.Message = "The email you entered is not registered. Please enter a valid email to reset your password.";
                    return View("Forgot");
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ResetPassword", "Account",
                    new { token, email = user.Email },
                    protocol: HttpContext.Request.Scheme);

                await emailSender.SendEmailAsync(
                    model.Email,
                    "Reset Password",
                    $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                ViewBag.Message = "Password reset link has been sent to your email.";
                return View("Forgot");
            }

            return View(model);
        }
        
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ViewBag.Message = "Invalid password reset token.";
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.Message = "Password has been reset successfully. Please log in.";
                return RedirectToAction("Login");
            }

            var resetPassResult = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            ViewBag.Message = "Password has been reset successfully. Please log in.";
            return RedirectToAction("Login");
        }
    }
}
