using System;
using System.Globalization;
using GymCore.Domain.Common;
using GymCore.Domain.Exceptions;

namespace GymCore.Domain.Entities
{
    public class User : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; }
        public string PasswordHash { get; private set; } 
        public bool IsActive { get; private set; }
        public Guid RoleId { get; private set; }
        public Role Role { get; private set; }

        protected User() 
        { 
        }

        public User(string name, string email, string passwordHash, Role role)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome de Usuário é obrigatório");

            if (String.IsNullOrWhiteSpace(email))
                throw new DomainException("O e-mail é obrigatório");

            if (String.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("O hash de senha é obrigatório");

            if (role is null)
                throw new DomainException("O papel de usuário é obrigatório");

            Name = name.Trim();
            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            Role = role;
            RoleId = role.Id;
            IsActive = true;

        }

        public void Activate()
        {
            IsActive = true;
            Touch();
        }

        public void Deactive()
        {
            IsActive = false;
            Touch();
        }

        public void ChangePasswordHash(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new DomainException("O hash de senha é obrigatório");

            PasswordHash = newPasswordHash;
            Touch();
        }
    }
}
