using GymCore.Application.Users;
using Microsoft.EntityFrameworkCore;

namespace GymCore.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly GymCoreDbContext _context;

        public UserRepository(GymCoreDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}