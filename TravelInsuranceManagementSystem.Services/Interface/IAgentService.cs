using TravelInsuranceManagementSystem.Models;

using TravelInsuranceManagementSystem.Repo.Models;

namespace TravelInsuranceManagementSystem.Services.Interfaces

{

    public interface IAgentService

    {

        Task<List<Policy>> GetPoliciesAsync();

        Task<List<Claim>> GetClaimsAsync();

        Task<List<SupportTicket>> GetSupportTicketsAsync();

        Task<List<Payment>> GetPaymentsAsync();

        Task<(bool Success, string Message)> UpdateClaimStatusAsync(int id, string status, int agentId);

        Task<bool> UpdateTicketStatusAsync(int id, string status);

        Task<bool> DeleteTicketAsync(int id);   

        Task<AgentDashboardViewModel> GetDashboardSummaryAsync();

    }

}
