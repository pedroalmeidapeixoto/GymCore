using GymCore.Application.Plans;
using GymCore.Domain.Entities;

namespace GymCore.UnitTests.Fakes
{
    public class FakePlanRepository : IPlanRepository
    {
        private readonly List<Plan> _plans = new();

        public Task<Plan?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_plans.FirstOrDefault(p => p.Id == id));
        }

        public Task<IReadOnlyList<Plan>> ListAsync()
        {
            return Task.FromResult<IReadOnlyList<Plan>>(_plans.ToList());
        }

        public Task AddAsync(Plan plan)
        {
            _plans.Add(plan);
            return Task.CompletedTask;
        }
    }
}