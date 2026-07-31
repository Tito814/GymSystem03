using GymSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.Config
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>

    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(p => p.Name)
                .IsRequired()
                .HasColumnType("nvarchar(50)");
            builder.Property(p => p.Description).HasMaxLength(200);
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p=>p.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Plan_Duration", "DurationInDays BETWEEN 1 AND 365"));

        }
    }
}
