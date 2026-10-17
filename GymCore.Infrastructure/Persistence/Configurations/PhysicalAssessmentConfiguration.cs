using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymCore.Infrastructure.Persistence.Configurations
{
    public class PhysicalAssessmentConfiguration : IEntityTypeConfiguration<PhysicalAssessment>
    {
        public void Configure(EntityTypeBuilder<PhysicalAssessment> builder)
        {
            builder.ToTable("physical_assessments");

            builder.HasKey(pa => pa.Id);

            builder.Property(pa => pa.Id)
                .ValueGeneratedNever();

            builder.Property(pa => pa.AssessmentDate)
                .IsRequired();

            builder.Property(pa => pa.Weight)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(pa => pa.Height)
               .IsRequired()
               .HasPrecision(5, 2);

            builder.Property(pa => pa.BodyFatPercentage)
                .HasPrecision(5, 2);

            builder.Property(pa => pa.MuscleMass)
              .HasPrecision(5, 2);

            builder.Property(pa => pa.Notes)
                .HasMaxLength(1000);

            builder.HasOne(pa => pa.Student)
                .WithMany()
                .HasForeignKey(pa => pa.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pa => pa.PersonalTrainer)
              .WithMany()
              .HasForeignKey(pa => pa.PersonalTrainerId)
              .OnDelete(DeleteBehavior.Restrict);

        }
    }
}