using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManager.Infrastructure.Configurations;
public class PersonConfiguration : IEntityTypeConfiguration<Core.Entities.Person>
{
    public void Configure(EntityTypeBuilder<Core.Entities.Person> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Email)
            .HasMaxLength(100);

        builder.HasIndex(p => p.Email)
            .IsUnique();
    }
}
