using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class LeadToRecruiterConfig : IEntityTypeConfiguration<LeadToRecruiter>
    {
        public void Configure(EntityTypeBuilder<LeadToRecruiter> entity)
        {
            entity.ToTable("lead_to_recruiter");
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.HasKey(e => new { e.Id, e.RecruiterId });

            entity.HasOne(e => e.Lead)
                  .WithMany()
                  .HasForeignKey(e => e.Id)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Recruiter)
                  .WithMany()
                  .HasForeignKey(e => e.RecruiterId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}