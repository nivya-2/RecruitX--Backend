using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecruitX.Models
{
    public class PanelEvaluationResponse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PanelEvaluationLinkId { get; set; } // Foreign key to the link used for submission

        [Required]
        [StringLength(255)]
        public string SubmittedByEmail { get; set; } // The email the interviewer entered in the form

        [Required]
        public string Feedback { get; set; } // Can be a long string or even JSON

        public DateTime SubmittedAt { get; set; }

        // --- Navigation Properties ---
        [ForeignKey("PanelEvaluationLinkId")]
        public virtual PanelEvaluationLink PanelEvaluationLink { get; set; }
    }
}
