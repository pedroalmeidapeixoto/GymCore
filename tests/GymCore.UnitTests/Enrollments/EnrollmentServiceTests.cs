using GymCore.Application.Common.Exceptions;
using GymCore.Application.Enrollments;
using GymCore.Application.Plans;
using GymCore.Application.Students;
using GymCore.Domain.Entities;
using GymCore.Domain.Enums;
using GymCore.UnitTests.Fakes;

namespace GymCore.UnitTests.Enrollments
{
    public class EnrollmentServiceTests
    {
        private readonly FakeEnrollmentRepository _enrollments = new();
        private readonly FakePaymentRepository _payments = new();
        private readonly FakeStudentRepository _students = new();
        private readonly FakePlanRepository _plans = new();
        private readonly FakeUnitOfWork _unitOfWork = new();
        private readonly EnrollmentService _service;

        public EnrollmentServiceTests()
        {
            _service = new EnrollmentService(_enrollments, _payments, _students, _plans, _unitOfWork);
        }

        private async Task<Student> CreateStudentAsync(string cpf = "52998224725", string email = "ana@gymcore.test")
        {
            var user = new User("Ana Aluna", email, "hash", new Role(RoleNames.Student));
            var student = new Student(user, cpf, new DateOnly(1995, 3, 12), "11980000001");
            await _students.AddAsync(student);
            return student;
        }

        private async Task<Plan> CreatePlanAsync(decimal price = 89.90m, int days = 30)
        {
            var plan = new Plan("Mensal", null, price, days);
            await _plans.AddAsync(plan);
            return plan;
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_CreatesPendingEnrollmentWithCalculatedEndDate()
        {
            var student = await CreateStudentAsync();
            var plan = await CreatePlanAsync(89.90m, 30);
            var start = new DateOnly(2026, 11, 1);

            var response = await _service.CreateAsync(new CreateEnrollmentRequest(student.Id, plan.Id, start, null));

            Assert.Equal(EnrollmentStatus.Pending, response.Status);
            Assert.Equal(new DateOnly(2026, 12, 1), response.EndDate);
            Assert.Equal("Ana Aluna", response.StudentName);
            Assert.Equal("Mensal", response.PlanName);
            Assert.Equal(1, _unitOfWork.SaveCount);
        }

        [Fact]
        public async Task CreateAsync_WithoutAmount_UsesTheCurrentPlanPrice()
        {
            var student = await CreateStudentAsync();
            var plan = await CreatePlanAsync(89.90m);

            var response = await _service.CreateAsync(
                new CreateEnrollmentRequest(student.Id, plan.Id, new DateOnly(2026, 11, 1), null));

            Assert.Equal(89.90m, response.Amount);
        }

        [Fact]
        public async Task CreateAsync_WithAmount_UsesTheNegotiatedAmount()
        {
            var student = await CreateStudentAsync();
            var plan = await CreatePlanAsync(89.90m);

            var response = await _service.CreateAsync(
                new CreateEnrollmentRequest(student.Id, plan.Id, new DateOnly(2026, 11, 1), 79.90m));

            Assert.Equal(79.90m, response.Amount);
        }

        [Fact]
        public async Task CreateAsync_PlanPriceChangeAfterwards_DoesNotAffectTheContractedAmount()
        {
            var student = await CreateStudentAsync();
            var plan = await CreatePlanAsync(89.90m);
            var created = await _service.CreateAsync(
                new CreateEnrollmentRequest(student.Id, plan.Id, new DateOnly(2026, 11, 1), null));

            plan.Update("Mensal", null, 120.00m, 30);

            var reloaded = await _service.GetByIdAsync(created.Id);
            Assert.Equal(89.90m, reloaded.Amount);
            Assert.Equal(120.00m, plan.Price);
        }

        [Fact]
        public async Task CreateAsync_CreatesTheFirstPaymentTogetherWithTheEnrollment()
        {
            var student = await CreateStudentAsync();
            var plan = await CreatePlanAsync(89.90m);
            var start = new DateOnly(2026, 11, 1);

            await _service.CreateAsync(new CreateEnrollmentRequest(student.Id, plan.Id, start, null));

            var payment = Assert.Single(_payments.Payments);
            Assert.Equal(89.90m, payment.Amount);
            Assert.Equal(start, payment.DueDate);
            Assert.Equal(PaymentStatus.Pending, payment.Status);
        }

        [Fact]
        public async Task CreateAsync_WithInactivePlan_ThrowsConflictExceptionAndDoesNotSave()
        {
            var student = await CreateStudentAsync();
            var plan = await CreatePlanAsync();
            plan.Deactivate();

            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(
                new CreateEnrollmentRequest(student.Id, plan.Id, new DateOnly(2026, 11, 1), null)));

            Assert.Equal(0, _unitOfWork.SaveCount);
            Assert.Empty(_payments.Payments);
        }

        [Fact]
        public async Task CreateAsync_WhenStudentAlreadyHasActiveEnrollment_ThrowsConflictException()
        {
            var student = await CreateStudentAsync();
            var plan = await CreatePlanAsync();
            var first = await _service.CreateAsync(
                new CreateEnrollmentRequest(student.Id, plan.Id, new DateOnly(2026, 11, 1), null));
            _enrollments.Enrollments.Single(e => e.Id == first.Id).Activate();

            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(
                new CreateEnrollmentRequest(student.Id, plan.Id, new DateOnly(2026, 12, 1), null)));
        }

        [Fact]
        public async Task CreateAsync_WithUnknownStudent_ThrowsNotFoundException()
        {
            var plan = await CreatePlanAsync();

            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(
                new CreateEnrollmentRequest(Guid.NewGuid(), plan.Id, new DateOnly(2026, 11, 1), null)));
        }

        [Fact]
        public async Task CreateAsync_WithUnknownPlan_ThrowsNotFoundException()
        {
            var student = await CreateStudentAsync();

            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(
                new CreateEnrollmentRequest(student.Id, Guid.NewGuid(), new DateOnly(2026, 11, 1), null)));
        }

        [Fact]
        public async Task GetByIdAsync_WithUnknownId_ThrowsNotFoundException()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task ListAsync_WithStatusFilter_ReturnsOnlyMatchingEnrollments()
        {
            var anaStudent = await CreateStudentAsync("52998224725", "ana@gymcore.test");
            var brunoStudent = await CreateStudentAsync("11144477735", "bruno@gymcore.test");
            var plan = await CreatePlanAsync();
            var ana = await _service.CreateAsync(
                new CreateEnrollmentRequest(anaStudent.Id, plan.Id, new DateOnly(2026, 11, 1), null));
            await _service.CreateAsync(
                new CreateEnrollmentRequest(brunoStudent.Id, plan.Id, new DateOnly(2026, 11, 1), null));
            _enrollments.Enrollments.Single(e => e.Id == ana.Id).Activate();

            var active = await _service.ListAsync(EnrollmentStatus.Active);
            var pending = await _service.ListAsync(EnrollmentStatus.Pending);
            var all = await _service.ListAsync(null);

            Assert.Single(active);
            Assert.Single(pending);
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public async Task ListByStudentAsync_WithUnknownStudent_ThrowsNotFoundException()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.ListByStudentAsync(Guid.NewGuid()));
        }
    }
}