namespace BikeBuddy.Repositories
{
    public interface IPaymentRepository
    {
        Task AddPaymentAsync(Payment payment);
    }
}
