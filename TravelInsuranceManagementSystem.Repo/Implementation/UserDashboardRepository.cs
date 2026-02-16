using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;

using TravelInsuranceManagementSystem.Models; 

using TravelInsuranceManagementSystem.Repo.Data;

using TravelInsuranceManagementSystem.Repo.Models;

using TravelInsuranceManagementSystem.Repo.Interfaces;

namespace TravelInsuranceManagementSystem.Repo.Implementation

{

    public class UserDashboardRepository : IUserDashboardRepository

    {

        private readonly ApplicationDbContext _context;

        public UserDashboardRepository(ApplicationDbContext context)

        {

            _context = context;

        }

        public async Task<List<Claim>> GetClaimsByUserIdAsync(int userId) =>

            await _context.Claims.Include(c => c.Policy).Where(c => c.Policy.UserId == userId).OrderByDescending(c => c.ClaimDate).ToListAsync();

        public async Task<List<Policy>> GetPoliciesByUserIdAsync(int userId) =>

            await _context.Policies.Where(p => p.UserId == userId).OrderByDescending(p => p.TravelStartDate).ToListAsync();

        public async Task RaiseSupportTicketAsync(IFormCollection form, int userId)

        {

            var ticket = new SupportTicket

            {
         

                UserId = userId,

                IssueDescription = form["Description"],

                TicketStatus = "Open",

                CreatedDate = DateTime.Now

            };

            var detail = new TicketDetail

            {

                Subject = form["Subject"],

                Category = form["Category"],

                Priority = form["Priority"],

                RelatedId = form["RelatedId"],

                ContactMethod = form["ContactMethod"],

                ContactDetail = form["ContactDetail"]

            };

            // Try to use a transaction. If the provider doesn't support transactions
            // (InMemory) or BeginTransactionAsync logs a warning-as-exception, fall back
            // to a non-transactional save sequence.
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.SupportTickets.Add(ticket);
                    await _context.SaveChangesAsync();

                    detail.TicketId = ticket.TicketId;
                    _context.TicketDetails.Add(detail);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch
            {
                // Fallback for providers that don't support transactions or where
                // BeginTransactionAsync triggers warnings treated as exceptions.
                _context.SupportTickets.Add(ticket);
                await _context.SaveChangesAsync();

                detail.TicketId = ticket.TicketId;
                _context.TicketDetails.Add(detail);
                await _context.SaveChangesAsync();
                return;
            }

        }

        

        public async Task<UserDashboardViewModel> GetDashboardSummaryAsync(int userId)

        {

            var dto = new UserDashboardViewModel();

            // 1. KPI Counts

            dto.ActivePolicies = await _context.Policies.CountAsync(p => p.UserId == userId && p.PolicyStatus == PolicyStatus.ACTIVE);

            dto.OpenClaims = await _context.Claims.Include(c => c.Policy).CountAsync(c => c.Policy.UserId == userId && c.Status == ClaimStatus.Pending);

            // FIX: Removed .ToString() logic here as well

            dto.Tickets = await _context.SupportTickets.CountAsync(t => t.UserId == userId);

            dto.UpcomingTrips = await _context.Policies.CountAsync(p => p.UserId == userId && p.TravelStartDate > DateTime.Now);

            // 2. Fetch Recent Activities (Top 5 from each)

            // Policies

            var recentPolicies = await _context.Policies

                .Where(p => p.UserId == userId)

                .OrderByDescending(p => p.TravelStartDate)

                .Take(5)

                .Select(p => new UserDashboardActivity

                {

                    Type = "Policy",

                    ReferenceId = "P-" + p.PolicyId,

                    Description = "Trip to " + p.DestinationCountry,

                    Date = p.TravelStartDate,

                    Status = p.PolicyStatus.ToString(),

                    StatusColor = p.PolicyStatus == PolicyStatus.ACTIVE ? "status-approved" : "status-pending"

                }).ToListAsync();

            // Claims

            var recentClaims = await _context.Claims

                .Include(c => c.Policy)

                .Where(c => c.Policy.UserId == userId)

                .OrderByDescending(c => c.ClaimDate)

                .Take(5)

                .Select(c => new UserDashboardActivity

                {

                    Type = "Claim",

                    ReferenceId = "C-" + c.ClaimId,

                    Description = c.IncidentType + " (₹" + c.ClaimAmount + ")",

                    Date = c.ClaimDate,

                    Status = c.Status.ToString(),

                    StatusColor = c.Status == ClaimStatus.Approved ? "status-approved" : (c.Status == ClaimStatus.Rejected ? "status-rejected" : "status-pending")

                }).ToListAsync();

            // Tickets

            var recentTickets = await _context.SupportTickets               

                .Where(t => t.UserId == userId)

                .OrderByDescending(t => t.CreatedDate)

                .Take(5)

                .Select(t => new UserDashboardActivity

                {

                    Type = "Ticket",

                    ReferenceId = "T-" + t.TicketId,

                    Description = "Support Request",

                    Date = t.CreatedDate,

                    Status = t.TicketStatus,

                    StatusColor = t.TicketStatus == "Open" ? "status-pending" : "status-approved"

                }).ToListAsync();

            // 3. Merge & Sort

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
