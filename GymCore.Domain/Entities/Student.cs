using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using GymCore.Domain.Common;
using GymCore.Domain.Exceptions;

namespace GymCore.Domain.Entities
{
    public class Student : Entity
    {
        public Guid UserId  { get; private set; }
        public User User { get; private set; } = null!;
        public string Cpf { get; private set; } = string.Empty;
        public DateOnly BirthDate { get; private set; }
        public string Phone { get; private set; } = string.Empty;

        protected Student() 
        { 
        }

        public Student(User user, string cpf, DateOnly birthDate, string phone)
        {
            if (user is null)
                throw new DomainException("O usuário do aluno é obrigatório");

            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("O telefone do aluno é obrigatório");

            var normalizedCpf = NormalizeCpf(cpf);

            if (normalizedCpf.Length != 11)
                throw new DomainException("O CPF do aluno é inválido");


            User = user;
            UserId = user.Id;
            Cpf = normalizedCpf;
            BirthDate = birthDate;
            Phone = phone.Trim();
        }

        private static string NormalizeCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return string.Empty;

            return new string(cpf.Where(char.IsDigit).ToArray());
        }
    }
}
