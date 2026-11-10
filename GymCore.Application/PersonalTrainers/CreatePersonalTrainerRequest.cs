namespace GymCore.Application.PersonalTrainers
{
    public record CreatePersonalTrainerRequest(
        string Name,
        string Email,
        string Password,
        string Cref,
        string Phone);
}