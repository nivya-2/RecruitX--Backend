using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Data;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class ApplicationConfig : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> entity)
        {
            entity.ToTable("applications");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasColumnName("Id")
                  .UseIdentityAlwaysColumn(); // PostgreSQL identity

            entity.Property(e => e.CandidateId)
                  .HasColumnName("candidate_id")
                  .IsRequired();

            entity.Property(e => e.JobDescriptionId)
                  .HasColumnName("jd_id")
                  .IsRequired();

            entity.Property(e => e.Status)
                  .HasColumnName("status")
                  .HasConversion<string>()
                  .HasMaxLength(50)
                  .HasDefaultValue(ApplicationStatus.Applied)
                  .IsRequired();

            entity.Property(e => e.SubmittedOn)
                  .HasColumnName("submitted_on");

            entity.Property(e => e.CreatedBy)
                  .HasColumnName("created_by");

            entity.Property(e => e.ExperienceYears)
                  .HasColumnName("experience_years")
                  .IsRequired();

            entity.Property(e => e.ExperienceMonths)
                  .HasColumnName("experience_months")
                  .IsRequired();
            // Add check constraint for experience months 0-11
            entity.HasCheckConstraint("CK_Application_ExperienceMonths", "experience_months >= 0 AND experience_months <= 11");

            // Relationships
            entity.HasOne(e => e.Candidate)
                  .WithMany(c => c.Applications)
                  .HasForeignKey(e => e.CandidateId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("fk_applications_candidate_id");

            entity.HasOne(e => e.JobDescription)
                  .WithMany(jd => jd.Applications)
                  .HasForeignKey(e => e.JobDescriptionId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("fk_applications_jd_id");

            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("fk_applications_created_by");

            entity.HasIndex(e => new { e.CandidateId, e.JobDescriptionId }).IsUnique(false);
        }
    }
}