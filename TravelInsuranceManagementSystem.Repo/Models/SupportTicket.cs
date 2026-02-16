using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace TravelInsuranceManagementSystem.Repo.Models

{

    public class SupportTicket

    {

        [Key]
        public int TicketId { get; set; }
       
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public int? AgentId { get; set; }

        [ForeignKey("AgentId")]
        public virtual User Agent { get; set; }

        [Required]
        [StringLength(1000)]
        public string IssueDescription { get; set; }

        [Required]
        public string TicketStatus { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ResolvedDate { get; set; }

        public virtual TicketDetail ExtensionData { get; set; }

    }

}
