using Microsoft.AspNetCore.Mvc;

using TravelInsuranceManagementSystem.Services.Interfaces;

namespace TravelInsuranceManagementSystem.Application.Controllers

{
   public class PaymentsController : Controller

    {

        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)

        {

            _paymentService = paymentService;

        }

        public IActionResult Checkout(int insuranceId)

        {

            var payment = _paymentService.InitializePayment(insuranceId);

            if (payment == null) return NotFound("Error: Policy not found.");

            return View(payment);

        }

        [HttpPost]

        public IActionResult ProcessPayment(int paymentId, string paymentMethod, string CardNumber)

        {

            bool isSuccess = _paymentService.ProcessPayment(paymentId, CardNumber);

            if (isSuccess)

            {

                return RedirectToAction("PaymentConfirmation", new { id = paymentId });

            }

            return RedirectToAction("PaymentFailed");

        }

        public IActionResult PaymentConfirmation(int id)

        {
            var payment = _paymentService.GetPaymentDetails(id);

            if (payment == null) return NotFound();

            return View(payment);
        }

        public IActionResult PaymentFailed() => View();

    }

}
