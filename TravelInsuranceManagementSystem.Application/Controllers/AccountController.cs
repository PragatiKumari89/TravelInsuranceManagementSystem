using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using TravelInsuranceManagementSystem.Application.Data;
using TravelInsuranceManagementSystem.Application.Models;

namespace TravelInsuranceManagementSystem.Application.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SignIn() => View("~/Views/Home/SignIn.cshtml");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(string Email, string Password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(Password, user.Password))
            {
                var claims = new List<Claim>
                {
                    // FIX: ClaimTypes.Name MUST be the Email for the Dashboard lookup to work
                    new Claim(ClaimTypes.Name, user.Email), 
                    new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                    });

                if (user.Role == "Admin") return RedirectToAction("Dashboard", "Admin");
                if (user.Role == "Agent") return RedirectToAction("Dashboard", "Agent");

                return RedirectToAction("Dashboard", "UserDashboard");
            }

            TempData["ErrorMessage"] = "Invalid email or password!";
            return View("~/Views/Home/SignIn.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(User user)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix the validation errors.";
                return View("~/Views/Home/SignIn.cshtml", user);
            }

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                TempData["ErrorMessage"] = "Email already registered!";
                return View("~/Views/Home/SignIn.cshtml");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            if (user.Email.ToLower().Contains("@admin")) user.Role = "Admin";
            else if (user.Email.ToLower().Contains("@agent")) user.Role = "Agent";
            else user.Role = "User";

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("SignIn");
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
=======
>>>>>>> 2c20d9fa570772274f1fb1bbeb45964d1ba10606
        // --- FORGOT PASSWORD LOGIC ---

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Email address not found.";
                return View();
            }
            // In a real system, you'd send an email here. 
            // For now, we redirect to reset directly for this specific email.
            return RedirectToAction("ResetPassword", new { email = email });
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string email, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                ViewBag.Email = email;
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                // Re-verify strict password rules manually for safety
                var regex = new System.Text.RegularExpressions.Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
                if (!regex.IsMatch(newPassword))
                {
                    ViewBag.Error = "Password must have 1 Uppercase, 1 Lowercase, 1 Number, and 1 Special Character.";
                    ViewBag.Email = email;
                    return View();
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _context.Update(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Password updated successfully!";
                return RedirectToAction("SignIn");
            }
            return RedirectToAction("SignIn");
        }

<<<<<<< HEAD
>>>>>>> upstream/main
=======
>>>>>>> 2c20d9fa570772274f1fb1bbeb45964d1ba10606
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();
    }
}