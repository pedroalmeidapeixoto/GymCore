using GymCore.Application.Common.Exceptions;
using GymCore.Application.PersonalTrainers;
using GymCore.Domain.Exceptions;
using GymCore.UnitTests.Fakes;

namespace GymCore.UnitTests.PersonalTrainers
{
    public class PersonalTrainerServiceTests
    {
        private readonly FakePersonalTrainerRepository _personalTrainers = new();
        private readonly FakeRoleRepository _roles = new();
        private readonly FakeUnitOfWork _unitOfWork = new();
        private readonly PersonalTrainerService _service;

        public PersonalTrainerServiceTests()
        {
            _service = new PersonalTrainerService(
                _personalTrainers, _personalTrainers, _roles, new FakePasswordHasher(), _unitOfWork);
        }

        private static CreatePersonalTrainerRequest ValidRequest(
            string email = "carlos@gymcore.test",
            string cref = "012345-G/SP")
        {
            return new CreatePersonalTrainerRequest(
                "Carlos Personal", email, "senha-segura-123", cref, "11990000001");
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_NormalizesEmailAndCrefAndSaves()
        {
            var response = await _service.CreateAsync(ValidRequest("CARLOS@Gymcore.test", "012345-g/sp"));

            Assert.Equal("carlos@gymcore.test", response.Email);
            Assert.Equal("012345-G/SP", response.Cref);
            Assert.True(response.IsActive);
            Assert.Equal(1, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task CreateAsync_StoresTheHashAndNeverThePlainPassword()
        {
            await _service.CreateAsync(ValidRequest());

            var storedHash = _personalTrainers.PersonalTrainers.Single().User.PasswordHash;

            Assert.Equal("hashed:senha-segura-123", storedHash);
        }

        [Fact]
        public async Task CreateAsync_WithShortPassword_ThrowsDomainExceptionAndDoesNotSave()
        {
            var request = ValidRequest() with { Password = "curta" };

            await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(request));

            Assert.Equal(0, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task CreateAsync_WithBlankCref_ThrowsDomainException()
        {
            var request = ValidRequest() with { Cref = "   " };

            await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateEmail_ThrowsConflictException()
        {
            await _service.CreateAsync(ValidRequest("carlos@gymcore.test", "012345-G/SP"));

            var duplicate = ValidRequest("CARLOS@gymcore.test", "067890-G/RJ");

            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(duplicate));
            Assert.Equal(1, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateCrefInDifferentCase_ThrowsConflictException()
        {
            await _service.CreateAsync(ValidRequest("carlos@gymcore.test", "012345-G/SP"));

            var duplicate = ValidRequest("marina@gymcore.test", "012345-g/sp");

            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(duplicate));
        }

        [Fact]
        public async Task CreateAsync_WithoutPersonalTrainerRole_ThrowsInvalidOperationException()
        {
            _roles.Roles.Clear();

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(ValidRequest()));
        }

        [Fact]
        public async Task GetByIdAsync_WithUnknownId_ThrowsNotFoundException()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateAsync_ChangesNameAndPhoneButNotCrefOrEmail()
        {
            var created = await _service.CreateAsync(ValidRequest());

            var response = await _service.UpdateAsync(
                created.Id,
                new UpdatePersonalTrainerRequest("Carlos Souza", "11999998888"));

            Assert.Equal("Carlos Souza", response.Name);
            Assert.Equal("11999998888", response.Phone);
            Assert.Equal("012345-G/SP", response.Cref);
            Assert.Equal("carlos@gymcore.test", response.Email);
        }

        [Fact]
        public async Task DeactivateAsync_ThenActivateAsync_TogglesIsActive()
        {
            var created = await _service.CreateAsync(ValidRequest());

            var deactivated = await _service.DeactivateAsync(created.Id);
            var reactivated = await _service.ActivateAsync(created.Id);

            Assert.False(deactivated.IsActive);
            Assert.True(reactivated.IsActive);
        }

        [Fact]
        public async Task ListAsync_ReturnsCreatedPersonalTrainers()
        {
            await _service.CreateAsync(ValidRequest("carlos@gymcore.test", "012345-G/SP"));
            await _service.CreateAsync(ValidRequest("marina@gymcore.test", "067890-G/RJ"));

            var personalTrainers = await _service.ListAsync(null);

            Assert.Equal(2, personalTrainers.Count);
        }
    }
}