using GymCore.Domain.Entities;

namespace GymCore.Application.Roles
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
    }
}