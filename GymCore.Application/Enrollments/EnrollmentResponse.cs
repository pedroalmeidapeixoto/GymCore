using GymCore.Domain.Enums;

namespace GymCore.Application.Enrollments
{
    public record EnrollmentResponse(
        Guid Id,
        Guid StudentId,
        string StudentName,
        Guid PlanId,
        string PlanName,
        DateOnly StartDate,
        DateOnly EndDate,
        decimal Amount,
        EnrollmentStatus Status);
}