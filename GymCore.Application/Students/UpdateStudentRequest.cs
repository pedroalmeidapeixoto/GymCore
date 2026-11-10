namespace GymCore.Application.Students
{
    public record UpdateStudentRequest(
        string Name,
        DateOnly BirthDate,
        string Phone);
}