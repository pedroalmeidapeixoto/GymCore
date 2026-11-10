namespace GymCore.Application.PersonalTrainers
{
    public record PersonalTrainerResponse(
        Guid Id,
        string Name,
        string Email,
        string Cref,
        string Phone,
        bool IsActive);
}