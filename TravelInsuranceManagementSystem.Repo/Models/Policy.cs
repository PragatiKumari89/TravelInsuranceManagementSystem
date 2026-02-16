using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace TravelInsuranceManagementSystem.Repo.Models

{
    public class Policy

    {
        [Key]
        public int PolicyId { get; set; }

        // Foreign Key to User
        public int? UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [StringLength(100)]
        public string DestinationCountry { get; set; } = string.Empty;

        [Required]
        public DateTime TravelStartDate { get; set; }

        [Required]
        public DateTime TravelEndDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CoverageAmount { get; set; }

        [StringLength(100)]
        public string CoverageType { get; set; } = string.Empty;

        [Required]
        public PolicyStatus PolicyStatus { get; set; }

        // One Policy can have multiple Members (One-to-Many Relationship)
        public List<PolicyMember> Members { get; set; } = new List<PolicyMember>();

    }

    public enum PolicyStatus
    {
        PENDING,
        ACTIVE,
        EXPIRED,
        CANCELLED
    }

    public class PolicyMember

    {

        [Key]
        public int MemberId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Relation { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string Mobile { get; set; }

        // Foreign Key linking back to Policy
        public int PolicyId { get; set; }
        public Policy Policy { get; set; }

    }

}
