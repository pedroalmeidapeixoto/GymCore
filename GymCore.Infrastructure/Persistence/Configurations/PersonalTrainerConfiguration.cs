using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace GymCore.Infrastructure.Persistence.Configurations
{
    public class PersonalTrainerConfiguration : IEntityTypeConfiguration<PersonalTrainer>
    {
        public void Configure(EntityTypeBuilder<PersonalTrainer> builder)
        {
            builder.ToTable("personal_trainers");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Cref)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.Phone)
               .IsRequired()
               .HasMaxLength(20);

            builder.HasIndex(p => p.Cref)
                .IsUnique();

            builder.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<PersonalTrainer>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.UserId)
                .IsUnique();
        }

    }
}