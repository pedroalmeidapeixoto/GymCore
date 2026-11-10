using GymCore.Application.Common;
using Microsoft.AspNetCore.Identity;

namespace GymCore.Infrastructure.Security
{
    public class IdentityPasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string Hash(string password)
        {
            return _hasher.HashPassword(new object(), password);
        }
    }
}