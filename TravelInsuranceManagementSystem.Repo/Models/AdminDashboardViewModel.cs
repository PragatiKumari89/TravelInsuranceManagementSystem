namespace TravelInsuranceManagementSystem.Repo.Models

{
    
    public class AdminDashboardViewModel

    {

        public int ActivePolicies { get; set; }

        public int OpenClaims { get; set; }

        public decimal TodayRevenue { get; set; }

        public int OpenTickets { get; set; }

        public List<AdminDashboardActivity> RecentActivities { get; set; } = new List<AdminDashboardActivity>();

    }
   

    public class AdminDashboardActivity

    {

        public string Type { get; set; } // "Policy", "Claim", "Ticket"

        public string ReferenceId { get; set; }

        public string CustomerName { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; }

        public string StatusColor { get; set; } // CSS class for badge styling

    }

}
