using System;
using System.Globalization;
using GymCore.Domain.Common;
using GymCore.Domain.Exceptions;

namespace GymCore.Domain.Entities
{
    public class StudentPersonal : Entity
    {
        public Guid StudentId { get; private set; }
        public Student Student { get; private set; } = null!;

        public Guid PersonalTrainerId { get; private set; }
        public PersonalTrainer PersonalTrainer { get; private set; } = null!;
        
        public DateOnly StartDate { get; private set; }
        public DateOnly? EndDate { get; private set; }
        public bool IsActive { get; private set; }

        protected StudentPersonal() 
        {
        }

        public StudentPersonal(Student student, PersonalTrainer personalTrainer, DateOnly startDate)
        {
            if (student is null)
                throw new DomainException("O aluno do vínculo é obrigatório!");

            if (personalTrainer is null)
                throw new DomainException("O personal do vínculo é obrigatório!");


            Student = student;
            StudentId = student.Id;
            PersonalTrainer = personalTrainer;
            PersonalTrainerId = personalTrainer.Id;
            StartDate = startDate;
            IsActive = true;
        }

        public void EndLink(DateOnly endDate)
        {
            if (!IsActive)
                throw new DomainException("Esse vínculo já está encerrado.");

            if (endDate < StartDate)
                throw new DomainException("A data de encerramento não pode ser utilizada anterior à data de início");

            EndDate = endDate;
            IsActive = false;
            Touch();
        }
    }
}
