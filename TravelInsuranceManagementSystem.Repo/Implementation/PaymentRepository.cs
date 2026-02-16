using Microsoft.EntityFrameworkCore;
using TravelInsuranceManagementSystem.Models;
using TravelInsuranceManagementSystem.Repo.Data;
using TravelInsuranceManagementSystem.Repo.Interfaces;
using TravelInsuranceManagementSystem.Repo.Models;

namespace TravelInsuranceManagementSystem.Repo.Implementation
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Payment GetPaymentById(int id)
        {
            return _context.Payments
                .Include(p => p.Policy)
                .FirstOrDefault(p => p.PaymentId == id);
        }

        public Payment GetOrCreatePayment(int policyId)
        {
            var existing = _context.Payments
                .Include(p => p.Policy)
                .FirstOrDefault(p => p.PolicyId == policyId && p.PaymentStatus == PaymentStatus.PENDING);

            if (existing != null) return existing;

            var policy = _context.Policies
                .Include(p => p.Members)
                .FirstOrDefault(p => p.PolicyId == policyId);

            if (policy == null) return null;

            int memberCount = (policy.Members != null && policy.Members.Any()) ? policy.Members.Count : 1;
            decimal ratePerPerson = 0;
            if (policy.CoverageType == "Premium")
            {
                ratePerPerson = 6000;
            }
            else
            {
                ratePerPerson = 4000;
            }

            decimal totalAmount = ratePerPerson * memberCount;

            var newPayment = new Payment
            {
                PolicyId = policyId,
                PaymentDate = DateTime.Now,
                PaymentStatus = PaymentStatus.PENDING,
                PaymentAmount = totalAmount
            };

            _context.Payments.Add(newPayment);
            _context.SaveChanges();

            newPayment.Policy = policy;
            return newPayment;
        }

        public bool ExecutePaymentProcessing(int paymentId, string cardNumber)
        {
           
            var payment = _context.Payments
                .Include(p => p.Policy)
                .FirstOrDefault(p => p.PaymentId == paymentId);
           
            if (payment == null) return false;

            bool isFailed = payment.PaymentAmount <= 0 ||
                            (!string.IsNullOrEmpty(cardNumber) && cardNumber.Replace(" ", "") == "0000000000000000");

            if (isFailed)
            {
                payment.PaymentStatus = PaymentStatus.FAILED;
            }
            else
            {
                payment.PaymentStatus = PaymentStatus.SUCCESS;

                // Update Policy Status on Success 
                if (payment.Policy != null)
                {
                    payment.Policy.PolicyStatus = PolicyStatus.ACTIVE;
                    _context.Policies.Update(payment.Policy);
                }
 
            }

            payment.PaymentDate = DateTime.Now;
            _context.Payments.Update(payment);
            _context.SaveChanges();

            return payment.PaymentStatus == PaymentStatus.SUCCESS;
        }
    }
}