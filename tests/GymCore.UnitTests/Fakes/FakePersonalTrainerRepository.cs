using GymCore.Application.PersonalTrainers;
using GymCore.Application.Users;
using GymCore.Domain.Entities;

namespace GymCore.UnitTests.Fakes
{
    public class FakePersonalTrainerRepository : IPersonalTrainerRepository, IUserRepository
    {
        private readonly List<PersonalTrainer> _personalTrainers = new();

        public IReadOnlyList<PersonalTrainer> PersonalTrainers => _personalTrainers;

        public Task<PersonalTrainer?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_personalTrainers.FirstOrDefault(p => p.Id == id));
        }

        public Task<IReadOnlyList<PersonalTrainer>> ListAsync(string? search)
        {
            return Task.FromResult<IReadOnlyList<PersonalTrainer>>(_personalTrainers.ToList());
        }

        public Task<bool> ExistsByCrefAsync(string cref)
        {
            return Task.FromResult(_personalTrainers.Any(p => p.Cref == cref));
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            return Task.FromResult(_personalTrainers.Any(p => p.User.Email == email));
        }

        public Task AddAsync(PersonalTrainer personalTrainer)
        {
            _personalTrainers.Add(personalTrainer);
            return Task.CompletedTask;
        }
    }
}