using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymCore.Infrastructure.Persistence.Configurations
{
    public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
    {
        public void Configure(EntityTypeBuilder<Workout> builder)
        {
            builder.ToTable("workouts");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id)
                .ValueGeneratedNever();

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(w => w.Description)
                .HasMaxLength(500);

            builder.Property(w => w.StartDate)
                .IsRequired();

            builder.Property(w => w.EndDate);

            builder.HasOne(w => w.Student)
                .WithMany()
                .HasForeignKey(w => w.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.PersonalTrainer)
                .WithMany()
                .HasForeignKey(w => w.PersonalTrainerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}