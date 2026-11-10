using GymCore.Application.Enrollments;
using GymCore.Domain.Entities;
using GymCore.Domain.Enums;

namespace GymCore.UnitTests.Fakes
{
    public class FakeEnrollmentRepository : IEnrollmentRepository
    {
        private readonly List<Enrollment> _enrollments = new();

        public IReadOnlyList<Enrollment> Enrollments => _enrollments;

        public Task<Enrollment?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_enrollments.FirstOrDefault(e => e.Id == id));
        }

        public Task<IReadOnlyList<Enrollment>> ListAsync(EnrollmentStatus? status)
        {
            var result = _enrollments
                .Where(e => status == null || e.Status == status)
                .ToList();

            return Task.FromResult<IReadOnlyList<Enrollment>>(result);
        }

        public Task<IReadOnlyList<Enrollment>> ListByStudentAsync(Guid studentId)
        {
            var result = _enrollments.Where(e => e.StudentId == studentId).ToList();

            return Task.FromResult<IReadOnlyList<Enrollment>>(result);
        }

        public Task<bool> HasActiveEnrollmentAsync(Guid studentId)
        {
            return Task.FromResult(_enrollments.Any(e =>
                e.StudentId == studentId && e.Status == EnrollmentStatus.Active));
        }

        public Task AddAsync(Enrollment enrollment)
        {
            _enrollments.Add(enrollment);
            return Task.CompletedTask;
        }
    }
}