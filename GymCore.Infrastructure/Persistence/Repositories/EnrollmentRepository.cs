using GymCore.Application.Enrollments;
using GymCore.Domain.Entities;
using GymCore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymCore.Infrastructure.Persistence.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly GymCoreDbContext _context;

        public EnrollmentRepository(GymCoreDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment?> GetByIdAsync(Guid id)
        {
            return await _context.Enrollments
                .Include(e => e.Student).ThenInclude(s => s.User)
                .Include(e => e.Plan)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IReadOnlyList<Enrollment>> ListAsync(EnrollmentStatus? status)
        {
            var query = _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student).ThenInclude(s => s.User)
                .Include(e => e.Plan)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);

            return await query
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Enrollment>> ListByStudentAsync(Guid studentId)
        {
            return await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student).ThenInclude(s => s.User)
                .Include(e => e.Plan)
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<bool> HasActiveEnrollmentAsync(Guid studentId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Active);
        }

        public async Task AddAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
        }
    }
}