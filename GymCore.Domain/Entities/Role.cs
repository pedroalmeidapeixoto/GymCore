using System;
using GymCore.Domain.Common;
using GymCore.Domain.Exceptions;

namespace GymCore.Domain.Entities
{
    public class Role : Entity
    {
        public string Name { get; private set; } = string.Empty;
        protected Role()
        {
        }

        public Role(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome do papel é obigatório");

            Name = name.Trim().ToUpperInvariant();
        }
    }
}
