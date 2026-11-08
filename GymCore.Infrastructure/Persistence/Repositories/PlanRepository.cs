using GymCore.Application.Plans;
using GymCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymCore.Infrastructure.Persistence.Repositories
{
    public class PlanRepository : IPlanRepository
    {
        private readonly GymCoreDbContext _context;

        public PlanRepository(GymCoreDbContext context)
        {
            _context = context;
        }

        public async Task<Plan?> GetByIdAsync(Guid id)
        {
            return await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Plan>> ListAsync()
        {
            return await _context.Plans
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task AddAsync(Plan plan)
        {
            await _context.Plans.AddAsync(plan);
        }
    }
}