namespace BikeBuddy.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public String UserId { get; set; }
        public string UserEmail { get; set; } // Email of the user receiving the notification
        public string Message { get; set; }  // Notification content
        public bool IsRead { get; set; } = false; // Read status
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Notification timestamp
    }

}
