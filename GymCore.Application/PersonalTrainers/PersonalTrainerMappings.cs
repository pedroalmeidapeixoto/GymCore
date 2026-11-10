using GymCore.Domain.Entities;

namespace GymCore.Application.PersonalTrainers
{
    public static class PersonalTrainerMappings
    {
        public static PersonalTrainerResponse ToResponse(this PersonalTrainer personalTrainer)
        {
            return new PersonalTrainerResponse(
                personalTrainer.Id,
                personalTrainer.User.Name,
                personalTrainer.User.Email,
                personalTrainer.Cref,
                personalTrainer.Phone,
                personalTrainer.User.IsActive);
        }
    }
}