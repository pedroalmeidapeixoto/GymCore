using GymCore.Application.Payments;
using GymCore.Domain.Entities;

namespace GymCore.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly GymCoreDbContext _context;

        public PaymentRepository(GymCoreDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }
    }
}