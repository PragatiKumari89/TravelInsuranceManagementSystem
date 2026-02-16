using Microsoft.AspNetCore.Mvc;
using TravelInsuranceManagementSystem.Repo.Models;
using TravelInsuranceManagementSystem.Services.Interfaces;

namespace TravelInsuranceManagementSystem.Application.Controllers
{
    public class InsuranceController : Controller
    {
        private readonly IPolicyService _policyService;

        public InsuranceController(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        [HttpGet]
        public IActionResult FamilyInsurance() => View();

        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFamily([FromBody] FamilyInsuranceDto data)
        {
            if (data == null || data.PolicyDetails == null)
                return BadRequest("No data received.");

            try
            {
                // Get User ID from Identity Claims safely
                var userIdString = User.FindFirst("UserId")?.Value;
                int userId = string.IsNullOrEmpty(userIdString) ? 0 : int.Parse(userIdString);

                // Call Service (which now delegates logic to Repo)
                int policyId = await _policyService.CreateFamilyPolicyAsync(data, userId);

                return Ok(new { message = "Policy generated successfully!", id = policyId });
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, "Internal server error: " + innerMsg);
            }
        }

        [HttpGet]
        public IActionResult Success(int id)
        {
            ViewBag.PolicyDisplayId = "P-" + id;
            return View();
        }
    }
}