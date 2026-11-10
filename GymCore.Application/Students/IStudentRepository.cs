using GymCore.Domain.Entities;

namespace GymCore.Application.Students
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(Guid id);

        Task<IReadOnlyList<Student>> ListAsync(string? search);

        Task<bool> ExistsByCpfAsync(string cpf);

        Task AddAsync(Student student);
    }
}