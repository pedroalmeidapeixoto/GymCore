using GymCore.Domain.Entities;

namespace GymCore.Application.Enrollments
{
    public static class EnrollmentMappings
    {
        public static EnrollmentResponse ToResponse(this Enrollment enrollment)
        {
            return new EnrollmentResponse(
                enrollment.Id,
                enrollment.StudentId,
                enrollment.Student.User.Name,
                enrollment.PlanId,
                enrollment.Plan.Name,
                enrollment.StartDate,
                enrollment.EndDate,
                enrollment.Amount,
                enrollment.Status);
        }
    }
}