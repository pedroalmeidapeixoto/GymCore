using GymCore.Application.Students;
using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymCore.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly GymCoreDbContext _context;

        public StudentRepository(GymCoreDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByIdAsync(Guid id)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IReadOnlyList<Student>> ListAsync(string? search)
        {
            var query = _context.Students
                .AsNoTracking()
                .Include(s => s.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                var pattern = $"%{term}%";
                var digits = new string(term.Where(char.IsDigit).ToArray());

                query = query.Where(s =>
                    EF.Functions.ILike(s.User.Name, pattern) ||
                    EF.Functions.ILike(s.User.Email, pattern) ||
                    (digits.Length > 0 && s.Cpf.Contains(digits)));
            }

            return await query
                .OrderBy(s => s.User.Name)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCpfAsync(string cpf)
        {
            return await _context.Students.AnyAsync(s => s.Cpf == cpf);
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
        }
    }
}