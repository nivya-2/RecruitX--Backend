using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class JobDescriptionConfig : IEntityTypeConfiguration<JobDescription>
    {
        public void Configure(EntityTypeBuilder<JobDescription> entity)
        {
            entity.ToTable("job_descriptions");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.Property(e => e.JobRequisitionId).HasColumnName("jr_id");
            ;
            entity.Property(e => e.JobDesc).HasColumnName("job_desc");
            entity.Property(e => e.FilledPositions).HasColumnName("fill_positions");
            entity.Property(e => e.Updates).HasColumnName("updates");
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");

            entity.HasOne<JobRequisition>()
                .WithMany()
                .HasForeignKey(e => e.JobRequisitionId);

            entity.HasOne(j => j.CreatedByUser)
    .WithMany()
    .HasForeignKey(j => j.CreatedBy)
    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}