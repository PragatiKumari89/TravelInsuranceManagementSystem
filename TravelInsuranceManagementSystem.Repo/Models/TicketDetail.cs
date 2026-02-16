using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelInsuranceManagementSystem.Repo.Models
{
    public class TicketDetail
    {
        [Key]
        public int DetailId { get; set; }

        [ForeignKey("SupportTicket")]
        public int TicketId { get; set; } // Foreign Key to main table

        public string Subject { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
        public string RelatedId { get; set; }
        public string ContactMethod { get; set; }
        public string ContactDetail { get; set; }

        public virtual SupportTicket SupportTicket { get; set; }
    }
}