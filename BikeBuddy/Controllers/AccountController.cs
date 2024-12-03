using System.Security.Claims;
namespace BikeBuddy.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public readonly EmailSender emailSender;
        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,EmailSender emailSender,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                        SendEmail(model, user);
                        TempData["Message"] = "A confirmation link has been sent to your email.";
                        model.Schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
                        return View(model);
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
                                TempData["Message"] = "Unauthorized role.";
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
                    TempData["Message"] = "User does not exist.";
                }
            }
            model.Schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
            return View(model);
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

        public IActionResult Index()
        {
            return View();
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
                    TempData["Message"] = "If your email is registered, you'll receive a password reset link.";
                    return View("ForgotPasswordConfirmation");
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

                TempData["Message"] = "Password reset link has been sent to your email.";
                return View("Forgot");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();  
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Account");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                var loginVM = new LoginViewModel()
                {
                    Schemes = await signInManager.GetExternalAuthenticationSchemesAsync()
                };
                return View("Login",loginVM);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                TempData["Message"] = "Invalid password reset token.";
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
                TempData["Message"] = "Password has been reset successfully. Please log in.";
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

            TempData["Message"] = "Password has been reset successfully. Please log in.";
            return RedirectToAction("Login");
        }
    }
}
