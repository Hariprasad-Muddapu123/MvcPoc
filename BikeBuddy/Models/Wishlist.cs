namespace BikeBuddy.Models
{
    public class Wishlist
    {
        public int WishlistId { get; set; }
        public string UserId { get; set; } // Foreign key to User
        public int BikeId { get; set; } // Foreign key to Bike

        public User User { get; set; }
        public Bike Bike { get; set; }
    }
}
