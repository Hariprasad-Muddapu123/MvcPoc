namespace BikeBuddy.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _paymentRepository.AddPaymentAsync(payment);
        }
    }
}
