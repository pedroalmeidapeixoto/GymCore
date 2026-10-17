using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymCore.Infrastructure.Persistence.Configurations
{
    public class StudentPersonalConfiguration : IEntityTypeConfiguration<StudentPersonal>
    {
        public void Configure(EntityTypeBuilder<StudentPersonal> builder)
        {
            builder.ToTable("student_personals");

            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.Id)
                .ValueGeneratedNever();

            builder.Property(sp => sp.StartDate)
                .IsRequired();

            builder.Property(sp => sp.EndDate);

            builder.Property(sp => sp.IsActive)
                .IsRequired();

            builder.HasOne(sp => sp.Student)
                .WithMany()
                .HasForeignKey(sp => sp.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sp => sp.PersonalTrainer)
                .WithMany()
                .HasForeignKey(sp => sp.PersonalTrainerId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}