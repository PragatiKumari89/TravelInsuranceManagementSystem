using TravelInsuranceManagementSystem.Models;

using TravelInsuranceManagementSystem.Repo.Interfaces;

using TravelInsuranceManagementSystem.Repo.Models;

using TravelInsuranceManagementSystem.Services.Interfaces;

namespace TravelInsuranceManagementSystem.Services.Implementation

{

    public class AgentService : IAgentService

    {

        private readonly IAgentRepository _agentRepo;

        public AgentService(IAgentRepository agentRepo) { _agentRepo = agentRepo; }

        public async Task<List<Policy>> GetPoliciesAsync() => await _agentRepo.GetPoliciesWithMembersAsync();

        public async Task<List<Claim>> GetClaimsAsync() => await _agentRepo.GetClaimsWithCustomerAsync();

        public async Task<List<SupportTicket>> GetSupportTicketsAsync() => await _agentRepo.GetSupportTicketsAsync();

        public async Task<List<Payment>> GetPaymentsAsync() => await _agentRepo.GetPaymentsWithPolicyAsync();

        public async Task<(bool Success, string Message)> UpdateClaimStatusAsync(int id, string status, int agentId) =>

            await _agentRepo.UpdateClaimStatusAsync(id, status, agentId);

        public async Task<bool> UpdateTicketStatusAsync(int id, string status) =>

            await _agentRepo.UpdateTicketStatusAsync(id, status);

        public async Task<bool> DeleteTicketAsync(int id) =>

            await _agentRepo.DeleteTicketAsync(id);

        public async Task<AgentDashboardViewModel> GetDashboardSummaryAsync() =>

            await _agentRepo.GetDashboardSummaryAsync();

    }

}
