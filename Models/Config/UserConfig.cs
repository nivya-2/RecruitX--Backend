using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("users");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasColumnName("Id")
                  .UseIdentityAlwaysColumn(); 

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");

            entity.Property(e => e.Username)
                  .HasColumnName("username")
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Password)
                  .HasColumnName("password")
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(e => e.Email)
                  .HasColumnName("email")
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.IsActive)
                  .HasColumnName("is_active")
                  .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            entity.HasOne(e => e.Employee)
                 .WithOne(emp => emp.User)
                 .HasForeignKey<User>(e => e.EmployeeId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        }
    }
}