using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecruitX.Models;

namespace RecruitX.Models.Config
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> entity)
        {
            entity.ToTable("roles");

            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id)
                  .HasColumnName("Id");

            entity.Property(r => r.RoleName)
                  .HasColumnName("role_name")
                  .HasMaxLength(50)
                  .IsRequired();
        }
    }
}