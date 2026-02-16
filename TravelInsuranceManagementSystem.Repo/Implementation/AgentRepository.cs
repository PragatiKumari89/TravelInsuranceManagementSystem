using Microsoft.EntityFrameworkCore;

using TravelInsuranceManagementSystem.Models;

using TravelInsuranceManagementSystem.Repo.Data;

using TravelInsuranceManagementSystem.Repo.Interfaces;

using TravelInsuranceManagementSystem.Repo.Models;

namespace TravelInsuranceManagementSystem.Repo.Implementation

{

    public class AgentRepository : IAgentRepository

    {

        private readonly ApplicationDbContext _context;

        public AgentRepository(ApplicationDbContext context)

        {

            _context = context;

        }

        public async Task<List<Policy>> GetPoliciesWithMembersAsync() =>

            await _context.Policies.Include(p => p.Members).Include(p => p.User).OrderByDescending(p => p.PolicyId).ToListAsync();

        public async Task<List<Claim>> GetClaimsWithCustomerAsync() =>

            await _context.Claims.Include(c => c.Policy).ThenInclude(p => p.User).OrderByDescending(c => c.ClaimDate).ToListAsync();
     
        public async Task<List<SupportTicket>> GetSupportTicketsAsync() =>

            await _context.SupportTickets

                .Include(t => t.ExtensionData) // Keeps contact info working

                .Include(t => t.User)          // Fetch Customer Details for the main list

                .OrderByDescending(t => t.TicketId)

                .ToListAsync();

        public async Task<List<Payment>> GetPaymentsWithPolicyAsync() =>
            // Return payments directly; consumers can include policy if needed.
            await _context.Payments.OrderByDescending(p => p.PaymentDate).ToListAsync();

        public async Task<(bool Success, string Message)> UpdateClaimStatusAsync(int id, string status, int agentId)

        {

            var claim = await _context.Claims.FindAsync(id);

            if (claim == null) return (false, "Claim not found");

            if (Enum.TryParse<ClaimStatus>(status, true, out var newStatus))

            {

                claim.Status = newStatus;

                claim.AgentId = agentId;

                _context.Claims.Update(claim);

                await _context.SaveChangesAsync();

                return (true, string.Empty);

            }

            return (false, "Invalid Status");

        }

        public async Task<bool> UpdateTicketStatusAsync(int id, string status)

        {

            var ticket = await _context.SupportTickets.FindAsync(id);

            if (ticket == null) return false;

            ticket.TicketStatus = status;

            _context.SupportTickets.Update(ticket);

            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> DeleteTicketAsync(int id)

        {

            var ticket = await _context.SupportTickets.FindAsync(id);

            if (ticket == null) return false;

            _context.SupportTickets.Remove(ticket);

            await _context.SaveChangesAsync();

            return true;

        }

        // --- DASHBOARD LOGIC (FIXED) ---

        public async Task<AgentDashboardViewModel> GetDashboardSummaryAsync()

        {

            var dto = new AgentDashboardViewModel();

            // 1. KPI Counts

            dto.ActivePolicies = await _context.Policies.CountAsync(p => p.PolicyStatus == PolicyStatus.ACTIVE);

            dto.ActiveClaims = await _context.Claims.CountAsync(c => c.Status == ClaimStatus.Pending);

            dto.OpenTickets = await _context.SupportTickets.CountAsync(t => t.TicketStatus == "Open");

            // 2. Revenue (Today)

            var today = DateTime.Today;

            dto.TodayRevenue = await _context.Payments

                .Where(p => p.PaymentStatus == PaymentStatus.SUCCESS && p.PaymentDate.Date == today)

                .SumAsync(p => p.PaymentAmount);

            // 3. Activity Feed

            // Policies

            var recentPolicies = await _context.Policies

                .Include(p => p.User)

                .OrderByDescending(p => p.TravelStartDate)

                .Take(5)

                .Select(p => new DashboardActivity

                {

                    Type = "policy",

                    ReferenceId = "P-" + p.PolicyId,

                    CustomerName = p.User != null ? p.User.FullName : "Unknown",

                    Date = p.TravelStartDate,

                    Status = p.PolicyStatus.ToString(),

                    StatusColor = p.PolicyStatus == PolicyStatus.ACTIVE ? "bg-active" : "bg-light-orange"

                }).ToListAsync();

            // Claims

            var recentClaims = await _context.Claims

                .Include(c => c.Policy).ThenInclude(p => p.User)

                .OrderByDescending(c => c.ClaimDate)

                .Take(5)

                .Select(c => new DashboardActivity

                {

                    Type = "claim",

                    ReferenceId = "C-" + c.ClaimId,

                    CustomerName = c.Policy.User != null ? c.Policy.User.FullName : "Unknown",

                    Date = c.ClaimDate,

                    Status = c.Status.ToString(),

                    StatusColor = c.Status == ClaimStatus.Approved ? "bg-active" : (c.Status == ClaimStatus.Rejected ? "bg-light-orange" : "bg-light-blue")

                }).ToListAsync();

            // Tickets (FIXED HERE)

            var recentTickets = await _context.SupportTickets

                .Include(t => t.User) // <--- CRITICAL: Fetches Customer Name

                .OrderByDescending(t => t.CreatedDate)

                .Take(5)

                .Select(t => new DashboardActivity

                {

                    Type = "ticket",

                    ReferenceId = "T-" + t.TicketId,

                    // FIX: Shows Real Name instead of User ID

                    CustomerName = t.User != null ? t.User.FullName : "User #" + t.UserId,

                    Date = t.CreatedDate,

                    Status = t.TicketStatus,

                    StatusColor = t.TicketStatus == "Open" ? "bg-light-purple" : "bg-active"

                }).ToListAsync();

            // 4. Combine and Sort

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
