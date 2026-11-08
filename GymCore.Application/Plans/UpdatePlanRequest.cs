namespace GymCore.Application.Plans
{
    public record UpdatePlanRequest(
        string Name,
        string? Description,
        decimal Price,
        int DurationInDays);
}