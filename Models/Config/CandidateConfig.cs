using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class CandidateConfig : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> entity)
        {
            entity.ToTable("candidates");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id)
                  .HasColumnName("Id");

            entity.Property(c => c.Source);
            entity.Property(c => c.Source)
                  .HasColumnName("source")
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(c => c.SubSource)
                  .HasColumnName("sub_source")
                  .HasMaxLength(50);

            entity.Property(c => c.CandidateName)
                  .HasColumnName("candidate_name")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(c => c.ProposedRole)
                  .HasColumnName("proposed_role")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(c => c.Email)
                  .HasColumnName("email")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.HasIndex(c => c.Email).IsUnique();

            entity.Property(c => c.ContactNumber)
                  .HasColumnName("contact_no")
                  .IsRequired();

            entity.Property(c => c.LinkedinUrl)
                  .HasColumnName("linkedin_url")
                  .HasMaxLength(255);

            entity.Property(c => c.TotalExperienceYears)
                  .HasColumnName("total_experience_required_years")
                  .IsRequired();

            entity.Property(c => c.TotalExperienceMonths)
                  .HasColumnName("total_experience_required_months")
                  .IsRequired();

            entity.Property(c => c.RelevantExperienceYears)
                  .HasColumnName("relevant_experience_required_years")
                  .IsRequired();

            entity.Property(c => c.RelevantExperienceMonths)
                  .HasColumnName("relevant_experience_required_months")
                  .IsRequired();

            entity.Property(c => c.CurrentEmployer)
                  .HasColumnName("current_employer")
                  .HasMaxLength(100);

            entity.Property(c => c.NoticePeriodDays)
                  .HasColumnName("notice_period_days");

            entity.Property(c => c.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(c => c.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(c => c.CurrentLocationId)
                  .HasColumnName("current_location_id");

            entity.Property(c => c.PreferredLocationId)
                  .HasColumnName("preferred_location_id");

            entity.HasOne(c => c.CurrentLocation)
                  .WithMany()
                  .HasForeignKey(c => c.CurrentLocationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.PreferredLocation)
                  .WithMany()
                  .HasForeignKey(c => c.PreferredLocationId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}