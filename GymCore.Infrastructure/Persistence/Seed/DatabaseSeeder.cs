using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymCore.Infrastructure.Persistence.Seed
{
    public class DatabaseSeeder
    {
        private const string SeedPasswordHash = "SEED-PLACEHODER-NOT-A-REAL-HASH";

        private const string DevelopmentMarkerEmail = "admin@gymcore.test";

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

        public async Task SeedDevelopmentDataAsync()
        {
            if (await _context.Users.AnyAsync(u => u.Email == DevelopmentMarkerEmail))
                return;

            await SeedPeopleAsync();
        }

            private async Task SeedPeopleAsync()
        {
            var roles = await _context.Roles.ToDictionaryAsync(r => r.Name);

            var admin = new User("Administrador GymCore", DevelopmentMarkerEmail
                ,SeedPasswordHash, roles[RoleNames.Administrator]);

            var personalUser1 = new User("Carlos Personal", "carlos.personal@gymcore.test",
                SeedPasswordHash, roles[RoleNames.PersonalTrainer]);
            var personalUser2 = new User("Marina Personal", "marina.personal@gymcore.test",
                SeedPasswordHash, roles[RoleNames.PersonalTrainer]);

            var personal1 = new PersonalTrainer(personalUser1, "012345-G/SP",
                "11990000001");

            var personal2 = new PersonalTrainer(personalUser2, "067890-G/RJ",
                "21990000002");


            var studentUser1 = new User("Ana Aluna", "ana.aluna@gymcore.test",
                SeedPasswordHash, roles[RoleNames.Student]);
            var studentUser2 = new User("Bruno Aluno", "bruno.aluno@gymcore.test",
                SeedPasswordHash, roles[RoleNames.Student]);
            var studentUser3 = new User("Clara Aluna", "clara.aluna@gymcore.test",
                SeedPasswordHash, roles[RoleNames.Student]);


            var student1 = new Student(studentUser1, "52998224725", new DateOnly(1995, 3,
                12), " 11980000001");
            var student2 = new Student(studentUser2, "11144477735", new DateOnly(1990, 7,
                25), "11980000002");
            var student3 = new Student(studentUser3, "39053344705", new DateOnly(2001, 11,
                3), "11980000003");


            _context.AddRange(admin, personal1, personal2, student1, student2, student3);

            await _context.SaveChangesAsync();

        }
    }
    
}