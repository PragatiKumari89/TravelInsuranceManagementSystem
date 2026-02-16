using Microsoft.AspNetCore.Http;

using TravelInsuranceManagementSystem.Repo.Models;

namespace TravelInsuranceManagementSystem.Services.Interfaces

{

    public interface IUserDashboardService

    {

        Task<List<Claim>> GetUserClaimsAsync(int userId);

        Task<List<Policy>> GetUserPoliciesAsync(int userId);

        Task RaiseSupportTicketAsync(IFormCollection form, int userId);

        Task<UserDashboardViewModel> GetDashboardSummaryAsync(int userId);

    }

}
