using System;
using System.Collections.Generic;
using System.Text;
using GymCore.Domain.Common;
using GymCore.Domain.Enums;
using GymCore.Domain.Exceptions;


namespace GymCore.Domain.Entities
{
    public class Exercise : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public MuscleGroup MuscleGroup { get; private set; }
        public bool IsActive { get; private set;  }

        protected Exercise()
        {
        }

        public Exercise(string name, string? description, MuscleGroup muscleGroup)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome do exercício é obrigatório");

            Name = name.Trim();
            Description = description?.Trim();
            MuscleGroup = muscleGroup;
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
