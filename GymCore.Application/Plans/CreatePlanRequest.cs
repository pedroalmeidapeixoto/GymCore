namespace GymCore.Application.Plans
{
    public record CreatePlanRequest(
        string Name,
        string? Description,
        decimal Price,
        int DurationInDays);
}