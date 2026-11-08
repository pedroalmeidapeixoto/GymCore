namespace GymCore.Application.Plans
{
    public record PlanResponse(
        Guid Id,
        string Name,
        string? Description,
        decimal Price,
        int DurationInDays,
        bool IsActive);
}