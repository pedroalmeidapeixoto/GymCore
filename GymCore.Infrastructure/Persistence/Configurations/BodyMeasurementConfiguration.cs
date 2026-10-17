using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymCore.Infrastructure.Persistence.Configurations
{
    public class BodyMeasurementConfiguration : IEntityTypeConfiguration<BodyMeasurement>
    {
        public void Configure(EntityTypeBuilder<BodyMeasurement> builder)
        {
            builder.ToTable("body_measurements");

            builder.HasKey(bm => bm.Id);

            builder.Property(bm => bm.Id)
                .ValueGeneratedNever();

            builder.Property(bm => bm.MeasurementType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(bm => bm.Value)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(bm => bm.Unit)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.HasOne(bm => bm.PhysicalAssessment)
                .WithMany()
                .HasForeignKey(bm => bm.PhysicalAssessmentId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}