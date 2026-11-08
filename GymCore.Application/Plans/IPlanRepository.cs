using GymCore.Domain.Entities;

namespace GymCore.Application.Plans
{
    public interface IPlanRepository
    {
        Task<Plan?> GetByIdAsync(Guid id);

        Task<IReadOnlyList<Plan>> ListAsync();

        Task AddAsync(Plan plan);
    }
}