using GymCore.Application.Common;
using GymCore.Application.Common.Exceptions;
using GymCore.Application.Payments;
using GymCore.Application.Plans;
using GymCore.Application.Students;
using GymCore.Domain.Entities;
using GymCore.Domain.Enums;

namespace GymCore.Application.Enrollments
{
    public class EnrollmentService
    {
        private readonly IEnrollmentRepository _enrollments;
        private readonly IPaymentRepository _payments;
        private readonly IStudentRepository _students;
        private readonly IPlanRepository _plans;
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(
            IEnrollmentRepository enrollments,
            IPaymentRepository payments,
            IStudentRepository students,
            IPlanRepository plans,
            IUnitOfWork unitOfWork)
        {
            _enrollments = enrollments;
            _payments = payments;
            _students = students;
            _plans = plans;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<EnrollmentResponse>> ListAsync(EnrollmentStatus? status)
        {
            var enrollments = await _enrollments.ListAsync(status);

            return enrollments.Select(e => e.ToResponse()).ToList();
        }

        public async Task<IReadOnlyList<EnrollmentResponse>> ListByStudentAsync(Guid studentId)
        {
            _ = await _students.GetByIdAsync(studentId)
                ?? throw new NotFoundException($"Aluno {studentId} não encontrado.");

            var enrollments = await _enrollments.ListByStudentAsync(studentId);

            return enrollments.Select(e => e.ToResponse()).ToList();
        }

        public async Task<EnrollmentResponse> GetByIdAsync(Guid id)
        {
            var enrollment = await _enrollments.GetByIdAsync(id)
                ?? throw new NotFoundException($"Matrícula {id} não encontrada.");

            return enrollment.ToResponse();
        }

        public async Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request)
        {
            var student = await _students.GetByIdAsync(request.StudentId)
                ?? throw new NotFoundException($"Aluno {request.StudentId} não encontrado.");

            var plan = await _plans.GetByIdAsync(request.PlanId)
                ?? throw new NotFoundException($"Plano {request.PlanId} não encontrado.");

            if (!plan.IsActive)
                throw new ConflictException("O plano está inativo e não pode receber novas matrículas.");

            if (await _enrollments.HasActiveEnrollmentAsync(student.Id))
                throw new ConflictException("O aluno já possui uma matrícula ativa.");

            var amount = request.Amount ?? plan.Price;
            var endDate = request.StartDate.AddDays(plan.DurationInDays);

            var enrollment = new Enrollment(student, plan, request.StartDate, endDate, amount);
            var firstPayment = new Payment(enrollment, amount, request.StartDate);

            await _enrollments.AddAsync(enrollment);
            await _payments.AddAsync(firstPayment);
            await _unitOfWork.SaveChangesAsync();

            return enrollment.ToResponse();
        }
    }
}