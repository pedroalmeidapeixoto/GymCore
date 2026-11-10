using GymCore.Application.Common;
using GymCore.Application.Common.Exceptions;
using GymCore.Application.Roles;
using GymCore.Application.Users;
using GymCore.Domain.Entities;

namespace GymCore.Application.Students
{
    public class StudentService
    {
        private readonly IStudentRepository _students;
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(
            IStudentRepository students,
            IUserRepository users,
            IRoleRepository roles,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _students = students;
            _users = users;
            _roles = roles;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<StudentResponse>> ListAsync(string? search)
        {
            var students = await _students.ListAsync(search);

            return students.Select(s => s.ToResponse()).ToList();
        }

        public async Task<StudentResponse> GetByIdAsync(Guid id)
        {
            var student = await GetStudentOrThrowAsync(id);

            return student.ToResponse();
        }

        public async Task<StudentResponse> CreateAsync(CreateStudentRequest request)
        {
            PasswordPolicy.EnsureValid(request.Password);

            var role = await _roles.GetByNameAsync(RoleNames.Student)
                ?? throw new InvalidOperationException($"O papel '{RoleNames.Student}' não está cadastrado.");

            var user = new User(
                request.Name,
                request.Email,
                _passwordHasher.Hash(request.Password),
                role);

            var student = new Student(user, request.Cpf, request.BirthDate, request.Phone);

            if (await _users.ExistsByEmailAsync(user.Email))
                throw new ConflictException("Já existe um usuário com este e-mail.");

            if (await _students.ExistsByCpfAsync(student.Cpf))
                throw new ConflictException("Já existe um aluno com este CPF.");

            await _students.AddAsync(student);
            await _unitOfWork.SaveChangesAsync();

            return student.ToResponse();
        }

        public async Task<StudentResponse> UpdateAsync(Guid id, UpdateStudentRequest request)
        {
            var student = await GetStudentOrThrowAsync(id);

            student.User.Rename(request.Name);
            student.Update(request.BirthDate, request.Phone);

            await _unitOfWork.SaveChangesAsync();

            return student.ToResponse();
        }

        public async Task<StudentResponse> DeactivateAsync(Guid id)
        {
            var student = await GetStudentOrThrowAsync(id);

            student.User.Deactivate();

            await _unitOfWork.SaveChangesAsync();

            return student.ToResponse();
        }

        public async Task<StudentResponse> ActivateAsync(Guid id)
        {
            var student = await GetStudentOrThrowAsync(id);

            student.User.Activate();

            await _unitOfWork.SaveChangesAsync();

            return student.ToResponse();
        }

        private async Task<Student> GetStudentOrThrowAsync(Guid id)
        {
            return await _students.GetByIdAsync(id)
                ?? throw new NotFoundException($"Aluno {id} não encontrado.");
        }
    }
}