using GymCore.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using GymCore.Domain.Exceptions;

namespace GymCore.Domain.Entities
{
    public class Workout : Entity
    {
        public Guid StudentId { get; private set; }
        public Student Student { get; private set; } = null!;

        public Guid PersonalTrainerId { get; private set; }
        public PersonalTrainer PersonalTrainer { get; private set; } = null!;

        public string Name { get; private set;  } = string.Empty;
        public string? Description { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly? EndDate { get; private set; }  

        protected Workout()
        {
        }

        public Workout(Student student, PersonalTrainer personalTrainer, string name, string? description, DateOnly startDate)
        {
            if (student is null)
                throw new DomainException("O aluno do treino é obrigatório");

            if (personalTrainer is null)
                throw new DomainException("O Personal Trainer do treino é obrigatório");

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome do treino é obrigatório");

            Student = student;
            StudentId = student.Id;
            PersonalTrainer = personalTrainer;
            PersonalTrainerId = personalTrainer.Id;
            Name = name.Trim();
            Description = description?.Trim();
            StartDate = startDate;
        }

        public void End(DateOnly endDate)
        {
            if (EndDate.HasValue)
                throw new DomainException("O treino já está encerrado.");

            if (endDate < StartDate)
                throw new DomainException("A data de encerramento não pode ser anterior à data de início");


            EndDate = endDate;
            Touch();
        }
    }
}
