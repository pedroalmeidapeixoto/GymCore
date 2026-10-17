using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymCore.Infrastructure.Persistence
{
    public class GymCoreDbContext : DbContext
    {
        public GymCoreDbContext(DbContextOptions<GymCoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<PersonalTrainer> PersonalTrainers => Set<PersonalTrainer>();
        public DbSet<StudentPersonal> StudentPersonals => Set<StudentPersonal>();
        public DbSet<Plan> Plans => Set<Plan>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<Workout> Workouts => Set<Workout>();
        public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
        public DbSet<PhysicalAssessment> PhysicalAssessments => Set<PhysicalAssessment>();
        public DbSet<BodyMeasurement> BodyMeasurements => Set<BodyMeasurement>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymCoreDbContext).Assembly);
        }
    }
}