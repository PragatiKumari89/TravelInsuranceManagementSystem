using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;

using TravelInsuranceManagementSystem.Repo.Models;

using TravelInsuranceManagementSystem.Services.Interfaces;

namespace TravelInsuranceManagementSystem.Application.Controllers

{

    [Authorize(Roles = "Agent")]

    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

    public class AgentController : Controller

    {

        private readonly IAgentService _agentService;

        private readonly SignInManager<User> _signInManager;

        public AgentController(IAgentService agentService, SignInManager<User> signInManager)

        {

            _agentService = agentService;

            _signInManager = signInManager;

        }

        //  UPDATED DASHBOARD METHOD

        public async Task<IActionResult> Dashboard()

        {

            // Fetch real data from service

            var summary = await _agentService.GetDashboardSummaryAsync();

            return View(summary);

        }

        public async Task<IActionResult> Policies()

        {

            var policies = await _agentService.GetPoliciesAsync();

            return View(policies);

        }

        public async Task<IActionResult> Claims()

        {

            var claims = await _agentService.GetClaimsAsync();

            return View(claims);

        }

        [HttpPost]
        public async Task<IActionResult> UpdateClaimStatus(int id, string status)

        {

            var agentIdString = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(agentIdString))

            {

                var user = await _signInManager.UserManager.GetUserAsync(User);

                if (user != null) agentIdString = user.Id.ToString();

            }

            if (string.IsNullOrEmpty(agentIdString))

                return Json(new { success = false, message = "Session expired. Please re-login." });

            var result = await _agentService.UpdateClaimStatusAsync(id, status, int.Parse(agentIdString));

            return Json(new { success = result.Success, message = result.Message });

        }

        public async Task<IActionResult> SupportTickets()

        {

            var tickets = await _agentService.GetSupportTicketsAsync();

            return View(tickets);

        }

        public async Task<IActionResult> Payments()

        {

            var payments = await _agentService.GetPaymentsAsync();

            return View(payments);

        }

        [HttpPost]
        public async Task<IActionResult> UpdateTicketStatus(int id, string status)

        {

            bool success = await _agentService.UpdateTicketStatusAsync(id, status);

            return success ? Json(new { success = true }) : NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> DeleteTicket(int id)

        {

            await _agentService.DeleteTicketAsync(id);

            return RedirectToAction(nameof(SupportTickets));

        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()

        {

            await _signInManager.SignOutAsync();

            HttpContext.Session.Clear();

            return RedirectToAction("SignIn", "Account");

        }

    }

}
