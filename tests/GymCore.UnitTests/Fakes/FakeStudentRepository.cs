using GymCore.Application.Students;
using GymCore.Application.Users;
using GymCore.Domain.Entities;

namespace GymCore.UnitTests.Fakes
{
    public class FakeStudentRepository : IStudentRepository, IUserRepository
    {
        private readonly List<Student> _students = new();

        public IReadOnlyList<Student> Students => _students;

        public Task<Student?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_students.FirstOrDefault(s => s.Id == id));
        }

        public Task<IReadOnlyList<Student>> ListAsync(string? search)
        {
            return Task.FromResult<IReadOnlyList<Student>>(_students.ToList());
        }

        public Task<bool> ExistsByCpfAsync(string cpf)
        {
            return Task.FromResult(_students.Any(s => s.Cpf == cpf));
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            return Task.FromResult(_students.Any(s => s.User.Email == email));
        }

        public Task AddAsync(Student student)
        {
            _students.Add(student);
            return Task.CompletedTask;
        }
    }
}