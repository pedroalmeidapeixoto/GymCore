using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymCore.Infrastructure.Persistence.Seed
{
    public class DatabaseSeeder
    {
        private readonly GymCoreDbContext _context;

        public DatabaseSeeder(GymCoreDbContext context)
        {
            _context = context;
        }

        public async Task SeedReferenceDataAsync()
        {
            string[] roleNames =
            {
                RoleNames.Administrator,
                RoleNames.PersonalTrainer,
                RoleNames.Student
            };

            var existingNames = await _context.Roles
                .Select(r => r.Name)
                .ToListAsync();

            foreach (var name in roleNames.Except(existingNames))
            {
                _context.Roles.Add(new Role(name));
            }

            await _context.SaveChangesAsync();
        }
    }
}