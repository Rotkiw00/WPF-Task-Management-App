using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace TaskManager.Infrastructure.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Core.Entities.WorkTask>
{
    public void Configure(EntityTypeBuilder<Core.Entities.WorkTask> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            ).HasColumnType("TEXT");

        builder.HasOne(t => t.AssignedTo)
            .WithMany(p => p.Tasks)
            .HasForeignKey("AssignedToId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.CreatedDateTime);
    }
}