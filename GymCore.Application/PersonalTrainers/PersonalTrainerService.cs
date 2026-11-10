using GymCore.Application.Common;
using GymCore.Application.Common.Exceptions;
using GymCore.Application.Roles;
using GymCore.Application.Users;
using GymCore.Domain.Entities;

namespace GymCore.Application.PersonalTrainers
{
    public class PersonalTrainerService
    {
        private readonly IPersonalTrainerRepository _personalTrainers;
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public PersonalTrainerService(
            IPersonalTrainerRepository personalTrainers,
            IUserRepository users,
            IRoleRepository roles,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _personalTrainers = personalTrainers;
            _users = users;
            _roles = roles;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<PersonalTrainerResponse>> ListAsync(string? search)
        {
            var personalTrainers = await _personalTrainers.ListAsync(search);

            return personalTrainers.Select(p => p.ToResponse()).ToList();
        }

        public async Task<PersonalTrainerResponse> GetByIdAsync(Guid id)
        {
            var personalTrainer = await GetPersonalTrainerOrThrowAsync(id);

            return personalTrainer.ToResponse();
        }

        public async Task<PersonalTrainerResponse> CreateAsync(CreatePersonalTrainerRequest request)
        {
            PasswordPolicy.EnsureValid(request.Password);

            var role = await _roles.GetByNameAsync(RoleNames.PersonalTrainer)
                ?? throw new InvalidOperationException($"O papel '{RoleNames.PersonalTrainer}' não está cadastrado.");

            var user = new User(
                request.Name,
                request.Email,
                _passwordHasher.Hash(request.Password),
                role);

            var personalTrainer = new PersonalTrainer(user, request.Cref, request.Phone);

            if (await _users.ExistsByEmailAsync(user.Email))
                throw new ConflictException("Já existe um usuário com este e-mail.");

            if (await _personalTrainers.ExistsByCrefAsync(personalTrainer.Cref))
                throw new ConflictException("Já existe um personal trainer com este CREF.");

            await _personalTrainers.AddAsync(personalTrainer);
            await _unitOfWork.SaveChangesAsync();

            return personalTrainer.ToResponse();
        }

        public async Task<PersonalTrainerResponse> UpdateAsync(Guid id, UpdatePersonalTrainerRequest request)
        {
            var personalTrainer = await GetPersonalTrainerOrThrowAsync(id);

            personalTrainer.User.Rename(request.Name);
            personalTrainer.Update(request.Phone);

            await _unitOfWork.SaveChangesAsync();

            return personalTrainer.ToResponse();
        }

        public async Task<PersonalTrainerResponse> DeactivateAsync(Guid id)
        {
            var personalTrainer = await GetPersonalTrainerOrThrowAsync(id);

            personalTrainer.User.Deactivate();

            await _unitOfWork.SaveChangesAsync();

            return personalTrainer.ToResponse();
        }

        public async Task<PersonalTrainerResponse> ActivateAsync(Guid id)
        {
            var personalTrainer = await GetPersonalTrainerOrThrowAsync(id);

            personalTrainer.User.Activate();

            await _unitOfWork.SaveChangesAsync();

            return personalTrainer.ToResponse();
        }

        private async Task<PersonalTrainer> GetPersonalTrainerOrThrowAsync(Guid id)
        {
            return await _personalTrainers.GetByIdAsync(id)
                ?? throw new NotFoundException($"Personal trainer {id} não encontrado.");
        }
    }
}