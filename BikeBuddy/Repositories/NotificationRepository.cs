namespace BikeBuddy.Repositories
{
    using BikeBuddy.Models;
    public class NotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkNotificationsAsReadAsync(IEnumerable<int> notificationIds)
        {
            var notifications = _context.Notifications
                .Where(n => notificationIds.Contains(n.Id));
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
