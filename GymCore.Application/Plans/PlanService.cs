using GymCore.Application.Common;
using GymCore.Application.Common.Exceptions;
using GymCore.Domain.Entities;

namespace GymCore.Application.Plans
{
    public class PlanService
    {
        private readonly IPlanRepository _plans;
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IPlanRepository plans, IUnitOfWork unitOfWork)
        {
            _plans = plans;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<PlanResponse>> ListAsync()
        {
            var plans = await _plans.ListAsync();

            return plans.Select(p => p.ToResponse()).ToList();
        }

        public async Task<PlanResponse> GetByIdAsync(Guid id)
        {
            var plan = await GetPlanOrThrowAsync(id);

            return plan.ToResponse();
        }

        public async Task<PlanResponse> CreateAsync(CreatePlanRequest request)
        {
            var plan = new Plan(
                request.Name,
                request.Description,
                request.Price,
                request.DurationInDays);

            await _plans.AddAsync(plan);
            await _unitOfWork.SaveChangesAsync();

            return plan.ToResponse();
        }

        public async Task<PlanResponse> UpdateAsync(Guid id, UpdatePlanRequest request)
        {
            var plan = await GetPlanOrThrowAsync(id);

            plan.Update(
                request.Name,
                request.Description,
                request.Price,
                request.DurationInDays);

            await _unitOfWork.SaveChangesAsync();

            return plan.ToResponse();
        }

        public async Task<PlanResponse> ActivateAsync(Guid id)
        {
            var plan = await GetPlanOrThrowAsync(id);

            plan.Activate();

            await _unitOfWork.SaveChangesAsync();

            return plan.ToResponse();
        }

        public async Task<PlanResponse> DeactivateAsync(Guid id)
        {
            var plan = await GetPlanOrThrowAsync(id);

            plan.Deactivate();

            await _unitOfWork.SaveChangesAsync();

            return plan.ToResponse();
        }

        private async Task<Plan> GetPlanOrThrowAsync(Guid id)
        {
            return await _plans.GetByIdAsync(id)
                ?? throw new NotFoundException($"Plano {id} não encontrado.");
        }
    }
}