using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class ApplicationStatusHistoryConfig : IEntityTypeConfiguration<ApplicationStatusHistory>
    {
        public void Configure(EntityTypeBuilder<ApplicationStatusHistory> entity)
        {
            entity.ToTable("application_status_history");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.Property(e => e.ApplicationId).HasColumnName("application_id").IsRequired();
            entity.Property(e => e.OldStatus)
                  .HasColumnName("old_status_id")
                  .HasConversion<int?>();

            entity.Property(e => e.NewStatus)
                  .HasColumnName("new_status_id")
                  .HasConversion<int>()
                  .IsRequired();

            entity.Property(e => e.ChangedAt)
                  .HasColumnName("changed_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");

            entity.HasOne(e => e.Application)
                  .WithMany()
                  .HasForeignKey(e => e.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ChangedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.ChangedBy);
        }
    }
}