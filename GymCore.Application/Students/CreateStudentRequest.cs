namespace GymCore.Application.Students
{
    public record CreateStudentRequest(
        string Name,
        string Email,
        string Password,
        string Cpf,
        DateOnly BirthDate,
        string Phone);
}