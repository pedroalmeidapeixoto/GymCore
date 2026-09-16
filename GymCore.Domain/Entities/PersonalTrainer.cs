using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using GymCore.Domain.Common;
using GymCore.Domain.Exceptions;

namespace GymCore.Domain.Entities
{
    public class PersonalTrainer : Entity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string Cref { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;

        protected PersonalTrainer()
        {
        }

        public PersonalTrainer(User user, string cref, string phone)
        {
            if (user is null)
                throw new DomainException("O usuário do Personal é obrigatório");

            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("O telefone do Personal é obrigatório");

            if (string.IsNullOrWhiteSpace(cref))
                throw new DomainException("O CREF do Personal é obrigatório");


            User = user;
            UserId = user.Id;
            Cref = cref.Trim().ToUpperInvariant();
            Phone = phone.Trim();
        }
    }
}
