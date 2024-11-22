namespace BikeBuddy.Models
{
    public class Payment
    {
        public int PaymentId { get; set; } 
        public string UserId { get; set; } 
        public int RideId { get; set; } 
        public int BikeId { get; set; }

        public string TransactionId { get; set; } 
        public DateTime TransactionDateTime { get; set; } 
        public decimal TotalAmount { get; set; } 
        public string PaymentStatus { get; set; } 
        public string Currency { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Ride Ride { get; set; }
        public Bike Bike { get; set; }
    }

}
