using GymCore.Domain.Exceptions;

namespace GymCore.Application.Common
{
    public static class PasswordPolicy
    {
        private const int MinimumLength = 8;

        public static void EnsureValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < MinimumLength)
                throw new DomainException($"A senha deve ter pelo menos {MinimumLength} caracteres.");
        }
    }
}