using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace TravelInsuranceManagementSystem.Application.Controllers
{
    public class AgentController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Policies()
        {
            return View();
        }

        public IActionResult Claims()
        {
            return View();
        }

        public IActionResult Payments()
        {
            return View();
        }

        public IActionResult SupportTickets()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Sign out of the authentication scheme
            await HttpContext.SignOutAsync();

            // Redirect to Sign In
            return RedirectToAction("SignIn", "Home");
        }
    }
}
