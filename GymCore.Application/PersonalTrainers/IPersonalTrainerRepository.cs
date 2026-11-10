using GymCore.Domain.Entities;

namespace GymCore.Application.PersonalTrainers
{
    public interface IPersonalTrainerRepository
    {
        Task<PersonalTrainer?> GetByIdAsync(Guid id);

        Task<IReadOnlyList<PersonalTrainer>> ListAsync(string? search);

        Task<bool> ExistsByCrefAsync(string cref);

        Task AddAsync(PersonalTrainer personalTrainer);
    }
}