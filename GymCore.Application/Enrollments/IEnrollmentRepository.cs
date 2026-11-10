using GymCore.Domain.Entities;
using GymCore.Domain.Enums;

namespace GymCore.Application.Enrollments
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment?> GetByIdAsync(Guid id);

        Task<IReadOnlyList<Enrollment>> ListAsync(EnrollmentStatus? status);

        Task<IReadOnlyList<Enrollment>> ListByStudentAsync(Guid studentId);

        Task<bool> HasActiveEnrollmentAsync(Guid studentId);

        Task AddAsync(Enrollment enrollment);
    }
}