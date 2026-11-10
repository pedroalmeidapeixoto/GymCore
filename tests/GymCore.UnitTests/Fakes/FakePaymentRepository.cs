using GymCore.Application.Payments;
using GymCore.Domain.Entities;

namespace GymCore.UnitTests.Fakes
{
    public class FakePaymentRepository : IPaymentRepository
    {
        private readonly List<Payment> _payments = new();

        public IReadOnlyList<Payment> Payments => _payments;

        public Task AddAsync(Payment payment)
        {
            _payments.Add(payment);
            return Task.CompletedTask;
        }
    }
}