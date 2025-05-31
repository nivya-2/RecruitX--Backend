using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class EvaluationTokenConfig : IEntityTypeConfiguration<EvaluationToken>
    {
        public void Configure(EntityTypeBuilder<EvaluationToken> entity)
        {
            entity.ToTable("evaluation_tokens");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.Property(e => e.Token)
                  .HasColumnName("token")
                  .HasMaxLength(20)
                  .IsRequired();

            entity.Property(e => e.InterviewId).HasColumnName("interview_id").IsRequired();
            entity.Property(e => e.IsUsed).HasColumnName("is_used").HasDefaultValue(false);
            entity.Property(e => e.UsedAt).HasColumnName("used_at");

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Interview)
                  .WithOne()
                  .HasForeignKey<EvaluationToken>(e => e.InterviewId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Token).IsUnique();
        }
    }
}