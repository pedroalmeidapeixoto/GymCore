using GymCore.Domain.Entities;

namespace GymCore.Application.Plans
{
    public static class PlanMappings
    {
        public static PlanResponse ToResponse(this Plan plan)
        {
            return new PlanResponse(
                plan.Id,
                plan.Name,
                plan.Description,
                plan.Price,
                plan.DurationInDays,
                plan.IsActive);
        }
    }
}