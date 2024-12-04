namespace BikeBuddy.Services
{
    public interface IPaymentService
    {
        Task AddPaymentAsync(Payment payment);
    }
}
