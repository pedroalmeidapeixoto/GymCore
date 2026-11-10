namespace GymCore.Application.Enrollments
{
    public record CreateEnrollmentRequest(
        Guid StudentId,
        Guid PlanId,
        DateOnly StartDate,
        decimal? Amount);
}