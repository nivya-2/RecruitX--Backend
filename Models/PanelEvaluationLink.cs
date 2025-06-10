using System.ComponentModel.DataAnnotations;

namespace RecruitX.Models
{
    public class PanelEvaluationLink
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(36)] // Standard GUID length
        public string Token { get; set; }

        [Required]
        public int InterviewId { get; set; } // Foreign key to a future 'Interviews' table or just an identifier

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // "PENDING", "SUBMITTED", "EXPIRED"

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; } // Optional: for links that expire after a certain time

        // --- Navigation Properties (Optional but recommended for future use) ---
        // [ForeignKey("InterviewId")]
        // public virtual Interview Interview { get; set; }
    }
}
