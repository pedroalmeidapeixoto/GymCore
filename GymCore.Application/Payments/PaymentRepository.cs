using GymCore.Domain.Entities;

namespace GymCore.Application.Payments
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
    }
}