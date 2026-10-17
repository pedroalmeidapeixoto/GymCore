using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymCore.Infrastructure.Persistence.Configurations
{
    public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
    {
        public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
        {
            builder.ToTable("workout_exercises");

            builder.HasKey(we => we.Id);

            builder.Property(we => we.Id)
                .ValueGeneratedNever();

            builder.Property(we => we.Repetitions)
                .IsRequired();

            builder.Property(we => we.Sets)
                .IsRequired();

            builder.Property(we => we.Load)
                .HasPrecision(7, 2);

            builder.Property(we => we.RestInSeconds);

            builder.Property(we => we.Notes)
                .HasMaxLength(500);

            builder.HasOne(we => we.Workout)
                .WithMany()
                .HasForeignKey(we => we.WorkoutId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(we => we.Exercise)
                .WithMany()
                .HasForeignKey(we => we.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}