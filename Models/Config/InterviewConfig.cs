using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class InterviewConfig : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> entity)
        {
            entity.ToTable("interviews");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.Property(e => e.ApplicationId).HasColumnName("application_id").IsRequired();
            entity.Property(e => e.ScheduledAt).HasColumnName("scheduled_at").IsRequired();
            entity.Property(e => e.ScheduledTo).HasColumnName("scheduled_to").IsRequired();

            entity.Property(e => e.Status)
                  .HasColumnName("status_id")
                  .HasConversion<int>()
                  .IsRequired();

            entity.Property(e => e.EvaluationDetails)
                  .HasColumnName("evaluation")
                  .HasColumnType("text");

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.CreatedBy).HasColumnName("created_by");

            entity.HasOne(e => e.Application)
                  .WithMany()
                  .HasForeignKey(e => e.ApplicationId);

            entity.HasOne(e => e.Creator)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy);
        }
    }
}