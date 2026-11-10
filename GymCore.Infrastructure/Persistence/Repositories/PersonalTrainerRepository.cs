using GymCore.Application.PersonalTrainers;
using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymCore.Infrastructure.Persistence.Repositories
{
    public class PersonalTrainerRepository : IPersonalTrainerRepository
    {
        private readonly GymCoreDbContext _context;

        public PersonalTrainerRepository(GymCoreDbContext context)
        {
            _context = context;
        }

        public async Task<PersonalTrainer?> GetByIdAsync(Guid id)
        {
            return await _context.PersonalTrainers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<PersonalTrainer>> ListAsync(string? search)
        {
            var query = _context.PersonalTrainers
                .AsNoTracking()
                .Include(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search.Trim()}%";

                query = query.Where(p =>
                    EF.Functions.ILike(p.User.Name, pattern) ||
                    EF.Functions.ILike(p.User.Email, pattern) ||
                    EF.Functions.ILike(p.Cref, pattern));
            }

            return await query
                .OrderBy(p => p.User.Name)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCrefAsync(string cref)
        {
            return await _context.PersonalTrainers.AnyAsync(p => p.Cref == cref);
        }

        public async Task AddAsync(PersonalTrainer personalTrainer)
        {
            await _context.PersonalTrainers.AddAsync(personalTrainer);
        }
    }
}