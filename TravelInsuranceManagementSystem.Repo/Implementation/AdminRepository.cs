using Microsoft.EntityFrameworkCore;

using TravelInsuranceManagementSystem.Repo.Models;

using TravelInsuranceManagementSystem.Models;

using TravelInsuranceManagementSystem.Repo.Data;

using TravelInsuranceManagementSystem.Repo.Interfaces;

namespace TravelInsuranceManagementSystem.Repo.Implementation

{

    public class AdminRepository : IAdminRepository

    {

        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)

        {

            _context = context;

        }

        public async Task<List<Policy>> GetAllPoliciesWithMembersAsync() =>

            await _context.Policies.Include(p => p.Members).OrderByDescending(p => p.PolicyId).ToListAsync();

        public async Task<List<Payment>> GetAllPaymentsWithPoliciesAsync() =>
            await _context.Payments
                .AsNoTracking()
                .Include(p => p.Policy)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

        public async Task<List<ClaimViewModel>> GetClaimsOverviewAsync()

        {

            return await (from c in _context.Claims

                          join p in _context.Policies on c.PolicyId equals p.PolicyId

                          join u in _context.Users on p.UserId equals u.Id

                          join a in _context.Users on c.AgentId equals a.Id into agentGroup

                          from agent in agentGroup.DefaultIfEmpty()

                          orderby c.ClaimDate descending

                          select new ClaimViewModel

                          {

                              ClaimId = c.ClaimId,

                              PolicyId = c.PolicyId,

                              CustomerId = u.Id,

                              CustomerName = u.FullName,

                              Plan = p.CoverageType,

                              CoverageType = c.IncidentType,

                              Amount = c.ClaimAmount,

                              ClaimDate = c.ClaimDate,

                              Status = c.Status.ToString(),

                              AgentName = agent != null ? agent.FullName : "Unassigned"

                          }).ToListAsync();

        }

        // ---  DASHBOARD LOGIC ---

        public async Task<AdminDashboardViewModel> GetDashboardSummaryAsync()

        {

            var dto = new AdminDashboardViewModel();

            // 1. KPI Counts

            dto.ActivePolicies = await _context.Policies.CountAsync(p => p.PolicyStatus == PolicyStatus.ACTIVE);

            dto.OpenClaims = await _context.Claims.CountAsync(c => c.Status == ClaimStatus.Pending);

            dto.OpenTickets = await _context.SupportTickets.CountAsync(t => t.TicketStatus == "Open");

            // 2. Revenue (Today)

            var today = DateTime.Today;

            dto.TodayRevenue = await _context.Payments

                .Where(p => p.PaymentStatus == PaymentStatus.SUCCESS && p.PaymentDate.Date == today)

                .SumAsync(p => p.PaymentAmount);

            // 3. Fetch Recent Activities (Top 5 from each category)

            // Policies

            var recentPolicies = await _context.Policies

                .Include(p => p.User)

                .OrderByDescending(p => p.TravelStartDate)

                .Take(5)

                .Select(p => new AdminDashboardActivity

                {

                    Type = "Policy",

                    ReferenceId = "P-" + p.PolicyId,

                    CustomerName = p.User != null ? p.User.FullName : "Unknown",

                    Date = p.TravelStartDate,

                    Status = p.PolicyStatus.ToString(),

                    StatusColor = p.PolicyStatus == PolicyStatus.ACTIVE ? "badge-approved" : "badge-pending"

                }).ToListAsync();

            // Claims

            var recentClaims = await _context.Claims

                .Include(c => c.Policy).ThenInclude(p => p.User)

                .OrderByDescending(c => c.ClaimDate)

                .Take(5)

                .Select(c => new AdminDashboardActivity

                {

                    Type = "Claim",

                    ReferenceId = "C-" + c.ClaimId,

                    CustomerName = c.Policy.User != null ? c.Policy.User.FullName : "Unknown",

                    Date = c.ClaimDate,

                    Status = c.Status.ToString(),

                    StatusColor = c.Status == ClaimStatus.Approved ? "badge-approved" : (c.Status == ClaimStatus.Rejected ? "badge-rejected" : "badge-pending")

                }).ToListAsync();

            // Tickets

            var recentTickets = await _context.SupportTickets

                .OrderByDescending(t => t.CreatedDate)

                .Take(5)

                .Select(t => new AdminDashboardActivity

                {

                    Type = "Ticket",

                    ReferenceId = "T-" + t.TicketId,

                    CustomerName = "User #" + t.UserId,

                    Date = t.CreatedDate,

                    Status = t.TicketStatus,

                    StatusColor = t.TicketStatus == "Open" ? "badge-pending" : "badge-open" // reusing badge-open style for Resolved/Closed if needed

                }).ToListAsync();

            // 4. Merge & Sort

            dto.RecentActivities.AddRange(recentPolicies);

            dto.RecentActivities.AddRange(recentClaims);

            dto.RecentActivities.AddRange(recentTickets);

            dto.RecentActivities = dto.RecentActivities

                .OrderByDescending(a => a.Date)

                .Take(10)

                .ToList();

            return dto;

        }

    }

}
