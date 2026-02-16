using Microsoft.EntityFrameworkCore;
using TravelInsuranceManagementSystem.Repo.Data;
using TravelInsuranceManagementSystem.Repo.Models;
using TravelInsuranceManagementSystem.Repo.Interfaces;

namespace TravelInsuranceManagementSystem.Repo.Implementation
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly ApplicationDbContext _context;

        public PolicyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Policy GetById(int id)
        {
            return _context.Policies.FirstOrDefault(p => p.PolicyId == id);
        }

        public async Task AddPolicyAsync(Policy policy) =>
            await _context.Policies.AddAsync(policy);

        public async Task SaveAsync() =>
            await _context.SaveChangesAsync();

        public async Task<int> CreateFamilyPolicyAsync(FamilyInsuranceDto data, int userId)
        {
            // 1. Logic: Calculate Coverage Amount
            decimal coverage = data.PolicyDetails.PlanType == "Premium" ? 50000 : 100000;

            // 2. Logic: Create Policy Object & Map Members
            var newPolicy = new Policy
            {
                UserId = userId,
                DestinationCountry = data.PolicyDetails.Destination,
                TravelStartDate = data.PolicyDetails.TripStart,
                TravelEndDate = data.PolicyDetails.TripEnd,
                CoverageType = data.PolicyDetails.PlanType,             
                PolicyStatus = PolicyStatus.PENDING,
                CoverageAmount = coverage,
                Members = data.Members.Select(m => new PolicyMember
                {
                    Title = m.Title,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Relation = m.Relation,
                    DOB = m.DOB,
                    Mobile = m.Mobile
                }).ToList()
            };

            // 3. Logic: Save to DB
            await _context.Policies.AddAsync(newPolicy);
            await _context.SaveChangesAsync();

            return newPolicy.PolicyId;
        }
    }
}