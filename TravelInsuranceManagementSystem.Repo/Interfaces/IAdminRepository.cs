using TravelInsuranceManagementSystem.Repo.Models;

using TravelInsuranceManagementSystem.Models;

namespace TravelInsuranceManagementSystem.Repo.Interfaces

{

    public interface IAdminRepository

    {

        Task<List<Policy>> GetAllPoliciesWithMembersAsync();

        Task<List<Payment>> GetAllPaymentsWithPoliciesAsync();

        Task<List<ClaimViewModel>> GetClaimsOverviewAsync();
      
        Task<AdminDashboardViewModel> GetDashboardSummaryAsync();

    }

}
