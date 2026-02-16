using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelInsuranceManagementSystem.Repo.Models;

namespace TravelInsuranceManagementSystem.Models
{
    // 1. Define the Enum for Payment Status 
    public enum PaymentStatus
    {
        PENDING,
        SUCCESS,
        FAILED
    }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PaymentAmount { get; set; }

        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        // Enum property
        public PaymentStatus PaymentStatus { get; set; }
     
        public int PolicyId { get; set; }

        // This allows your code to see the Policy details
        [ForeignKey("PolicyId")]
        public virtual Policy Policy { get; set; }
    }
}