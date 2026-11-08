using GymCore.Application.Common.Exceptions;
using GymCore.Application.Plans;
using GymCore.Domain.Exceptions;
using GymCore.UnitTests.Fakes;

namespace GymCore.UnitTests.Plans
{
    public class PlanServiceTests
    {
        private readonly FakePlanRepository _repository = new();
        private readonly FakeUnitOfWork _unitOfWork = new();
        private readonly PlanService _service;

        public PlanServiceTests()
        {
            _service = new PlanService(_repository, _unitOfWork);
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsActivePlanAndSaves()
        {
            var request = new CreatePlanRequest("Mensal", "Acesso por 30 dias", 89.90m, 30);

            var response = await _service.CreateAsync(request);

            Assert.Equal("Mensal", response.Name);
            Assert.Equal(89.90m, response.Price);
            Assert.True(response.IsActive);
            Assert.Equal(1, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task CreateAsync_WithZeroPrice_ThrowsDomainExceptionAndDoesNotSave()
        {
            var request = new CreatePlanRequest("Mensal", null, 0m, 30);

            await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(request));

            Assert.Equal(0, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task GetByIdAsync_WithUnknownId_ThrowsNotFoundException()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task DeactivateAsync_WithActivePlan_ReturnsInactivePlanAndSaves()
        {
            var created = await _service.CreateAsync(new CreatePlanRequest("Mensal", null, 89.90m, 30));

            var response = await _service.DeactivateAsync(created.Id);

            Assert.False(response.IsActive);
            Assert.Equal(2, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_ReturnsNewValuesAndSaves()
        {
            var created = await _service.CreateAsync(new CreatePlanRequest("Mensal", null, 89.90m, 30));

            var response = await _service.UpdateAsync(
                created.Id,
                new UpdatePlanRequest("Mensal Plus", "Com avaliação inclusa", 99.90m, 30));

            Assert.Equal("Mensal Plus", response.Name);
            Assert.Equal(99.90m, response.Price);

            var reloaded = await _service.GetByIdAsync(created.Id);
            Assert.Equal(99.90m, reloaded.Price);
            Assert.Equal(2, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task UpdateAsync_WithUnknownId_ThrowsNotFoundException()
        {
            var request = new UpdatePlanRequest("Mensal", null, 89.90m, 30);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(Guid.NewGuid(), request));
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidPrice_ThrowsDomainExceptionAndKeepsOldValues()
        {
            var created = await _service.CreateAsync(new CreatePlanRequest("Mensal", null, 89.90m, 30));

            await Assert.ThrowsAsync<DomainException>(() =>
                _service.UpdateAsync(created.Id, new UpdatePlanRequest("Mensal Plus", null, -5m, 30)));

            var reloaded = await _service.GetByIdAsync(created.Id);
            Assert.Equal("Mensal", reloaded.Name);
            Assert.Equal(89.90m, reloaded.Price);
        }

        [Fact]
        public async Task ActivateAsync_WithInactivePlan_ReturnsActivePlan()
        {
            var created = await _service.CreateAsync(new CreatePlanRequest("Mensal", null, 89.90m, 30));
            await _service.DeactivateAsync(created.Id);

            var response = await _service.ActivateAsync(created.Id);

            Assert.True(response.IsActive);
        }

        [Fact]
        public async Task ListAsync_WithTwoPlans_ReturnsBoth()
        {
            await _service.CreateAsync(new CreatePlanRequest("Mensal", null, 89.90m, 30));
            await _service.CreateAsync(new CreatePlanRequest("Anual", null, 799.00m, 365));

            var plans = await _service.ListAsync();

            Assert.Equal(2, plans.Count);
        }
    }
}