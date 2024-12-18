namespace BikeBuddy.Repositories
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task<Notification> GetNotificationByIdAsync(int notificationId);
        Task UpdateNotificationAsync(Notification notification);
    }
}
