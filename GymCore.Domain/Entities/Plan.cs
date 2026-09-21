using System;
using GymCore.Domain.Common;
using GymCore.Domain.Exceptions;using System.Text;

namespace GymCore.Domain.Entities
{
    public class Plan : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public int DurationInDays { get; private set; }
        public bool IsActive { get; private set; }

        protected Plan()
        {
        }

        public Plan(string name, string? description,  decimal price, int durationInDays)
            {

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome do plano é obrigatório!");

            if (price <= 0)
                throw new DomainException("O preço do plano deve ser maior que zero");

            if (durationInDays <= 0)
                throw new DomainException("A duração do plano deve ser maior que zero");

            Name = name.Trim();
            Description = description?.Trim();
            Price = price;
            DurationInDays = durationInDays;
            IsActive = true;
        }

        public void Activate()
        {
            IsActive = true;
            Touch();
        }

        public void Deactivate()
        {
            IsActive = false;
            Touch();
        }

    }
}
