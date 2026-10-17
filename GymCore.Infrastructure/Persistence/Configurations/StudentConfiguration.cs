using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace GymCore.Infrastructure.Persistence.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder) 
        {
            builder.ToTable("students");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedNever();

            builder.Property(s => s.Cpf)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(s => s.BirthDate)
                .IsRequired();

            builder.Property(s => s.Phone)
               .IsRequired()
               .HasMaxLength(20);

            builder.HasIndex(s => s.Cpf)
                .IsUnique();

            builder.HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.UserId)
                .IsUnique();
        }
        
    }
}