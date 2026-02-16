using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TravelInsuranceManagementSystem.Repo.Models;

namespace TravelInsuranceManagementSystem.Application.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Insuarance()
        {
            return View("Insuarance");
        }


        [HttpGet]
        public IActionResult Family() => View("Family");

        [HttpGet]
        public IActionResult Student() => View("Student");



        public IActionResult Privacy()
        {
            return View();
        }



        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult SingleInsurance()
        {
            return View();
        }

        public IActionResult SeniorCitizen()
        {
            return View();
        }

        public IActionResult FamilyInsuranceForm()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
