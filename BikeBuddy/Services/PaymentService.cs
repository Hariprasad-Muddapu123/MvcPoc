namespace BikeBuddy.Services
{
    public class PaymentService
    {
        private readonly ApplicationDbContext _context;
        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddPaymentAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }
    }
}
