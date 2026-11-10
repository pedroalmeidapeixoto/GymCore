using GymCore.Application.Common.Exceptions;
using GymCore.Application.Students;
using GymCore.Domain.Exceptions;
using GymCore.UnitTests.Fakes;

namespace GymCore.UnitTests.Students
{
    public class StudentServiceTests
    {
        private readonly FakeStudentRepository _students = new();
        private readonly FakeRoleRepository _roles = new();
        private readonly FakeUnitOfWork _unitOfWork = new();
        private readonly StudentService _service;

        public StudentServiceTests()
        {
            _service = new StudentService(
                _students, _students, _roles, new FakePasswordHasher(), _unitOfWork);
        }

        private static CreateStudentRequest ValidRequest(
            string email = "ana@gymcore.test",
            string cpf = "52998224725")
        {
            return new CreateStudentRequest(
                "Ana Aluna", email, "senha-segura-123", cpf,
                new DateOnly(1995, 3, 12), "11980000001");
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsMaskedCpfAndSaves()
        {
            var response = await _service.CreateAsync(ValidRequest("ANA@Gymcore.test"));

            Assert.Equal("ana@gymcore.test", response.Email);
            Assert.Equal("***.982.247-**", response.MaskedCpf);
            Assert.True(response.IsActive);
            Assert.Equal(1, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task CreateAsync_StoresTheHashAndNeverThePlainPassword()
        {
            await _service.CreateAsync(ValidRequest());

            var storedHash = _students.Students.Single().User.PasswordHash;

            Assert.Equal("hashed:senha-segura-123", storedHash);
            Assert.NotEqual("senha-segura-123", storedHash);
        }

        [Fact]
        public async Task CreateAsync_WithShortPassword_ThrowsDomainExceptionAndDoesNotSave()
        {
            var request = ValidRequest() with { Password = "curta" };

            await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(request));

            Assert.Equal(0, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidCpf_ThrowsDomainException()
        {
            var request = ValidRequest() with { Cpf = "123" };

            await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateEmail_ThrowsConflictException()
        {
            await _service.CreateAsync(ValidRequest("ana@gymcore.test", "52998224725"));

            var duplicate = ValidRequest("ANA@gymcore.test", "11144477735");

            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(duplicate));
            Assert.Equal(1, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateCpfInDifferentFormat_ThrowsConflictException()
        {
            await _service.CreateAsync(ValidRequest("ana@gymcore.test", "52998224725"));

            var duplicate = ValidRequest("outra@gymcore.test", "529.982.247-25");

            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(duplicate));
        }

        [Fact]
        public async Task CreateAsync_WithoutStudentRole_ThrowsInvalidOperationException()
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
        public async Task UpdateAsync_ChangesNameBirthDateAndPhoneButNotEmailOrCpf()
        {
            var created = await _service.CreateAsync(ValidRequest());

            var response = await _service.UpdateAsync(
                created.Id,
                new UpdateStudentRequest("Ana Souza", new DateOnly(1996, 4, 20), "11999998888"));

            Assert.Equal("Ana Souza", response.Name);
            Assert.Equal(new DateOnly(1996, 4, 20), response.BirthDate);
            Assert.Equal("11999998888", response.Phone);
            Assert.Equal("ana@gymcore.test", response.Email);
            Assert.Equal("***.982.247-**", response.MaskedCpf);
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
        public async Task ListAsync_ReturnsCreatedStudents()
        {
            await _service.CreateAsync(ValidRequest("ana@gymcore.test", "52998224725"));
            await _service.CreateAsync(ValidRequest("bruno@gymcore.test", "11144477735"));

            var students = await _service.ListAsync(null);

            Assert.Equal(2, students.Count);
        }
    }
}