namespace BikeBuddy.Services
{
    public class PaymentService
    {
        //private readonly IPaymentRepository _paymentRepository;

        //public PaymentService(IPaymentRepository paymentRepository)
        //{
        //    _paymentRepository = paymentRepository;
        //}

        //public async Task AddPaymentAsync(Payment payment)
        //{
        //    await _paymentRepository.AddPaymentAsync(payment);
        //}

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
