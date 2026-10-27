using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using GymCore.Domain.Enums;

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
            if (!await _context.Users.AnyAsync(u => u.Email == DevelopmentMarkerEmail))
                await SeedPeopleAsync();

            if (!await _context.Plans.AnyAsync())
                await SeedFinancialAsync();

            if (!await _context.Exercises.AnyAsync())
                await SeedTrainingAsync();
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

        private async Task SeedFinancialAsync()
        {
            var students = await _context.Students
                .Include(s => s.User)
                .ToDictionaryAsync(s => s.User.Email);

            var ana = students["ana.aluna@gymcore.test"];
            var bruno = students["bruno.aluno@gymcore.test"];
            var clara = students["clara.aluna@gymcore.test"];

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var monthly = new Plan("Mensal", "Acesso completo por 30 dias", 89.90m, 30);
            var quartely = new Plan("Trimestral", "Acesso completo por 90 dias, com desconto", 239.90m, 90);
            var annual = new Plan("Anual", null, 799.00m, 365);
            var legacy = new Plan("Promocional 2025", "Plano encerrado, mantido pelo histórico", 59.90m, 30);
            legacy.Deactivate();

            var anaOld = new Enrollment(ana, monthly, today.AddDays(-90), today.AddDays(-60), 79.90m);
            anaOld.Activate();
            anaOld.Expire();

            var anaCurrent = new Enrollment(ana, monthly, today.AddDays(-10), today.AddDays(20), monthly.Price);
            anaCurrent.Activate();


            var brunoOld = new Enrollment(bruno, legacy, today.AddDays(-200), today.AddDays(-170), legacy.Price);
            brunoOld.Cancel();

            var brunoPending = new Enrollment(bruno, quartely, today, today.AddDays(90), quartely.Price);

            var claraActive = new Enrollment(clara, quartely, today.AddDays(-45), today.AddDays(45), quartely.Price);
            claraActive.Activate();

            var anaOldPayment = new Payment(anaOld, 79.90m, today.AddDays(-90));
            anaOldPayment.Pay(today.AddDays(-88));

            var anaPayment = new Payment(anaCurrent, monthly.Price, today.AddDays(-10));
            anaPayment.Pay(today.AddDays(-9));


            var brunoOldPayment = new Payment(brunoOld, legacy.Price, today.AddDays(-200));
            brunoOldPayment.Cancel();

            var brunoPendingPayment = new Payment(brunoPending, quartely.Price, today.AddDays(5));

            var claraPayment1 = new Payment(claraActive, 119.95m, today.AddDays(-45));
            claraPayment1.Pay(today.AddDays(-44));

            var claraPayment2 = new Payment(claraActive, 119.95m, today.AddDays(-15));
            claraPayment2.MarkAsOverdue();


            _context.AddRange(
                annual,
                anaOldPayment, anaPayment,
                brunoOldPayment, brunoPendingPayment,
                claraPayment1, claraPayment2);

            await _context.SaveChangesAsync();
        }

        private async Task SeedTrainingAsync()
        {
            var students = await _context.Students
                .Include(s => s.User)
                .ToDictionaryAsync(s => s.User.Email);


            var personals = await _context.PersonalTrainers
                .Include(p => p.User)
                .ToDictionaryAsync (s => s.User.Email);

            var ana = students["ana.aluna@gymcore.test"];
            var bruno = students["bruno.aluno@gymcore.test"];
            var clara = students["clara.aluna@gymcore.test"];

            var carlos = personals["carlos.personal@gymcore.test"];
            var marina = personals["marina.personal@gymcore.test"];


            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var anaWithMarina = new StudentPersonal(ana, marina, today.AddDays(-300));
            anaWithMarina.EndLink(today.AddDays(-125));
            var anaWithCarlos = new StudentPersonal(ana, carlos, today.AddDays(-120));
            var brunoWithCarlos = new StudentPersonal(bruno, carlos, today.AddDays(-60));
            var claraWithMarina = new StudentPersonal(clara, marina, today.AddDays(-45));


            var bench = new Exercise("Supino reto", "Exercício para a base peitoral",
                MuscleGroup.Chest);
            var squat = new Exercise("Agachamento livre", "Exercício base para membros inferiores",
                MuscleGroup.Legs);
            var row = new Exercise("Remada curvada", "Exercício base para costas",
                MuscleGroup.Back);
            var shoulderPress = new Exercise("Desenvolvimento co Halteres", null,
                MuscleGroup.Shoulders);
            var hipThrust = new Exercise("Elevação Pélvica", "Foco em glúteos",
                MuscleGroup.Glutes);
            var plank = new Exercise("Prancha abdominal", "Isometria para o core",
                MuscleGroup.Abs);
            var legPress = new Exercise("Leg Press 45", "Aparelho fora de uso",
                MuscleGroup.Legs);
            legPress.Deactivate();


            var anaBase = new Workout(ana, marina, "Treino de base", "Fase inicial de adaptação,",
                today.AddDays(-290));
            anaBase.End(today.AddDays(-130));


            var anaAdaptation = new Workout(ana, carlos, "Adaptação", "Primeiras semanas com o novo personal",
                today.AddDays(-120));
            anaAdaptation.End(today.AddDays(-60));

            var anaHyperthrophy = new Workout(ana, carlos, "Hipertrofia A", "Foco de força e volume",
                today.AddDays(-60));
            var brunoFullBody = new Workout(bruno, carlos, "Full Body iniciante", null,
                today.AddDays(-55));
            var claraFuncional = new Workout(clara, marina, "Funcional e Core", "Condicionamento geral",
                today.AddDays(-40));

            var items = new[]
            {
                new WorkoutExercise(anaBase, squat, 3, 15, null, 60, null),
                new WorkoutExercise(anaBase, plank, 3, 1, null, 45, "manter 30 segundos"),
                new WorkoutExercise(anaHyperthrophy, bench, 4, 10, 40m, 90, null),
                new WorkoutExercise(anaHyperthrophy, squat, 4, 12, 60m, 120, null),
                new WorkoutExercise(anaHyperthrophy, row, 3, 12, 35m, 90, null),
                new WorkoutExercise(anaAdaptation, legPress, 3, 15, 80m, 90, "aparelho depois desativado do catálogo"),
                new WorkoutExercise(anaAdaptation, bench, 3, 12, 20m, 60, null),
                new WorkoutExercise(brunoFullBody, squat, 3, 12, 40m, 90, null),
                new WorkoutExercise(brunoFullBody, bench, 3, 12, 30m, 90, null),
                new WorkoutExercise(brunoFullBody, shoulderPress, 3, 12, 12m, 60, "carga por halter"),
                new WorkoutExercise(claraFuncional, hipThrust, 4, 15, 30m, 60, null),
                new WorkoutExercise(claraFuncional, plank, 3, 1, null, 45, "manter 40 segundos"),

            };

            _context.AddRange(anaWithMarina, anaWithCarlos, brunoWithCarlos, claraWithMarina);
            _context.AddRange(items);

            await _context.SaveChangesAsync();
        }
    }
    
}