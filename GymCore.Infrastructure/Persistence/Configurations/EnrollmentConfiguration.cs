using GymCore.Domain.Entities;
using GymCore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymCore.Infrastructure.Persistence.Configurations
{
    public class EnrollmentConfiguration: IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("enrollments");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.StartDate)
                .IsRequired();

            builder.Property(e => e.EndDate)
                .IsRequired();

            builder.Property(e => e.Amount)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasOne(e => e.Plan)
                .WithMany()
                .HasForeignKey(e => e.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.StudentId)
                .HasDatabaseName("ux_enrollments_student_active")
                .IsUnique()
                .HasFilter($"\"{nameof(Enrollment.Status)}\" = '{EnrollmentStatus.Active}'");
        }
    }
}