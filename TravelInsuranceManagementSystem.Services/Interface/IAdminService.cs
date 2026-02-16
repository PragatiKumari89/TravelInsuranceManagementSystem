using TravelInsuranceManagementSystem.Models;
using TravelInsuranceManagementSystem.Models.ViewModels;
using TravelInsuranceManagementSystem.Repo.Models;

namespace TravelInsuranceManagementSystem.Services.Interfaces

{

    public interface IAdminService

    {

        Task<List<Policy>> GetPolicyManagementDataAsync();

        Task<List<Payment>> GetPaymentHistoryAsync();

        Task<List<ClaimViewModel>> GetClaimsOverviewAsync();

        Task<AdminDashboardViewModel> GetDashboardSummaryAsync();

        Task<List<AgentWorkloadViewModel>> GetAgentsWithWorkloadAsync();

        Task<bool> DeleteAgentAsync(int userId);

    }

}
