using System;
using System.Collections.Generic;
using System.Text;
using GymCore.Domain.Common;
using GymCore.Domain.Enums;
using GymCore.Domain.Exceptions;

namespace GymCore.Domain.Entities
{
    public class Enrollment : Entity
    {
        public Guid StudentId { get; private set; }
        public Student Student { get; private set; } = null!;

        public Guid PlanId { get; private set; }
        public Plan Plan { get; private set; } = null!;

        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }   
        public decimal Amount { get; private set; } 
        public EnrollmentStatus Status { get; private set; }  

        protected Enrollment()
        {
        }

        public Enrollment(Student student, Plan plan, DateOnly startDate, DateOnly endDate, decimal amount)
            {

            if (student is null)
                throw new DomainException("O aluno da matrícula é obrigatório");

            if (plan is null)
                throw new DomainException("O plano da matrícula é obrigatório");

            if (endDate < startDate)
                throw new DomainException("A data final não pode ser anterior a data de início");

            if (amount <= 0)
                throw new DomainException("O valor contratado deve ser maior que zero");

            Student = student;
            StudentId = student.Id;
            Plan = plan;
            PlanId = plan.Id;
            StartDate = startDate;
            EndDate = endDate;
            Amount = amount;
            Status = EnrollmentStatus.Pending;

        }

        public void Activate()
        {

            if (Status != EnrollmentStatus.Pending)
                throw new DomainException("Somente uma matrícula pendente pode ser ativada");

            Status = EnrollmentStatus.Active;
            Touch();

        }

        public void Cancel()
        {
            if (Status == EnrollmentStatus.Cancelled)
                throw new DomainException("Essa matrícula já está cancelada");

            Status = EnrollmentStatus.Cancelled;
            Touch();
        }

        public void Expire()
        {

            if (Status != EnrollmentStatus.Active)
                throw new DomainException("Somente uma matrícula ativa pode expirar.");

            Status = EnrollmentStatus.Expired;
            Touch();
        }

    }
}
