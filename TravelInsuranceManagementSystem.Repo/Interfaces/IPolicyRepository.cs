using TravelInsuranceManagementSystem.Repo.Models;

namespace TravelInsuranceManagementSystem.Repo.Interfaces

{

    public interface IPolicyRepository

    {
        Policy GetById(int id);

        Task<int> CreateFamilyPolicyAsync(FamilyInsuranceDto data, int userId);

        Task AddPolicyAsync(Policy policy);

        Task SaveAsync();

    }

}
