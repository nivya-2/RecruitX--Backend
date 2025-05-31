using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class JrAssignmentConfig : IEntityTypeConfiguration<JrAssignment>
    {
        public void Configure(EntityTypeBuilder<JrAssignment> entity)
        {
            entity.ToTable("jr_assignments");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("Id")
                  .UseIdentityAlwaysColumn(); // PostgreSQL identity column

            entity.Property(e => e.JobRequisitionId)
                  .HasColumnName("jr_id")
                  .IsRequired();

            entity.Property(e => e.AssignedTo)
                  .HasColumnName("assigned_to")
                  .IsRequired();

            entity.Property(e => e.AssignedBy)
                  .HasColumnName("assigned_by")
                  .IsRequired();

            entity.Property(e => e.AssignedAt)
                  .HasColumnName("assigned_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Foreign Keys
            entity.HasOne(e => e.JobRequisition)
                  .WithMany()
                  .HasForeignKey(e => e.JobRequisitionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedToUser)
                  .WithMany()
                  .HasForeignKey(e => e.AssignedTo)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.AssignedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}