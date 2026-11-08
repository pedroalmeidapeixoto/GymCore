namespace GymCore.Application.Students
{
    public record StudentResponse(
        Guid Id,
        string Name,
        string Email,
        string MaskedCpf,
        DateOnly BirthDate,
        string Phone,
        bool IsActive);
}