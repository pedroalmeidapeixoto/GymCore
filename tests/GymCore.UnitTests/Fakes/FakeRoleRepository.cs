using GymCore.Application.Roles;
using GymCore.Domain.Entities;

namespace GymCore.UnitTests.Fakes
{
    public class FakeRoleRepository : IRoleRepository
    {
        public List<Role> Roles { get; } = new() { new Role(RoleNames.Student) };

        public Task<Role?> GetByNameAsync(string name)
        {
            return Task.FromResult(Roles.FirstOrDefault(r => r.Name == name));
        }
    }
}