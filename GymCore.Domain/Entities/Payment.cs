using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using GymCore.Domain.Common;
using GymCore.Domain.Enums;
using GymCore.Domain.Exceptions;

namespace GymCore.Domain.Entities
{
    public class Payment: Entity
    {
        public Guid EnrollmentId { get; private set; }
        public Enrollment Enrollment { get; private set; } = null!;

        public decimal Amount { get; private set; } 
        public DateOnly DueDate { get; private set; }
        public DateOnly? PaymentDate { get; private set; }  
        public PaymentStatus Status { get; private set; }   

        protected Payment()
        {
        }

        public Payment(Enrollment enrollment, decimal amount, DateOnly dueDate)
        {
            if (enrollment is null)
                throw new DomainException("A matrícula do pagamento é obrigatória");

            if (amount <= 0)
                throw new DomainException("O valor do pagamento deve ser maior que zero");

            Enrollment = enrollment;
            EnrollmentId = enrollment.Id;
            Amount = amount;
            DueDate = dueDate;
            Status = PaymentStatus.Pending;

        }

        public void Pay(DateOnly paymentDate)
        {
            if (Status != PaymentStatus.Pending && Status != PaymentStatus.Overdue)
                throw new DomainException("Somente um pagamento pendente ou vencido pode ser confirmado");

            PaymentDate = paymentDate;
            Status = PaymentStatus.Paid;
            Touch();
        } 

        public void MarkAsOverdue()
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException("Somente um pagamento pendente pode ser marcado como vencido");
            
            Status = PaymentStatus.Overdue;
            Touch();
        }

        public void Cancel()
        {
            if (Status == PaymentStatus.Paid)
                throw new DomainException("Um pagamento já confirmado não pode ser cancelado");

            if (Status == PaymentStatus.Cancelled)
                throw new DomainException("Este pagamento já está cancelado");

            Status = PaymentStatus.Cancelled;
            Touch();
        }
    }
}
