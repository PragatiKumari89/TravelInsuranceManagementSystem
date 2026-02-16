using Microsoft.AspNetCore.Mvc;
using TravelInsuranceManagementSystem.Services.Interfaces;
using InsuranceClaim = TravelInsuranceManagementSystem.Repo.Models.Claim;
 
namespace TravelInsuranceManagementSystem.Application.Controllers

{
    public class ClaimsController : Controller

    {

        private readonly IClaimService _claimService;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClaimsController(IClaimService claimService, IWebHostEnvironment webHostEnvironment)

        {

            _claimService = claimService;

            _webHostEnvironment = webHostEnvironment;

        }

        [HttpGet]

        public IActionResult Create()

        {

            return View("~/Views/UserDashboard/ClaimCreate.cshtml");

        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(InsuranceClaim claim)

        {

            // 1. Get Logged-in User ID

            var userIdString = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdString))

            {

                return RedirectToAction("SignIn", "Account");

            }

            int userId = int.Parse(userIdString);

            string uniqueFileName = null;

            // 2. Handle File Upload 

            if (claim.DocumentFile != null)

            {

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))

                {

                    Directory.CreateDirectory(uploadsFolder);

                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + claim.DocumentFile.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))

                {

                    await claim.DocumentFile.CopyToAsync(fileStream);

                }

            }

            // 3. Call Service 

            var result = await _claimService.SubmitClaimAsync(claim, userId, uniqueFileName);

            if (result.Success)

            {

                TempData["SuccessMessage"] = "Claim submitted successfully!";

                return RedirectToAction("Claims", "UserDashboard");

            }

            // 4. Handle Errors

            ModelState.AddModelError("PolicyId", result.Message);

            return View("~/Views/UserDashboard/ClaimCreate.cshtml", claim);

        }

    }

}
