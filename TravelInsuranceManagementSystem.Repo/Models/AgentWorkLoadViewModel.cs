namespace TravelInsuranceManagementSystem.Models.ViewModels
{
    public class AgentWorkloadViewModel

    {

        public int Id { get; set; } // User ID 

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        // Workload Stats   
        public int ClaimsHandled { get; set; }
      
        public bool IsActive { get; set; } = true; // Default status

    }

}
