namespace BikeBuddy.Notification
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    public class NotificationHub : Hub
    {
        private readonly IServiceProvider _serviceProvider;
        private static List<string> _connectedUsers = new();

        public NotificationHub(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Method to add user to the SignalR group and track them as connected
        public async Task JoinChat(string userId)
        {
            // Add user to the group
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            _connectedUsers.Add(userId);
            Console.WriteLine($"User {Context.UserIdentifier} joined group {userId}");
        }

        // Method to handle disconnections and remove users from the group
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                // Remove user from the group when disconnected
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                // Remove user from the connected users list
                _connectedUsers.Remove(userId);
                Console.WriteLine($"User {userId} disconnected.");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Fetch unread notifications for a user and mark them as read
        public async Task FetchNotifications(string userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var unreadNotifications = await dbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                await Clients.Caller.SendAsync("ReceiveNotification", notification.Message);
                notification.IsRead = true;
            }

            await dbContext.SaveChangesAsync();
        }

        // Method to send a message/notification to a user
        public async Task SendMessage(string userId, string message)
        {
            // Check if user is online
            if (_connectedUsers.Contains(userId))
            {
                // If user is online, send notification directly
                await Clients.Group(userId).SendAsync("ReceiveNotification", message);
            }
            else
            {
                // Store in the database if user is offline
                await StoreNotification(userId, message);
            }
        }

        // Method to store notifications for offline users
        private async Task StoreNotification(string userId, string message)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var notification = new Models.Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            dbContext.Notifications.Add(notification);
            await dbContext.SaveChangesAsync();
        }

        // Method to check if a user is online
        public static bool IsUserOnline(string userId)
        {
            return _connectedUsers.Contains(userId);
        }
    }
}
