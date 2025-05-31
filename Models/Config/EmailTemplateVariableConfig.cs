using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class EmailTemplateVariableConfig : IEntityTypeConfiguration<EmailTemplateVariable>
    {
        public void Configure(EntityTypeBuilder<EmailTemplateVariable> entity)
        {
            entity.ToTable("email_template_variables");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.Property(e => e.TemplateId).HasColumnName("template_id").IsRequired();
            entity.Property(e => e.VariableName)
                  .HasColumnName("variable_name")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Template)
                  .WithMany()
                  .HasForeignKey(e => e.TemplateId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.TemplateId, e.VariableName }).IsUnique();
        }
    }
}