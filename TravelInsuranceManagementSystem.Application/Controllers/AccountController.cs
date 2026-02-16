using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Identity;

using TravelInsuranceManagementSystem.Repo.Models;

using TravelInsuranceManagementSystem.Services.Interfaces;

namespace TravelInsuranceManagementSystem.Application.Controllers

{
    public class AccountController : Controller

    {

        private readonly IAccountService _accountService;

        private readonly SignInManager<User> _signInManager;

        private readonly UserManager<User> _userManager;

        public AccountController(IAccountService accountService, SignInManager<User> signInManager, UserManager<User> userManager)

        {

            _accountService = accountService;

            _signInManager = signInManager;

            _userManager = userManager;

        }

        [HttpGet]

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult SignIn(string returnUrl = null)

        {

            // If already logged in, redirect to home

            if (User.Identity.IsAuthenticated)

            {

                return RedirectToAction("Index", "Home");

            }

            // Store the return URL (e.g., /Home/Insurance) so the View can put it in the hidden field

            ViewData["ReturnUrl"] = returnUrl;

            return View("~/Views/Home/SignIn.cshtml");

        }

        [HttpPost]

        public async Task<IActionResult> SignIn(string Email, string Password, string returnUrl = null)

        {

            // 1. Fetch user

            var user = await _accountService.GetUserByEmail(Email);

            if (user != null)

            {

                // 2. Initial password check

                var result = await _signInManager.PasswordSignInAsync(user, Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)

                {

                    // 3. Create Claims List
                    var claims = new List<System.Security.Claims.Claim>

                    {

                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role),

                        new System.Security.Claims.Claim("UserId", user.Id.ToString()),

                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email),

                        // Add FullName so your dashboards show the Name instead of Email

                        new System.Security.Claims.Claim("FullName", user.FullName ?? user.Email.Split('@')[0])

                    };

                    // 4. Sign in with the specific claims attached

                    await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);

                    // 5. REDIRECTION LOGIC

                    // Case A: If user came from a specific page (like Insurance), go back there

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))

                    {

                        return LocalRedirect(returnUrl);

                    }

                    // Case B: Default Dashboard Redirection based on Role

                    return user.Role switch

                    {

                        "Admin" => RedirectToAction("Dashboard", "Admin"),

                        "Agent" => RedirectToAction("Dashboard", "Agent"),

                        _ => RedirectToAction("Index", "Home") // Users go to Home Page

                    };

                }

            }

            TempData["ErrorMessage"] = "Invalid email or password!";

            ViewData["ReturnUrl"] = returnUrl; // Keep the return URL even if login fails

            return View("~/Views/Home/SignIn.cshtml");

        }

        [HttpPost]

        public async Task<IActionResult> SignUp(User user)

        {

            var result = await _accountService.RegisterUser(user, user.Password);

            if (result.Succeeded)

            {

                TempData["SuccessMessage"] = "Registration successful! Please sign in.";

                return RedirectToAction("SignIn");

            }

            TempData["ErrorMessage"] = string.Join(" ", result.Errors.Select(e => e.Description));

            return View("~/Views/Home/SignIn.cshtml");

        }

        [HttpGet]

        public IActionResult AccessDenied()

        {

            return View();

        }

        [HttpGet]

        public IActionResult ForgotPassword() => View();

        [HttpPost]

        public async Task<IActionResult> ForgotPassword(string email)

        {

            var user = await _accountService.GetUserByEmail(email);

            if (user == null)

            {

                ViewBag.Error = "Email address not found.";

                return View();

            }

            var token = await _accountService.GeneratePasswordResetToken(user);

            return RedirectToAction("ResetPassword", new { email = email, token = token });

        }

        [HttpGet]

        public IActionResult ResetPassword(string email, string token)

        {

            ViewBag.Email = email;

            ViewBag.Token = token;

            return View();

        }

        [HttpPost]

        public async Task<IActionResult> ResetPassword(string email, string token, string newPassword, string confirmPassword)

        {

            if (newPassword != confirmPassword)

            {

                ViewBag.Error = "Passwords do not match.";

                ViewBag.Email = email;

                ViewBag.Token = token;

                return View();

            }

            var user = await _accountService.GetUserByEmail(email);

            if (user != null)

            {

                var result = await _accountService.ResetPassword(user, token, newPassword);

                if (result.Succeeded)

                {

                    TempData["SuccessMessage"] = "Password reset successfully!";

                    return RedirectToAction("SignIn");

                }

                ViewBag.Error = string.Join(" ", result.Errors.Select(e => e.Description));

            }

            return View();

        }

        public async Task<IActionResult> Logout()

        {

            await _signInManager.SignOutAsync();

            HttpContext.Session.Clear(); // Added clear session just in case

            return RedirectToAction("Index", "Home");

        }

    }

}
